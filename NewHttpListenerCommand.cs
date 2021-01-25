using System.Collections.Generics;
using System.Net;
using System.Management.Automation;

namespace PowerShell.REST
{
    [Cmdlet(VerbsCommon.New, "HttpListener")]
    [OutputType(HttpListener)]
    public sealed class NewHttpListenerCommand : Cmdlet
    {
        [Parameter(Mandatory, ValueFromPipeline)]
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
            if (0 != AuthenticationSchemes.Count)
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
            WriteOutput(Output);
        }
    }
}
