namespace Dominio;

public class Armas
{
    public string _nombre { get; private set; } = string.Empty;
    public int _potencia { get; private set; }

    private Armas(string nombre, int potencia)
    {
        _nombre = nombre;
        _potencia = potencia;
    }

    public static Armas Create(string nombre, int potencia)
    {
        if(string.IsNullOrWhiteSpace(nombre))
            throw new Exception("El nombre del arma no puede estar vacio");

        if (nombre.Length > 100)
            throw new Exception("Supera la capacidad de caracteres");

        if (potencia < 0)
            throw new Exception("No puede ser negativo");

        return new Armas(nombre, potencia);
    }
}
