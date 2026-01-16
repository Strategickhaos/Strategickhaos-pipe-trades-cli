@echo off
REM Build script for Feature Engine Excel-DNA Add-In (Windows)

echo =========================================
echo Building Feature Engine Excel-DNA Add-In
echo =========================================
echo.

REM Check if .NET SDK is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Error: .NET SDK is not installed.
    echo Please install .NET 6.0 SDK or later from https://dotnet.microsoft.com/download
    exit /b 1
)

REM Display .NET version
echo. ✓ .NET SDK found:
dotnet --version
echo.

REM Navigate to script directory
cd /d "%~dp0"

REM Restore dependencies
echo Restoring dependencies...
dotnet restore
echo.

REM Build the project
echo Building project...
dotnet build --configuration Release --no-restore
echo.

REM Check if build was successful
if exist "bin\Release\net6.0\PipeTradesFeatureEngine.dll" (
    echo =========================================
    echo ✓ Build successful!
    echo =========================================
    echo.
    echo Output files:
    dir /b bin\Release\net6.0\*.dll
    dir /b bin\Release\net6.0\*.xll 2>nul
    echo.
    echo Next steps:
    echo 1. Find the .xll file in bin\Release\net6.0\
    echo 2. Copy it to a permanent location
    echo 3. In Excel: File → Options → Add-ins → Excel Add-ins → Browse
    echo 4. Select the .xll file and click OK
    echo 5. The Feature Engine functions will be available in Excel
) else (
    echo =========================================
    echo ✗ Build failed
    echo =========================================
    exit /b 1
)
