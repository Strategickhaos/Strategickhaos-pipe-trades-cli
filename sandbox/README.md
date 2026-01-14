# Simulation Sandbox

## Purpose
Train ML models on field measurements + engineering specs to:
1. Predict beam dimensions from partial measurements
2. Estimate fireproofing material requirements
3. Simulate debris containment scenarios

## Data Pipeline
```
Field Measurements (CLI)
↓
Recon Data (Grok)
↓
Training Dataset
↓
ML Models
↓
LLM Fine-tuning
↓
Simulation Engine
```
## Models
- `beam_predictor.pkl` - Predicts full beam specs from partial input
- `material_estimator.pkl` - Estimates band/mesh from beam type
- `gps_structural_classifier.pkl` - Identifies structure type from coordinates

## Usage
```bash
python sandbox/train.py --data recon/data/ --output sandbox/models/
python sandbox/simulate.py --model beam_predictor --scenario debris_fall
```
