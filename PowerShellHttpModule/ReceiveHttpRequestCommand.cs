using System;
using System.Collections.Generics;
using System.Net;
using System.IO;
using System.Text;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsCommunications.Receive, "HttpResponse")]
    [OutputType(String)]
    public sealed class ReceiveHttpResponseCommand : Cmdlet
    {
        [Parameter(Mandatory, ValueFromPipeline)]
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
                    WriteWarning(new WarningRecord("No ContentType", Context));
                }
                String request = reader.ReadToEnd();
                WriteOutput(request);
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
                WriteOutput(null);
            }
        }
    }
}
