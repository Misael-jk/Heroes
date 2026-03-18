using Dominio.ValueObjects;

namespace Dominio;

public class Compania
{
    public NombreCompania _nombre { get; private set; }

    private List<Individuo> _empleados => new();
    public IReadOnlyCollection<Individuo> Empleados => _empleados.AsReadOnly();
    private List<Armas> _armas => new();
    public IReadOnlyCollection<Armas> Armitas => _armas.AsReadOnly();

    private Compania(NombreCompania nombre)
    {
        _nombre = nombre;
    }

    public static Compania Create(NombreCompania nombre)
    {
        return new Compania(nombre);
    }

    public void AgregarEmpleado(Individuo individuo)
    {
        if (_empleados.Contains(individuo))
            throw new Exception("El empleado ya pertenece a la compañía");

        _empleados.Add(individuo);

        individuo.AsignarCompania(this);
    }

    public void AgregarArma(string nombre, int potencia)
    {
        if (_armas.Any(a => a._nombre == nombre))
            throw new Exception("No se puede repetir la misma armas");

        Armas armas = Armas.Create(nombre, potencia);
        _armas.Add(armas);
    }

    public List<Individuo?> OponentesMasPoderosos()
    {
        return _empleados.Select(e => e.OponenteMasPoderoso()).Where(o => o != null).ToList()!;
    }

    public int PotenciaTotal()
    {
        return _empleados.Where(e => e.EsConfiable()).Sum(e => e.CalcularPotencia());
    }

    public bool PuedeDestruir(Compania compania)
    {
        var individuosOponentes = compania._empleados;
        var aliados = _empleados;

        foreach (var oponente in individuosOponentes)
        {
            bool alguienPuede = aliados.Any(
                a => a.CantidadBatallas() > oponente.CantidadBatallas() 
                && a.CalcularPotencia() > oponente.CalcularPotencia());

            if(!alguienPuede)
                return false;
        }

        return true;
    }
}
