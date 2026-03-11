using Dominio.Common;

namespace Dominio.ValueObjects;

public class NombreCompania : ValueObject
{
    public string Valor { get; private set; }

    private NombreCompania(string valor)
    {
        Valor = valor;
    }

    public static NombreCompania Create(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("El nombre de la compañia no puede estar vacio");

        if (valor.Length > 100)
            throw new Exception("Supera la capacidad de caracteres");

        return new NombreCompania(valor);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }
}
