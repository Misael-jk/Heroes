using Dominio.Common;

namespace Dominio.ValueObjects;

public class Entrenamiento : ValueObject
{
    public int Valor { get; private set; }
    private Entrenamiento(int valor)
    {
        Valor = valor;
    }
    public static Entrenamiento Create(int valor)
    {
        if (valor < 0)
            throw new Exception("El nivel de entrenamiento no puede ser negativo");

        return new Entrenamiento(valor);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }
}
