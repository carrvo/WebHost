using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Retrieves the body of the <see cref="HttpListenerRequest"/>.</para>
    /// <para type="description">
    /// The body of the <see cref="HttpListenerRequest"/>, if it is JSON, is expected to be
    /// piped to the <code>ConvertFrom-Json</code> <see cref="Cmdlet"/>.
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
    [Cmdlet(VerbsCommunications.Receive, "HttpRequest")]
    [OutputType(typeof(String))]
    public sealed class ReceiveHttpRequestCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">The <see cref="HttpListenerContext"/> that corresponds to the client request.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [Alias("Request")]
        public HttpListenerContext Context { get; set; }

        protected override void ProcessRecord()
        {
            WriteVerbose($"{nameof(ReceiveHttpRequestCommand)} - {nameof(ProcessRecord)} - checking {nameof(HttpListenerRequest.HasEntityBody)}");
            if (Context.Request != null && Context.Request.HasEntityBody)
            {
                WriteVerbose($"{nameof(ReceiveHttpRequestCommand)} - {nameof(ProcessRecord)} - getting {nameof(HttpListenerRequest.InputStream)}");
                Stream body = Context.Request.InputStream;
                WriteVerbose($"{nameof(ReceiveHttpRequestCommand)} - {nameof(ProcessRecord)} - getting {nameof(HttpListenerRequest.ContentEncoding)}");
                Encoding encoding = Context.Request.ContentEncoding;
                WriteVerbose($"{nameof(ReceiveHttpRequestCommand)} - {nameof(ProcessRecord)} - getting {nameof(StreamReader)}");
                StreamReader reader = new StreamReader(Context.Request.InputStream, encoding);
                WriteVerbose($"{nameof(ReceiveHttpRequestCommand)} - {nameof(ProcessRecord)} - checking {nameof(HttpListenerRequest.ContentType)}");
                if (Context.Request.ContentType == null)
                {
                    WriteWarning("No ContentType");
                }
                WriteVerbose($"{nameof(ReceiveHttpRequestCommand)} - {nameof(ProcessRecord)} - reading body content");
                String request = reader.ReadToEnd();
                WriteVerbose($"{nameof(ReceiveHttpRequestCommand)} - {nameof(ProcessRecord)} - output");
                WriteObject(request);
                WriteVerbose($"{nameof(ReceiveHttpRequestCommand)} - {nameof(ProcessRecord)} - closing {nameof(HttpListenerRequest.InputStream)}");
                Context.Request.InputStream.Close();
                WriteVerbose($"{nameof(ReceiveHttpRequestCommand)} - {nameof(ProcessRecord)} - closing {nameof(StreamReader)}");
                reader.Close();
            }
            else
            {
                WriteError(new ErrorRecord(
                    new WebException("No Request Body"),
                    "No Request Body",
                    ErrorCategory.InvalidData,
                    Context));
                WriteObject(null);
            }
        }
    }
}
