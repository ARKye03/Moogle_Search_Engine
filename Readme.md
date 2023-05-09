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

    Esta es la función invocada presente en "Moogle.cs": Ln-13
    `public static void LetsGetStarted(string path){ corpus = new Corpus(path); }`
   - 1.1 Aqui se le da paso al motor de busqueda, que tratará de crear el Diccionario "GeneralFiler" que contendrá todas las palabras de los documentos 'MASE Corpus -> Ln4'
   - 1.2 Tambien se creará el diccionario casi más relevante del proyecto, "Docs", que almacenará cada documento con sus datos 'MASE Corpus -> Ln5'
2. Corpus
    - 2.1 Se ejecuta el constructor de esta clase:
        - 2.1.1 - GetInfo(), esta función extraerá los archivos de la carpeta content y los agregará al GeneralFiler(VocabularioGeneral)
        - 2.1.2 - IDF(), esta funcion calculará el IDF de las de los documentos, llamando la funcion IDF de la linea 72 de esa misma clase.
        - 2.1.3 - WW() o Peso de la palabra, esta funcion la uso para guardar el peso de cada palabra en su documento
            - 2.1.3.1 Aquí se ejecuta la funcion Peso(), perteneciente a la clase Data(Explicada más adelante), pero no hace más que calcular el peso de cada palabra y darle valor modular al documento procesado en cuestión .
    - 2.2 En las anteriores funciones se utilizaba la función BuildGeneralFiler, la cual como su nombre indica, se encarga de construir en GeneralFiler(VocabularioGeneral), pero además procesar y desarrollar el diccionario "Docs".
        - 2.2.1 Aqui creo el objeto data `Data data = new Data();` y el conteo que indica cada palabra `int count = 0`
        
        Esta zona del código fue un descubrimiento excepcional, estoy orgulloso de ello, horas en la página de Microsoft(no es broma)

        ```C#
        private void BuildGeneralFiler(string nombre, int i){   
        Data data = new Data();
        // Index count of each word
        int count = 0; 
        // Splitting with signos de puntuacion

        string txt = File.ReadAllText(Directory.GetFiles(Path, "*.txt")[i]).ToLower();
        string[] palabras = txt.Split(new char[] { ' ', ',', '.', ';', '?', '!', '¿', '¡', ':', '"' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in palabras){
            if (word.Length == 1 && Char.IsPunctuation(word[0])) continue;
            // aqui voy guardando cada una de las palabras en el Vocabulary del documento con sus indices
            if (!data.Vocabulary.ContainsKey(word)){
                data.Vocabulary.Add(word, new List<int>());
                data.pesos.Add(word, 0);
                if (!GeneralFiler.ContainsKey(word)){
                    GeneralFiler.Add(word, 0);
                }
            }
            data.Vocabulary[word].Add(count);
            if (data.Vocabulary[word].Count > data.MaxWordAppereance){
                data.MaxWordAppereance = data.Vocabulary[word].Count;
            }
            count++;
        }
        Docs.Add(nombre, data);
    }
    ```
