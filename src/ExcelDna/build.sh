#!/bin/bash
# Build script for Feature Engine Excel-DNA Add-In

set -e

echo "========================================="
echo "Building Feature Engine Excel-DNA Add-In"
echo "========================================="
echo ""

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed."
    echo "Please install .NET 6.0 SDK or later from https://dotnet.microsoft.com/download"
    exit 1
fi

# Display .NET version
echo "✓ .NET SDK found:"
dotnet --version
echo ""

# Navigate to project directory
cd "$(dirname "$0")"

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore
echo ""

# Build the project
echo "Building project..."
dotnet build --configuration Release --no-restore
echo ""

# Check if build was successful
if [ -f "bin/Release/net6.0/PipeTradesFeatureEngine.dll" ]; then
    echo "========================================="
    echo "✓ Build successful!"
    echo "========================================="
    echo ""
    echo "Output files:"
    ls -lh bin/Release/net6.0/*.dll
    ls -lh bin/Release/net6.0/*.xll 2>/dev/null || echo "(Excel add-in .xll will be generated on Windows)"
    echo ""
    echo "Next steps:"
    echo "1. Copy the .xll file to a permanent location"
    echo "2. In Excel: File → Options → Add-ins → Excel Add-ins → Browse"
    echo "3. Select the .xll file and click OK"
    echo "4. The Feature Engine functions will be available in Excel"
else
    echo "========================================="
    echo "✗ Build failed"
    echo "========================================="
    exit 1
fi
