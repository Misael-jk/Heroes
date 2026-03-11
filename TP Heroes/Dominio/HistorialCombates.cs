using Dominio.ValueObjects;

namespace Dominio;

public class HistorialCombates
{
    public Individuo _individuo { get; private set; } = null!;
    public LugarBatalla _lugar { get; private set; }
    public DateTime _fecha { get; private set; }

    private HistorialCombates(Individuo individuo, LugarBatalla lugar)
    {
        _individuo = individuo;
        _lugar = lugar;
        _fecha = DateTime.Now;
    }

    public static HistorialCombates Create(Individuo individuo, LugarBatalla lugar)
    {
        return new HistorialCombates(individuo, lugar);
    }
}
