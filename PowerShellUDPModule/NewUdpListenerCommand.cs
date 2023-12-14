using System;
using System.Management.Automation;
using System.Net.Sockets;
using System.Net;

namespace PowerShell.UDP
{
    /// <summary>
    /// <para type="synopsis">Creates a new object.</para>
    /// <para type="description"> objects are used to listen for client requests.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "UdpListener")]
    [OutputType(typeof(UdpClient))]
    public sealed class NewUdpListenerCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">The port to listen on.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public Int32 Port { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecord()
        {
            //Binding to local port
            UdpClient receivingUdpClient = new UdpClient(Port);
            //var localBinding = new IPEndPoint(IPAddress.Any, Port);
            //UdpClient receivingUdpClient = new UdpClient(localBinding);

            GetUdpListenerCommand.Listeners.Add(receivingUdpClient);
            WriteObject(receivingUdpClient);
        }
    }
}
