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
    [Cmdlet(VerbsLifecycle.Wait, "UdpRequest")]
    [OutputType(typeof(UdpRequest))]
    public sealed class WaitUdpRequestCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">Used to listen for client requests.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Infinite")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Limited")]
        public UdpClient Listener { get; set; }

        /// <summary>
        /// <para type="description">
        /// An <see cref="IPEndPoint"/> specifying what senders to receive from.
        /// Default: IPEndPoint(IpAddress.Any, 0) # receives datagrams sent from any source.
        /// </para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Infinite")]
        [Parameter(Mandatory = false, ParameterSetName = "Limited")]
        IPEndPoint ClientIpEndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 0);

        /// <summary>
        /// <para type="description">Number of requests to listen for before exiting the listener loop.</para>
        /// </summary>
        [Parameter(ParameterSetName = "Limited")]
        [Alias("Count")]
        public Int64 NumberOfRequests { get; set; } = 1;

        /// <summary>
        /// <para type="description">Never stop the listener loop.</para>
        /// <para type="description">WARNING: be careful of how you are using this because it results in an infinite blocking loop.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Infinite")]
        [Alias("Infinity")]
        public SwitchParameter InfiniteRequests { get; set; }

        private Func<Boolean> NextRequest { get; set; }

        private void ListenForSingleRequest()
        {
            WriteVerbose($"{nameof(WaitUdpRequestCommand)} - {nameof(ProcessRecord)} - waiting for request");

            //Creates an IPEndPoint to record the IP Address and port number of the sender.
            var client = new IPEndPoint(ClientIpEndPoint.Address, ClientIpEndPoint.Port);

            //Receiving from remote port(ref records who the sender was)
            Byte[] receivedBytes = Listener.Receive(ref client);

            UdpRequest context = new UdpRequest(client, receivedBytes);
            WriteObject(context);
        }

        private Boolean GetInfiniteRequests()
        {
            WriteVerbose($"{nameof(WaitUdpRequestCommand)} - {nameof(ProcessRecord)} - remaining(infinite)");
            return true;
        }

        private Boolean GetLimitedRequests()
        {
            WriteVerbose($"{nameof(WaitUdpRequestCommand)} - {nameof(ProcessRecord)} - remaining({NumberOfRequests})");
            return 0 != NumberOfRequests--;
        }

        /// <inheritdoc/>
        protected override void BeginProcessing()
        {
            WriteVerbose($"{nameof(WaitUdpRequestCommand)} - {nameof(BeginProcessing)} - determine {nameof(NumberOfRequests)}");
            if (InfiniteRequests.IsPresent)
            {
                NextRequest = GetInfiniteRequests;
            }
            else
            {
                NextRequest = GetLimitedRequests;
            }
            WriteVerbose($"{nameof(WaitUdpRequestCommand)} - {nameof(BeginProcessing)} - end");
        }

        /// <inheritdoc/>
        protected override void ProcessRecord()
        {
            WriteVerbose($"{nameof(WaitUdpRequestCommand)} - {nameof(ProcessRecord)} - while {nameof(NextRequest)}({NumberOfRequests})");
            while (NextRequest())
            {
                WriteVerbose($"{nameof(WaitUdpRequestCommand)} - {nameof(ProcessRecord)} - {nameof(NextRequest)}");
                ListenForSingleRequest();
            }
            WriteVerbose($"{nameof(WaitUdpRequestCommand)} - {nameof(ProcessRecord)} - end");
        }
    }
}
