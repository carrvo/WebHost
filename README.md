# PowerShell-REST
For writing a REST API in PowerShell

# Quick Start

## Installation

## First API

```powershell
try {
    'http://localhost/api' |
    New-HttpListener -AuthenticationSchemes Basic |
    Start-HttpListener |
    Wait-HttpRequest -Count 1 | # specify how many requests or infinite
    ForEach-Object {
        $request = $_ | Receive-HttpRequest | ConvertFrom-Json
        # all your business logic
        # including constructing an object or hashtable to send the client
        $response = @{Message="Hello $($request.Name)"}
        # send the object or hashtable to the client
        $response | ConvertTo-Json | Submit-HttpResponse -Request $_
        # or drop the connection
        #Deny-HttpResponse -Request $_
    }
} finally {
    Get-HttpListener | Stop-HttpListener
}
```

## Calling the API

```powershell
$cred = Get-Credential -Message "For PowerShell-REST" -UserName "$ENV:COMPUTERNAME\$ENV:USERNAME" -Title "PowerShell-REST"
Invoke-RestMethod -Method Post -Uri 'http://localhost/api' -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication
```

# References
- Inspired by https://www.powershellgallery.com/packages/HttpListener/1.0.2/Content/HTTPListener.psm1
