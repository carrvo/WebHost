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
    ///     <para>Submits Single Response</para>
    ///     <code>
    ///     Start-Job -Name "single response" -ScriptBlock {
    ///         try {
    ///             New-HttpListener $uri |
    ///                 Start-HttpListener |
    ///                 Wait-HttpRequest -Count 1 |
    ///                 ForEach-Object {
    ///                     $request = $_ | Receive-HttpRequest | ConvertFrom-Json
    ///                     @{Message="Hello $($request.Name)"} |
    ///                         ConvertTo-Json | Submit-HttpResponse -Request $_
    ///             }
    ///         } finally {
    ///             Get-HttpListener | Stop-HttpListener
    ///         }
    ///     }
    ///     </code>
    /// </example>
    /// <example>
    ///     <para>Submits Indefinite Responses</para>
    ///     <code>
    ///     Start-Job -Name "indefinte responses" -ScriptBlock {
    ///         try {
    ///             New-HttpListener $uri |
    ///                 Start-HttpListener |
    ///                 Wait-HttpRequest -Infinity |
    ///                 ForEach-Object {
    ///                     $request = $_ | Receive-HttpRequest | ConvertFrom-Json
    ///                     @{Message="Hello $($request.Name)"} |
    ///                         ConvertToJson | Submit-HttpResponse -Request $_
    ///                 }
    ///         } finally {
    ///             Get-HttpListener | Stop-HttpListener
    ///         }
    ///     }
    ///     </code>
    /// </example>
    /// <example>
    ///     <para>Denies Single Response</para>
    ///     <code>
    ///     Start-Job -Name "single response" -ScriptBlock {
    ///         try {
    ///             New-HttpListener $uri |
    ///                 Start-HttpListener |
    ///                 Wait-HttpRequest -Count 1 |
    ///                 ForEach-Object {
    ///                     $request = $_ | Receive-HttpRequest | ConvertFrom-Json
    ///                     Deny-HttpResponse -Request $_
    ///                 }
    ///         } finally {
    ///             Get-HttpListener | Stop-HttpListener
    ///         }
    ///     }
    ///     </code>
    /// </example>
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
