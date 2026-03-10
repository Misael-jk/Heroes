namespace Dominio;

public class Compania
{
    public string _nombre { get; private set; } = string.Empty;

    private List<Individuo> _empleados => new();
    public IReadOnlyCollection<Individuo> Empleados => _empleados.AsReadOnly();
    private List<Armas> _armas => new();
    public IReadOnlyCollection<Armas> Armas => _armas.AsReadOnly();

    private Compania(string nombre)
    {
        _nombre = nombre;
    }

    public Compania Create(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new Exception("El nombre del arma no puede estar vacio");

        if (nombre.Length > 100)
            throw new Exception("Supera la capacidad de caracteres");

        return new Compania(nombre);
    }

    public void AgregarEmpleado(Individuo individuo)
    {
        if (_empleados.Contains(individuo))
            throw new Exception("El empleado ya pertenece a la compañía");

        _empleados.Add(individuo);

        individuo.CambiarCompania(this);
    }
}
