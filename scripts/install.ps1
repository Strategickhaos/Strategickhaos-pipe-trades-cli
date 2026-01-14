git clone https://github.com/Strategickhaos/Strategickhaos-pipe-trades-cli.git $env:USERPROFILE\.ptc
echo 'function ptc { python $env:USERPROFILE\.ptc\main.py $args }' >> $PROFILE
Write-Host "Done! Restart terminal, then: ptc beam --circ 44 --shoes 4 --boot 6 --rise 30"
