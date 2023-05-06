namespace MoogleEngine;



public class Corpus{
    string Path = "";

    //Uno para todos, y todas para el diccionario ^~^
    public static Dictionary<string, double> GeneralFiler = new Dictionary<string, double>();

    public Dictionary<string, Data> Docs = new Dictionary<string, Data>();
    public Corpus(string path){
        this.Path = path;
        //GetInfo(Directory.GetFiles(path, ".txt"));
        BuildGeneralFiler(path);
    }

    /*static void GetInfo(string[] files){
        for (int i = 0; i < files.Length; i++){
            BuildGeneralFiler(files[i], i);
        }
    }*/

    //Calcular el Tf de todas las palabras al inicio del pograma
    static void TF(string[] words){
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
    }





    static void BuildGeneralFiler(string path){ //Add all the words of all files in the path
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
        }
    }
    double IDF(string palabra) // obvio lo que hace esto no?... no
    {
        int suma = 0;
        foreach (var par in Docs)
        {
            if (!par.Value.vocabulario.ContainsKey(palabra)) continue;
            suma++;
        }
        return Math.Log10((double)Docs.Count / (double)suma);
    }
}