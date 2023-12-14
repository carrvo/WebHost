using System;
using System.Management.Automation;
using System.Net.Sockets;

namespace PowerShell.UDP
{
    [Cmdlet(VerbsLifecycle.Stop, "UdpListener")]
    public sealed class StopUdpListenerCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">Used to listen for client requests.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public UdpClient Listener { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecord()
        {
            Listener.Dispose();
        }
    }
}
