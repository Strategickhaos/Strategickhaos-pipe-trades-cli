# Strategickhaos-pipe-trades-cli
Field-calibrated pipefitter calculation ecosystem. GPS coordinate verification, beam wrap material estimation, rolling offset calculations. Mirrors Pipe Trades Pro 4095 + TI-nspire CX II programmability. Built for rope access crews doing fireproofing containment.

## Features

### 🔧 CLI Tools
- **GPS Decoding**: Decode Plus Codes and GPS coordinates
- **Beam Wrap Calculator**: Material estimation for fireproofing
- **Rolling Offset**: Pipe fitting calculations
- **Field Calibration**: Satellite vs field measurement verification

### 🌐 Browser Tools
- **Claude Code Injector**: Browser-based assistant panel for GitHub project analysis
  - Paste into DevTools console on any GitHub page
  - Extract project data, analyze issues, list PRs
  - Optional WebSocket bridge for enhanced features
  - See [browser-tools/README.md](browser-tools/README.md) for details

## Quick Start

### CLI Installation

```bash
# Clone and install
git clone https://github.com/Strategickhaos/Strategickhaos-pipe-trades-cli.git ~/.ptc
echo 'alias ptc="python3 ~/.ptc/main.py"' >> ~/.bashrc
source ~/.bashrc

# Example usage
ptc beam --circ 44 --shoes 4 --boot 6 --rise 30
ptc offset --angle 45 --offset 5
ptc decode "5MHH+P8G Lake Charles, Louisiana"
```

### Browser Tools

1. Navigate to any GitHub page
2. Open DevTools Console (F12)
3. Copy and paste `browser-tools/claude-code-injector.js`
4. Use the Claude Code panel to analyze the page

**Optional**: Run `python3 process_monitor.py` for WebSocket bridge support.

## Usage Examples

### Beam Wrap Calculation
```bash
python main.py beam --circ 44 --shoes 4 --boot 6 --rise 30
```

### Rolling Offset
```bash
python main.py offset --angle 45 --offset 5
```

### GPS Decode
```bash
python main.py decode "5MHH+P8G Lake Charles, Louisiana"
```

### Field Calibration
```bash
python main.py calibrate --satellite 305 --field 305 --unit ft
```

### Interactive Mode
```bash
python interactive.py
```

## Project Structure

```
.
├── main.py              # Main CLI application
├── interactive.py       # Interactive field mode
├── process_monitor.py   # WebSocket bridge for browser tools
├── browser-tools/       # Browser-based tools
│   ├── claude-code-injector.js
│   └── README.md
├── recon/               # Reconnaissance configs
├── sandbox/             # ML simulation sandbox
├── scripts/             # Installation scripts
└── k8s/                 # Kubernetes configs
```

## Contributing

This project supports field operations for rope access crews. Contributions welcome for:
- Additional pipe fitting calculations
- Enhanced GPS coordinate handling
- Material estimation improvements
- Field data collection features

## License

See LICENSE file for details.
