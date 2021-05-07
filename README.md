# PowerShell-REST
For writing a REST API in PowerShell

# Quick Start

## Installation

The package can be found on [PSGallery](https://www.powershellgallery.com/packages/PowerShellHttpModule/).

```powershell
Install-Module -Name PowerShellHttpModule
```

## First API

```powershell
try {
    'http://localhost/api' |
    New-HttpListener -AuthenticationSchemes Basic |
    Start-HttpListener |
    Wait-HttpRequest -Count 1 | # specify how many requests or infinite
    ForEach-Object {
        $request = $_ | Receive-HttpRequestBody | ConvertFrom-Json
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
$cred = Get-Credential -Message "For PowerShell-REST" -UserName "$ENV:COMPUTERNAME\$ENV:USERNAME"
Invoke-RestMethod -Method Post -Uri 'http://localhost/api' -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication
```

## Troubleshooting Your API

Call your business logic interactively:
```powershell
$listener = 'http://localhost/api' |
    New-HttpListener -AuthenticationSchemes Basic |
    Start-HttpListener
$connection = $listener | Wait-HttpRequest -Count 1 # only want the one you are debugging
# call the API while this console waits
$request = $connection | Receive-HttpRequest | ConvertFrom-Json
# business logic / troubleshooting here
$response = @{Message="Hello $($request.Name)"}
$response | ConvertTo-Json | Submit-HttpResponse -Request $connection
$listener | Stop-HttpListener
```

# References
- Inspired by https://www.powershellgallery.com/packages/HttpListener/1.0.2/Content/HTTPListener.psm1
