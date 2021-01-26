Describe "Http Listener" {
    BeforeAll {
        Import-Module $PSScriptRoot\bin\netstandard2.0\PowerShellHttpModule.dll
        $uri = 'http://localhost/'
    }
    It "submits a single response" {
        Start-Job -Name "single response" -ScriptBlock {
            try {
                New-HttpListener $uri |
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
        }
        Invoke-RestMethod -Method Get -Uri $uri -Body @{Name='test'} | Select-Object -ExpandProperty Message | Should -Be 'Hello test'
        Get-Job -Name "single response" | Stop-Job | Remove-Job
    }
    It "submits indefinite responses" {
        Start-Job -Name "indefinte responses" -ScriptBlock {
            try {
                New-HttpListener $uri |
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
        }
        Invoke-RestMethod -Method Get -Uri $uri -Body @{Name='test'} | Select-Object -ExpandProperty Message | Should -Be 'Hello test'
        Invoke-RestMethod -Method Get -Uri $uri -Body @{Name='test'} | Select-Object -ExpandProperty Message | Should -Be 'Hello test'
        Get-Job -Name "indefinite responses" | Stop-Job | Remove-Job
    }
    It "denies a single response" {
        Start-Job -Name "single response" -ScriptBlock {
            try {
                New-HttpListener $uri |
                    Start-HttpListener |
                    Wait-HttpRequest -Count 1 |
                    ForEach-Object {
                        $request = $_ | Receive-HttpRequest | ConvertFrom-Json
                        Deny-HttpResponse -Request $_
                    }
            } finally {
                Get-HttpListener | Stop-HttpListener
            }
        }
        { Invoke-RestMethod -Method Get -Uri $uri -Body @{Name='test'} } | Should -Throw
        Get-Job -Name "single response" | Stop-Job | Remove-Job
    }
}
