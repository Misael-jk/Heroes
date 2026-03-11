using Dominio.ValueObjects;

namespace Dominio;

public class Individuo
{
    public NombreIndividuo _nombre { get; private set; }
    public Apodo _apodo { get; private set; }
    public int _nivelEntrenamiento { get; private set; }
    public Compania? _compania { get; private set; }

    private List<Armas> _armas = new();
    public IReadOnlyCollection<Armas> Armitas => _armas.AsReadOnly();
    private List<HistorialCombates> _historialPeleas = new();
    public IReadOnlyCollection<HistorialCombates> HistorialPeleas => _historialPeleas.AsReadOnly();

    #region Constructores
    protected Individuo(NombreIndividuo nombre, Apodo apodo)
    {
        _nombre = nombre;
        _apodo = apodo;
        _nivelEntrenamiento = 0;
    }

    public static Individuo Create(NombreIndividuo nombre, Apodo apodo)
    {
        return new(nombre, apodo);
    }
    #endregion

    #region Setters
    public void AgregarArma(string nombre, int potencia)
    {
        if (_armas.Any(a => a._nombre == nombre))
            throw new Exception("No se puede repetir la misma armas");

        Armas armas = Armas.Create(nombre, potencia);
        _armas.Add(armas);
    }

    public void AsignarCompania(Compania compania)
        => _compania = compania;

    public int CantidadBatallas()
        => _historialPeleas.Count;

    #endregion

    public virtual int CalcularPotencia()
    {
        int potenciaArma = 0;
        var armasTotales = _armas.ToList();

        if (armasTotales.Any())
            potenciaArma = armasTotales.Max(a => a._potencia);

        int potenciaTotal = _nivelEntrenamiento + potenciaArma;

        return potenciaTotal;
    }

    public virtual bool EsConfiable() 
        => _historialPeleas.Count > 10 && _nivelEntrenamiento < 1000;

    public Individuo? OponenteMasPoderoso()
    {
        if (!_historialPeleas.Any()) return null;

        Individuo masPoderoso = _historialPeleas[0]._individuo;

        foreach (var oponente in _historialPeleas)
        {
            if (oponente._individuo.CalcularPotencia() > masPoderoso.CalcularPotencia())
                masPoderoso = oponente._individuo;
        }
        return masPoderoso;

    }

    public void Pelear(Individuo vencido, LugarBatalla lugar)
    {
        var historial = HistorialCombates.Create(vencido, lugar);
        _historialPeleas.Add(historial);

        var historialOponente = HistorialCombates.Create(this, lugar);
        vencido._historialPeleas.Add(historialOponente);

        if (this.CalcularPotencia() > vencido.CalcularPotencia())
        {
            Ganar(vencido);
        }
        else if (vencido.CalcularPotencia() > this.CalcularPotencia())
        {
            vencido.Ganar(this);
        }
    }

    public virtual void Ganar(Individuo vencido)
    {
        if(_nivelEntrenamiento < 1000)
            _nivelEntrenamiento++;
    }

    public virtual Poder Perder()
        => new SuperFuerza(5);
}
