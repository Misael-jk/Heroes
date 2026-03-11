namespace Dominio;

public class SuperFuerza : Poder
{
    private int _potenciaAdicional { get; set; }

    public SuperFuerza(int potenciaAdicional)
    {
        if (potenciaAdicional < 0)
            throw new Exception("No puede ser negativo");

        _potenciaAdicional = potenciaAdicional;
    }

    public override int CalcularPotencia()
    {
        return _potenciaAdicional;
    }

    public override void Ganar(Individuo vencido)
    {
        _potenciaAdicional++;
    }

    public override bool EsConfiable()
    {
        return _potenciaAdicional < 100;
    }
}
