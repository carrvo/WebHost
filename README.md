# PowerShell-REST
For writing a REST API in PowerShell.

This is better suited for prototyping and troubleshooting an API and is **not** recommeded for production code.

DISCLAIMER: The security is left up to the scripter writing the API *and not this library*.
The only security this library provides is calling `$connection.Request.IsAuthenticated` when not passed `Anonymous`.

NOTE: This library will make it tempting to write REST*-like* over REST*ful* APIs. See "API Styles" for more information.

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
    Wait-HttpRequest -Count 1 -PipelineVariable $connection | # specify how many requests or infinite
    ForEach-Object {
        $request = $connection | Receive-HttpRequestBody | ConvertFrom-Json
        # all your business logic
        # including constructing an object or hashtable to send the client
        $response = @{Message="Hello $($request.Name)"}
        # send the object or hashtable to the client
        $response | ConvertTo-Json | Submit-HttpResponse -Request $connection
        # or drop the connection
        #Deny-HttpResponse -Request $connection
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
- [API Styles](https://api-university.com/blog/styles-for-apis-soap-rest-and-rpc/)
