using System;
using System.Collections.Generic;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    /// <summary>
    /// <para type="synopsis">Waits for a client request.</para>
    /// <para type="description">This will block while it waits for a client request</para>
    /// <para type="link" uri="https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener.getcontext?view=net-5.0#System_Net_HttpListener_GetContext">Citation.</para>
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
    [Cmdlet(VerbsLifecycle.Wait, "HttpRequest")]
    [OutputType(typeof(HttpListenerContext))]
    public sealed class WaitHttpRequestCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">Used to listen for client requests.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Infinite")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Limited")]
        public HttpListener Listener { get; set; }

        /// <summary>
        /// <para type="description">Number of requests to listen for before exiting the listener loop.</para>
        /// </summary>
        [Parameter(ParameterSetName = "Limited")]
        [Alias("Count")]
        public Int64 NumberOfRequests { get; set; } = 1;

        /// <summary>
        /// <para type="description">Never stop the listener loop.</para>
        /// <para type="description">WARNING: be careful of how you are using this because it results in an infinite blocking loop.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Infinite")]
        [Alias("Infinity")]
        public SwitchParameter InfiniteRequests { get; set; }

        private Func<Boolean> NextRequest { get; set; }

        private void ListenForSingleRequest(HttpListener listener)
        {
            WriteVerbose($"{nameof(WaitHttpRequestCommand)} - {nameof(ProcessRecord)} - checking {nameof(HttpListener.IsListening)}");
            if (listener.IsListening)
            {
                WriteVerbose($"{nameof(WaitHttpRequestCommand)} - {nameof(ProcessRecord)} - waiting for request");
                HttpListenerContext context = listener.GetContext();
                WriteVerbose($"{nameof(WaitHttpRequestCommand)} - {nameof(ProcessRecord)} - checking {nameof(HttpListenerRequest.IsAuthenticated)}");
                if (context.Request.IsAuthenticated)
                {
                    WriteVerbose($"{nameof(WaitHttpRequestCommand)} - {nameof(ProcessRecord)} - output");
                    WriteObject(context);
                }
                else
                {
                    WriteError(new ErrorRecord(
                        new WebException("Unauthorized"),
                        "Unauthorized",
                        ErrorCategory.AuthenticationError,
                        context));
                    context.Response.StatusCode = 403;
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Unauthorized");
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer,0,buffer.Length);
                    context.Response.OutputStream.Close();
                    context.Response.Close();
                }
            }
            else
            {
                ThrowTerminatingError(new ErrorRecord(
                    new WebException("Listener Not Active"),
                    "Listener Not Active",
                    ErrorCategory.ConnectionError,
                    listener));
            }
        }

        protected override void BeginProcessing()
        {
            WriteVerbose($"{nameof(WaitHttpRequestCommand)} - {nameof(BeginProcessing)} - determine {nameof(NumberOfRequests)}");
            if (InfiniteRequests.IsPresent)
            {
                NextRequest = () => true;
            }
            else
            {
                NextRequest = () => 0 == --NumberOfRequests;
            }
            WriteVerbose($"{nameof(WaitHttpRequestCommand)} - {nameof(BeginProcessing)} - end");
        }

        protected override void ProcessRecord()
        {
            WriteVerbose($"{nameof(WaitHttpRequestCommand)} - {nameof(ProcessRecord)} - while {nameof(NextRequest)}({NumberOfRequests})");
            while (NextRequest())
            {
                WriteVerbose($"{nameof(WaitHttpRequestCommand)} - {nameof(ProcessRecord)} - {nameof(NextRequest)}");
                ListenForSingleRequest(Listener);
            }
            WriteVerbose($"{nameof(WaitHttpRequestCommand)} - {nameof(ProcessRecord)} - end");
        }
    }
}
