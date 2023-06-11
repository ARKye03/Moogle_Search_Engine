# -- *Bienvenido a Moogle!* --

!["Yo habia ponido mi foto aqui :("](moogle.png "Best Search Engine")

> 1er Proyecto de Programación
>
> MatCom
>
> Curso 2023-24
>
> Grupo: C-122
>
> Estudiante: Rafael A. Sánchez Martínez

## Features

- Soporta búsqueda de temas varios.
- Modo Oscuro y Modo Claro.
- Relativamente rápido, probado con 30 documentos(~40mb).
- Capacidad de uso de operadores de Inclusión ('^'), Exclusión('!') y Cercanía('~').
- Posibilidad de devolver sugerencias, una vez la consulta sea procesada y determinada incorrecta o inexistente en el Corpus.
- Muestras de pequeñas secciones de los documentos donde se haya encontrado lo solicitado.
- Muestra el Puntaje otorgado a cada documento dependiendo de lo consultado.

## Funcionamiento

---

0. Como comenzar:
    - Si usted se encuentra en Linux, ejecutar en la carpeta del proyecto desde una terminal:

        ```Shell
        make dev
        ```

    - Si usted se encuentra en Windows, ejecutar en la carpeta del proyecto desde una terminal:

        ```Shell
        dotnet watch run --project MoogleServer
        ```

1. Inicio:

   - En la parte superior derecha elegir modo.(Oscuro, OscuroSólido, ClaroSólido)
   - El programa inicia en `"Program.cs"`

    `Moogle.LetsGetStarted(@"..//Content");` Ln-5

    Esta es la función invocada presente en "Moogle.cs": Ln-13
    `public static void LetsGetStarted(string path){ corpus = new Corpus(path); }`
   - 1.1 Aqui se le da paso al motor de busqueda, que tratará de crear el Diccionario "GeneralFiler" que contendrá todas las palabras de los documentos 'MASE Corpus -> Ln4'
   - 1.2 Tambien se creará el diccionario casi más relevante del proyecto, "Docs", que almacenará cada documento con sus datos 'MASE Corpus -> Ln5'
2. Corpus:
    - 2.1 Se ejecuta el constructor de esta clase:
        - 2.1.1 - `GetInfo()`, esta función extraerá los archivos de la carpeta content y los agregará al `GeneralFiler`(VocabularioGeneral)
        - 2.1.2 - `IDF()`, esta funcion calculará el IDF de las de los documentos, llamando la funcion `IDF` de la linea 72 de esa misma clase.
        - 2.1.3 - `WW()` o Peso de la palabra, esta funcion la uso para guardar el peso de cada palabra en su documento
            - 2.1.3.1 Aquí se ejecuta la funcion `Peso()`, perteneciente a la clase `Data`(Explicada más adelante), pero no hace más que calcular el peso de cada palabra y darle valor modular al documento procesado en cuestión .
    - 2.2 En las anteriores funciones se utilizaba la función `BuildGeneralFiler`, la cual como su nombre indica, se encarga de construir en `GeneralFiler`(VocabularioGeneral), pero además procesar y desarrollar el diccionario "Docs".
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

    - 2.3.2 Como se puede ver, aquí ocurre casi toda la "magia", aquí se usan métodos propios de los diccionarios como `"ContainsKey"`, `"Add"`, etc. Aqui se va a separar el texto de cada documento por espacios y diferentes caracteres(como se observa arriba), y se va a ir almacenando cada palabra resultante en el vocabulario con sus indices, y finalmente se va a formar el Diccionario docs, con los nombres de los documentos y sus datos
        - 2.3.2.1 Estos datos se van recopilando a lo largo del bucle foreach dentro de esta funcion `BuildGeneralFiler`
    - 2.4 Data, la clase que contiene los datos de cada documento donde se calcula el peso de cada palabra en el documento que esté analizando en dicho momento, además le da valor a la variable `"Module"` del documento, que no es mas que el módulo del vector de peso

    ```C#
        public void Peso(Dictionary<string, double> GFiler){
        foreach (var par in pesos){
            pesos[par.Key] = (double)Vocabulary[par.Key].Count / (double)MaxWordAppereance * GFiler[par.Key];
            Module += Math.Pow(pesos[par.Key], 2);
        }
        Module = Math.Sqrt(Module);
    }
    ```

    - 2.4.1 Otros valores da Data
        - int MaxWordAppereance = 0; -> Frecuencia de la palabra que más aparece
        - Dictionary Vocabulary -> Este es el vocabulario del documento contra los indices de las palabras de ese vocabulario

