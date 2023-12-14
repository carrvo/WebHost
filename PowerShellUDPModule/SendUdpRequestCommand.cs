using System;
using System.Management.Automation;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerShell.UDP
{
    /// <summary>
    /// <para type="synopsis"></para>
    /// <para type="description"></para>
    /// </summary>
    [Cmdlet(VerbsCommunications.Send, "UdpRequest")]
    [OutputType(typeof(UdpClient))]
    public sealed class SendUdpRequestCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">The port to send datagram to.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Port-Byte")]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Port-UTF8")]
        [Alias("Port")]
        public Int32 ServerPort { get; set; }

        /// <summary>
        /// <para type="description">
        /// An <see cref="IPEndPoint"/> specifying the server to send to.
        /// Default: IPEndPoint(IpAddress.Broadcast, 11000) # sends datagrams for broadcast on 11000.
        /// </para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "IPEndPoint-Byte")]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "IPEndPoint-UTF8")]
        public IPEndPoint ServerIpEndPoint { get; set; }

        /// <summary>
        /// <para type="description">The raw data to send.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Port-Byte")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "IPEndPoint-Byte")]
        [Alias("RawData", "RawBody", "RawContent")]
        public Byte[] RawDatagram { get; set; }

        /// <summary>
        /// <para type="description">The data to send (in UTF8).</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Port-UTF8")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "IPEndPoint-UTF8")]
        [Alias("Datagram", "Data", "Body", "Content")]
        public String UTF8Datagram { get; set; }

        /// <summary>
        /// <see cref="UdpClient"/>
        /// </summary>
        /// <remarks>send does not need local port</remarks>
        private UdpClient Client { get; } = new UdpClient();

        /// <inheritdoc/>
        protected override void BeginProcessing()
        {
            if (ServerIpEndPoint is null)
            {
                ServerIpEndPoint = new IPEndPoint(IPAddress.Broadcast, ServerPort);
            }
        }

        /// <inheritdoc/>
        protected override void ProcessRecord()
        {
            var raw = String.IsNullOrEmpty(UTF8Datagram)
                ? RawDatagram
                : Encoding.UTF8.GetBytes(UTF8Datagram);

            //Sending to remote port
            Client.Send(raw, raw.Length, ServerIpEndPoint);

            WriteObject(Client);
        }

        /// <inheritdoc/>
        protected override void EndProcessing()
        {
            Client.Dispose();
        }
    }
}
