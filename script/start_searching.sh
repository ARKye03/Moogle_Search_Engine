#!/bin/bash

# Use zsh instead
if [ -n "$ZSH_VERSION" ]; then
    exec zsh "$0" "$@"
fi
#Vars
folder_path="../Content"
file_count=$(find "$folder_path" -type f | wc -l)
visore=""
# Run Moogle
run() {
if [[ $file_count -gt 1 ]]; then
  if [[ $OSTYPE == "linux-gnu" || $OSTYPE == "linux" ]]; then #Using Linux
    echo "Linux detected"
    dotnet watch run --project ../MoogleServer
    #make dev
  else
    echo "Windows detected... or something else..." #Using Windows or MacOS
    dotnet watch run --project ../MoogleServer
  fi
else
  echo "La carpeta 'Content' contiene al parecer un solo archivo, coloque más archivos en la carpeta, y ejecute nuevamente el script"
fi
}
build() {
if [[ $OSTYPE == "linux-gnu" || $OSTYPE == "linux" ]]; then #Using Linux
  echo "Linux detected"
  dotnet build ../
  #make builda
else
  echo "Windows detected... or something else..." #Using Windows or MacOS
  dotnet build ../
fi
}
# Función para compilar y generar el PDF del informe
report() {
  echo "Compilando y generando el PDF del informe..."
  cd ../Informe && pdflatex Informe.tex && cd ../script/
  if [ $? -eq 0 ]; then
    echo "pdflatex succesfull"
    read -n 1 -s -r -p "Press any key to continue..."
  else
    echo "pdflatex falló, probando latexmk"
    cd ../Informe && latexmk -c Informe.tex && cd ../script/
    read -n 1 -s -r -p "Press any key to continue..."
  fi
}
# Función para compilar y generar el PDF de presentación
slides() {
  echo "Compilando y generando PDF de la presentación..."
  cd ../Presentacion && pdflatex Presentacion.tex && cd ../script/
  if [ $? -eq 0 ]; then
    echo "pdflatex succesfull"
    read -n 1 -s -r -p "Press any key to continue..."
  else
    echo "pdflatex falló, probando latexmk"
    cd ../Presentacion && latexmk -c Presentacion.tex && cd ../script/
    read -n 1 -s -r -p "Press any key to continue..."
  fi
}
# Función para visualizar el informe
show_report() {
  if [ ! -f "../Informe/Informe.pdf" ]; then
      report
  fi
  
  if [ $# -eq 1 ]; then
    visore="$1"
    echo "Leyendo visualizador"
    $visore ../Informe/Informe.pdf
  else
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
      echo "Linux Detected"
      xdg-open ../Informe/Informe.pdf 
    elif [[ "$OSTYPE" == "darwin"* ]]; then
      echo "macOS Detected"
      open ../Informe/Informe.pdf 
    elif [[ "$OSTYPE" == "msys"* ]]; then
      echo "Windows Detected"
      start ../Informe/Informe.pdf 
    else
      echo "No se pudo determinar el sistema operativo"
    fi
  fi
}
# Función para visualizar presentación
show_slides() {
  if [ ! -f "../Presentacion/Presentacion.pdf" ]; then
      slides
  fi
  if [ $# -eq 1 ]; then
    visore="$1"
    echo "Leyendo visualizador"
    $visore ../Presentacion/Presentacion.pdf
  else
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
      echo "Linux Detected"
      xdg-open ../Presentacion/Presentacion.pdf 
    elif [[ "$OSTYPE" == "darwin"* ]]; then
      echo "macOS Detected"
      open ../Presentacion/Presentacion.pdf 
    elif [[ "$OSTYPE" == "msys"* ]]; then
      echo "Windows Detected"
      start ../Presentacion/Presentacion.pdf 
    else
      echo "No se pudo determinar el sistema operativo :("
      read -n 1 -s -r -p "Press any key to continue..."
    fi
  fi
}
clean(){
  echo "Limpiando archivos extras..."
  find ../ -type f -name "*.aux" -exec rm -v {} +
  find ../ -type f -name "*.fdb_latexmk" -exec rm -v {} +
  find ../ -type f -name "*.fls" -exec rm -v {} +
  find ../ -type f -name "*.log" -exec rm -v {} +
  find ../ -type f -name "*.synctex.gz" -exec rm -v {} +
  find ../ -type f -name "*.pdf" -exec rm -v {} +
  find ../ -type f -name "*.nav" -exec rm -v {} +
  find ../ -type f -name "*.out" -exec rm -v {} +
  find ../ -type f -name "*.snm" -exec rm -v {} +
  find ../ -type f -name "*.toc" -exec rm -v {} +
  echo "Eliminando los ficheros objeto :|"
  rm -v -r ../MoogleEngine/obj/ 
  rm -v -r ../MoogleEngine/bin/ 
  rm -v -r ../MoogleServer/obj/ 
  rm -v -r ../MoogleServer/bin/

  sleep 1
  echo "Limpieza completada"
  read -n 1 -s -r -p "Press any key to continue..."
  clear
}
help(){
  clear
  echo "Estas son tus opciones, pásalas como argumentos al script:"
  echo "----------------------------------------------------------"
  echo "--help:                 |-h   |  Ayuda"
  echo "--run:                  |-r   |  Compila y ejectutar el proyecto"
  echo "--report:               |-re  |  Compilar y generar el pdf del proyecto latex relativo al informe"
  echo "--slides:               |-sl  |  Compilar y generar el pdf del proyecto latex relativo a la presentación"
  echo "--show_report <args>:   |-sr  |  Visualizar el informe <opcional>"
  echo "--show_slides <args>:   |-ss  |  Visualizar la presentación <opcional>"
  echo "--clean:                |-c   |  Eliminar todos los ficheros auxiliares. Do it at my own risk :)"
  echo "--build:                |-b   |  Compila el proyecto"
  echo "--interactive           |-i   |  Interactuar con el script"
}

interactive(){
  clear
  option=""
while [[ $option != "salir" ]]; do
    # Mostrar las opciones disponibles
    echo "Estas son tus opciones:"
    echo "-----------------------"
    echo "(1) - help:         |  Ayuda"
    echo "(2) - run:          |  Ejectutar el proyecto"
    echo "(3) - report:       |  Compilar y generar el pdf del proyecto latex relativo al informe"
    echo "(4) - slide:        |  Compilar y generar el pdf del proyecto latex relativo a la presentación"
    echo "(5) - show_report:  |  Visualizar el informe"
    echo "(6) - show_slide:   |  Visualizar la presentación"
    echo "(7) - clean:        |  Eliminar todos los ficheros auxiliares. Do it at my own risk :)"
    echo "(8) - build:        |  Compila el proyecto"
    echo "----------------------------------------------------------------------------------------------"
    echo "q - Salir:"
    echo "c - Clear"
#    pwd 
    # Leer la opción seleccionada por el usuario
    read -n 1 -s -r -p "Selecciona una opción: " option

    # Ejecutar la acción correspondiente a la opción seleccionada
    case $option in
        1)
            clear
            echo "-_-"
            ;;
        2)
            run
            ;;
        3)
            report
            ;;
        4)
            slides
            ;;       
        5)  
            echo
            read -p "Con que lo deseas abrir: (Default=Aplicacion Predeterminada)" arg
            show_report $arg
            ;;       
        6)
            echo
            read -p "Con que lo deseas abrir: (Default=Aplicacion Predeterminada)" arg
            show_slides $arg
            ;;       
        7)
            clean
            ;;
        8)
            build
            ;;
        q)
            echo "Saliendo..."
            exit
            ;; 
        c)
            clear
            ;;         
        *)
            echo "Opción inválida"
            ;;
    esac
    
#    pwd
    echo # BlankedSpaces
done
}

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
            shift
            show_report "$@"
            ;;
        --show_slides | -ss)
            shift
            show_slides "$@"
            ;;
        --build | -b)
            build 
            ;;
        --interactive | -i)
            interactive
            ;;
        *)
            echo "$option es invalido, escribe --help para mas opciones"
            exit 1
            ;;
    esac
done
