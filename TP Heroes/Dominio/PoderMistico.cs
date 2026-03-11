namespace Dominio;

public class PoderMistico : Poder
{
    private List<Poder> _poderesAcumulados = new();
    public IReadOnlyCollection<Poder> PoderesAcumulados => _poderesAcumulados.AsReadOnly();

    public PoderMistico(List<Poder> poderesAcumulados)
    {
        _poderesAcumulados = poderesAcumulados;
    }

    public override int CalcularPotencia()
    {
        return _poderesAcumulados.Sum(p => p.CalcularPotencia());
    }

    public override bool EsConfiable()
    {
        return false;
    }

    public override void Ganar(Individuo vencido)
    {
        _poderesAcumulados.Add(vencido.Perder());
    }
}
