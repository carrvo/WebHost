using System.Collections.Generics;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsLifecycle.Submit, "HttpResponse")]
    public sealed class SubmitHttpResponseCommand : Cmdlet
    {
        [Parameter(Mandatory)]
        [Alias("Request")]
        public HttpListenerContext Context { get; set; }

        protected override void EndProcessing()
        {
            Context.Response.Abort();
        }
    }
}
