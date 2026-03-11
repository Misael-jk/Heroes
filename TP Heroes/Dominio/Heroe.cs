using Dominio.ValueObjects;

namespace Dominio;

public class Heroe : Individuo
{
    public Poder _poder { get; private set; }

    public Heroe(NombreIndividuo nombre, Apodo apodo, int nivelEntrenamiento, Poder poder) 
        : base(nombre, apodo)
    {
        _poder = poder;
    }

    public override int CalcularPotencia()
    {
        return base.CalcularPotencia() + _poder.CalcularPotencia();
    }

    public override bool EsConfiable()
    {
        return base.HistorialPeleas.Count > 10 && _poder.EsConfiable();
    }

    public override void Ganar(Individuo vencido)
    {
        _poder.Ganar(vencido);
    }

    public override Poder Perder()
    {
        return _poder;
    }
}
