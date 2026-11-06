@echo off
cd /d "%~dp0"
echo KutubxonaAt ishga tushmoqda... Brauzer: http://localhost:5030
echo Admin: admin@kutubxona.uz  /  Admin123!
dotnet run --launch-profile http
pause
