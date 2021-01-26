using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
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
