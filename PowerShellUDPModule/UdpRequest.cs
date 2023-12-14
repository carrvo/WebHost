using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PowerShell.UDP
{
    /// <summary>
    /// Container for a <see cref="UdpClient"/> request.
    /// </summary>
    public sealed class UdpRequest
    {
        internal UdpRequest(IPEndPoint remoteIpEndPoint, byte[] receivedBytes)
        {
            Client = remoteIpEndPoint;
            RawPayload = receivedBytes;
        }

        /// <summary>
        /// Client who sent the datagram.
        /// </summary>
        public IPEndPoint Client { get; }

        /// <summary>
        /// The datagram content.
        /// This will need to be parsed according to its expected format.
        /// </summary>
        public Byte[] RawPayload { get; }

        /// <summary>
        /// The datagram content parsed into UTF8.
        /// </summary>
        public String UTF8Payload => Encoding.UTF8.GetString(RawPayload);
    }
}
