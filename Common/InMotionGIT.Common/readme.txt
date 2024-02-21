##Instalacion de dotnet-svcutil
dotnet tool install --global dotnet-svcutil
dotnet-svcutil https://www.inmotiontools.com:8083/FrontOffice/DataManager.svc?wsdl -d "/Services"  -o DataManager -n "*,InMotionGIT.Common.Proxy" /reference:InMotionGIT.Common.dll

