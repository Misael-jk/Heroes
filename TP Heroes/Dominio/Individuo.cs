namespace Dominio;

public class Individuo
{
    public string _nombre { get; private set; } = string.Empty;
    public string _apodo { get; private set; } = string.Empty;
    public int _nivelEntrenamiento { get; private set; }
    public Compania? _compania { get; private set; }

    private List<Armas> _armas = new();
    public IReadOnlyCollection<Armas> Armitas => _armas.AsReadOnly();

    protected Individuo(string nombre, string apodo)
    {
        _nombre = nombre;
        _apodo = apodo;
        _nivelEntrenamiento = 0;
    }

    public static Individuo Create(string nombre, string apodo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new Exception("El nombre del arma no puede estar vacio");

        if (nombre.Length > 100)
            throw new Exception("Supera la capacidad de caracteres");

        if (string.IsNullOrWhiteSpace(apodo))
            throw new Exception("El nombre del arma no puede estar vacio");

        if (apodo.Length > 100)
            throw new Exception("Supera la capacidad de caracteres");

        return new(nombre, apodo);
    }

    public void AgregarArma(string nombre, int potencia)
    {
        if (_armas.Any(a => a._nombre == nombre))
            throw new Exception("No se puede repetir la misma armas");

        Armas armas = Armas.Create(nombre, potencia);
        _armas.Add(armas);
    }

    public void CambiarCompania(Compania compania)
        => _compania = compania;

    public virtual int CalcularPotencia()
    {
        int potenciaArma = 0;
        var armasTotales = _armas.ToList();

        if (armasTotales.Any())
            potenciaArma = armasTotales.Max(a => a._potencia);

        int potenciaTotal = _nivelEntrenamiento + potenciaArma;

        return potenciaTotal;
    }

    public virtual void Pelear(Individuo vencido)
    {
        if (_nivelEntrenamiento < 1000)
            _nivelEntrenamiento++;

        //_historialPeleas.Add(vencido);
    }
}
