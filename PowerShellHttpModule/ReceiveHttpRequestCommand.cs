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
    /// </summary>
    [Cmdlet(VerbsCommunications.Receive, "HttpResponse")]
    [OutputType(typeof(String))]
    public sealed class ReceiveHttpResponseCommand : Cmdlet
    {
        /// <summary>
        /// <para type="description">The <see cref="HttpListenerContext"/> that corresponds to the client request.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [Alias("Request")]
        public HttpListenerContext Context { get; set; }

        protected override void ProcessRecord()
        {
            if (Context.Request.HasEntityBody)
            {
                Stream body = Context.Request.InputStream;
                Encoding encoding = Context.Request.ContentEncoding;
                StreamReader reader = new StreamReader(Context.Request.InputStream, encoding);
                if (Context.Request.ContentType == null)
                {
                    WriteWarning("No ContentType");
                }
                String request = reader.ReadToEnd();
                WriteObject(request);
                Context.Request.InputStream.Close();
                reader.Close();
            }
            else
            {
                WriteError(new ErrorRecord(
                    new WebException(),
                    "No Request Body",
                    ErrorCategory.InvalidData,
                    Context));
                WriteObject(null);
            }
        }
    }
}
