Describe "Http Listener" {
    BeforeAll {
        $uri = 'http://localhost/api'
        $cred = Get-Credential -Message "For PowerShell-REST" -UserName "$ENV:COMPUTERNAME\$ENV:USERNAME" -Title "PowerShell-REST"
    }
    It "submits a single response" {
        Start-Job -Name "single response" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellHttpModule.psd1
            try {
                "$uri/single-accept" |
                New-HttpListener -AuthenticationSchemes Basic |
                    Start-HttpListener |
                    Wait-HttpRequest -Count 1 |
                    ForEach-Object {
                        $request = $_ | Receive-HttpRequest | ConvertFrom-Json
                        @{Message="Hello $($request.Name)"} |
                            ConvertTo-Json | Submit-HttpResponse -Request $_
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Invoke-RestMethod -Method Post -Uri "$uri/single-accept" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "single response" | Stop-Job | Remove-Job
    }
    It "submits indefinite responses" {
        Start-Job -Name "indefinte responses" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellHttpModule.psd1
            try {
                "$uri/indefinite-accept" |
                New-HttpListener -AuthenticationSchemes Basic |
                    Start-HttpListener |
                    Wait-HttpRequest -Infinity |
                    ForEach-Object {
                        $request = $_ | Receive-HttpRequest | ConvertFrom-Json
                        @{Message="Hello $($request.Name)"} |
                            ConvertTo-Json | Submit-HttpResponse -Request $_
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Invoke-RestMethod -Method Post -Uri "$uri/indefinite-accept" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Invoke-RestMethod -Method Post -Uri "$uri/indefinite-accept" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "indefinite responses" | Stop-Job | Remove-Job
    }
    It "denies a single response" {
        Start-Job -Name "single response" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellHttpModule.psd1
            try {
                "$uri/single-deny" |
                New-HttpListener -AuthenticationSchemes Basic |
                    Start-HttpListener |
                    Wait-HttpRequest -Count 1 |
                    ForEach-Object {
                        $request = $_ | Receive-HttpRequest | ConvertFrom-Json
                        Deny-HttpResponse -Request $_
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        { Invoke-RestMethod -Method Post -Uri "$uri/single-deny" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication } |
            Should Throw
        Get-Job -Name "single response" | Stop-Job | Remove-Job
    }
}
