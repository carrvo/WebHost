using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;

namespace PowerShell.UDP
{
    [Cmdlet(VerbsCommon.Get, "UdpListener")]
    [OutputType(typeof(UdpClient))]
    public sealed class GetUdpListenerCommand : Cmdlet
    {
        internal static IList<UdpClient> Listeners { get; }
            = new List<UdpClient>();

        /// <inheritdoc/>
        protected override void BeginProcessing()
        {
            foreach (var listener in Listeners)
            {
                WriteObject(listener);
            }
        }
    }
}
