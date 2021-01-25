using System;
using System.Collections.Generics;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsLifecycle.Wait, "HttpRequest")]
    [OutputType(HttpListenerContext)]
    public sealed class WaitHttpRequestCommand : Cmdlet
    {
        [Parameter(Mandatory, ValueFromPipeline, ParameterSetName = "Infinite")]
        [Parameter(Mandatory, ValueFromPipeline, ParameterSetName = "Limited")]
        public HttpListener Listener { get; set; }

        [Parameter(ParameterSetName = "Limited")]
        [Alias("Count")]
        public Int64 NumberOfRequests { get; set; } = 1;

        [Parameter(Mandatory, ParameterSetName = "Infinite")]
        [Alias("Infinity")]
        public SwitchParameter InfiniteRequests { get; set; }

        private void ListenForSingleRequest(HttpListener listener)
        {
            if (listener.IsListening)
            {
                HttpListenerContext context = listener.GetContext();
                if (context.Request.IsAuthenticated)
                {
                    WriteOutput(context);
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
                    new WebException()
                    "Listener Not Active",
                    ErrorCategory.ConnectionError,
                    listener));
            }
        }

        protected override void ProcessRecord()
        {
            Func<Boolean> nextRequest = InfiniteRequests ?
                () => true,
                () => 0 == NumberOfRequests--;
            while(nextRequest())
            {
                ListenForSingleRequest(Listener);
            }
        }
    }
}
