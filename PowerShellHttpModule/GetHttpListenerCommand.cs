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
