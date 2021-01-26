using System.Collections.Generics;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsLifecycle.Start, "HttpListener")]
    [OutputType(HttpListener)]
    public sealed class StartHttpListenerCommand : Cmdlet
    {
        [Parameter(Mandatory, ValueFromPipeline)]
        public HttpListener Listener { get; set; }

        protected override void ProcessRecord()
        {
            if (!Listener.IsListening)
            {
                Listener.Start();
            }
            WriteOutput(Listener);
        }
    }
}
