# Feature Engine - Excel-DNA Add-In

## Vision → Computation

This is the **executable specification** of the vision-to-computation architecture. It transforms abstract concepts into measurable outputs through a three-layer system:

1. **Interface Layer**: Excel cells (the oscilloscope face)
2. **Orchestration Layer**: C# functions (the nervous system)  
3. **Engine Layer**: Pure feature functions (the physics)

---

## Architecture

```
┌─────────────────────────────────────────┐
│  INTERFACE: Excel Cells                 │
│  - Displays rows                        │
│  - Collects inputs                      │
│  - Shows outputs                        │
│  - Zero interpretation                  │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│  ORCHESTRATION: C# Functions            │
│  - Normalizes inputs                    │
│  - Dispatches to engines                │
│  - Aggregates results                   │
│  - Enforces contracts                   │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│  ENGINES: Feature Functions             │
│  - Narrow & swappable                   │
│  - Return numbers/vectors/labels        │
│  - No "meaning", only metrics           │
│  - Falsifiable                          │
└─────────────────────────────────────────┘
```

---

## Feature Functions (Engines)

### 1. Physics Engine

**`FREQUENCY_HZ(wavelength_mm)`**
- Converts wavelength (mm) → frequency (Hz)
- Pure physics: `f = c / λ`
- Example: `=FREQUENCY_HZ(300000)` → `999308.19` Hz

### 2. Chemistry Engine

**`ELEMENT_Z(symbol)`**
- Converts element symbol → atomic number
- Maps categorical → ordinal data
- Example: `=ELEMENT_Z("Fe")` → `26` (Iron)
- Example: `=ELEMENT_Z("Au")` → `79` (Gold)

### 3. Geometry Engine

**`GEO_DISTANCE_KM(lat1, lon1, lat2, lon2)`**
- Calculates great-circle distance using Haversine formula
- Geometric anchor: position → separation
- Example: `=GEO_DISTANCE_KM(30.2266, -93.2174, 30.2366, -93.3774)` → distance in km

### 4. Text Analysis Engines (Stubs)

**`FALLACY_SCORE(text)`**
- Detects logical fallacies
- Returns: 0.0 (no fallacies) → 1.0 (high fallacy density)
- **Note**: Currently stub implementation; production would use LLM

**`ENTROPY_SCORE(text)`**
- Calculates Shannon entropy
- Measures information density
- Higher = more diverse, Lower = more repetitive

**`CONTRADICTION_SCORE(text)`**
- Detects internal contradictions
- Returns: 0.0 (consistent) → 1.0 (contradictory)
- **Note**: Currently stub implementation; production would use semantic analysis

**`SIGNAL_SHARPNESS(text)`**
- Measures clarity/precision of text
- Higher = specific/precise, Lower = vague/ambiguous

### 5. Composite Engine

**`UTRD(text)`**
- Universal Truth Reliability Detector
- Aggregates all text metrics into composite score
- Returns JSON with all metrics:

```json
{
  "input": "Sample text...",
  "metrics": {
    "fallacy_score": 0.25,
    "entropy_score": 0.73,
    "contradiction_score": 0.0,
    "signal_sharpness": 0.45,
    "composite_score": 0.625
  },
  "timestamp": "2026-01-16T18:30:00.000Z"
}
```

---

## Row Model (Feature Table)

One row = one observation. Each column is a feature.

| Col | Feature | Function | Domain |
|-----|---------|----------|--------|
| A | Raw Input | - | Any |
| B | Wavelength (mm) | - | Physics |
| C | Element Symbol | - | Chemistry |
| D | Latitude | - | GPS |
| E | Longitude | - | GPS |
| F | Frequency (Hz) | `=FREQUENCY_HZ(B2)` | Physics |
| G | Atomic Number | `=ELEMENT_Z(C2)` | Chemistry |
| H | Distance (km) | `=GEO_DISTANCE_KM(D2,E2,D3,E3)` | Geometry |
| I | Fallacy Score | `=FALLACY_SCORE(A2)` | Text |
| J | Entropy Score | `=ENTROPY_SCORE(A2)` | Text |
| K | Contradiction | `=CONTRADICTION_SCORE(A2)` | Text |
| L | Signal Sharpness | `=SIGNAL_SHARPNESS(A2)` | Text |
| M | UTRD Composite | `=UTRD(A2)` | Synthesis |

At this point:
- **Excel is a live feature table**
- **C# is a feature compiler**
- **Your "vision" only decides which columns to add next**

Nothing else.

---

## Building the Add-In

### Prerequisites

