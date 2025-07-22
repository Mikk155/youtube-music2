@echo off

dotnet publish youtube-music2.csproj -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true

pause
