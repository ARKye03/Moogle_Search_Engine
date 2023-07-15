#!/bin/bash

# Use zsh instead
if [ -n "$ZSH_VERSION" ]; then
    exec zsh "$0" "$@"
fi

folder_path="Content/"

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
  sleep 5
fi
