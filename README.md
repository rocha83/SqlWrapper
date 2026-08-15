# Rochas.SqlWrapper

[English](#english) | [Português](#português) | [Español](#español) | [Français](#français) | [Deutsch](#deutsch)

---

## English

**Rochas.SqlWrapper** is the multi-dialect SQL translation layer shared by the Rochas components (`Rochas.DapperRepository`, `Rochas.BWOQ`).

It concentrates all entity-to-ANSI-SQL conversion intelligence (`Poco`/`Anaemic Model`) for the **MySQL, SQL Server, PostgreSQL and SQLite** dialects — including attribute paraphrasing, `LIKE` filters, value ranges (`RangeFilter`), aggregations, pagination, relations and entity composition.

### Installation

```bash
dotnet add package Rochas.SqlWrapper
```

### Class names

```text
EntitySqlParser      --> Entity-to-ANSI-SQL parsing (CRUD, query, count, paginated)
EntityReflector      --> Entity reflection/metadata with thread-safe caches
Helpers.SQL.*        --> SQL statement constants/templates per dialect
Exceptions           --> Layer domain exceptions (e.g. PropertyNotListableException)
```

### Usage

```csharp
using Rochas.SqlWrapper.Helpers;
using Rochas.Data.Specification.Enums;

var sql = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite,
                                      PersistenceAction.Query, filter);
```

> Requires annotations from `Rochas.Data.Specification` (`[Table]`, `[Key]`, `[Column]` optional — without `[Table]` the class name is used, `[Key]` required).

### Tests and coverage

![line coverage](https://img.shields.io/badge/line%20coverage-99.7%25-brightgreen)
![tests](https://img.shields.io/badge/tests-91-brightgreen)

The `Rochas.SqlWrapper` assembly has **91 unit tests** (xUnit) with **99.7% line coverage** (981/984 lines), measured via [coverlet](https://github.com/coverlet-coverage/coverlet) (`cobertura`):

| Class | Line coverage |
|-------|---------------|
| `EntitySqlParser` | 99.6% |
| `EntityReflector` | 99.7% |
| `Helpers.SQL.SQLStatements` | 100% |
| `Exceptions.PropertyNotListableException` | 100% |

### Multi-database support

| Feature               | MySQL | SQL Server         | PostgreSQL | SQLite |
|-----------------------|-------|--------------------|------------|--------|
| `LIMIT`/pagination    | ✔     | `OFFSET FETCH`/`TOP` | ✔ | ✔ |
| Booleans              | 1/0   | 1/0                | TRUE/FALSE | 1/0 |
| Identifier quoting    | —     | —                  | `"column"` | — |

### License

GPL v2 — see `GNUv2_License.txt`.

---

## Português

**Rochas.SqlWrapper** é a camada de tradução SQL multi-dialeto compartilhada pelos componentes Rochas (`Rochas.DapperRepository`, `Rochas.BWOQ`).

Ela concentra toda a inteligência de conversão de entidades (`Poco`/`Anaemic Model`) em instruções SQL ANSI para os dialetos **MySQL, SQL Server, PostgreSQL e SQLite** — incluindo paráfrase de atributos, filtros `LIKE`, intervalos de valores (`RangeFilter`), agregações, paginação, relações e composição de entidades.

### Instalação

```bash
dotnet add package Rochas.SqlWrapper
```

### Nome das classes

```text
EntitySqlParser      --> Parse de entidades para SQL ANSI (CRUD, consulta, count, paginado)
EntityReflector      --> Reflexão/metadata das entidades com caches thread-safe
Helpers.SQL.*        --> Constantes/templates de instruções SQL por dialeto
Exceptions           --> Exceções de domínio da camada (ex.: PropertyNotListableException)
```

### Como usar

```csharp
using Rochas.SqlWrapper.Helpers;
using Rochas.Data.Specification.Enums;

var sql = EntitySqlParser.ParseEntity(filtro, DatabaseEngine.SQLite,
                                      PersistenceAction.Query, filtro);
```

> Requer as annotations de `Rochas.Data.Specification` (`[Table]`, `[Key]`, `[Column]` opcional — sem `[Table]` usa o nome da classe, `[Key]` obrigatório).

### Testes e cobertura

![line coverage](https://img.shields.io/badge/line%20coverage-99.7%25-brightgreen)
![tests](https://img.shields.io/badge/tests-91-brightgreen)

O assembly `Rochas.SqlWrapper` possui **91 testes unitários** (xUnit) com **99,7% de cobertura de linha** (981/984 linhas), medidos via [coverlet](https://github.com/coverlet-coverage/coverlet) (`cobertura`):

| Classe | Cobertura de linha |
|--------|--------------------|
| `EntitySqlParser` | 99,6% |
| `EntityReflector` | 99,7% |
| `Helpers.SQL.SQLStatements` | 100% |
| `Exceptions.PropertyNotListableException` | 100% |

### Suporte a múltiplos bancos

| Recurso              | MySQL | SQL Server         | PostgreSQL | SQLite |
|----------------------|-------|--------------------|------------|--------|
| `LIMIT`/paginação    | ✔     | `OFFSET FETCH`/`TOP` | ✔ | ✔ |
| Booleanos            | 1/0   | 1/0                | TRUE/FALSE | 1/0 |
| Quote de identificador| —    | —                  | `"coluna"` | — |

### Licença

GPL v2 — veja `GNUv2_License.txt`.

---

## Español

**Rochas.SqlWrapper** es la capa de traducción SQL multi-dialecto compartida por los componentes Rochas (`Rochas.DapperRepository`, `Rochas.BWOQ`).

Concentra toda la inteligencia de conversión de entidades (`Poco`/`Anaemic Model`) a instrucciones SQL ANSI para los dialectos **MySQL, SQL Server, PostgreSQL y SQLite** — incluyendo parafraseo de atributos, filtros `LIKE`, rangos de valores (`RangeFilter`), agregaciones, paginación, relaciones y composición de entidades.

### Instalación

```bash
dotnet add package Rochas.SqlWrapper
```

### Nombres de clases

```text
EntitySqlParser      --> Parse de entidades a SQL ANSI (CRUD, consulta, count, paginado)
EntityReflector      --> Reflexión/metadata de entidades con caches thread-safe
Helpers.SQL.*        --> Constantes/plantillas de instrucciones SQL por dialecto
Exceptions           --> Excepciones de dominio de la capa (ej.: PropertyNotListableException)
```

### Uso

```csharp
using Rochas.SqlWrapper.Helpers;
using Rochas.Data.Specification.Enums;

var sql = EntitySqlParser.ParseEntity(filtro, DatabaseEngine.SQLite,
                                      PersistenceAction.Query, filtro);
```

> Requiere las anotaciones de `Rochas.Data.Specification` (`[Table]`, `[Key]`, `[Column]` opcional — sin `[Table]` usa el nombre de la clase, `[Key]` obligatorio).

### Pruebas y cobertura

![line coverage](https://img.shields.io/badge/line%20coverage-99.7%25-brightgreen)
![tests](https://img.shields.io/badge/tests-91-brightgreen)

El ensamblado `Rochas.SqlWrapper` tiene **91 pruebas unitarias** (xUnit) con **99,7% de cobertura de línea** (981/984 líneas), medidas con [coverlet](https://github.com/coverlet-coverage/coverlet) (`cobertura`):

| Clase | Cobertura de línea |
|-------|--------------------|
| `EntitySqlParser` | 99,6% |
| `EntityReflector` | 99,7% |
| `Helpers.SQL.SQLStatements` | 100% |
| `Exceptions.PropertyNotListableException` | 100% |

### Soporte multi-bases de datos

| Característica        | MySQL | SQL Server         | PostgreSQL | SQLite |
|-----------------------|-------|--------------------|------------|--------|
| `LIMIT`/paginación    | ✔     | `OFFSET FETCH`/`TOP` | ✔ | ✔ |
| Booleanos             | 1/0   | 1/0                | TRUE/FALSE | 1/0 |
| Quotes de identificador| —    | —                  | `"columna"` | — |

### Licencia

GPL v2 — consulte `GNUv2_License.txt`.

---

## Français

**Rochas.SqlWrapper** est la couche de traduction SQL multi-dialecte partagée par les composants Rochas (`Rochas.DapperRepository`, `Rochas.BWOQ`).

Elle concentre toute l'intelligence de conversion d'entités (`Poco`/`Anaemic Model`) en instructions SQL ANSI pour les dialectes **MySQL, SQL Server, PostgreSQL et SQLite** — y compris le paraphrase d'attributs, les filtres `LIKE`, les plages de valeurs (`RangeFilter`), les agrégations, la pagination, les relations et la composition d'entités.

### Installation

```bash
dotnet add package Rochas.SqlWrapper
```

### Noms des classes

```text
EntitySqlParser      --> Parse d'entités en SQL ANSI (CRUD, requête, count, paginé)
EntityReflector      --> Réflexion/métadonnées d'entités avec caches thread-safe
Helpers.SQL.*        --> Constantes/modèles d'instructions SQL par dialecte
Exceptions           --> Exceptions du domaine de la couche (ex. : PropertyNotListableException)
```

### Utilisation

```csharp
using Rochas.SqlWrapper.Helpers;
using Rochas.Data.Specification.Enums;

var sql = EntitySqlParser.ParseEntity(filtre, DatabaseEngine.SQLite,
                                      PersistenceAction.Query, filtre);
```

> Nécessite les annotations de `Rochas.Data.Specification` (`[Table]`, `[Key]`, `[Column]` optionnel — sans `[Table]` le nom de la classe est utilisé, `[Key]` obligatoire).

### Tests et couverture

![line coverage](https://img.shields.io/badge/line%20coverage-99.7%25-brightgreen)
![tests](https://img.shields.io/badge/tests-91-brightgreen)

L'assembly `Rochas.SqlWrapper` contient **91 tests unitaires** (xUnit) avec **99,7% de couverture de ligne** (981/984 lignes), mesurés avec [coverlet](https://github.com/coverlet-coverage/coverlet) (`cobertura`) :

| Classe | Couverture de ligne |
|--------|---------------------|
| `EntitySqlParser` | 99,6% |
| `EntityReflector` | 99,7% |
| `Helpers.SQL.SQLStatements` | 100% |
| `Exceptions.PropertyNotListableException` | 100% |

### Support multi-bases de données

| Fonctionnalité       | MySQL | SQL Server         | PostgreSQL | SQLite |
|----------------------|-------|--------------------|------------|--------|
| `LIMIT`/pagination   | ✔     | `OFFSET FETCH`/`TOP` | ✔ | ✔ |
| Booléens             | 1/0   | 1/0                | TRUE/FALSE | 1/0 |
| Citation d'identifiant| —    | —                  | `"colonne"` | — |

### Licence

GPL v2 — voir `GNUv2_License.txt`.

---

## Deutsch

**Rochas.SqlWrapper** ist die Multi-Dialekt-SQL-Übersetzungsschicht, die von den Rochas-Komponenten (`Rochas.DapperRepository`, `Rochas.BWOQ`) gemeinsam genutzt wird.

Sie konzentriert die gesamte Intelligenz der Entitäts-zu-ANSI-SQL-Konvertierung (`Poco`/`Anaemic Model`) für die Dialekte **MySQL, SQL Server, PostgreSQL und SQLite** — einschließlich Attribut-Paraphrase, `LIKE`-Filter, Wertbereiche (`RangeFilter`), Aggregationen, Paginierung, Relationen und Entitätskomposition.

### Installation

```bash
dotnet add package Rochas.SqlWrapper
```

### Klassennamen

```text
EntitySqlParser      --> Entitäten-zu-ANSI-SQL-Parsing (CRUD, Abfrage, Count, paginiert)
EntityReflector      --> Entitäten-Reflexion/Metadaten mit thread-safe Caches
Helpers.SQL.*        --> SQL-Anweisungskonstanten/Vorlagen pro Dialekt
Exceptions           --> Schicht-Domänen-Ausnahmen (z.B. PropertyNotListableException)
```

### Verwendung

```csharp
using Rochas.SqlWrapper.Helpers;
using Rochas.Data.Specification.Enums;

var sql = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite,
                                      PersistenceAction.Query, filter);
```

> Erfordert Annotationen von `Rochas.Data.Specification` (`[Table]`, `[Key]`, `[Column]` optional — ohne `[Table]` wird der Klassenname verwendet, `[Key]` erforderlich).

### Tests und Abdeckung

![line coverage](https://img.shields.io/badge/line%20coverage-99.7%25-brightgreen)
![tests](https://img.shields.io/badge/tests-91-brightgreen)

Die Assembly `Rochas.SqlWrapper` enthält **91 Unit-Tests** (xUnit) mit **99,7% Zeilenabdeckung** (981/984 Zeilen), gemessen mit [coverlet](https://github.com/coverlet-coverage/coverlet) (`cobertura`):

| Klasse | Zeilenabdeckung |
|--------|-----------------|
| `EntitySqlParser` | 99,6% |
| `EntityReflector` | 99,7% |
| `Helpers.SQL.SQLStatements` | 100% |
| `Exceptions.PropertyNotListableException` | 100% |

### Multi-Datenbank-Unterstützung

| Funktion             | MySQL | SQL Server         | PostgreSQL | SQLite |
|----------------------|-------|--------------------|------------|--------|
| `LIMIT`/Paginierung  | ✔     | `OFFSET FETCH`/`TOP` | ✔ | ✔ |
| Boolesche Werte      | 1/0   | 1/0                | TRUE/FALSE | 1/0 |
| Bezeichner-Anführung | —     | —                  | `"Spalte"` | — |

### Lizenz

GPL v2 — siehe `GNUv2_License.txt`.
