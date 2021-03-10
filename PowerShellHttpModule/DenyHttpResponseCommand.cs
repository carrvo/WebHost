using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Closes the connection to the client without sending a response.</para>
    /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistenerresponse.abort?view=net-5.0#System_Net_HttpListenerResponse_Abort">Citation.</para>
    /// <para type="description">This is expected to *only* be called once.</para>
    /// <example>
    ///     <para>Denies Single Response</para>
    ///     <code>
    ///     try {
    ///         'http://localhost/api' |
    ///         New-HttpListener -AuthenticationSchemes Basic |
    ///             Start-HttpListener |
    ///             Wait-HttpRequest -Count 1 |
    ///             ForEach-Object {
    ///                 $request = $_ | Receive-HttpRequest | ConvertFrom-Json
    ///                 Deny-HttpResponse -Request $_
    ///             }
    ///     } finally {
    ///         Get-HttpListener | Stop-HttpListener
    ///     }
    ///     </code>
    ///     <para>Call from *another* shell:</para>
    ///     <code>
    ///     $cred = Get-Credential -Message "For PowerShell-REST" -UserName "$ENV:COMPUTERNAME\$ENV:USERNAME"
    ///     Invoke-RestMethod -Method Post -Uri 'http://localhost/api' -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication
    ///     </code>
    /// </example>
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