3. MASE: Consulta(Seacher) y Puntaje(Score)

    - 3.1 El constructor de la clase, recibe la entrada del usuario(Query) desde el apartado grafico, y un Corpus(El ya creado anteriormente y creado al inicio del proyecto)
        - `UsrInp` es la consulta del usuario ya procesada, por el metodo ProcessQuery, que lo que hace no es más que separar en terminos la consulta
        - `LetMeIn`(Lista de Inclusion), `LetMeOut`(Lista de Exclusion), `Closeness`(Lista de cercanía), se encargaran de recibir los terminos de la búsqueda según los operadores colocados.
        - `GetInfo`, es la función casi que más caótica, aquí primeramente se separan los terminos segun sus operadores, posteriormente cada palabra de la consulta va para el diccionario `Frqhzy` con su cantidad de repeticiones:

    ```C#
    if (!Frqhzy.ContainsKey(UsrInp[i])){
                Frqhzy.Add(UsrInp[i], 0);
            }
            Frqhzy[UsrInp[i]] += count1 + 1;
            if (MaxWordAppereance < Frqhzy[UsrInp[i]]){
                MaxWordAppereance = Frqhzy[UsrInp[i]];
            }
        }
        UsrInp = new string[Frqhzy.Count];
        foreach (var par in Frqhzy)
        {
            UsrInp[count] = par.Key;
            count++;
        }
    ```

    - 3.1

        - Luego en la linea 43, el corpus declarado en esta clase searcher, pasa a ser el corpus enviado a consultar
        - `GSuggest`, funcion que usando la distancia de Levensthein(Aun no optimizada), sustituye las palabras mal escritas o no encontradas de la consulta, por otras posiblemente más adecuadas. Además incorpora el método `Suggestion()`
            - `Suggestion()`

            ```C#
            private static string Suggestion(string word, Corpus corpus){
            string suggestion = "";

            if (!corpus.GeneralFiler.ContainsKey(word)){
                for (int i = 1; i < word.Length / 3 + 1; i++){
                    foreach (var pair in corpus.GeneralFiler){
                        if (LevenstheinDistance(word, pair.Key) == i){ suggestion = Compare(suggestion, pair.Key, word, corpus); }
                    }
                    if (suggestion != "") return suggestion;
                }
            }
            return suggestion;
            }
            ```

            Usando la `distancia de Levensthein`(No optimizada aún), recorre palabra por palabra, para buscar por poca diferencia, la palabra más semejante de la consulta, para finalmente devolverla.
            [Mi idea es crear al inicio un diccionario extra, o varios, que abarquen todo el Vocabualrio de mis Docs y ordene por tamaño todas las palabras, asó a la hora de sugerir una palabra solo tendría que calcular con palabras 1 caracter más o menos grande, o de igual tamaño]
        - `Snippets = new string[corpus.Docs.Count]` crea el string Snippets con longitud igual a la cantidad de documentos, este string pues almacenará eso, los snippets con score != 0
        - `WVal = new double[UsrInp.Length]` es un array de dobles, que tendrá los valores de los terminos de la consulta, con capacidad == cantidad de terminos del UsrInp(Consulta).
        - `Save_W_Value()` usando la conocida formula de TF*IDF, pues calcula los valores de peso de los terminos de la consulta. MASE LN -> 57
        - `Mod()` Calculará el vector de pesos de la consulta

        ```C#
        public void Mod(){
        for (int i = 0; i < WVal.Length; i++){
            Module += Math.Pow(WVal[i], 2);
        }
        Module = Math.Sqrt(Module);
        }
        ```

        - `FillSuggest()` Metodo que modifica la sugerencia a devolver.

