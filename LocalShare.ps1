Set-ExecutionPolicy Unrestricted
Write-Output "Krzysztof Sobocinski id: 1";
Write-Output "Pawel Szynal id: 2";

$id = Read-Host -Prompt 'Get id'

if ($id -eq 1) {

    Write-Output "Set for Krzysztof Sobocinski";
    New-NetIPAddress -InterfaceAlias 'Ethernet' -IPAddress 192.168.1.3 -PrefixLength 24 -DefaultGateway 192.168.1.1
    Start-Sleep -s 7
    Set-NetConnectionProfile -NetworkCategory Private
    Write-Output "Done!";
} elseif ($id -eq 2) {

    Write-Output "Set for Pawel Szynal";
    New-NetIPAddress -InterfaceAlias 'Ethernet' -IPAddress 192.168.1.2 -PrefixLength 24 -DefaultGateway 192.168.1.1
    Start-Sleep -s 7
    Set-NetConnectionProfile -NetworkCategory Private
    Write-Output "Done!";
}else{

	Write-Warning -Message "Wrong input."
}
