using System.Collections.Generics;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsLifecycle.Submit, "HttpResponse")]
    public sealed class SubmitHttpResponseCommand : Cmdlet
    {
        [Parameter(Mandatory, ValueFromPipeline)]
        public String Json { get; set; }

        [Parameter(Mandatory)]
        [Alias("Request")]
        public HttpListenerContext Context { get; set; }

        protected override void ProcessRecord()
        {
            Context.Response.StatusCode = 400;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Json);
            Context.Response.ContentLength64 = buffer.Length;
            Context.Response.OutputStream.Write(buffer,0,buffer.Length);
            Context.Response.OutputStream.Close();
            Context.Response.Close();
        }
    }
}
