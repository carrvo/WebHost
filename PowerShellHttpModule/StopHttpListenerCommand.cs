using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Causes this instance to stop receiving new incoming requests and terminates processing of all ongoing requests.</para>
    /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener.stop?view=net-5.0#System_Net_HttpListener_Stop">Citation.</para>
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "HttpListener")]
    public sealed class StopHttpListenerCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">Used to listen for client requests.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
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
