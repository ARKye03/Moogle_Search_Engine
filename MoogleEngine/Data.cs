namespace MoogleEngine;
// Datos de los documentos

public class Data
{
    // este es el vocabulario del documento contra los indices de las palabras de dicho vocabulario
    public Dictionary<string, List<int>> vocabulario = new Dictionary<string, List<int>>();
    
    // Max word frzhcy
    public int MaxWordAppereance = 0;
    
    // esta propiedad represente el valor del Module del vertor pesos del documento             Valor |pesos|
    public double Module = 0;
    
    // Peso de cada palabra en el documento
    public Dictionary<string, double> pesos = new Dictionary<string, double>();

    public Data(){ }
    
    // este es el metodo q calcula el peso de cada palabra en el documento y le da valor al Module del documento
    public void Peso(Dictionary<string, double> vocabulariogeneral)
    {
        foreach (var par in pesos)
        {
            pesos[par.Key] = (double)vocabulario[par.Key].Count / (double)MaxWordAppereance * vocabulariogeneral[par.Key];
            Module += Math.Pow(pesos[par.Key], 2);
        }
        Module = Math.Sqrt(Module);
    }
}