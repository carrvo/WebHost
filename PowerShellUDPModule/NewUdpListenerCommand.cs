using System;
using System.Management.Automation;

namespace PowerShellUDPModule
{
    /// <summary>
    /// <para type="synopsis">Creates a new object.</para>
    /// <para type="description"> objects are used to listen for client requests.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "UdpListener")]
    public class NewUdpListenerCommand : Cmdlet
    {
    }
}
