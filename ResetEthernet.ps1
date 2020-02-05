Set-NetConnectionProfile -NetworkCategory Public
Set-NetIPInterface -InterfaceIndex 4 -Dhcp Enabled
Start-Sleep -s 5
Get-NetIPAddress -InterfaceIndex 4 | Remove-NetRoute
netsh interface set interface "Ethernet" DISABLED
netsh interface set interface "Ethernet" ENABLED
Set-ExecutionPolicy Restricted
