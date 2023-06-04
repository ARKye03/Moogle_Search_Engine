namespace MoogleEngine;
public class Corpus
{
    string Path = ""; // Directorio content(por lo general)
    public Dictionary<string, double> GeneralFiler = new Dictionary<string, double>(); //Uno para todos, y todas para el diccionario ^~^
    public Dictionary<string, Data> Docs = new Dictionary<string, Data>(); // Diccionario que almacena el nombre de cada documento y sus datos

    public Corpus(string path)
    {
        this.Path = path;
        GetInfo(Directory.GetFiles(path, "*.txt"));
        //BuildGeneralFiler(path);
        IDF();
        WW(); //WeightWord
    }
    private void WW()
    {
        foreach (var par in Docs)
        {
            par.Value.Peso(GeneralFiler);
        }
    }
    private void IDF()
    {
        foreach (var par in GeneralFiler)
        {
            GeneralFiler[par.Key] = IDF(par.Key);
        }
    }
    private void GetInfo(string[] files)
    {
        for (int i = 0; i < files.Length; i++)
        {
            BuildGeneralFiler(files[i], i);
        }
    }
    //Function that calculates the IDF of a word in a document
    double IDF(string word)
    {
        int count = 0;
        foreach (var par in Docs)
        {
            if (!par.Value.Vocabulary.ContainsKey(word)) continue;
            count++;
        }
        return Math.Log10((double)Docs.Count / (double)count);
    }
    private void BuildGeneralFiler(string nombre, int i)
    {
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
    }
}