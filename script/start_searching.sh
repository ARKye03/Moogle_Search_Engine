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
build() {
if [[ $OSTYPE == "linux-gnu" || $OSTYPE == "linux" ]]; then #Using Linux
  echo "Linux detected"
  make build
else
  echo "Windows detected... or something else..." #Using Windows or MacOS
  dotnet build
fi
}
# Función para compilar y generar el PDF del informe
report() {
  echo "Compilando y generando el PDF del informe..."
  cd Informe/
  pdflatex Informe.tex
  if [ $? -eq 0 ]; then
    echo "pdflatex succesfull"
    read -n 1 -s -r -p "Press any key to continue..."
  else
    echo "pdflatex falló, probando latexmk"
    latexmk -c Informe.tex
    read -n 1 -s -r -p "Press any key to continue..."
  fi
}
# Función para compilar y generar el PDF de presentación
slides() {
  echo "Compilando y generando PDF de la presentación..."
  cd Presentacion/  
  pdflatex Presentacion.tex
  if [ $? -eq 0 ]; then
    echo "pdflatex succesfull"
    read -n 1 -s -r -p "Press any key to continue..."
  else
    echo "pdflatex falló, probando latexmk"
    latexmk -c Presentacion.tex
    read -n 1 -s -r -p "Press any key to continue..."
  fi
}
# Función para visualizar el informe
show_report() {
  if [ ! -f "Informe/Informe.pdf" ]; then
      report
  fi
  cd Informe/

  if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    echo "Linux Detected"
    xdg-open Informe.pdf 
  elif [[ "$OSTYPE" == "darwin"* ]]; then
    echo "macOS Detected"
    open Informe.pdf 
  elif [[ "$OSTYPE" == "msys"* ]]; then
    echo "Windows Detected"
    start Informe.pdf 
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
    xdg-open Presentacion.pdf 
  elif [[ "$OSTYPE" == "darwin"* ]]; then
    echo "macOS Detected"
    open Presentacion.pdf 
  elif [[ "$OSTYPE" == "msys"* ]]; then
    echo "Windows Detected"
    start Presentacion.pdf 
  else
    echo "No se pudo determinar el sistema operativo :("
    read -n 1 -s -r -p "Press any key to continue..."
  fi
}
# Función para limpiar los ficheros auxiliares
clean() {
  echo "Limpiando elementos del informe..."
  echo "Limpiando archivos .aux..."
  rm Informe/*.aux
  echo "Limpiando archivos .log..."
  rm Informe/*.log
  echo "Limpiando archivos .fdb_latexmk..."
  rm Informe/*.fdb_latexmk
  echo "Limpiando archivos .fls..."
  rm Informe/*.fls
  echo "Limpiando archivos .synctex.gz..."
  rm Informe/*.synctex.gz

  echo "Limpiando elementos de la presentación..."
  echo "Limpiando archivos .aux..."
  rm Presentacion/*.aux
  echo "Limpiando archivos .log..."
  rm Presentacion/*.log
  echo "Limpiando archivos .fdb_latexmk..."
  rm Presentacion/*.fdb_latexmk
  echo "Limpiando archivos .fls..."
  rm Presentacion/*.fls
  echo "Limpiando archivos .synctex.gz..."
  rm Presentacion/*.synctex.gz

  echo "Eliminando los ficheros objeto :|"
  rm -r MoogleEngine/obj/
  rm -r MoogleEngine/bin/
  rm -r MoogleServer/obj/
  rm -r MoogleServer/bin/

  sleep 1
  clear
  echo "Limpieza completada"
  read -n 1 -s -r -p "Press any key to continue..."
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
  echo "--build:       |-b   |  Compila el proyecto"
  #echo "--interactive  |-i   |  Interactuar con el script"
}

#interactive(){
#  option=""
#while [[ $option != "salir" ]]; do
#    # Mostrar las opciones disponibles
#    echo "1 - help:         |  Ayuda"
#    echo "2 - run:          |  Ejectutar el proyecto"
#    echo "3 - report:       |  Compilar y generar el pdf del proyecto latex relativo al informe"
#    echo "4 - slide:        |  Compilar y generar el pdf del proyecto latex relativo a la presentación"
#    echo "5 - show_report:  |  Visualizar el informe"
#    echo "6 - show_slide:   |  Visualizar la presentación"
#    echo "7 - clean:        |  Eliminar todos los ficheros auxiliares. Do it at my own risk :)"
#    echo "8 - Salir:"
#    echo "c - Clear"
#    # Leer la opción seleccionada por el usuario
#    read -n 1 -s -r -p "Selecciona una opción: " option
#
#    # Ejecutar la acción correspondiente a la opción seleccionada
#    case $option in
#        1)
#            echo "-_-"
#            ;;
#        2)
#            run
#            ;;
#        3)
#            report
#            ;;
#        4)
#            slides
#            ;;       
#        5)
#            show_report
#            ;;       
#        6)
#            show_slides
#            ;;       
#        7)
#            clean
#            ;;
#        8)
#            echo "Saliendo..."
#            ;; 
#        c)
#            clear
#            ;;         
#        *)
#            echo "Opción inválida"
#            ;;
#    esac
#
#    cd script/
#    echo # BlankedSpaces
#done
#}

# Verificar los argumentos pasados al script
if [ $# -eq 0 ]; then
  echo "Debe proporcionar al menos una opción, escribe './start_searching.sh --help' para mas opciones"
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
        --build | -b)
            build 
            ;;
#        --interactive | -i)
#            interactive
#            ;;
        *)
            echo "$option es invalido, escribe --help para mas opciones"
            exit 1
            ;;
    esac
done
