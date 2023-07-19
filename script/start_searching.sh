#!/bin/bash

# Use zsh instead
if [ -n "$ZSH_VERSION" ]; then
    exec zsh "$0" "$@"
fi

#Vars
folder_path="../Content"
file_count=$(find "$folder_path" -type f | wc -l)

cd ..

# Run Moogle
run() {
if [[ $file_count -gt 1 ]]; then
  if [[ $OSTYPE == "linux-gnu" || $OSTYPE == "linux" ]]; then #Using Linux
    echo "Linux detected"
    make dev
  else
    echo "Windows detected... or something else..." #Using Windows or MacOS
    dotnet watch run --project MoogleServer
  fi
else
  echo "La carpeta 'Content' contiene al parecer un solo archivo, coloque más archivos en la carpeta, y ejecute nuevamente el script"
fi
}

# Función para compilar y generar el PDF del informe
report() {
  echo "Compilando y generando el PDF del informe..."
  
  pdflatex Informe/Informe.tex
  if [ $? -eq 0 ]; then
    echo "pdflatex succesfull"
    read -n 1 -s -r -p "Press any key to continue..."
  else
    echo "pdflatex falló, probando latexmk"
    latexmk -c Informe/Informe.tex
    read -n 1 -s -r -p "Press any key to continue..."
  fi
}

# Función para compilar y generar el PDF de presentación
slides() {
  echo "Compilando y generando PDF de la presentación..."
    
  pdflatex Presentacion/Presentacion.tex
  if [ $? -eq 0 ]; then
    echo "pdflatex succesfull"
    read -n 1 -s -r -p "Press any key to continue..."
  else
    echo "pdflatex falló, probando latexmk"
    latexmk -c Presentacion/Presentacion.tex
    read -n 1 -s -r -p "Press any key to continue..."
  fi
}
# Función para visualizar el informe
show_report() {
  if [ ! -f "Informe/Informe.pdf" ]; then
      report
  fi

  if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    echo "Linux Detected"
    xdg-open Informe/Informe.pdf 
  elif [[ "$OSTYPE" == "darwin"* ]]; then
    echo "macOS Detected"
    open Informe/Informe.pdf 
  elif [[ "$OSTYPE" == "msys"* ]]; then
    echo "Windows Detected"
    start Informe/Informe.pdf 
  else
    echo "No se pudo determinar el sistema operativo"
  fi
}

# Función para visualizar presentación
show_slides() {
  if [ ! -f "Presentacion/Presentacion.pdf" ]; then
      slides
  fi

  if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    echo "Linux Detected"
    xdg-open Presentacion/Presentacion.pdf 
  elif [[ "$OSTYPE" == "darwin"* ]]; then
    echo "macOS Detected"
    open Presentacion/Presentacion.pdf 
  elif [[ "$OSTYPE" == "msys"* ]]; then
    echo "Windows Detected"
    start Presentacion/Presentacion.pdf 
  else
    echo "No se pudo determinar el sistema operativo :("
    read -n 1 -s -r -p "Press any key to continue..."
  fi
}

# Función para limpiar los ficheros auxiliares
clean() {
  echo "Limpiando los ficheros auxiliares..."
  rm *.aux
  rm *.log
  rm *.fdb_latexmk
  rm *.fls
  rm *.synctex.gz
  echo "Eliminando los ficheros objeto :|"
  rm -r MoogleEngine/obj/Debug/
  rm -r MoogleServer/obj/Debug/
}
help(){
  echo "Estas son tus opciones, pasalas como argumentos al script:"
  echo "----------------------------------------------------------"
  echo "--help:        |-h   |  Ayuda"
  echo "--run:         |-r   |  Ejectutar el proyecto"
  echo "--report:      |-re  |  Compilar y generar el pdf del proyecto latex relativo al informe"
  echo "--slides:      |-sl  |  Compilar y generar el pdf del proyecto latex relativo a la presentación"
  echo "--show_report: |-sr  |  Visualizar el informe"
  echo "--show_slides: |-ss  |  Visualizar la presentación"
  echo "--clean:       |-c   |  Eliminar todos los ficheros auxiliares. Do it at my own risk :)"
}

# Verificar los argumentos pasados al script
if [ $# -eq 0 ]; then
  echo "Debe proporcionar al menos una opción: run, clean, report, slides, show_report, show_slides"
  read -n 1 -s -r -p "Press any key to continue..."
  exit 1
fi

# Procesar las opciones pasadas al script
for option in "$@"; do
    case $option in
        --help | -h)
            help
            ;;
        --run | -r)
            run
            ;;
        --clean | -c)
            clean
            ;;
        --report | -re)
            report
            ;;
        --slides | -sl)
            slides
            ;;
        --show_report | -sr)
            show_report
            ;;
        --show_slides | -ss)
            show_slides
            ;;
        *)
            echo "Opción inválida: $option"
            exit 1
            ;;
    esac
done
