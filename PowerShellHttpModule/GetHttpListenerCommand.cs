using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Gets all the created <see cref="HttpListener"/> objects.</para>
    /// <para type="description">
    /// Gets all created <see cref="HttpListener"/> objects through this module's
    /// <see cref="Cmdlet"/>s, regardless of if they are still active or not.
    /// </para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "HttpListener")]
    [OutputType(typeof(HttpListener))]
    public sealed class GetHttpListenerCommand : Cmdlet
    {
        internal static IList<HttpListener> Listeners { get; }
            = new List<HttpListener>();

        protected override void BeginProcessing()
        {
            foreach (var listener in Listeners)
            {
                WriteObject(listener);
            }
        }
    }
}
