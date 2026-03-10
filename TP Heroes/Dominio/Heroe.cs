using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dominio;

public class Heroe : Individuo
{
    public Poder _poder { get; private set; }

    public Heroe(string nombre, string apodo, int nivelEntrenamiento, Poder poder) 
        : base(nombre, apodo)
    {
        _poder = poder;
    }
}
