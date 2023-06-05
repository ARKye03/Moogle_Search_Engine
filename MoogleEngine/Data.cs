namespace MoogleEngine;
// Datos de los documentos
// Se almacenará el peso de cada palabra del documento, resumidamente
public class Data
{
    public Dictionary<string, List<int>> Vocabulary = new Dictionary<string, List<int>>(); // Este es el vocabulario del documento contra los indices de las palabras de dicho vocabulario
    public int MaxWordAppereance = 0; // Max word frzhcy 
    public double Module = 0; // Modulo del vector de pesos -> Valor |pesos|
    public Dictionary<string, double> pesos = new Dictionary<string, double>(); // Peso de cada palabra en el documento
    public Data() { } //Bob
    public void Peso(Dictionary<string, double> GFiler)
    { // Este es el metodo q calcula el peso de cada palabra en el documento y le da valor al Module del documento
        foreach (var par in pesos)
        {
            pesos[par.Key] = (double)Vocabulary[par.Key].Count / (double)MaxWordAppereance * GFiler[par.Key];
            Module += Math.Pow(pesos[par.Key], 2);
        }
        Module = Math.Sqrt(Module);
    }
}