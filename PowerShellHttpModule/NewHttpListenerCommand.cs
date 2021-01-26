using System;
using System.Collections.Generic;
using System.Net;
using System.Management.Automation;
using System.Linq;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Creates a new <see cref="HttpListener"/> object.</para>
    /// <para type="description"><see cref="HttpListener"/> objects are used to listen for client requests.</para>
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
    [Cmdlet(VerbsCommon.New, "HttpListener")]
    [OutputType(typeof(HttpListener))]
    public sealed class NewHttpListenerCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">URI prefixes that the <see cref="HttpListener"/> will listen for.</para>
        /// <para type="description">This specifies a root URI whereby the <see cref="HttpListener"/> will respond to any URI under its path.</para>
        /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener.prefixes?view=net-5.0#System_Net_HttpListener_Prefixes">Citation.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public String UriPrefix { get; set; }

        /// <summary>
        /// <para type="description">Declare how a user can Authenticate.</para>
        /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener.authenticationschemes?view=net-5.0#System_Net_HttpListener_AuthenticationSchemes">Citation.</para>
        /// </summary>
        [Parameter()]
        public IEnumerable<AuthenticationSchemes> AuthenticationSchemes { get; set; }

        private HttpListener Output { get; set; }

        protected override void BeginProcessing()
        {
            if (!HttpListener.IsSupported)
            {
                ThrowTerminatingError(new ErrorRecord(
                    new Exception(),
                    "OS Not Supported",
                    ErrorCategory.NotSpecified,
                    null));
            }
            Output = new HttpListener();
            if (0 != AuthenticationSchemes.Count())
            {
                foreach (var scheme in AuthenticationSchemes)
                {
                    Output.AuthenticationSchemes = Output.AuthenticationSchemes | scheme;
                }
            }
        }

        protected override void ProcessRecord()
        {
            if (!UriPrefix.EndsWith("/"))
            {
                UriPrefix += "/";
            }
            Output.Prefixes.Add(UriPrefix);
        }

        protected override void EndProcessing()
        {
            GetHttpListenerCommand.Listeners.Add(Output);
            WriteObject(Output);
        }
    }
}
