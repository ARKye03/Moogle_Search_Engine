#!/bin/bash

# Use zsh instead
if [ -n "$ZSH_VERSION" ]; then
    exec zsh "$0" "$@"
fi

#Vars
folder_path="Content/"

#Check -h flag
if [[ "$1" == "-h" && ! -z "$2" ]]; then
    echo "I've always liked flags, there was supposed to be a flag -m here to change the browser mode, but after a devastating bug I decided to remove it... Pray for Moogle!"
    read -n 1 -s -r -p "Press any key to continue..."
    exit 0
fi
file_count=$(find "$folder_path" -type f | wc -l)

if [[ $file_count -gt 1 ]]; then
  if [[ $OSTYPE == "linux-gnu" || $OSTYPE == "linux" ]]; then
    echo "Linux detected"
    make dev
  else
    echo "Windows detected... or something else..."
    dotnet watch run --project MoogleServer
  fi
else
  echo "La carpeta 'Content' contiene al parecer un solo archivo, coloque más archivos en la carpeta, y ejecute nuevamente el script"
  read -n 1 -s -r -p "Press any key to continue..."
fi
