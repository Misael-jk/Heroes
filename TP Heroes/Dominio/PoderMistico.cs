namespace Dominio;

public class PoderMistico : Poder
{
    private List<Poder> _poderesAcumulados = new();
    IReadOnlyCollection<Poder> PoderesAcumulados => _poderesAcumulados.AsReadOnly();
}
