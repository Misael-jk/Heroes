<h1 align="center">E.T. Nº12 D.E. 1º "Libertador Gral. José de San Martín"</h1>
<p align="center">
  <img src="https://et12.edu.ar/imgs/computacion/vamoaprogramabanner.png">
</p>

## 📋 Índice

1. [Descripción del Problema](#-descripción-del-problema)
2. [Principios de POO Aplicados](#-principios-de-poo-aplicados)
3. [Arquitectura y Estructura del Proyecto](#-arquitectura-y-estructura-del-proyecto)
4. [Modelado de Entidades](#-modelado-de-entidades)
   - [Value Objects](#value-objects)
   - [Armas](#armas)
   - [Poder (abstracta)](#poder-abstracta)
   - [SuperFuerza](#superfuerza)
   - [Sabiduria](#sabiduria)
   - [PoderMistico](#podermistico)
   - [HistorialCombates](#historialcombates)
   - [Individuo](#individuo)
   - [Heroe](#heroe)
   - [Compania](#compania)
5. [Diagrama de Relaciones](#-diagrama-de-relaciones)
6. [Resolución de los Puntos del Enunciado](#-resolución-de-los-puntos-del-enunciado)
7. [Decisiones de Diseño](#-decisiones-de-diseño)

---

## 📖 Descripción del Problema

El sistema modela un universo donde conviven **individuos comunes** y **héroes con poderes especiales**, organizados dentro de **compañías secretas**. Cada entidad tiene comportamientos bien definidos:

- Los individuos pueden **pelear** entre sí; el ganador mejora sus capacidades.
- Los héroes poseen un **poder único** que amplifica su potencia y condiciona su comportamiento al ganar.
- Las compañías agrupan individuos, les proveen armas adicionales y pueden **enfrentarse** entre sí.

El objetivo del TP es modelar este dominio aplicando correctamente los principios de la **Programación Orientada a Objetos**, de modo que el sistema sea extensible, mantenible y fiel a las reglas del enunciado.

---

## 🧠 Principios de POO Aplicados

### Abstracción

La abstracción consiste en representar únicamente lo que el sistema necesita conocer de cada entidad, ocultando los detalles que no son relevantes para el problema.

En este sistema, la clase `Poder` es el ejemplo más claro: captura la esencia de *"algo que tiene potencia, puede ganar y puede ser confiable"*, sin comprometerse con ninguna implementación concreta. El código que usa un `Poder` no necesita saber si es `SuperFuerza`, `Sabiduria` o `PoderMistico`; simplemente le envía un mensaje y el objeto correcto responde.

```csharp
public abstract class Poder
{
    public abstract int CalcularPotencia();
    public abstract bool EsConfiable();
    public abstract void Ganar(Individuo vencido);
}
```

De igual forma, los **Value Objects** como `NombreIndividuo` o `LugarBatalla` abstraen la validación y el concepto de identidad de un simple `string`, dejando al resto del dominio libre de esas preocupaciones.

---

### Encapsulamiento

Todos los atributos de las clases son **privados o protegidos**. El estado interno de un objeto sólo puede modificarse a través de sus propios métodos, que garantizan que las reglas del dominio siempre se respeten.

Ejemplos concretos:
- `_nivelEntrenamiento` en `Individuo` es privado. La única forma de incrementarlo es mediante `Ganar()`, que además valida el tope de 1000.
- `_poderesAcumulados` en `PoderMistico` no se puede manipular directamente desde afuera; sólo se expone como `IReadOnlyCollection<Poder>`.
- `_potenciaAdicional` en `SuperFuerza` sólo se incrementa dentro de `Ganar()`, siguiendo la regla del enunciado.

```csharp
// Nadie puede incrementar el entrenamiento directamente
private int _nivelEntrenamiento { get; private set; }

// Solo el método Ganar() lo modifica, con sus reglas
public virtual void Ganar(Individuo vencido)
{
    if (_nivelEntrenamiento < 1000)
        _nivelEntrenamiento++;
}
```

---

### Herencia

La herencia modela relaciones **"es-un"** entre clases: reutiliza comportamiento existente y lo extiende donde es necesario.

```
Individuo
    └── Heroe

Poder (abstracta)
    ├── SuperFuerza
    ├── Sabiduria
    └── PoderMistico
```

`Heroe` hereda de `Individuo` porque un héroe **es** un individuo. Reutiliza toda la lógica de armas, historial y potencia base, y sólo sobreescribe lo que cambia: la contribución del poder a la potencia, el comportamiento al ganar, y las condiciones de confiabilidad.

Las tres subclases de `Poder` heredan de ella porque las tres **son** poderes: comparten el contrato de los tres métodos abstractos, pero cada una lo implementa de forma completamente diferente.

---

### Polimorfismo

El polimorfismo es el mecanismo más importante del sistema. Permite que distintos objetos respondan al **mismo mensaje** de maneras diferentes, sin que quien envía el mensaje necesite saber con qué tipo de objeto está hablando.

Se manifiesta en cuatro puntos clave:

| Método | Individuo | Heroe | SuperFuerza | Sabiduria | PoderMistico |
|---|---|---|---|---|---|
| `CalcularPotencia()` | entrenamiento + mejor arma | base + poder | potenciaAdicional | peleas × 3 | suma de poderes |
| `EsConfiable()` | > 10 peleas y < 1000 entren. | > 10 peleas y poder confiable | potencia ≤ 100 | > 20 peleas | siempre `false` |
| `Ganar(vencido)` | incrementa entrenamiento | delega al poder | incrementa potencia | no hace nada | acumula poder del vencido |
| `Perder()` | retorna `SuperFuerza(5)` | retorna su propio poder | — | — | — |

Gracias al polimorfismo, `Compania` puede iterar sobre una lista de `Individuo` y llamar a `EsConfiable()` o `CalcularPotencia()` sin saber si cada uno es un héroe o un individuo normal:

```csharp
public int PotenciaTotal()
{
    // No importa si son héroes o personas normales:
    // cada uno responde a su manera
    return _empleados.Where(e => e.EsConfiable())
                     .Sum(e => e.CalcularPotencia());
}
```

---

### Composición

La composición modela relaciones **"tiene-un"**, donde un objeto contiene a otro como parte constitutiva de sí mismo.

- `Individuo` **tiene** una lista de `Armas` y una lista de `HistorialCombates`.
- `Individuo` **tiene** una referencia opcional a `Compania` (por si pertenece a una).
- `Heroe` **tiene** exactamente un `Poder`.
- `Compania` **tiene** una lista de `Individuo` y una lista de `Armas` compartidas.
- `PoderMistico` **tiene** una lista de `Poder` (composición recursiva).
- `HistorialCombates` **tiene** una referencia a `Individuo` (el oponente) y a `LugarBatalla`.

La composición recursiva en `PoderMistico` es especialmente interesante: al acumular poderes que a su vez pueden ser otros `PoderMistico`, se forma una estructura de árbol de profundidad arbitraria.

---

## 🗂 Arquitectura y Estructura del Proyecto

```
TP Heroes/
└── Dominio/
    ├── Common/
    │   └── ValueObject.cs          # Clase base para Value Objects
    ├── ValueObjects/
    │   ├── Apodo.cs
    │   ├── Armas.cs
    │   ├── Entrenamiento.cs
    │   ├── LugarBatalla.cs
    │   ├── NombreCompania.cs
    │   └── NombreIndividuo.cs
    ├── Individuo.cs                 # Persona común (clase base)
    ├── Heroe.cs                     # Subclase de Individuo con poder
    ├── Poder.cs                     # Clase abstracta para los poderes
    ├── SuperFuerza.cs
    ├── Sabiduria.cs
    ├── PoderMistico.cs
    ├── HistorialCombates.cs         # Registro de un combate
    ├── Compania.cs
    └── Dominio.csproj
```

El proyecto tiene una única capa de **Dominio puro**: no depende de frameworks externos, no tiene acceso a base de datos ni a interfaces de usuario. Esto es correcto en un diseño orientado a objetos limpio — el dominio expresa las reglas del negocio y nada más.

---

## 🧩 Modelado de Entidades

### Value Objects

**Ubicación:** `Dominio/Common/ValueObject.cs` y `Dominio/ValueObjects/`

Los Value Objects son objetos cuya **identidad está definida por su valor**, no por una referencia en memoria. Dos `NombreIndividuo` con el mismo texto son idénticos; no importa si son instancias distintas.

La clase base `ValueObject` implementa `IEquatable<ValueObject>` y provee comparación estructural mediante `GetEqualityComponents()`:

```csharp
public abstract class ValueObject : IEquatable
{
    protected abstract IEnumerable GetEqualityComponents();

    public bool Equals(ValueObject? other)
        => GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
}
```

#### ¿Por qué usar Value Objects en lugar de `string` o `int` directamente?

Un `string` no sabe nada sobre las reglas de un nombre. Un Value Object sí:

```csharp
// Sin Value Object: la validación se dispersa por todo el código
string nombre = ""; // ← ¿quién valida esto?

// Con Value Object: la validación vive en el tipo
NombreIndividuo nombre = NombreIndividuo.Create(""); // ← lanza excepción aquí
```

Los Value Objects implementados son:

| Clase | Qué representa | Validaciones |
|---|---|---|
| `NombreIndividuo` | Nombre de una persona | No vacío, máx. 100 caracteres |
| `Apodo` | Apodo de un individuo | No vacío, máx. 100 caracteres |
| `NombreCompania` | Nombre de una compañía | No vacío, máx. 100 caracteres |
| `LugarBatalla` | Lugar donde ocurrió un combate | No vacío, máx. 100 caracteres |
| `Entrenamiento` | Nivel de entrenamiento | No negativo |
| `Armas` | Un arma con nombre y potencia | Nombre no vacío, potencia ≥ 0 |

> **Nota:** `Armas` es también un Value Object porque dos armas son iguales si tienen el mismo nombre y la misma potencia, independientemente de quién las tenga.

---

### Armas

**Ubicación:** `Dominio/ValueObjects/Armas.cs`

Representa un arma del sistema. Es un **Value Object** con dos componentes: nombre y potencia.

```csharp
public class Armas : ValueObject
{
    public string _nombre { get; }
    public int _potencia { get; }

    public static Armas Create(string nombre, int potencia) { ... }

    protected override IEnumerable GetEqualityComponents()
    {
        yield return _potencia;
        yield return _nombre;
    }
}
```

**Atributos:**
- `_nombre`: identifica el arma dentro de la colección (se usa para evitar duplicados).
- `_potencia`: valor numérico que contribuye al cálculo de potencia de quien la porta.

**¿Por qué es Value Object?** Porque el arma no tiene una identidad propia más allá de sus datos. Una espada con potencia 30 es igual a cualquier otra espada con potencia 30. Esto además habilita el uso de `.Contains()` y `.Any()` con comparación por valor en las listas.

---

### Poder (abstracta)

**Ubicación:** `Dominio/Poder.cs`

Clase abstracta que define el **contrato** que deben cumplir todos los tipos de poder. Es abstracta porque no existe un "poder genérico" en el dominio — siempre se trabaja con un tipo concreto.

```csharp
public abstract class Poder
{
    public abstract int CalcularPotencia();
    public abstract bool EsConfiable();
    public abstract void Ganar(Individuo vencido);
}
```

**Métodos abstractos:**

| Método | Qué declara |
|---|---|
| `CalcularPotencia()` | Cuánta potencia adicional aporta este poder al héroe |
| `EsConfiable()` | Si el héroe que tiene este poder cumple sus criterios de confiabilidad |
| `Ganar(vencido)` | Qué sucede con el poder cuando el héroe gana una pelea |

Al declarar estos tres métodos como `abstract`, se garantiza en **tiempo de compilación** que toda subclase los implemente. No es posible crear un `Poder` que no responda a estos mensajes.

---

### SuperFuerza

**Ubicación:** `Dominio/SuperFuerza.cs`

Representa un poder de fuerza bruta con un valor numérico que crece con las victorias.

```csharp
public class SuperFuerza : Poder
{
    private int _potenciaAdicional { get; set; }

    public SuperFuerza(int potenciaAdicional) { ... }

    public override int CalcularPotencia() => _potenciaAdicional;

    public override void Ganar(Individuo vencido)
        => _potenciaAdicional++;

    public override bool EsConfiable()
        => _potenciaAdicional < 100;
}
```

**Atributo:**
- `_potenciaAdicional`: la potencia que aporta la superfuerza. Es mutable porque crece al ganar peleas.

**Métodos:**
- `CalcularPotencia()`: retorna directamente `_potenciaAdicional`. No depende de contexto externo.
- `Ganar()`: ignora al vencido (no le importa quién fue); simplemente incrementa su propia potencia en 1. Esta es la regla del enunciado: *"la superfuerza incrementa su potencia en un punto"*.
- `EsConfiable()`: retorna `true` si la potencia aún no llegó a 100. Una superfuerza demasiado poderosa indica un héroe fuera de control.

---

### Sabiduria

**Ubicación:** `Dominio/Sabiduria.cs`

Representa un poder cuya potencia **crece dinámicamente** con la experiencia del héroe.

```csharp
public class Sabiduria : Poder
{
    public Heroe _heroe { get; private set; }

    public Sabiduria(Heroe heroe) { ... }

    public override int CalcularPotencia()
        => _heroe.CantidadBatallas() * 3;

    public override bool EsConfiable()
        => _heroe.CantidadBatallas() > 20;

    public override void Ganar(Individuo vencido)
    {
        // La sabiduría no obtiene nada al ganar
    }
}
```

**Atributo:**
- `_heroe`: referencia al héroe que posee esta sabiduría. Es necesaria porque la potencia de la sabiduría depende de cuántas batallas haya tenido *ese héroe en particular*. Sin esta referencia, la sabiduría no puede calcular su propia potencia.

**Métodos:**
- `CalcularPotencia()`: consulta cuántas batallas tiene el héroe y multiplica por 3. La potencia crece automáticamente a medida que el héroe pelea, sin ninguna modificación explícita.
- `Ganar()`: no hace nada. La sabiduría no se beneficia directamente de ganar; su crecimiento es implícito (más peleas = más potencia en la próxima consulta).
- `EsConfiable()`: un héroe sabio se considera confiable sólo si tiene más de 20 peleas, lo que indica que su "sabiduría" está realmente probada.

> **Nota de diseño:** el acoplamiento de `Sabiduria` hacia `Heroe` es intencional e inevitable dado el enunciado. La sabiduría *necesita* saber quién es su héroe para calcular su potencia.

---

### PoderMistico

**Ubicación:** `Dominio/PoderMistico.cs`

Representa un poder que **acumula otros poderes**. Es el más complejo del sistema porque puede contener `SuperFuerza`, `Sabiduria` y otros `PoderMistico`, formando una estructura recursiva.

```csharp
public class PoderMistico : Poder
{
    private List _poderesAcumulados = new();
    public IReadOnlyCollection PoderesAcumulados => _poderesAcumulados.AsReadOnly();

    public override int CalcularPotencia()
        => _poderesAcumulados.Sum(p => p.CalcularPotencia());

    public override void Ganar(Individuo vencido)
        => _poderesAcumulados.Add(vencido.Perder());

    public override bool EsConfiable() => false;
}
```

**Atributo:**
- `_poderesAcumulados`: lista de poderes que este poder místico ha absorbido. Empieza vacía y crece cada vez que el héroe gana una pelea. Se expone como `IReadOnlyCollection` para proteger la lista interna.

**Métodos:**
- `CalcularPotencia()`: suma recursivamente las potencias de todos los poderes acumulados. Como cada elemento de la lista es un `Poder` (polimórfico), si hay un `PoderMistico` anidado, éste a su vez sumará los suyos.
- `Ganar(vencido)`: consulta `vencido.Perder()` para obtener el poder que el vencido otorga y lo agrega a la lista. No se roba el poder: el vencido lo conserva; simplemente se agrega una **copia/referencia** al mismo poder.
- `EsConfiable()`: siempre `false`. Un héroe con poder místico es intrínsecamente inestable.

---

### HistorialCombates

**Ubicación:** `Dominio/HistorialCombates.cs`

Representa el **registro de un único combate**: quién fue el oponente, dónde ocurrió y cuándo.

```csharp
public class HistorialCombates
{
    public Individuo _individuo { get; private set; }
    public LugarBatalla _lugar { get; private set; }
    public DateTime _fecha { get; private set; }

    public static HistorialCombates Create(Individuo individuo, LugarBatalla lugar)
    {
        return new HistorialCombates(individuo, lugar);
    }
}
```

**Atributos:**
- `_individuo`: referencia al **oponente** (no a sí mismo). Cada individuo en su historial guarda con quién peleó.
- `_lugar`: Value Object que indica dónde ocurrió el combate (`LugarBatalla`).
- `_fecha`: timestamp automático de cuándo se registró el combate.

**¿Por qué existe esta clase y no una simple `List<Individuo>`?**

Porque el historial no es solo una lista de nombres: tiene **contexto** (lugar, fecha). Esta clase permite que en el futuro se agreguen más datos al registro (resultado, árbitro, etc.) sin modificar `Individuo`. Es un ejemplo de **encapsulamiento de concepto**: el "registro de combate" es una entidad propia, no un detalle de implementación.

---

### Individuo

**Ubicación:** `Dominio/Individuo.cs`

Es la **clase base** del sistema. Modela a cualquier persona: tiene nombre, apodo, nivel de entrenamiento, armas propias, una compañía opcional y un historial de combates.

```csharp
public class Individuo
{
    public NombreIndividuo _nombre { get; private set; }
    public Apodo _apodo { get; private set; }
    public int _nivelEntrenamiento { get; private set; }
    public Compania? _compania { get; private set; }

    private List _armas = new();
    private List _historialPeleas = new();

    public IReadOnlyCollection Armitas => _armas.AsReadOnly();
    public IReadOnlyCollection HistorialPeleas => _historialPeleas.AsReadOnly();
}
```

#### Atributos

| Atributo | Tipo | Por qué existe |
|---|---|---|
| `_nombre` | `NombreIndividuo` | Identifica al individuo; es un VO para garantizar validaciones |
| `_apodo` | `Apodo` | Nombre alternativo/secreto; VO por las mismas razones |
| `_nivelEntrenamiento` | `int` | Contribuye directamente a la potencia; crece al ganar |
| `_compania` | `Compania?` | Referencia a la organización; nullable porque puede no pertenecer a ninguna |
| `_armas` | `List<Armas>` | Armas propias del individuo; la más potente aporta a su potencia |
| `_historialPeleas` | `List<HistorialCombates>` | Registro de todos sus combates; expuesto como `IReadOnlyCollection` |

#### Métodos

**`CalcularPotencia()`** — *virtual*
```csharp
public virtual int CalcularPotencia()
{
    int potenciaArma = 0;
    var armasTotales = _armas.ToList();
    if (armasTotales.Any())
        potenciaArma = armasTotales.Max(a => a._potencia);
    return _nivelEntrenamiento + potenciaArma;
}
```
Implementa la regla: *potencia = nivel de entrenamiento + arma más poderosa*. Es `virtual` para que `Heroe` pueda extenderlo sumando la potencia del poder.

> **¿Por qué no incluye las armas de la compañía?** Esta implementación sólo considera armas propias. Una extensión natural sería también consultar `_compania?.Armitas` para encontrar el máximo global.

**`EsConfiable()`** — *virtual*
```csharp
public virtual bool EsConfiable()
    => _historialPeleas.Count > 10 && _nivelEntrenamiento < 1000;
```
Regla del enunciado: más de 10 peleas y sin llegar al tope de entrenamiento. Es `virtual` para que `Heroe` la sobreescriba con condiciones adicionales.

**`Pelear(otro, lugar)`**
```csharp
public void Pelear(Individuo vencido, LugarBatalla lugar)
{
    var historial = HistorialCombates.Create(vencido, lugar);
    _historialPeleas.Add(historial);
    var historialOponente = HistorialCombates.Create(this, lugar);
    vencido._historialPeleas.Add(historialOponente);

    if (this.CalcularPotencia() > vencido.CalcularPotencia())
        Ganar(vencido);
    else if (vencido.CalcularPotencia() > this.CalcularPotencia())
        vencido.Ganar(this);
}
```
Gestiona toda la lógica de un combate:
1. Registra el combate en el historial de **ambos** participantes.
2. Determina al ganador comparando potencias.
3. Llama a `Ganar()` del vencedor (polimórfico).
4. En empate, nadie mejora.

Recibe `LugarBatalla` para enriquecer el registro histórico.

**`Ganar(vencido)`** — *virtual*
```csharp
public virtual void Ganar(Individuo vencido)
{
    if (_nivelEntrenamiento < 1000)
        _nivelEntrenamiento++;
}
```
Un individuo normal mejora su entrenamiento en 1 al ganar, respetando el tope de 1000. Es `virtual` para que `Heroe` lo sobreescriba y delegue en su poder.

**`Perder()`** — *virtual*
```csharp
public virtual Poder Perder()
    => new SuperFuerza(5);
```
Retorna el poder que este individuo otorga al ser vencido por un héroe con `PoderMistico`. Un individuo normal siempre otorga `SuperFuerza(5)`. Es `virtual` para que `Heroe` retorne su propio poder.

**`OponenteMasPoderoso()`**
```csharp
public Individuo? OponenteMasPoderoso()
{
    if (!_historialPeleas.Any()) return null;
    Individuo masPoderoso = _historialPeleas[0]._individuo;
    foreach (var oponente in _historialPeleas)
        if (oponente._individuo.CalcularPotencia() > masPoderoso.CalcularPotencia())
            masPoderoso = oponente._individuo;
    return masPoderoso;
}
```
Recorre el historial y retorna el oponente con mayor potencia **en el momento de la consulta** (la potencia puede haber cambiado desde el combate). Retorna `null` si no tiene peleas.

**`CantidadBatallas()`**
```csharp
public int CantidadBatallas() => _historialPeleas.Count;
```
Método utilitario que expone la cantidad de combates. Lo usa `Sabiduria` para calcular su potencia y los métodos de confiabilidad.

---

### Heroe

**Ubicación:** `Dominio/Heroe.cs`

Subclase de `Individuo` que agrega un `Poder`. Sobreescribe los métodos que cambian por la presencia del poder.

```csharp
public class Heroe : Individuo
{
    public Poder _poder { get; private set; }

    public Heroe(NombreIndividuo nombre, Apodo apodo, int nivelEntrenamiento, Poder poder)
        : base(nombre, apodo)
    {
        _poder = poder;
    }
}
```

**Atributo:**
- `_poder`: el poder del héroe. Es `Poder` (tipo abstracto), no `SuperFuerza` ni ningún concreto — esto es polimorfismo en acción. El héroe no sabe qué tipo de poder tiene; simplemente le manda mensajes.

#### Métodos sobreescritos

**`CalcularPotencia()`**
```csharp
public override int CalcularPotencia()
    => base.CalcularPotencia() + _poder.CalcularPotencia();
```
Extiende la potencia base del individuo sumando la contribución del poder. Usa `base.CalcularPotencia()` para no duplicar la lógica de entrenamiento + armas.

**`EsConfiable()`**
```csharp
public override bool EsConfiable()
    => base.HistorialPeleas.Count > 10 && _poder.EsConfiable();
```
Un héroe necesita más de 10 peleas **y** que su poder específico también sea confiable. Cada tipo de poder define sus propias condiciones de confiabilidad.

**`Ganar(vencido)`**
```csharp
public override void Ganar(Individuo vencido)
    => _poder.Ganar(vencido);
```
Delega completamente en el poder. El héroe no sabe qué hace `Ganar()` — eso es responsabilidad del tipo de poder concreto. Este es el **patrón Strategy** aplicado naturalmente.

**`Perder()`**
```csharp
public override Poder Perder()
    => _poder;
```
Al ser vencido, un héroe otorga **su propio poder** (sin perderlo). Retorna la referencia al poder, no una copia — esto significa que si el `PoderMistico` del vencedor lo modifica, ambos comparten el mismo objeto.

---

### Compania

**Ubicación:** `Dominio/Compania.cs`

Organización que agrupa individuos y les provee armas adicionales. Implementa las operaciones de más alto nivel del sistema (puntos 5, 6 y 7 del enunciado).

```csharp
public class Compania
{
    public NombreCompania _nombre { get; private set; }

    private List _empleados => new();
    private List _armas => new();

    public IReadOnlyCollection Empleados => _empleados.AsReadOnly();
    public IReadOnlyCollection Armitas => _armas.AsReadOnly();
}
```

**Atributos:**
- `_nombre`: identificador de la compañía como Value Object.
- `_empleados`: lista de todos los individuos (héroes o personas normales) que trabajan para la compañía.
- `_armas`: armas que la compañía pone a disposición de todos sus empleados.

#### Métodos

**`AgregarEmpleado(individuo)`**
```csharp
public void AgregarEmpleado(Individuo individuo)
{
    if (_empleados.Contains(individuo))
        throw new Exception("El empleado ya pertenece a la compañía");
    _empleados.Add(individuo);
    individuo.AsignarCompania(this);
}
```
Agrega un individuo y le notifica a quién pertenece. La relación es bidireccional: la compañía conoce a sus empleados y el empleado conoce a su compañía.

**`AgregarArma(nombre, potencia)`**
```csharp
public void AgregarArma(string nombre, int potencia)
{
    if (_armas.Any(a => a._nombre == nombre))
        throw new Exception("No se puede repetir la misma armas");
    _armas.Add(Armas.Create(nombre, potencia));
}
```
Crea y agrega un arma. Valida que no se repitan nombres. Estas armas son accesibles por todos los empleados cuando calculan su potencia.

**`OponentesMasPoderosos()`** — *Punto 5*
```csharp
public List OponentesMasPoderosos()
    => _empleados.Select(e => e.OponenteMasPoderoso())
                 .Where(o => o != null).ToList()!;
```
Retorna el oponente más poderoso de cada empleado, filtrando los que aún no pelearon.

**`PotenciaTotal()`** — *Punto 6*
```csharp
public int PotenciaTotal()
    => _empleados.Where(e => e.EsConfiable())
                 .Sum(e => e.CalcularPotencia());
```
Suma la potencia sólo de los empleados confiables. Usa polimorfismo: tanto `EsConfiable()` como `CalcularPotencia()` se comportan diferente para héroes y personas normales.

**`PuedeDestruir(compania)`** — *Punto 7*
```csharp
public bool PuedeDestruir(Compania compania)
{
    var individuosOponentes = compania._empleados;
    var aliados = _empleados;

    foreach (var oponente in individuosOponentes)
    {
        bool alguienPuede = aliados.Any(
            a => a.CantidadBatallas() > oponente.CantidadBatallas()
              && a.CalcularPotencia() > oponente.CalcularPotencia());

        if (!alguienPuede)
            return false;
    }
    return true;
}
```
Para **cada** individuo de la compañía oponente, verifica que exista **al menos uno** de los propios que lo supere tanto en potencia como en cantidad de batallas. Si hay algún individuo de la oponente que nadie de los propios pueda superar, retorna `false`.

