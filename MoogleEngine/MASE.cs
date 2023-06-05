//MASE - Moogle Advance Search Engine
//Aqui proceso la searcher    \^-^/
//                              |
//                             / \ 
using System.Text.RegularExpressions;

namespace MoogleEngine
{
    public class Searcher
    {
        //Splitted Query
        public string[] UsrInp;
        //Words values for Splitted Query
        public double[] WVal;
        //Evidently
        public Corpus corpus;
        //Snippets
        public string[] Snippets;
        //Repeticiones de una palabra
        public Dictionary<string, int> Frqhzy = new Dictionary<string, int>();
        // Max word frqhzy
        public int MaxWordAppereance = 0;
        // Module del vector de WVal -> Valor |WVal|
        public double Module = 0; 
        // Listas que varian en dependencia de los operadores opcionales colocados en la consulta
        public List<string> LetMeIn;
        public List<string> LetMeOut;
        public List<(string, string)> Closeness;

        //Hilo que guarda la sugerencia
        public string? _suggestion;

        public Searcher(string Query, Corpus corpus)
        {
            UsrInp = ProcessQuery(Query);
            LetMeIn = new List<string>();
            LetMeOut = new List<string>();
            Closeness = new List<(string, string)>();
            GetInfo(UsrInp);
            this.corpus = corpus;
            GSuggest(Frqhzy, corpus);
            Snippets = new string[corpus.Docs.Count];
            WVal = new double[UsrInp.Length];
            Save_W_Value();
            Mod();
            FillSuggest();
        }
        //Split query by the pieces
        static string[] ProcessQuery(string UI)
        {
            string[] PUI = UI.Split(new char[] { ' ', ',', '.', ';', '?', '¿', '¡', ':', '"' }, StringSplitOptions.RemoveEmptyEntries);
            return PUI;
        }
        // Splitted Query weight values TF * IDF
        public void Save_W_Value()
        {
            int count = 0;
            foreach (var par in Frqhzy)
            {
                if (corpus.GeneralFiler.ContainsKey(par.Key))
                {
                    WVal[count] = (double)Frqhzy[par.Key] / (double)MaxWordAppereance * corpus.GeneralFiler[par.Key];
                }
                else
                {
                    WVal[count] = 0;
                }
                count++;
            }
        }
        //Vector de pesos de la searcher/consulta
        public void Mod()
        {
            for (int i = 0; i < WVal.Length; i++)
            {
                Module += Math.Pow(WVal[i], 2);
            }
            Module = Math.Sqrt(Module);
        }
        //Me apodero de los datos de la searcher/consulta
        public void GetInfo(string[] UsrInp)
        {
            int count = 0;
            for (int i = 0; i < UsrInp.Length; i++)
            {
                int count1 = 0;

                if (UsrInp[i][0] == '!')
                {
                    UsrInp[i] = UsrInp[i].Substring(1);
                    LetMeOut.Add(UsrInp[i]);
                }
                if (UsrInp[i][0] == '^')
                {
                    UsrInp[i] = UsrInp[i].Substring(1);
                    LetMeIn.Add(UsrInp[i]);
                }
                while (UsrInp[i][0] == '*')
                {
                    UsrInp[i] = UsrInp[i].Substring(1);
                    count1++;

                }
                if (UsrInp[i] == "~")
                {
                    Closeness.Add((UsrInp[i - 1], UsrInp[i + 1]));
                    continue;
                }
                // Hasta aquí el método lo que hace es añadir las palabras con operadores a sus listas correspondientes.
                //-------------------------------------------------------------------------------------------------------\\
                // Cada palabra de la searcher va para el diccionario Frqhzy(frqhzy) con su cantidad de repeticiones

                if (!Frqhzy.ContainsKey(UsrInp[i]))
                {
                    Frqhzy.Add(UsrInp[i], 0);
                }
                Frqhzy[UsrInp[i]] += count1 + 1;
                if (MaxWordAppereance < Frqhzy[UsrInp[i]])
                {
                    MaxWordAppereance = Frqhzy[UsrInp[i]];
                }
            }
            UsrInp = new string[Frqhzy.Count];
            foreach (var par in Frqhzy)
            {
                UsrInp[count] = par.Key;
                count++;
            }
        }
        public void FillSnippet((string, double)[] tupla)
        {
            for (int i = tupla.Length - 1; i > -1 && tupla[i].Item2 != 0; i--)
            {
                string Relevant = ""; // esta variable contendra el termino mas importante de los presentes en el documento i, el cual estara presente en el snippet
                for (int j = 0; j < Frqhzy.Count; j++)
                {
                    if (!corpus.Docs[tupla[i].Item1].pesos.ContainsKey(Frqhzy.ElementAt(j).Key)) continue;
                    Relevant = Frqhzy.ElementAt(j).Key;
                }
                for (int j = 0; j < Frqhzy.Count; j++)
                {
                    if (!corpus.Docs[tupla[i].Item1].pesos.ContainsKey(Frqhzy.ElementAt(j).Key)) continue;
                    if (corpus.Docs[tupla[i].Item1].pesos[Frqhzy.ElementAt(j).Key] > corpus.Docs[tupla[i].Item1].pesos[Relevant])
                    {
                        Relevant = Frqhzy.ElementAt(j).Key;
                    }
                }
                string txt = File.ReadAllText(tupla[i].Item1).ToLower();
                string[] palabras = txt.Split(new char[] { ' ' });
                if (Relevant == "") return;
                // Una vez determinado el termino mas importante, se crea un snippet donde se encuentre
                Snippets[i] = RetSnippet(txt, Relevant);
            }
        }
        public string RetSnippet(string txt, string Relevant)
        {
            while (true)
            {
                // Esta condicion es para seleccionar un snippet que contenga a la palabra mas importante
                if ((!(Char.IsPunctuation(txt[txt.IndexOf(Relevant) - 1])) && !(txt[txt.IndexOf(Relevant) - 1] == ' ')) || (!(Char.IsPunctuation(txt[txt.IndexOf(Relevant) + Relevant.Length])) && !(txt[txt.IndexOf(Relevant) + Relevant.Length] == ' ')))
                {
                    txt = txt.Substring(txt.IndexOf(Relevant) + Relevant.Length);
                    continue;
                }
                if (txt.IndexOf(Relevant) < 50)
                {
                    return txt.Substring(0, 100);
                }
                if (txt.IndexOf(Relevant) > txt.Length - 50)
                {
                    return txt.Substring(txt.Length - 100);
                }
                return txt.Substring(txt.IndexOf(Relevant) - 50, 100);
            }
        }
        public void GSuggest(Dictionary<string, int> Frequency, Corpus corpus)
        {
            for (int i = 0; i < UsrInp.Length; i++)
            {
                if (Suggestion(UsrInp[i], corpus) == "") continue;

                else
                {
                    if (Frequency.ContainsKey(UsrInp[i]))
                    {
                        Frequency.Add(Suggestion(UsrInp[i], corpus), Frequency[UsrInp[i]]);  /*------*/
                        Frequency.Remove(UsrInp[i]);
                    }
                    UsrInp[i] = Suggestion(UsrInp[i], corpus);
                }
            }
        }
        // A tirar la casa por la ventana, no hay más, palabra a palabra para sugerir lo mejor.
        private static string Suggestion(string word, Corpus corpus)
        {
            string suggestion = "";

            if (!corpus.GeneralFiler.ContainsKey(word))
            {
                for (int i = 1; i < word.Length / 3 + 1; i++)
                {
                    foreach (var pair in corpus.GeneralFiler)
                    {
                        if (LevenstheinDistance(word, pair.Key) == i) { suggestion = Compare(suggestion, pair.Key, word, corpus); }
                    }
                    if (suggestion != "") return suggestion;
                }
            }
            return suggestion;
        }
        // Entre dos palabras devuelve la mas relevante
        private static string Compare(string suggestion, string newword, string word, Corpus corpus)
        {
            if (suggestion == "") suggestion = newword;

            if (Score.TotalWeight(suggestion, corpus) < Score.TotalWeight(newword, corpus)) suggestion = newword;

            return suggestion;
        }
        public void FillSuggest()
        {
            for (int i = 0; i < UsrInp.Length; i++)
            {
                _suggestion = _suggestion + UsrInp[i] + " ";
            }
        }
        //Method that calculates the levensthein distance between two words
        static int LevenstheinDistance(string s1, string s2)
        {
            int n = s1.Length;
            int m = s2.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }
    //Esta clase almacenará los puntajes de cada documento, para luego ser mostrado
    public class Score
    {
        // Searcher
        public Searcher searcher;
        // Corpus
        public Corpus Corpus;
        //Array de tamaño == cantidad de documentos y sus puntajes
        public (string, double)[] tupla;

