using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Allows this instance to receive incoming requests.</para>
    /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener.start?view=net-5.0#System_Net_HttpListener_Start">Citation.</para>
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "HttpListener")]
    [OutputType(typeof(HttpListener))]
    public sealed class StartHttpListenerCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">Used to listen for client requests.</para>
        /// </summary>
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
