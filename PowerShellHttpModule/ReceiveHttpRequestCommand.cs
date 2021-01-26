using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsCommunications.Receive, "HttpResponse")]
    [OutputType(typeof(String))]
    public sealed class ReceiveHttpResponseCommand : Cmdlet
    {
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
