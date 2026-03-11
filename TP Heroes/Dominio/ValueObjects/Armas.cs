using Dominio.Common;

namespace Dominio.ValueObjects;

public class Armas : ValueObject
{
    public string _nombre { get; } = string.Empty;
    public int _potencia { get; }

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

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return _potencia;
        yield return _nombre;
    }
}
