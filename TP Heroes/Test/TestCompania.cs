using Dominio;
using Dominio.ValueObjects;

namespace Test
{
    public class TestCompania
    {
        [Fact]
        public void AgregarEmpleado_DebeAgregarIndividuoYAsignarCompania()
        {
            // Arrange
            var nombre = NombreCompania.Create("La Alianza");
            var compania = Compania.Create(nombre);
            var individuo = Individuo.Create(NombreIndividuo.Create("Aragorn"), Apodo.Create("El Rey"));

            // Act
            compania.AgregarEmpleado(individuo);
            individuo.AsignarCompania(compania);

            // Assert
            Assert.Single(compania.Empleados);
            Assert.Contains(individuo, compania.Empleados);
        }

        [Fact]
        public void AgregarEmpleado_CuandoYaExiste_DebeLanzarExcepcion()
        {
            // Arrange
            var compania = Compania.Create(NombreCompania.Create("Gondor"));
            var individuo = Individuo.Create(NombreIndividuo.Create("Aragorn"), Apodo.Create("El Rey"));
            compania.AgregarEmpleado(individuo);

            

            var ex = Assert.Throws<Exception>(() => compania.AgregarEmpleado(individuo));
            Assert.Equal("El empleado ya pertenece a la compañía", ex.Message);
        }

        [Fact]
        public void AgregarArma_Nueva_DebeAparecerEnArmitas()
        {
            // Arrange
            var compania = Compania.Create(NombreCompania.Create("Gondor"));

            // Act
            compania.AgregarArma("Excalibur", 100);

            // Assert
            Assert.NotEmpty(compania.Armitas);
            Assert.Contains(compania.Armitas, a => a._nombre == "Excalibur");
        }
    }
}