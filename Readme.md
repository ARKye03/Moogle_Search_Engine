# -- Bienvenido a MASE! --

>--Moogle Advanced Search Engine--

![](moogle.png)

> 1er Proyecto de Programación
> MatCom
> Curso 2023-24
> Grupo: C-122
> Estudiante: Rafael A. Sánchez Martínez

## Features

- Soporta búsqueda de temas varios
- Relativamente rápido, probado con 30 documentos(~40mb).
- Menos de 1gb de memoria RAM de uso(Con los 30 docs, probado en Arch Linux).
- Capacidad de uso de operadores de Inclusión ('^'), Exclusién('!') y Cercanía('~').
- Posibilidad de devolver sugerencias, una vez la consulta sea procesada y determinada incorrecta o inexistente en el Corpus.
- Muestras de pequeñas secciones de los documentos donde se haya encontrado lo solicitado.
- Muestra el Puntaje otorgado a cada documento dependiendo de lo consultado.

### Funcionamiento

1. Primeramente el programa inicia en "Program.cs"

    `Moogle.LetsGetStarted(@"..//Content");` Ln-5
    
    Esta es la función invocada presente en "Moogle.cs"
    `public static void LetsGetStarted(string path){ corpus = new Corpus(path); }`
   - 1.1 Aqui se le da paso al motor de busqueda, que tratará de crear el Diccionario "GeneralFiler" que contendrá todas las palabras de los documentos 'MASE Corpus -> Ln4'
   - 1.2 Tambien se creará el diccionario casi más relevante del proyecto, "Docs", que almacenará cada documento con sus datos 'MASE Corpus -> Ln4'
2. Corpus
