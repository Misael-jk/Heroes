using Dominio.Common;

namespace Dominio.ValueObjects;

public class Apodo : ValueObject
{
    public string Valor { get; private set; }

    private Apodo(string valor)
    {
        Valor = valor;
    }

    public static Apodo Create(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("El apodo no puede estar vacio");

        if (valor.Length > 100)
            throw new Exception("Supera la capacidad de caracteres");

        return new Apodo(valor);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }
}
