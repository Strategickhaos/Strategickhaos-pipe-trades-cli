#!/usr/bin/env python3
"""
PIPE TRADES CLI - Field Calibration System
==========================================
Decode Plus Codes / GPS coordinates
Calculate distances between points
Beam wrap material estimation
Rolling offset calculations (mirrors Pipe Trades Pro 4095)

Usage:
    python main.py decode "5MHH+P8G Lake Charles, Louisiana"
    python main.py beam --circ 44 --shoes 4 --boot 6 --rise 30
    python main.py offset --angle 45 --offset 5
    python main.py calibrate --satellite 305 --field 305 --unit ft
"""

import argparse
import math
import sys
from dataclasses import dataclass
from typing import Tuple

# ============================================================
# CONSTANTS
# ============================================================

SHOE_SIZE = 14  # inches - field measurement constant
CODE_ALPHABET = "23456789CFGHJMPQRVWX"
SEPARATOR = "+"
LATITUDE_MAX = 90
LONGITUDE_MAX = 180

REFERENCE_POINTS = {
    "lake charles": (30.2266, -93.2174),
    "sulphur": (30.2366, -93.3774),
    "louisiana": (30.9843, -91.9623),
}

# ============================================================
# GPS / PLUS CODE
# ============================================================

@dataclass
class CodeArea:
    south: float
    west: float
    north: float
    east: float
    
    @property
    def lat(self) -> float:
        return (self.south + self.north) / 2
    
    @property
    def lon(self) -> float:
        return (self.west + self.east) / 2


