# README - SqlWrapper (Rochas.SqlWrapper)

**Rochas.SqlWrapper** é a camada de tradução SQL multi-dialeto compartilhada pelos componentes Rochas (`Rochas.DapperRepository`, `Rochas.BWOQ`).

Ela concentra toda a inteligência de conversão de entidades (`Poco`/`Anaemic Model`) em instruções SQL ANSI para os dialetos **MySQL, SQL Server, PostgreSQL e SQLite** — incluindo paráfrase de atributos, filtros `LIKE`, intervalos de valores (`RangeFilter`), agregações, paginação, relações e composição de entidades.

---

## 📌 Instalação

```bash
dotnet add package Rochas.SqlWrapper
```

---

## 📌 Nome das Classes

```text
EntitySqlParser      --> Parse de entidades para SQL ANSI (CRUD, consulta, count, paginado)
EntityReflector      --> Reflexão/metadata das entidades com caches thread-safe
Helpers.SQL.*        --> Constantes/templates de instruções SQL por dialeto
Exceptions           --> Exceções de domínio da camada (ex.: PropertyNotListableException)
```

## 📌 Como usar

```csharp
using Rochas.SqlWrapper.Helpers;
using Rochas.DapperRepository.Specification.Enums;

var sql = EntitySqlParser.ParseEntity(filtro, DatabaseEngine.SQLite,
                                      PersistenceAction.Query, filtro);
```

> Requer as annotations de `Rochas.DapperRepository.Specification` (`[Table]`, `[Key]`, `[Column]` opcional — sem `[Table]` usa o nome da classe, `[Key]` obrigatório).

## 📌 Suporte a múltiplos bancos

| Recurso              | MySQL | SQL Server | PostgreSQL | SQLite |
|----------------------|-------|------------|------------|--------|
| `LIMIT`/pagination    | ✔     | `OFFSET FETCH`/`TOP` | ✔ | ✔ |
| Booleanos            | 1/0   | 1/0        | TRUE/FALSE | 1/0 |
| Quote de identificador| —    | —          | `"coluna"` | — |

---