- Windows OS (Excel-DNA requires Windows)
- .NET 6.0 SDK or later
- Microsoft Excel (2010 or later)

### Build Steps

```bash
cd src/ExcelDna
dotnet restore
dotnet build
```

This produces:
- `PipeTradesFeatureEngine.dll` (the compiled code)
- `PipeTradesFeatureEngine.xll` (the Excel add-in)
- `PipeTradesFeatureEngine-AddIn.dna` (packed configuration)

### Installation

1. Build the project (see above)
2. Copy `PipeTradesFeatureEngine.xll` to a permanent location
3. In Excel, go to **File → Options → Add-ins**
4. Select **Excel Add-ins** from the dropdown, click **Go**
5. Click **Browse** and select the `.xll` file
6. Click **OK**

The functions will now be available in Excel under the **Feature Engine** categories.

---

## Usage Examples

### Example 1: Physics Transform

```
A1: 300000
B1: =FREQUENCY_HZ(A1)
```

Result: `999308.19` Hz

### Example 2: Chemistry Lookup

```
A2: Fe
B2: =ELEMENT_Z(A2)
```

Result: `26`

### Example 3: GPS Distance

```
A3: Lake Charles    B3: 30.2266    C3: -93.2174
A4: Sulphur         B4: 30.2366    C4: -93.3774
D4: =GEO_DISTANCE_KM(B3, C3, B4, C4)
```

Result: Distance in kilometers

### Example 4: Text Analysis

```
A5: Everyone knows this is true, so it must be correct!
B5: =FALLACY_SCORE(A5)
C5: =ENTROPY_SCORE(A5)
D5: =CONTRADICTION_SCORE(A5)
E5: =SIGNAL_SHARPNESS(A5)
F5: =UTRD(A5)
```

Results:
- B5: Fallacy score (detects argumentum ad populum)
- C5: Entropy score
- D5: Contradiction score
- E5: Sharpness score
- F5: Full JSON metrics

---

## JSON Contract

All engines can return structured data via JSON. The UTRD function demonstrates the contract:

```json
{
  "input": "text sample...",
  "metrics": {
    "fallacy_score": 0.25,
    "entropy_score": 0.73,
    "contradiction_score": 0.41,
    "signal_sharpness": 0.88,
    "composite_score": 0.625
  },
  "timestamp": "2026-01-16T18:30:00.000Z"
}
```

This is a **real, enforceable interface**.

---

## Extending the Engine

### Adding a New Feature Function

1. Add the function to `FeatureEngine.cs`
2. Mark it with `[ExcelFunction]` attribute
3. Rebuild the project
4. Reinstall the add-in

Example:

```csharp
[ExcelFunction(
    Name = "MY_FEATURE",
    Description = "My new feature",
    Category = "Feature Engine - Custom"
)]
public static object MY_FEATURE(string input)
{
    // Your logic here
    return result;
}
```

### Integrating with Claude/Ollama

Replace stub implementations in text analysis functions:

```csharp
public static object FALLACY_SCORE(string text)
{
    // Instead of stub logic:
    var response = await CallClaudeAPI(text, "detect fallacies");
    return ParseFallacyScore(response);
}
```

Add HTTP client configuration and API endpoints as needed.

---

## Why This Architecture?

### It Protects You

Because now:
- **Every claim produces numbers**
- **Every number can drift**
- **Every metric can fail**
- **Every synthesis can be wrong**

Which means **the system can correct you**.

That's the difference between:
- **An instrument** ← (you are here)
- **An altar**

### It's Falsifiable

Each function:
- Has clear inputs and outputs
- Can be tested independently
- Can be swapped without breaking the system
- Returns confidence bounds (not "truth")

### It Scales

Add new engines without modifying existing ones:
- Each column is independent
- Parallel instrumentation first
- No unified primitive (unification is a report, not a core type)

---

## Future Directions

1. **LLM Integration**: Replace stubs with real Claude/Ollama calls
2. **More Engines**: Add frequency transforms, geometry distances, etc.
3. **Confidence Bounds**: Return error bars with each metric
4. **Engine Agreement**: Compare outputs from multiple engines
5. **Web Bridge**: HTTP server for non-Excel clients
6. **Office Add-in**: Live UI pane for interactive analysis

---

## License

Same as parent project (Apache 2.0)

---

## Contact

For questions or contributions, see main repository README.

---

**Remember**: This is not philosophy. This is a system contract.

Every concept → a column  
Every detector → a function  
Every intuition → a feature  
Every synthesis → an explicit formula

Welcome to the instrument panel.
