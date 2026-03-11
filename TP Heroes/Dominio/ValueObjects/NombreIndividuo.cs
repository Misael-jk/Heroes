using Dominio.Common;

namespace Dominio.ValueObjects;

public class NombreIndividuo : ValueObject
{
    public string Valor { get; private set; }

    private NombreIndividuo(string valor)
    {
        Valor = valor;
    }

    public static NombreIndividuo Create(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("El nombre no puede estar vacío");

        if (valor.Length > 100)
            throw new Exception("El nombre no puede superar los 100 caracteres");

        return new NombreIndividuo(valor);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }
}
