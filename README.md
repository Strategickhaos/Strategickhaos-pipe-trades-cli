# Strategickhaos-pipe-trades-cli
Field-calibrated pipefitter calculation ecosystem. GPS coordinate verification, beam wrap material estimation, rolling offset calculations. Mirrors Pipe Trades Pro 4095 + TI-nspire CX II programmability. Built for rope access crews doing fireproofing containment.

## Building

```bash
make
```

This will create the `pipe-trades` executable.

## Usage

```bash
./pipe-trades <command> [options]
```

### Commands

#### GPS Coordinate Verification
Calculate distance between two GPS coordinates using the Haversine formula:

```bash
./pipe-trades gps-verify <lat1> <lon1> <lat2> <lon2>
```

Example:
```bash
./pipe-trades gps-verify 40.7128 -74.0060 34.0522 -118.2437
```

#### Beam Wrap Material Estimation
Calculate material needed for beam wrapping with 10% overlap:

```bash
./pipe-trades beam-wrap <diameter> <length>
```

Example:
```bash
./pipe-trades beam-wrap 12 10
```
- diameter: beam diameter in inches
- length: beam length in feet

#### Rolling Offset Calculation
Calculate true offset, set, and diagonal travel for rolling offset:

```bash
./pipe-trades rolling-offset <offset> <roll> <travel>
```

Example:
```bash
./pipe-trades rolling-offset 6 8 24
```
- offset: offset distance in inches
- roll: roll distance in inches
- travel: travel distance in inches

### Other Commands

```bash
./pipe-trades help       # Show help message
./pipe-trades version    # Show version information
```

## Installation

To install system-wide (requires sudo):

```bash
sudo make install
```

This installs to `/usr/local/bin/pipe-trades`.

## License

Apache License 2.0

