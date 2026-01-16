# Feature Engine - Quick Start Examples

## Installation Note

**Important**: This Excel-DNA add-in requires Windows and Microsoft Excel. The functions will only work when the `.xll` file is loaded into Excel on a Windows machine.

## Building on Windows

```powershell
cd src\ExcelDna
.\build.bat
```

Or using .NET CLI:
```powershell
dotnet restore
dotnet build --configuration Release
```

The build will generate:
- `bin\Release\net6.0-windows\PipeTradesFeatureEngine64.xll` (64-bit Excel)
- `bin\Release\net6.0-windows\PipeTradesFeatureEngine.xll` (32-bit Excel)

## Installation in Excel

1. Copy the appropriate `.xll` file to a permanent location (e.g., `C:\ExcelAddIns\`)
2. In Excel, go to **File** → **Options** → **Add-ins**
3. At the bottom, select **Excel Add-ins** from the dropdown and click **Go...**
4. Click **Browse...** and navigate to your `.xll` file
5. Check the box next to **Pipe Trades Feature Engine** and click **OK**

## Example Workbook Structure

Once installed, create a workbook with the following structure:

### Sheet: Feature Lab

| A | B | C | D | E | F | G | H |
|---|---|---|---|---|---|---|---|
| **Raw Input** | **Wavelength (mm)** | **Element** | **Lat** | **Lon** | **Frequency (Hz)** | **Atomic #** | **Distance (km)** |
| Radio wave | 300000 | H | 30.2266 | -93.2174 | =FREQUENCY_HZ(B2) | =ELEMENT_Z(C2) |  |
| Microwave | 10 | He | 30.2366 | -93.3774 | =FREQUENCY_HZ(B3) | =ELEMENT_Z(C3) | =GEO_DISTANCE_KM(D2,E2,D3,E3) |
| Visible light | 0.0005 | C |  |  | =FREQUENCY_HZ(B4) | =ELEMENT_Z(C4) |  |

### Sheet: Text Analysis Lab

| A | B | C | D | E |
|---|---|---|---|---|
| **Text** | **Fallacy** | **Entropy** | **Contradiction** | **Sharpness** |
| Everyone says this is true! | =FALLACY_SCORE(A2) | =ENTROPY_SCORE(A2) | =CONTRADICTION_SCORE(A2) | =SIGNAL_SHARPNESS(A2) |
| The temperature was 72.5°F at 14:23 | =FALLACY_SCORE(A3) | =ENTROPY_SCORE(A3) | =CONTRADICTION_SCORE(A3) | =SIGNAL_SHARPNESS(A3) |
| Maybe it could possibly work | =FALLACY_SCORE(A4) | =ENTROPY_SCORE(A4) | =CONTRADICTION_SCORE(A4) | =SIGNAL_SHARPNESS(A4) |

### Sheet: UTRD Composite

| A | B |
|---|---|
| **Text** | **UTRD Analysis** |
| Everyone knows this is obviously true! | =UTRD(A2) |

The UTRD function returns JSON with all metrics:
```json
{
  "input": "Everyone knows this is obviously true!...",
  "metrics": {
    "fallacy_score": 0.5,
    "entropy_score": 0.68,
    "contradiction_score": 0.0,
    "signal_sharpness": 0.3,
    "composite_score": 0.52
  },
  "timestamp": "2026-01-16T18:30:00.000Z"
}
```

## Function Reference Card

### Physics
```
=FREQUENCY_HZ(300000)
→ 999308.19  (Hz)
```

### Chemistry
```
=ELEMENT_Z("Fe")
→ 26
```

### Geometry
```
=GEO_DISTANCE_KM(30.2266, -93.2174, 30.2366, -93.3774)
→ 14.82  (km between Lake Charles and Sulphur, LA)
```

### Text Analysis
```
=FALLACY_SCORE("Everyone says it's true")
→ 0.25  (0.0 = no fallacies, 1.0 = high fallacy density)

=ENTROPY_SCORE("aaaaaa")
→ 0.15  (low entropy = repetitive)

=CONTRADICTION_SCORE("I love cats. I hate cats.")
→ 0.5  (0.0 = consistent, 1.0 = contradictory)

=SIGNAL_SHARPNESS("Temperature was 72.5°F at 14:23")
→ 0.68  (high = specific/precise, low = vague)
```

### Composite
```
=UTRD("Sample text to analyze")
→ Returns JSON with all metrics
```

## Working with UTRD JSON in Excel

Since UTRD returns JSON, you can:

1. **View directly**: Just display the cell with JSON text
2. **Extract with formulas**: Use `TEXTBEFORE()`, `TEXTAFTER()`, or Power Query
3. **Parse with VBA**: Write a custom function to extract specific metrics

Example VBA to extract composite_score:
```vba
Function ExtractCompositeScore(jsonText As String) As Double
    Dim startPos As Integer
    Dim endPos As Integer
    startPos = InStr(jsonText, """composite_score"": ") + 19
    endPos = InStr(startPos, jsonText, ",")
    If endPos = 0 Then endPos = InStr(startPos, jsonText, "}")
    ExtractCompositeScore = CDbl(Mid(jsonText, startPos, endPos - startPos))
End Function
```

Then use:
```
=ExtractCompositeScore(UTRD(A2))
→ 0.625
```

## Understanding the Row Model

Each row represents one **observation**. Each column represents one **feature**.

The power of this architecture is that you can:
- Add new columns (features) without breaking existing ones
- Compare outputs from different engines
- Detect drift in metrics over time
- Build composite scores from primitive features

This is **instrumentation**, not **interpretation**.

## Next Steps

1. **Start simple**: Test physics and chemistry functions first
2. **Build feature tables**: Create worksheets that transform raw inputs into metrics
3. **Add new engines**: Extend `FeatureEngine.cs` with domain-specific functions
4. **Integrate LLMs**: Replace text analysis stubs with Claude/Ollama API calls
5. **Export data**: Use the feature tables for training ML models

---

## Testing (For Developers)

Unit tests are provided in `src/Tests/` but can only run on Windows with .NET 6.0 runtime installed. To run tests:

```powershell
cd src\Tests
dotnet test
```

On Linux/Mac, the tests cannot run because:
1. Excel-DNA requires Windows
2. The project targets `net6.0-windows` which is Windows-specific

This is expected and by design.

---

## Troubleshooting

### Functions not appearing in Excel

- Verify the add-in is checked in **File** → **Options** → **Add-ins** → **Excel Add-ins**
- Try restarting Excel
- Check that you're using the correct architecture (32-bit vs 64-bit)

### Build errors on Linux/Mac

- This project requires Windows to build the Excel add-in
- Use a Windows machine or Windows VM
- Alternatively, use GitHub Actions with Windows runners

### Excel shows #VALUE! error

- Check that input parameters match the expected types
- Verify that coordinates are in valid ranges (lat: -90 to 90, lon: -180 to 180)
- For text functions, ensure the input is a string

---

**Remember**: This is not philosophy. This is an executable specification.

Every concept → a column  
Every detector → a function  
Every intuition → a feature  
Every synthesis → an explicit formula
