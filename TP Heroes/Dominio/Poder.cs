namespace Dominio;

public abstract class Poder
{
    public abstract int CalcularPotencia();
    public abstract bool EsConfiable();
    public abstract void Ganar(Individuo vencido);
}