        // Otorgamiento de scores a los respectivos documentos
        public Score(Searcher searcher, Corpus corpus)
        {
            this.searcher = searcher;
            this.Corpus = corpus;
            tupla = new (string, double)[corpus.Docs.Count];
            FillScores();
        }
        // Puntaje\Valor resultante del producto del vector searcher y el vector de los documentos
        // Tuplas de mayor a menor
        public void FillScores()
        {
            for (int i = 0; i < Corpus.Docs.Count; i++)
            {
                tupla![i].Item2 = VecMultiply(i);
                tupla[i].Item1 = Corpus.Docs.ElementAt(i).Key;
                ModScore(i);
            }

            BubbleSort(tupla);
            if (tupla[tupla.Length - 1].Item2 != 0)
            {
                searcher.FillSnippet(tupla);
            }

        }
        public double VecMultiply(int i)
        {
            if (!ValidateDoc(i) || searcher.Module == 0) return 0;
            double suma = 0;
            for (int j = 0; j < searcher.Frqhzy.Count; j++)
            {
                if (!Corpus.Docs.ElementAt(i).Value.pesos.ContainsKey(searcher.Frqhzy.ElementAt(j).Key)) continue;
                suma += Corpus.Docs.ElementAt(i).Value.pesos[searcher.Frqhzy.ElementAt(j).Key] * searcher.WVal[j];
            }
            suma = suma / (searcher.Module * Corpus.Docs.ElementAt(i).Value.Module);
            return suma;
        }
        // Metodo que organiza elementos de menor a mayor
        public static void BubbleSort((string, double)[] tupla)
        {
            for (int i = 0; i < tupla.Length; i++)
                for (int j = 0; j < tupla.Length - 1; j++)
                    if (tupla[j].Item2 > tupla[j + 1].Item2)
                        Swap(tupla, j, j + 1);
        }
        // Intercambio de 2 elementos de un array, clásico Swap
        private static void Swap((string, double)[] tupla, int a, int b)
        {
            (string, double) tmp = tupla[a];
            tupla[a] = tupla[b];
            tupla[b] = tmp;
        }

