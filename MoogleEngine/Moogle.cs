using System.Diagnostics;

namespace MoogleEngine;


public static class Moogle
{
    public static Corpus? corpus;
    public static Searcher? searcher;
    public static Score? score;
    public static Stopwatch Timer = new Stopwatch();
    public static double Time = Timer.ElapsedMilliseconds/1000;
    public static void LetsGetStarted(string path){ corpus = new Corpus(path);  } //Start
    public static SearchResult Query(string query) {
        Timer.Start();
        // Modifiqué este método para responder a la búsqueda
        searcher = new Searcher(query, corpus!);
        score = new Score(searcher, corpus!);

        List<SearchItem> items = new List<SearchItem>();

        for (int i = score.tupla.Length - 1; i > -1 && score.tupla[i].Item2 != 0; i--){
            items.Add(new SearchItem(score.tupla[i].Item1.Substring(12), searcher.Snippets[i], score.tupla[i].Item2));
        }
        Timer.Stop();
        return new SearchResult(items, searcher._suggestion!);
    }
    
}
