Set-NetConnectionProfile -NetworkCategory Public
Set-NetIPInterface -InterfaceAlias 'Ethernet' -Dhcp Enabled
Start-Sleep -s 5
Get-NetIPAddress -InterfaceAlias 'Ethernet' | Remove-NetRoute
netsh interface set interface "Ethernet" DISABLED
netsh interface set interface "Ethernet" ENABLED
Set-ExecutionPolicy Restricted
