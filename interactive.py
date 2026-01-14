#!/usr/bin/env python3
"""
PIPE TRADES CLI - Interactive Field Mode
========================================
Guided prompts for field measurements
GPS capture and validation
Auto-calculation with verification
"""

from main import BeamCalc, decode_plus_code, rolling_offset, calibrate
import json
from datetime import datetime
from pathlib import Path

def get_float(prompt, default=None):
    while True:
        val = input(f"{prompt} [{default}]: ").strip()
        if not val and default is not None:
            return float(default)
        try:
            return float(val)
        except ValueError:
            print("Invalid number. Try again.")

def get_int(prompt, default=None):
    while True:
        val = input(f"{prompt} [{default}]: ").strip()
        if not val and default is not None:
            return int(default)
        try:
            return int(val)
        except ValueError:
            print("Invalid integer. Try again.")

def beam_wizard():
    print("\n=== BEAM WRAP WIZARD ===\n")
    
    # GPS/Location
    location = input("GPS or Plus Code (optional): ").strip()
    if location:
        try:
            area = decode_plus_code(location)
            print(f"  → Decoded: {area.lat:.6f}, {area.lon:.6f}")
        except:
            print("  → Could not decode location")
    
    # Measurements
    circ = get_float("Beam circumference (inches)", 44)
    shoes = get_int("Number of shoes walked", 0)
    boot = get_float("Final boot measurement (inches)", 0)
    rise = get_float("Rise/elevation change (inches)", 0)
    
    # Calculate
    calc = BeamCalc(circ, shoes, boot, rise)
    print(calc.report())
    
    # Save job
    save = input("Save this job? (y/n): ").strip().lower()
    if save == 'y':
        jobs_dir = Path("jobs")
        jobs_dir.mkdir(exist_ok=True)
        
        job = {
            "timestamp": datetime.now().isoformat(),
            "location": location,
            "inputs": {
                "circumference": circ,
                "shoes": shoes,
                "boot": boot,
                "rise": rise
            },
            "outputs": {
                "beam_length": calc.beam_length,
                "band_qty": calc.band_qty,
                "mesh_panels": calc.mesh_qty,
                "total_mesh_sqft": calc.mesh_qty * calc.mesh_length * 40 / 144
            }
        }
        
        filename = jobs_dir / f"job_{datetime.now().strftime('%Y%m%d_%H%M%S')}.json"
        with open(filename, 'w') as f:
            json.dump(job, f, indent=2)
        print(f"Saved: {filename}")
    
    return calc

def main_menu():
    while True:
        print("\n" + "="*50)
        print("PIPE TRADES CLI - FIELD MODE")
        print("="*50)
        print("1. Beam Wrap Calculator")
        print("2. Rolling Offset")
        print("3. GPS Decode")
        print("4. Calibration Check")
        print("5. View Saved Jobs")
        print("0. Exit")
        print("-"*50)
        
        choice = input("Select: ").strip()
        
        if choice == "1":
            beam_wizard()
        elif choice == "2":
            angle = get_float("Angle (degrees)", 45)
            offset = get_float("Offset (inches)", 5)
            r = rolling_offset(angle, offset)
            print(f"\nTravel:  {r['travel']:.4f}\"")
            print(f"Advance: {r['advance']:.4f}\"")
        elif choice == "3":
            code = input("Plus Code or GPS: ").strip()
            try:
                area = decode_plus_code(code)
                print(f"\nLat: {area.lat:.6f}")
                print(f"Lon: {area.lon:.6f}")
                print(f"https://maps.google.com/?q={area.lat},{area.lon}")
            except Exception as e:
                print(f"Error: {e}")
        elif choice == "4":
            sat = get_float("Satellite measurement", 305)
            fld = get_float("Field measurement", 305)
            r = calibrate(sat, fld)
            status = "✓ CALIBRATED" if r["calibrated"] else "✗ NEEDS ADJUSTMENT"
            print(f"\nDiff: {r['difference']:+.2f} ({r['pct_error']:+.2f}%)")
            print(status)
        elif choice == "5":
            jobs_dir = Path("jobs")
            if jobs_dir.exists():
                for f in sorted(jobs_dir.glob("*.json")):
                    print(f"  {f.name}")
            else:
                print("No saved jobs.")
        elif choice == "0":
            print("Exiting.")
            break

if __name__ == "__main__":
    main_menu()

git add interactive.py recon sandbox board_minutes.yaml
git commit -m "feat: add interactive mode, recon config, sandbox scaffold"
git push
docker login
docker push strategickhaos/pipe-trades-cli:latest
