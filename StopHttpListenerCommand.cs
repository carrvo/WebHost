using System.Collections.Generics;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsLifecycle.Stop, "HttpListener")]
    public sealed class StopHttpListenerCommand : Cmdlet
    {
        [Parameter(Mandatory, ValueFromPipeline)]
        public HttpListener Listener { get; set; }

        protected override void ProcessRecord()
        {
            if (Listener.IsListening)
            {
                Listener.Stop();
            }
        }
    }
}
