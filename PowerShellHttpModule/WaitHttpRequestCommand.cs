using System;
using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsLifecycle.Wait, "HttpRequest")]
    [OutputType(typeof(HttpListenerContext))]
    public sealed class WaitHttpRequestCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Infinite")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Limited")]
        public HttpListener Listener { get; set; }

        [Parameter(ParameterSetName = "Limited")]
        [Alias("Count")]
        public Int64 NumberOfRequests { get; set; } = 1;

        [Parameter(Mandatory = true, ParameterSetName = "Infinite")]
        [Alias("Infinity")]
        public SwitchParameter InfiniteRequests { get; set; }

        private Func<Boolean> NextRequest { get; set; }

        private void ListenForSingleRequest(HttpListener listener)
        {
            if (listener.IsListening)
            {
                HttpListenerContext context = listener.GetContext();
                if (context.Request.IsAuthenticated)
                {
                    WriteObject(context);
                }
                else
                {
                    WriteError(new ErrorRecord(
                        new WebException(),
                        "Unauthorized",
                        ErrorCategory.AuthenticationError,
                        context));
                    context.Response.StatusCode = 403;
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Unauthorized");
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer,0,buffer.Length);
                    context.Response.OutputStream.Close();
                    context.Response.Close();
                }
            }
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new WebException(),
                    "Listener Not Active",
                    ErrorCategory.ConnectionError,
                    listener));
            }
        }

        protected override void BeginProcessing()
        {
            if (InfiniteRequests.IsPresent)
            {
                NextRequest = () => true;
            }
            else
            {
                NextRequest = () => 0 == NumberOfRequests--;
            }
        }

        protected override void ProcessRecord()
        {
            while(NextRequest())
            {
                ListenForSingleRequest(Listener);
            }
        }
    }
}
