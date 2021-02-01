using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Stops listening for client requests.</para>
    /// <para type="description">Causes this instance to stop receiving new incoming requests and terminates processing of all ongoing requests.</para>
    /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener.stop?view=net-5.0#System_Net_HttpListener_Stop">Citation.</para>
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
    [Cmdlet(VerbsLifecycle.Stop, "HttpListener")]
    public sealed class StopHttpListenerCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">Used to listen for client requests.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public HttpListener Listener { get; set; }

        protected override void ProcessRecord()
        {
            WriteVerbose($"{nameof(StopHttpListenerCommand)} - {nameof(ProcessRecord)} - checking {nameof(HttpListener.IsListening)}");
            if (Listener.IsListening)
            {
                WriteVerbose($"{nameof(StopHttpListenerCommand)} - {nameof(ProcessRecord)} - {nameof(HttpListener.Stop)}");
                Listener.Stop();
            }
            WriteVerbose($"{nameof(StopHttpListenerCommand)} - {nameof(ProcessRecord)} - end");
        }
    }
}