def decode_plus_code(code: str) -> CodeArea:
    original = code.strip()
    ref_lat, ref_lon = None, None
    
    for loc_key, coords in REFERENCE_POINTS.items():
        if loc_key in original.lower():
            ref_lat, ref_lon = coords
            break
    
    code_part = original.upper().split()[0]
    
    if SEPARATOR in code_part:
        prefix, suffix = code_part.split(SEPARATOR)
        if len(prefix) < 8 and ref_lat is not None:
            lat_val = ref_lat + LATITUDE_MAX
            lon_val = ref_lon + LONGITUDE_MAX
            c1 = CODE_ALPHABET[int(lat_val / 20)]
            c2 = CODE_ALPHABET[int(lon_val / 20)]
            c3 = CODE_ALPHABET[int((lat_val % 20) / 1)]
            c4 = CODE_ALPHABET[int((lon_val % 20) / 1)]
            code_part = f"{c1}{c2}{c3}{c4}{code_part}"
    
    code = code_part.replace(SEPARATOR, "")
    
    PAIR_RESOLUTIONS = [20.0, 1.0, 0.05, 0.0025, 0.000125]
    south, west = 0.0, 0.0
    lat_res, lon_res = PAIR_RESOLUTIONS[0], PAIR_RESOLUTIONS[0]
    
    for i in range(0, min(len(code), 10), 2):
        lat_res = PAIR_RESOLUTIONS[i // 2]
        lon_res = PAIR_RESOLUTIONS[i // 2]
        south += CODE_ALPHABET.index(code[i]) * lat_res
        west += CODE_ALPHABET.index(code[i + 1]) * lon_res
    
    south -= LATITUDE_MAX
    west -= LONGITUDE_MAX
    
    return CodeArea(south=south, west=west, north=south + lat_res, east=west + lon_res)


# ============================================================
# DISTANCE CALCULATIONS
# ============================================================

def haversine(lat1: float, lon1: float, lat2: float, lon2: float, unit: str = "ft") -> float:
    R_FT = 20902231
    lat1_r, lat2_r = math.radians(lat1), math.radians(lat2)
    dlat, dlon = math.radians(lat2 - lat1), math.radians(lon2 - lon1)
    
    a = math.sin(dlat/2)**2 + math.cos(lat1_r) * math.cos(lat2_r) * math.sin(dlon/2)**2
    dist_ft = R_FT * 2 * math.atan2(math.sqrt(a), math.sqrt(1-a))
    
    conv = {"ft": 1, "in": 12, "m": 0.3048, "mi": 1/5280}
    return dist_ft * conv.get(unit, 1)


def pythagorean(run: float, rise: float) -> float:
    return math.sqrt(run**2 + rise**2)


# ============================================================
# BEAM WRAP CALCULATIONS
# ============================================================

@dataclass
class BeamCalc:
    circumference: float
    shoe_count: int
    boot_final: float
    rise: float
    shoe_size: float = SHOE_SIZE
    
    @property
    def run(self) -> float:
        return (self.shoe_count * self.shoe_size) + self.boot_final
    
    @property
    def beam_length(self) -> float:
        return pythagorean(self.run, self.rise) if self.rise else self.run
    
    @property
    def band_length(self) -> float:
        return self.circumference + 8  # +7" bander grab +1" clip
    
    @property
    def band_qty(self) -> int:
        return math.ceil(self.beam_length / 40) + 1
    
    @property
    def mesh_length(self) -> float:
        return self.circumference + 19  # +12" corners +3" overlap +4" edge
    
    @property
    def mesh_qty(self) -> int:
        return max(self.band_qty - 1, 1)
    
    def report(self) -> str:
        beam_type = "Angled" if self.rise else "Horizontal"
        return f"""
{'='*50}
BEAM WRAP CALCULATION
{'='*50}
Type:           {beam_type}
Circumference:  {self.circumference}"
Run:            {self.run}" ({self.shoe_count} shoes + {self.boot_final}" boot)
Rise:           {self.rise}"
{'-'*50}
BEAM LENGTH:    {self.beam_length:.2f}" ({self.beam_length/12:.2f} ft)
{'-'*50}
Band Length:    {self.band_length}" (circ + 8")
Band Qty:       {self.band_qty}
Total Band:     {self.band_qty * self.band_length}" ({self.band_qty * self.band_length/12:.2f} ft)
{'-'*50}
Mesh Length:    {self.mesh_length}" (circ + 19")
Mesh Panels:    {self.mesh_qty}
Mesh Width:     40"
Total Mesh:     {self.mesh_qty * self.mesh_length * 40:.0f} sq in ({self.mesh_qty * self.mesh_length * 40/144:.2f} sq ft)
{'='*50}
"""


# ============================================================
# PIPE TRADES PRO FUNCTIONS
# ============================================================

def rolling_offset(angle: float, offset: float) -> dict:
    rad = math.radians(angle)
    return {
        "angle": angle,
        "offset": offset,
        "travel": offset / math.sin(rad) if angle else 0,
        "advance": offset / math.tan(rad) if angle else 0,
    }


def cutback(angle: float, offset: float) -> dict:
    return {
        "angle": angle,
        "offset": offset,
        "cut": offset * math.tan(math.radians(angle / 2)),
    }


def calibrate(satellite: float, field: float) -> dict:
    diff = field - satellite
    pct = (diff / satellite) * 100 if satellite else 0
    return {
        "satellite": satellite,
        "field": field,
        "difference": diff,
        "pct_error": pct,
        "calibrated": abs(pct) < 2,
    }


# ============================================================
# CLI
# ============================================================

def main():
    parser = argparse.ArgumentParser(description="Pipe Trades CLI")
    sub = parser.add_subparsers(dest="cmd")
    
    # decode
    p = sub.add_parser("decode")
    p.add_argument("code")
    
    # beam
    p = sub.add_parser("beam")
    p.add_argument("--circ", type=float, required=True)
    p.add_argument("--shoes", type=int, default=0)
    p.add_argument("--boot", type=float, default=0)
    p.add_argument("--rise", type=float, default=0)
    
    # offset
    p = sub.add_parser("offset")
    p.add_argument("--angle", type=float, required=True)
    p.add_argument("--offset", type=float, required=True)
    
    # cutback
    p = sub.add_parser("cutback")
    p.add_argument("--angle", type=float, required=True)
    p.add_argument("--offset", type=float, required=True)
    
    # calibrate
    p = sub.add_parser("calibrate")
    p.add_argument("--satellite", type=float, required=True)
    p.add_argument("--field", type=float, required=True)
    p.add_argument("--unit", default="ft")
    
    # hyp
    
    # swarm
    p = sub.add_parser("swarm")
    p.add_argument("--broker", default="mqtt.eclipseprojects.io")
    p.add_argument("--port", type=int, default=1883)
    p = sub.add_parser("hyp")
    p.add_argument("--run", type=float, required=True)
    p.add_argument("--rise", type=float, required=True)
    
    args = parser.parse_args()
    
    if args.cmd == "decode":
        area = decode_plus_code(args.code)
        print(f"Lat: {area.lat:.6f}\nLon: {area.lon:.6f}")
        print(f"https://maps.google.com/?q={area.lat},{area.lon}")
    
    elif args.cmd == "beam":
        b = BeamCalc(args.circ, args.shoes, args.boot, args.rise)
        print(b.report())
    
    elif args.cmd == "offset":
        r = rolling_offset(args.angle, args.offset)
        print(f"Travel:  {r['travel']:.4f}\"\nAdvance: {r['advance']:.4f}\"")
    
    elif args.cmd == "cutback":
        r = cutback(args.angle, args.offset)
        print(f"Cut: {r['cut']:.4f}\"")
    
    elif args.cmd == "calibrate":
        r = calibrate(args.satellite, args.field)
        status = "✓ CALIBRATED" if r["calibrated"] else "✗ ADJUST"
        print(f"Diff: {r['difference']:+.2f} {args.unit} ({r['pct_error']:+.2f}%)\n{status}")
    
    elif args.cmd == "hyp":
        t = pythagorean(args.run, args.rise)
        print(f"Travel: {t:.4f}\" ({t/12:.4f} ft)")
    
    else:
    elif args.cmd == "swarm":
        swarm_mode(args.broker, args.port)

        parser.print_help()


if __name__ == "__main__":
    main()


# ============================================================
# SWARM MODE - Field Crew Coordination
# ============================================================

def swarm_mode(broker="mqtt.eclipseprojects.io", port=1883):
    """Connect to swarm network for real-time crew coordination."""
    try:
        import paho.mqtt.client as mqtt
    except ImportError:
        print("ERROR: paho-mqtt not installed. Run: pip install paho-mqtt")
        return
    
    import json
    import time
    import socket
    
    crew_id = socket.gethostname()
    topic = "strategickhaos/pipe-trades/field-updates"
    
    def on_connect(client, userdata, flags, rc):
        status = "✓ CONNECTED" if rc == 0 else f"✗ FAILED ({rc})"
        print(f"Swarm Status: {status}")
        print(f"Crew ID: {crew_id}")
        print(f"Topic: {topic}")
        print("-" * 50)
        client.subscribe(topic)
    
    def on_message(client, userdata, msg):
        try:
            data = json.loads(msg.payload)
            print(f"[SWARM] {data.get('crew_id', 'unknown')}: {data.get('calculation', {})}")
        except:
            print(f"[SWARM] Raw: {msg.payload.decode()}")
    
    client = mqtt.Client()
    client.on_connect = on_connect
    client.on_message = on_message
    
    print("=" * 50)
    print("PIPE TRADES CLI - SWARM MODE")
    print("=" * 50)
    print(f"Connecting to {broker}:{port}...")
    
    try:
        client.connect(broker, port, 60)
        client.loop_start()
        
        # Publish presence
        presence = {
            "crew_id": crew_id,
            "status": "online",
            "capabilities": ["beam", "offset", "calibrate", "gps"]
        }
        client.publish(topic, json.dumps(presence))
        
        # Interactive swarm loop
        print("\nSwarm active. Commands:")
        print("  beam <circ> <shoes> <boot> <rise> - Share beam calc")
        print("  offset <angle> <offset> - Share offset calc")
        print("  exit - Leave swarm")
        print("-" * 50)
        
        while True:
            cmd = input("swarm> ").strip().split()
            if not cmd:
                continue
            
            if cmd[0] == "exit":
                print("Leaving swarm...")
                break
            
            elif cmd[0] == "beam" and len(cmd) == 5:
                calc = BeamCalc(float(cmd[1]), int(cmd[2]), float(cmd[3]), float(cmd[4]))
                data = {
                    "crew_id": crew_id,
                    "calculation": {
                        "type": "beam",
                        "length": round(calc.beam_length, 2),
                        "band_qty": calc.band_qty,
                        "mesh_panels": calc.mesh_qty
                    }
                }
                client.publish(topic, json.dumps(data))
                print(f"[SENT] {data['calculation']}")
            
            elif cmd[0] == "offset" and len(cmd) == 3:
                r = rolling_offset(float(cmd[1]), float(cmd[2]))
                data = {
                    "crew_id": crew_id,
                    "calculation": {
                        "type": "offset",
                        "travel": round(r["travel"], 4),
                        "advance": round(r["advance"], 4)
                    }
                }
                client.publish(topic, json.dumps(data))
                print(f"[SENT] {data['calculation']}")
            
            else:
                print("Unknown command. Try: beam, offset, exit")
        
        client.loop_stop()
        client.disconnect()
        
    except Exception as e:
        print(f"Swarm error: {e}")

# Add swarm to CLI (append to main function manually or run):
# In the main() function, add after the 'hyp' subparser:
#     p = sub.add_parser("swarm")
#     p.add_argument("--broker", default="mqtt.eclipseprojects.io")
#     p.add_argument("--port", type=int, default=1883)
#
# And in the if/elif chain:
#     elif args.cmd == "swarm":
#         swarm_mode(args.broker, args.port)
