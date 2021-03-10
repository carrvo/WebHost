using System;
using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Sends the response to the client and releases the resources held by this <see cref="HttpListenerResponse"/> instance.</para>
    /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistenerresponse.close?view=net-5.0#System_Net_HttpListenerResponse_Close">Citation.</para>
    /// <para type="description">This should *only* be called once.</para>
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
    /// </summary>
    [Cmdlet(VerbsLifecycle.Submit, "HttpResponse")]
    public sealed class SubmitHttpResponseCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">The body for the <see cref="HttpListenerResponse"/>.</para>
        /// <para type="description">This is expected to be piped from the <code>ConvertTo-Json</code> <see cref="Cmdlet"/>.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public String Json { get; set; }

        /// <summary>
        /// <para type="description">The <see cref="HttpListenerContext"/> that corresponds to the client request.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [Alias("Request")]
        public HttpListenerContext Context { get; set; }

        protected override void ProcessRecord()
        {
            Context.Response.StatusCode = 200;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Json);
            Context.Response.ContentLength64 = buffer.Length;
            Context.Response.OutputStream.Write(buffer,0,buffer.Length);
            Context.Response.OutputStream.Close();
            Context.Response.Close();
        }
    }
}
