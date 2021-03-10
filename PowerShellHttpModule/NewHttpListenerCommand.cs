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
    ///     try {
    ///         'http://localhost/api' |
    ///         New-HttpListener -AuthenticationSchemes Basic |
    ///             Start-HttpListener |
    ///             Wait-HttpRequest -Count 1 |
    ///             ForEach-Object {
    ///                 $request = $_ | Receive-HttpRequest | ConvertFrom-Json
    ///                 @{Message="Hello $($request.Name)"} |
    ///                     ConvertTo-Json | Submit-HttpResponse -Request $_
    ///         }
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
    /// <example>
    ///     <para>Submits Indefinite Responses</para>
    ///     <code>
    ///     try {
    ///         'http://localhost/api' |
    ///         New-HttpListener -AuthenticationSchemes Basic |
    ///             Start-HttpListener |
    ///             Wait-HttpRequest -Infinity |
    ///             ForEach-Object {
    ///                 $request = $_ | Receive-HttpRequest | ConvertFrom-Json
    ///                 @{Message="Hello $($request.Name)"} |
    ///                     ConvertToJson | Submit-HttpResponse -Request $_
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
        public AuthenticationSchemes[] AuthenticationSchemes { get; set; }

        private HttpListener Output { get; set; }

        protected override void BeginProcessing()
        {
            WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(BeginProcessing)} - checking for {nameof(HttpListener.IsSupported)}");
            if (!HttpListener.IsSupported)
            {
                ThrowTerminatingError(new ErrorRecord(
                    new NotSupportedException("OS Not Supported"),
                    "OS Not Supported",
                    ErrorCategory.NotEnabled,
                    null));
            }
            try
            {
                WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(BeginProcessing)} - instantiating {nameof(HttpListener)}");
                Output = new HttpListener();
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(new ErrorRecord(
                    ex,
                    $"Failed to instantiate a {nameof(HttpListener)}",
                    ErrorCategory.NotSpecified,
                    Output));
            }
            WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(BeginProcessing)} - checking {nameof(AuthenticationSchemes)}");
            if (AuthenticationSchemes != null && 0 != AuthenticationSchemes.Count())
            {
                WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(BeginProcessing)} - adding {nameof(HttpListener.AuthenticationSchemes)}");
                Int32 schemes = 0;
                foreach (var scheme in AuthenticationSchemes)
                {
                    schemes |= (Int32)scheme;
                }
                Output.AuthenticationSchemes = (AuthenticationSchemes)schemes;
            }
            WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(BeginProcessing)} - end");
        }

        protected override void ProcessRecord()
        {
            WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(ProcessRecord)} - checking {nameof(UriPrefix)}");
            if (!UriPrefix.EndsWith("/"))
            {
                UriPrefix += "/";
            }
            WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(ProcessRecord)} - adding {nameof(HttpListener.Prefixes)}");
            Output.Prefixes.Add(UriPrefix);
            WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(ProcessRecord)} - end");
        }

        protected override void EndProcessing()
        {
            WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(EndProcessing)} - adding reference");
            GetHttpListenerCommand.Listeners.Add(Output);
            WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(EndProcessing)} - output");
            WriteObject(Output);
            WriteVerbose($"{nameof(NewHttpListenerCommand)} - {nameof(EndProcessing)} - end");
        }
    }
}