        // Devuelve la suma de los pesos de una palabra en cada uno de los documentos
        public static double TotalWeight(string word, Corpus corpus)
        {
            double totalweight = 0;

            for (int i = 0; i < corpus.Docs.Count; i++) { totalweight += (corpus.Docs.ElementAt(i).Value.pesos.ContainsKey(word)) ? corpus.Docs.ElementAt(i).Value.pesos[word] : 0; }

            return totalweight;
        }
        public bool ValidateDoc(int i)
        {
            for (int j = 0; j < searcher.LetMeOut.Count; j++)
            {
                if (Corpus.Docs.ElementAt(i).Value.pesos.ContainsKey(searcher.LetMeOut[j])) { return false; }
            }
            for (int j = 0; j < searcher.LetMeIn.Count; j++)
            {
                if (!Corpus.Docs.ElementAt(i).Value.pesos.ContainsKey(searcher.LetMeIn[j])) { return false; }
            }
            return true;
        }

        // Si existe algun par de palabras en la lista de Closeness, modifica el score de los documentos dependiendo de la cercania entre los terminos que pertenecen a dicha lista
        public void ModScore(int i)
        {
            if (searcher.Closeness.Count == 0) return;
            for (int j = 0; j < searcher.Closeness.Count; j++)
            {
                if (!Corpus.Docs.ElementAt(i).Value.pesos.ContainsKey(searcher.Closeness[j].Item1) || !Corpus.Docs.ElementAt(i).Value.pesos.ContainsKey(searcher.Closeness[j].Item2))
                {
                    tupla[i].Item2 = tupla[i].Item2 / Corpus.Docs.ElementAt(i).Value.pesos.Count;
                    return;
                }
            }
            // Si alguna de las 2 palabras que poseen un operador de Closeness entre ellas no se encuentra en un determinado documento el score de este documento sera dividido entre el largo de este

            // Si aparecen ambas, el score sera dividido por la menor distancia entre ellas
            List<int> a = new List<int>();
            List<int> b = new List<int>();
            for (int j = 0; j < searcher.Closeness.Count; j++)
            {
                a = Corpus.Docs.ElementAt(i).Value.Vocabulary[searcher.Closeness[j].Item1];
                b = Corpus.Docs.ElementAt(i).Value.Vocabulary[searcher.Closeness[j].Item2];
                int menordist = LowestDistance(a, b);
                tupla[i].Item2 = tupla[i].Item2 / menordist;
            }
        }
        private int LowestDistance(List<int> a, List<int> b)
        {
            int i = 0; int j = 0;
            int min = Math.Abs(a[0] - b[0]);
            while (i + j < a.Count + b.Count - 2)
            {
                if (j == b.Count - 1 || (i < a.Count - 1 && a[i] < b[j])) i++;
                else j++;
                min = Math.Min(min, Math.Abs(a[i] - b[j]));
            }
            return min;
        }
    }
}