using Dominio.Common;

namespace Dominio.ValueObjects;

public class LugarBatalla : ValueObject
{
    public string Valor { get; private set; }

    private LugarBatalla(string valor)
    {
        Valor = valor;
    }

    public static LugarBatalla Create(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("El lugar de batalla no puede estar vacio");

        if (valor.Length > 100)
            throw new Exception("Supera la capacidad de caracteres");

        return new LugarBatalla(valor);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }
}