4. Score:
    - 4.1 Clase que guardará los puntajes de cada documento, para finalmente ser mostrado en el partado gráfico
        - `public Searcher searcher` Se declara una consulta
        - `public Corpus Corpus` Se declara un corpus
        - `public (string, double)[] tupla` Se declara este array que será de igual tamaño que la cantidad de documentos, ademas almacena sus puntajes.
    - 4.2 El constructor de esta clase
        - Comienza recibiendo y tomando una consulta y un corpus
        - Como había dicho, la tupla se crea, con igual longitud que la cantidad de documentos
        - `FillScores()` función que ordena las tuplas de mayor a menor, luego de haber ejecutado un producto vectorial con la funcion VecMultiply:

        ```C#
        public double VecMultiply(int i){
        if (!ValidateDoc(i) || searcher.Module == 0) return 0;
        double suma = 0;
        for(int j = 0; j< searcher.Frqhzy.Count;j++){
            if (!Corpus.Docs.ElementAt(i).Value.pesos.ContainsKey(searcher.Frqhzy.ElementAt(j).Key)) continue;
            suma += Corpus.Docs.ElementAt(i).Value.pesos[searcher.Frqhzy.ElementAt(j).Key] * searcher.WVal[j];
        }
        suma = suma / (searcher.Module * Corpus.Docs.ElementAt(i).Value.Module);
        return suma;
        }
        ```

        - Esta funcion de `FillScores()`, tambien da inicio a la función `FillSnippet(tupla)` que va rellenando los snippet(Ver más adelante en la sección 5), además usa un metodo para modifcar el score, llamada `ModScore()`, que depende de si alguna(s) palabra(s), perteneces a la lista de Closeness y por tanto cambia el score en dependencia de la cercania entre esos terminos. Además usa un `BubbleSort()`, el metodo de sorteo más sencillo, recorre los terminos a pares y los intercambia si estan en el lugar equivocado.
        - El metodo `ModScore()` usa además la función `LowestDistance`, que como su nombre indica devuelve la menor distancia entre dos términos en un documento.
    - 4.3 Además uso varias funciones extras como:
        - `Swap()` Simple funcion que cambia dos elementos de lugar en un array.
        - `TotalWeight()` Metodo que suma y devuelve los pesos de una palabra en cada aparicion de esta en cada documento. Se usa en la funcion `Compare()`.
        - `ValidateDoc()` Funcion que otorgará puntaje igual a 0 a aquellos documentos que contengan una palabra exluida y tambien otorgará 0 a cada documento que no contenga a una palabra incluida.
        - `Compare()` metodo presente en la clase Searcher, Ln -> 199, que se encargará de devolver entre dos palabras, la más importante usando el método `TotalWeight()`.
5. Snippets:

    - 5.1 `FillSnippet()` es la función, que se encarga de llenar los snippets de aquellos documentos que pasaron el Score(!=0)

        - Usa el hilo "Relevant" sinónimo de MASIMPORTANTE, que contendrá la palabra con la cual se presentará el Snippet más adelante.
        - Al final de la función se invoca el método `RetSnippet()`.
    - 5.2 `RetSnippet()` Esta recibirá, esa palabra "Relevant" y el documento donde se encuentre y creará un snippet que contenga esa palabra.
6. Cambios en `SearchItem` y `SearchResult`
    - 6.1 `SearchItem` Score, lo cambié de float a double.
    - 6.2 `SearchResult`
        - No declaro un objeto `SearchItem[]`, sino una `List<SearchItem> items`, del mismo nombre.
        - El constructor de esta clase ahora recibe `List<SearchItem> items`, y su sobrecarga ahora hereda una Lista igualmente.
        - La variable "Count" devuelve `this.items.Count` en lugar de `this.items.Length`

    ```C#
    public class SearchResult
    {
    -public List<SearchItem> items;-

    public SearchResult(--List<SearchItem>-- items, string suggestion="")
    {
        if (items == null) {
            throw new ArgumentNullException("items");
        }

        this.items = items;
        this.Suggestion = suggestion;
    }

    public SearchResult() : this(new --List<SearchItem>()--) {

    }

    public string Suggestion { get; private set; }

    public IEnumerable<SearchItem> Items() {
        return this.items;
    }

    public int Count { get { return --this.items.Count;-- } }
    }
    ```

7. Retornando al inicio:
    - 7.1 La clase Moogle ahora:
        - Declara un `Corpus`, el mismo que da inicio al programa, y el cual se usará para la búsqueda.
        - Declara una `Consulta(searcher)`, la cual hará todos los pasos y metodos anteriormente explicados.
        - Declara un `Score`, puntaje que una vez procesado, será mostrado en pantalla, una vez culmine la búsqueda
        - Un cronómetro(No Funcional... por ahora) que mostrará cuanto tardó la consulta (Google-like)
        - Finalmente método "Query" de tipo `SearchResult`
            - Dará inicio al cronómetro
            - Dará orden de inicio a Searcher
            - Dará orden de inicio a Score
        - Esta función "Query" devolverá los "items"(Titulo, Snippet, Score) y la sugerencia final.
    - 7.2 En Index.razor
        - Existe una nueva variable double que tendrá el valor en segundos del tiempo tomado en procesar la consulta.
        - A esta región le agregué el "Score".

        ```C#
        <ul class="results">
        @foreach (var item in result.Items()) {
            <li>
                <div class="item">
                    <p class="title">@item.Title</p>
                    <p>-->  @item.Snippet ..</p>
                    <p>-->  @item.Score ...</p>
                </div>
            </li>
        }
        </ul>
        ```

        - Debajo de la sugerencia irá el tiempo tomado. (Se me ocurrió a ultima hora).
8. Fin. 😁😁😁
