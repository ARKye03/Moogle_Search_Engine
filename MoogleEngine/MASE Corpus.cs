namespace MoogleEngine;
public class Corpus{
    string Path = ""; // Directorio content(por lo general)
    public Dictionary<string, double> GeneralFiler = new Dictionary<string, double>(); //Uno para todos, y todas para el diccionario ^~^
    public Dictionary<string, Data> Docs = new Dictionary<string, Data>(); // Diccionario que almacena el nombre de cada documento y sus datos
    
    public Corpus(string path){
        this.Path = path;
        GetInfo(Directory.GetFiles(path, "*.txt"));
        //BuildGeneralFiler(path);
        IDF();
        WW(); //WeightWord
    }
    private void WW(){
        foreach (var par in Docs){
            par.Value.Peso(GeneralFiler);
        }
    }  
    private void IDF(){
        foreach (var par in GeneralFiler){
            GeneralFiler[par.Key] = IDF(par.Key);
        }
    }
    private void GetInfo(string[] files){
        for (int i = 0; i < files.Length; i++){
            BuildGeneralFiler(files[i], i);
        }
    }

    //Calcular el Tf de todas las palabras al inicio del pograma
    /*private void TF(string[] words){
        //Diccionario que almacena el valor tf de cada palabra
        Dictionary<string, double> tf = new Dictionary<string, double>();
        foreach (string word in words)
        {
            if (tf.ContainsKey(word))
                tf[word]++;
            else
                tf[word] = 1;
        }
        int totalWords = words.Length;
        foreach (string word in tf.Keys.ToList()){
            tf[word] = tf[word] / totalWords;
        }
    }*/

    /*static void BuildGeneralFiler(string path){ //Add all the words of all files in the path
        // Leer todos los archivos del directorio
        foreach (string archivo in Directory.GetFiles(path)){
            // Leer todas las palabras del archivo y agregarlas al GeneralFiler
            foreach (string palabra in File.ReadAllText(archivo).Split(new char[] { ' ', ',', '.', ';', '?', '!', '¿', '¡', ':', '"' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!GeneralFiler.ContainsKey(palabra))
                    GeneralFiler[palabra] = 1;
                
                else
                    GeneralFiler[palabra]++;
            }
        }*
    }*/
    /*double IDF(string palabra) // obvio lo que hace esto no?... no
    {
        int count = 0;
        foreach (var par in Docs)
        {
            if (!par.Value.Vocabulary.ContainsKey(palabra)) continue;
            count++;
        }
        return Math.Log10((double)Docs.Count / (double)count);
    }*/
    //Function that calculates the IDF of a word in a document
    double IDF(string word){
        int count = 0;
        foreach (var par in Docs){
            if (!par.Value.Vocabulary.ContainsKey(word)) continue;
            count++;
        }
        return Math.Log10((double)Docs.Count / (double)count);
    }
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
}


/*private void BuildGeneralFiler(string nombre, int i){   
        Data data = new Data();
        // Index count of each word
        int count = 0; 
        // Splitting with signos de puntuacion

        string txt = File.ReadAllText(Directory.GetFiles(Path, "*.txt")[i]).ToLower();
        string[] palabras = txt.Split(new char[] { ' ', ',', '.', ';', '?', '!', '¿', '¡', ':', '"' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in palabras)
        {
            if (word.Length == 1 && Char.IsPunctuation(word[0])) continue;
        // aqui voy guardando cada una de las palabras en el Vocabulary del documento con sus indices
            if (!data.Vocabulary.ContainsKey(word))
            {
                data.Vocabulary.Add(word, new List<int>());
                data.pesos.Add(word, 0);
                if (!GeneralFiler.ContainsKey(word))
                {
                    GeneralFiler.Add(word, 0);
                }
            }
            data.Vocabulary[word].Add(count);
            if (data.Vocabulary[word].Count > data.MaxWordAppereance)
            {
                data.MaxWordAppereance = data.Vocabulary[word].Count;
            }
            count++;
        }
        Docs.Add(nombre, data);
    }*/
    /*private void BuildGeneralFiler(string filename, int index){   
        var data = new Data();
        var count = 0; // Count of each word's index
        var text = File.ReadAllText(Directory.GetFiles(Path, "*.txt")[index]).ToLower();
        var words = text.Split(new char[] { ' ', ',', '.', ';', '?', '!', '¿', '¡', ':', '"' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words){
            if (word.Length == 1 && Char.IsPunctuation(word[0])) continue;
            // Add word to document vocabulary with its index
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
    Docs.Add(filename, data);
}*/