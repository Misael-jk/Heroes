namespace Dominio;

public class Sabiduria : Poder
{
    public Heroe _heroe { get; private set; }
    public Sabiduria(Heroe heroe)
    {
        _heroe = heroe;
    }

    public override int CalcularPotencia()
    {
        return _heroe.CantidadBatallas() * 3;
    }

    public override bool EsConfiable()
    {
        return _heroe.CantidadBatallas() > 20;
    }

    public override void Ganar(Individuo vencido)
    {
        throw new NotImplementedException();
    }
}
