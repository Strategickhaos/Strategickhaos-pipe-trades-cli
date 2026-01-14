# Phase 7: Control Unit Implementation

## Overview

This directory contains the Lua-based control sequencer for orchestrated evolution in the quantum-symbolic CPU architecture. The control unit implements a fetch-decode-execute cycle using trigonometric matrix transformations to sequence module loads and agent swarm deployments.

## Architecture

### Sequencer Logic (`sequencer.lua`)

The sequencer implements:
- **Trig-Matrix Sequencing**: Uses cos/sin transformations on probability angles to generate weighted control steps
- **Logical AI Branching**: Conditional logic based on cognitive architecture motifs (e.g., neurological resilience)
- **Swarm Orchestration**: Publishes orchestrated plans to MQTT for downstream consumption
- **GPT Integration**: Dispatches GitHub events for AI-generated control flow code

### Control Flow

1. **Load Configuration**: Reads `logs/evo_plan.yaml` (Phase 6 output) and `DOM_COGNITIVE_ARCHITECTURE.yaml`
2. **Phase Angle Calculation**: Converts probability to phase angle (prob × π)
3. **Step Generation**:
   - Fetch: cos(angle) weight for module loading
   - Decode: sin(angle) weight for motif interpretation
   - Execute: Deploy swarm agents
   - Adapt: Add resilience bias if neurological motif detected
4. **Branch Logic**: If weight < 0.5, trigger recursive perturbation
5. **Output**: Generate `logs/control_sequence.yaml` and publish to MQTT

## Files

- `sequencer.lua`: Main control sequencer implementation
- `README.md`: This documentation file

## Dependencies

Runtime requirements (provided by `Dockerfile.control`):
- Lua 5.4
- lua-yaml (or lyaml) for YAML parsing
- mosquitto-libs for MQTT client
- curl for GitHub API integration

## Usage

### Local Testing (requires Lua 5.4)
```bash
lua control/sequencer.lua
```

### Container Build
```bash
podman build -t ptc-control-unit:latest -f Dockerfile.control .
# or
docker build -t ptc-control-unit:latest -f Dockerfile.control .
```

### Kubernetes Deployment
```bash
kubectl apply -f k8s/control_statefulset.yaml
```

### Automated Deployment
```bash
./scripts/phase_deploy.sh 7
# With conflict resolution
./scripts/phase_deploy.sh 7 --resolve-conflicts
```

## Integration Points

- **Input**: `logs/evo_plan.yaml` (from Phase 6 probabilistic selection)
- **Configuration**: `DOM_COGNITIVE_ARCHITECTURE.yaml` (cognitive motifs and control strategies)
- **Output**: `logs/control_sequence.yaml` (executable control sequences)
- **MQTT Topic**: `strategickhaos/pipe-trades/field-updates` (swarm orchestration)
- **GitHub API**: Repository dispatch events for GPT control generation

## Verification

Successful deployment produces:
1. `logs/control_sequence.yaml` with ops/steps
2. MQTT publish logs confirming swarm notification
3. GitHub dispatch event for GPT module flow generation
4. Console output showing control branches for low-weight steps
5. StatefulSet running in Kubernetes with persistent sequence storage

## Architecture Concepts

### Quadrilateral Collapse
The control unit uses the "Quadrilateral Collapse" motif from the cognitive architecture to fold probabilistic states into executable sequences through trigonometric transformations.

### Neurological Adaptation
When the cognitive architecture specifies neurological-adapted resilience, the sequencer adds adaptive control steps with increased bias (0.3) for enhanced system resilience.

### Trig-Matrix Cores
Uses cosine/sine functions to weight control steps, creating phase-shifted execution patterns that enable recursive branching and perturbation handling.
