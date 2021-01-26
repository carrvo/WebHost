using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsLifecycle.Deny, "HttpResponse")]
    public sealed class DenyHttpResponseCommand : Cmdlet
    {
        [Parameter(Mandatory = true)]
        [Alias("Request")]
        public HttpListenerContext Context { get; set; }

        protected override void EndProcessing()
        {
            Context.Response.Abort();
        }
    }
}
