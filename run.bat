@echo off

@REM Test application

set PLAYLIST="https://youtube.com/playlist?list=PLBhdBDUsTb7QWR59cy44ulQVO8d6Q3d-7&si=Ey9H3jIRoPQbHn79"

dotnet run %PLAYLIST% -format "mp3" -hideprogress

pause
