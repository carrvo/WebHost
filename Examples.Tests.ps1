Describe "Http Listener" {
    BeforeAll {
        $uri = 'http://localhost/api'
        $cred = Get-Credential -Message "For PowerShell-REST" -UserName "$ENV:COMPUTERNAME\$ENV:USERNAME"
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
                    Wait-HttpRequest -Count 1 -PipelineVariable context |
                    ForEach-Object {
                        $request = $context | Receive-HttpRequestBody | ConvertFrom-Json
                        @{Message="Hello $($request.Name)"} |
                            ConvertTo-Json | Submit-HttpResponse -Request $context
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Start-Sleep -Seconds 5 # let the job start listening
        Invoke-RestMethod -Method Post -Uri "$uri/single-accept" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "single response" | Stop-Job | Remove-Job
    }
    It "submits indefinite responses" {
        Start-Job -Name "indefinite responses" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellHttpModule.psd1
            try {
                "$uri/indefinite-accept" |
                New-HttpListener -AuthenticationSchemes Basic |
                    Start-HttpListener |
                    Wait-HttpRequest -Infinity -PipelineVariable context |
                    ForEach-Object {
                        $request = $context | Receive-HttpRequestBody | ConvertFrom-Json
                        @{Message="Hello $($request.Name)"} |
                            ConvertTo-Json | Submit-HttpResponse -Request $context
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Start-Sleep -Seconds 5 # let the job start listening
        Invoke-RestMethod -Method Post -Uri "$uri/indefinite-accept" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Invoke-RestMethod -Method Post -Uri "$uri/indefinite-accept" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "indefinite responses" | Stop-Job | Remove-Job
    }
    It "denies a single response" {
        Start-Job -Name "deny response" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellHttpModule.psd1
            try {
                "$uri/single-deny" |
                New-HttpListener -AuthenticationSchemes Basic |
                    Start-HttpListener |
                    Wait-HttpRequest -Count 1 -PipelineVariable context |
                    ForEach-Object {
                        $request = $context | Receive-HttpRequestBody | ConvertFrom-Json
                        Deny-HttpResponse -Request $context
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Start-Sleep -Seconds 5 # let the job start listening
        { Invoke-RestMethod -Method Post -Uri "$uri/single-deny" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication -ErrorAction Stop } |
            Should Throw
        Get-Job -Name "deny response" | Stop-Job | Remove-Job
    }
    It "supports Anonymous" {
        Start-Job -Name "Anonymous response" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellHttpModule.psd1
            try {
                "$uri/anonymous-accept" |
                New-HttpListener -AuthenticationSchemes Anonymous |
                    Start-HttpListener |
                    Wait-HttpRequest -Count 1 -PipelineVariable context |
                    ForEach-Object {
                        $request = $context | Receive-HttpRequestBody | ConvertFrom-Json
                        @{Message="Hello $($request.Name)"} |
                            ConvertTo-Json | Submit-HttpResponse -Request $context
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Start-Sleep -Seconds 5 # let the job start listening
        Invoke-RestMethod -Method Post -Uri "$uri/anonymous-accept" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication None |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "Anonymous response" | Stop-Job | Remove-Job
    }
    It "does not require a body" {
        Start-Job -Name "no body" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellHttpModule.psd1
            try {
                "$uri/no-body" |
                New-HttpListener -AuthenticationSchemes Basic |
                    Start-HttpListener |
                    Wait-HttpRequest -Count 1 -PipelineVariable context |
                    ForEach-Object {
                        $name = $context.Request.QueryString.Get('Name')
                        @{Message="Hello $($name)"} |
                            ConvertTo-Json | Submit-HttpResponse -Request $context
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Start-Sleep -Seconds 5 # let the job start listening
        Invoke-RestMethod -Method Get -Uri "$uri/no-body?Name=test" -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "no body" | Stop-Job | Remove-Job
    }
    It "handles multiple endpoints" {
        Start-Job -Name "multiple endpoints" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellHttpModule.psd1
            try {
                "$uri/multiple-endpoints/" |
                New-HttpListener -AuthenticationSchemes Basic |
                    Start-HttpListener |
                    Wait-HttpRequest -Count 3 -PipelineVariable context |
                    ForEach-Object {
                        $request = $context | Receive-HttpRequestBody | ConvertFrom-Json
                        switch ($_.Request.Url.AbsolutePath) {
                            '/api/multiple-endpoints/hello' {
                                Write-Verbose -Message "Saying hello to $($request.Name)"
                                @{Message="Hello $($request.Name)"} |
                                    ConvertTo-Json | Submit-HttpResponse -Request $context
                            }
                            '/api/multiple-endpoints/goodbye' {
                                Write-Verbose -Message "Saying goodbye to $($request.Name)"
                                @{Message="Goodbye $($request.Name)"} |
                                    ConvertTo-Json | Submit-HttpResponse -Request $context
                            }
                            default {
                                Write-Verbose -Message "Denying $($request.Name)"
                                Deny-HttpResponse -Request $context
                            }
                        }
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Start-Sleep -Seconds 5 # let the job start listening
        Invoke-RestMethod -Method Post -Uri "$uri/multiple-endpoints/hello" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Invoke-RestMethod -Method Post -Uri "$uri/multiple-endpoints/goodbye" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication |
            Select-Object -ExpandProperty Message |
            Should Be 'Goodbye test'
        { Invoke-RestMethod -Method Post -Uri "$uri/multiple-endpoints/not-valid" -Body $(@{Name='test'} | ConvertTo-Json) -ContentType 'application/json' -Authentication Basic -Credential $cred -AllowUnencryptedAuthentication -ErrorAction Stop } |
            Should Throw
        Get-Job -Name "multiple endpoints" | Stop-Job | Remove-Job
    }
}
