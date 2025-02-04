#!/bin/bash

# List of new names
names=(
    Kowhoremis Edte Serdouklmime Trauerin Corep Melleekenk Hotipiccharbreyleser
    Nioronith Guami Bogageresleyn Gugos Rnghutater Verkwis Wrndavear Schal
    Kellubeindlloorsa Byforomhas Ngenus Adun Jodarchos Grulle Llabawisode
    Ganertaker Dgreriler Nogttc Tarertompieoomchasheeleret
    )

# Get a list of all PNG files in the directory
files=(*.png)

# Check if the number of files matches the number of names
  if [ "${#files[@]}" -ne "${#names[@]}" ]; then
  echo "Error: Number of PNG files (${#files[@]}) does not match the number of names (${#names[@]})."
  exit 1
  fi

# Rename the files
  for i in "${!files[@]}"; do
  mv "${files[i]}" "${names[i]}.png"
  done

  echo "Renaming complete."

