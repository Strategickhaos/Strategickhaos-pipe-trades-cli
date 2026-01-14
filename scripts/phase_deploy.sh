#!/bin/bash
# Phase deployment script for quantum-symbolic evolution
# Usage: ./scripts/phase_deploy.sh <phase_number> [--resolve-conflicts]

set -e

PHASE=${1:-7}
RESOLVE_CONFLICTS=false

# Parse arguments
for arg in "$@"; do
  case $arg in
    --resolve-conflicts)
      RESOLVE_CONFLICTS=true
      shift
      ;;
  esac
done

echo "=========================================="
echo "Deploying Phase $PHASE: Control Unit"
echo "=========================================="

# Phase-specific deployment logic
case $PHASE in
  7)
    echo "Phase 7: Instantiate Control Unit"
    echo "----------------------------------"
    
    # Check if control directory exists
    if [ ! -d "control" ]; then
      echo "Error: control/ directory not found"
      exit 1
    fi
    
    # Check if logs directory exists
    if [ ! -d "logs" ]; then
      echo "Error: logs/ directory not found"
      exit 1
    fi
    
    # Build control unit container if podman/docker available
    if command -v podman &> /dev/null; then
      echo "Building control unit container with podman..."
      podman build -t ptc-control-unit:latest -f Dockerfile.control .
    elif command -v docker &> /dev/null; then
      echo "Building control unit container with docker..."
      docker build -t ptc-control-unit:latest -f Dockerfile.control .
    else
      echo "Warning: Neither podman nor docker found, skipping container build"
    fi
    
    # Deploy to Kubernetes if kubectl available
    if command -v kubectl &> /dev/null; then
      echo "Deploying control unit to Kubernetes..."
      kubectl apply -f k8s/control_statefulset.yaml
      echo "Control unit deployed successfully"
    else
      echo "Warning: kubectl not found, skipping K8s deployment"
      echo "To deploy manually, run: kubectl apply -f k8s/control_statefulset.yaml"
    fi
    
    # Handle conflict resolution if requested
    if [ "$RESOLVE_CONFLICTS" = true ]; then
      echo "Resolving merge conflicts..."
      # Preserve local evolutions
      git checkout --ours deployment.yaml 2>/dev/null || true
      git checkout --ours main.py 2>/dev/null || true
      git add deployment.yaml main.py 2>/dev/null || true
    fi
    
    echo "Phase 7 deployment complete!"
    echo "Verifiable outputs:"
    echo "  - control/sequencer.lua: Lua-based control sequencer"
    echo "  - logs/control_sequence.yaml: Generated control sequences"
    echo "  - k8s/control_statefulset.yaml: Kubernetes StatefulSet"
    echo "  - Dockerfile.control: Control unit container"
    ;;
    
  *)
    echo "Unknown phase: $PHASE"
    echo "Currently only Phase 7 is implemented"
    exit 1
    ;;
esac

echo "=========================================="
echo "Deployment verification:"
echo "=========================================="
ls -la control/ 2>/dev/null || echo "control/ directory: Not found"
ls -la logs/ 2>/dev/null || echo "logs/ directory: Not found"
ls -la k8s/control_statefulset.yaml 2>/dev/null || echo "StatefulSet: Not found"
echo "=========================================="
