using System.Collections.Generics;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsCommon.Get, "HttpListener")]
    [OutputType(HttpListener)]
    public sealed class GetHttpListenerCommand : Cmdlet
    {
        internal static IEnumerable<HttpListener> Listeners { get; }
            = new List<HttpListener>();

        public override BeginProcessing()
        {
            foreach (var listener in Listeners)
            {
                WriteOutput(listener);
            }
        }
    }
}
