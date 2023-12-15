Describe "UDP Listener" {
	BeforeAll {
		Import-Module $PSScriptRoot\PowerShellUDPModule.psd1
	}
    It "receives a single request" {
        Start-Job -Name "single request" -ScriptBlock {
            Param(
                $PSScriptRoot
            )
            Import-Module $PSScriptRoot\PowerShellUDPModule.psd1
            try {
                New-UdpListener -Port 11000 |
                    Wait-UdpRequest -Count 1 -PipelineVariable context |
                    ForEach-Object {
                        $request = $context.UTF8Payload | ConvertFrom-Json
                        ([pscustomobject]@{Message="Hello $($request.Name)"}) | Write-Output -NoEnumerate
                    }
            } finally {
                Get-UdpListener | Stop-UdpListener
            }
        } -ArgumentList $PSScriptRoot
        Start-Sleep -Seconds 5 # let the job start listening
        Send-UdpRequest -Port 11000 -Body $(@{Name='test'} | ConvertTo-Json)
        Start-Sleep -Seconds 1 # let the job output
		[pscustomobject]$reply = Get-Job |
			Receive-Job
		$reply |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "single request" | Stop-Job -PassThru | Remove-Job
    }
    It "receives indefinite requests" -Skip { # cannot remove job
        Start-Job -Name "indefinite requests" -ScriptBlock {
            Param(
                $PSScriptRoot
            )
            Import-Module $PSScriptRoot\PowerShellUDPModule.psd1
            try {
                New-UdpListener -Port 11001 |
                    Wait-UdpRequest -Infinity -PipelineVariable context |
                    ForEach-Object {
                        $request = $context.UTF8Payload | ConvertFrom-Json
                        ([pscustomobject]@{Message="Hello $($request.Name)"}) | Write-Output -NoEnumerate
                    }
            } finally {
                Get-UdpListener | Stop-UdpListener
            }
        } -ArgumentList $PSScriptRoot
        Start-Sleep -Seconds 5 # let the job start listening
        Send-UdpRequest -Port 11001 -Body $(@{Name='test'} | ConvertTo-Json)
        Start-Sleep -Seconds 1 # let the job output
		[pscustomobject]$reply = Get-Job |
			Receive-Job
		$reply |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Send-UdpRequest -Port 11001 -Body $(@{Name='test'} | ConvertTo-Json)
        Start-Sleep -Seconds 1 # let the job output
		[pscustomobject]$reply = Get-Job |
			Receive-Job
		$reply |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "indefinite requests" | Remove-Job -Force
    }
    It "does not require a body" {
        Start-Job -Name "no body" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellUDPModule.psd1
            try {
                New-UdpListener -Port 11002 |
                    Wait-UdpRequest -Count 1 -PipelineVariable context |
                    ForEach-Object {
                        $request = $context.UTF8Payload | ConvertFrom-Json
                        ([pscustomobject]@{Message="Hello $($request.Name)"}) | Write-Output -NoEnumerate
                    }
            } finally {
                Get-UdpListener | Stop-UdpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Start-Sleep -Seconds 5 # let the job start listening
        Send-UdpRequest -Port 11002 -Body $(@{Name='test'} | ConvertTo-Json)
        Start-Sleep -Seconds 1 # let the job output
		[pscustomobject]$reply = Get-Job |
			Receive-Job
		$reply |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "no body" | Stop-Job -PassThru | Remove-Job
    }
    It "can respond to a request" {
        Start-Job -Name "response" -ScriptBlock {
            Param(
                $PSScriptRoot,
                $uri
            )
            Import-Module $PSScriptRoot\PowerShellUDPModule.psd1
            try {
                New-UdpListener -Port 11003 |
                    Wait-UdpRequest -Count 1 -PipelineVariable context |
                    ForEach-Object {
                        $request = $context.UTF8Payload | ConvertFrom-Json
                        @{Message="Hello $($request.Name)"} |
                            ConvertTo-Json | Send-UdpRequest -ServerIpEndPoint $context.Client
                    }
            } finally {
                Get-UdpListener | Stop-UdpListener
            }
        } -ArgumentList $PSScriptRoot,$uri
        Start-Sleep -Seconds 5 # let the job start listening
        Send-UdpRequest -Port 11003 -Body $(@{Name='test'} | ConvertTo-Json) |
			Wait-UdpRequest -Count 1 |
			Select-Object -ExpandProperty UTF8Payload |
			ConvertFrom-Json |
            Select-Object -ExpandProperty Message |
            Should Be 'Hello test'
        Get-Job -Name "response" | Stop-Job -PassThru | Remove-Job
    }
}
