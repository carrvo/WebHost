using System;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Starts listening for client requests.</para>
    /// <para type="description">Allows this instance to receive incoming requests.</para>
    /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener.start?view=net-5.0#System_Net_HttpListener_Start">Citation.</para>
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
    ///     $cred = Get-Credential -Message "For PowerShell-REST" -UserName "$ENV:COMPUTERNAME\$ENV:USERNAME" -Title "PowerShell-REST"
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
    ///     $cred = Get-Credential -Message "For PowerShell-REST" -UserName "$ENV:COMPUTERNAME\$ENV:USERNAME" -Title "PowerShell-REST"
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
    ///     $cred = Get-Credential -Message "For PowerShell-REST" -UserName "$ENV:COMPUTERNAME\$ENV:USERNAME" -Title "PowerShell-REST"
    ///     Invoke-RestMethod -Method Post -Uri 'http://localhost/api' -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication
    ///     </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "HttpListener")]
    [OutputType(typeof(HttpListener))]
    public sealed class StartHttpListenerCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">Used to listen for client requests.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public HttpListener Listener { get; set; }

        protected override void ProcessRecord()
        {
            WriteVerbose($"{nameof(StartHttpListenerCommand)} - {nameof(ProcessRecord)} - checking {nameof(HttpListener.IsListening)}");
            if (!Listener.IsListening)
            {
                try
                {
                    WriteVerbose($"{nameof(StartHttpListenerCommand)} - {nameof(ProcessRecord)} - {nameof(HttpListener.Start)}");
                    Listener.Start();
                }
                catch (Exception ex)
                {
                    ThrowTerminatingError(new ErrorRecord(
                        ex,
                        $"Failed to start a {nameof(HttpListener)}",
                        ErrorCategory.AuthenticationError,
                        Listener));
                }
            }
            WriteVerbose($"{nameof(StartHttpListenerCommand)} - {nameof(ProcessRecord)} - pass along");
            WriteObject(Listener);
            WriteVerbose($"{nameof(StartHttpListenerCommand)} - {nameof(ProcessRecord)} - end");
        }
    }
}
