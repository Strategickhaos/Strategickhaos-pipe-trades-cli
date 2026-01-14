#!/bin/bash
git clone https://github.com/Strategickhaos/Strategickhaos-pipe-trades-cli.git ~/.ptc
echo 'alias ptc="python3 ~/.ptc/main.py"' >> ~/.bashrc
echo "Done! Run: source ~/.bashrc && ptc beam --circ 44 --shoes 4 --boot 6 --rise 30"
