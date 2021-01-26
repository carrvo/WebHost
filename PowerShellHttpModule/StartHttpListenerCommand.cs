using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsLifecycle.Start, "HttpListener")]
    [OutputType(typeof(HttpListener))]
    public sealed class StartHttpListenerCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public HttpListener Listener { get; set; }

        protected override void ProcessRecord()
        {
            if (!Listener.IsListening)
            {
                Listener.Start();
            }
            WriteObject(Listener);
        }
    }
}
