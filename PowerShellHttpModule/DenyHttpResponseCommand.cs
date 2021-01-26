using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Closes the connection to the client without sending a response.</para>
    /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistenerresponse.abort?view=net-5.0#System_Net_HttpListenerResponse_Abort">Citation.</para>
    /// <para type="description">This is expected to *only* be called once.</para>
    /// </summary>
    [Cmdlet(VerbsLifecycle.Deny, "HttpResponse")]
    public sealed class DenyHttpResponseCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">The <see cref="HttpListenerContext"/> that corresponds to the client request.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [Alias("Request")]
        public HttpListenerContext Context { get; set; }

        protected override void EndProcessing()
        {
            Context.Response.Abort();
        }
    }
}
