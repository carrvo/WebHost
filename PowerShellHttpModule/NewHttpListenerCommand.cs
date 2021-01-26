using System;
using System.Collections.Generic;
using System.Net;
using System.Management.Automation;
using System.Linq;

namespace PowerShell.REST
{
    [Cmdlet(VerbsCommon.New, "HttpListener")]
    [OutputType(typeof(HttpListener))]
    public sealed class NewHttpListenerCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public String UriPrefix { get; set; }

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
