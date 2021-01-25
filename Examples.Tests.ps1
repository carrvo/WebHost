<#
.NOTES
Inspired by https://www.powershellgallery.com/packages/HttpListener/1.0.2/Content/HTTPListener.psm1
#>
Describe "Http Listener" {
    It "submits a single response" {
        New-HttpListener "http://localhost/" |
            Start-HttpListener |
            Wait-HttpRequest -Count 1 |
            ForEach-Object {
                $request = $_ | Receive-HttpRequest | ConvertFrom-Json
                @{Message="Hello $($request.Name)"} |
                    ConvertTo-Json | Submit-HttpResponse -Request $_
            } |
            Stop-HttpListener
    }
    It "submits indefinite responses" {
        New-HttpListener "http://localhost/" |
            Start-HttpListener |
            Wait-HttpRequest -Infinity |
            ForEach-Object {
                $request = $_ | Receive-HttpRequest | ConvertFrom-Json
                @{Message="Hello $($request.Name)"} |
                    ConvertTo-Json | Submit-HttpResponse -Request $_
            } |
            Stop-HttpListener
    }
    It "denies a single response" {
        New-HttpListener "http://localhost/" |
            Start-HttpListener |
            Wait-HttpRequest -Count 1 |
            ForEach-Object {
                $request = $_ | Receive-HttpRequest | ConvertFrom-Json
                Deny-HttpResponse -Request $_
            } |
            Stop-HttpListener
    }
}
