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
using Rochas.Data.Specification.Enums;

var sql = EntitySqlParser.ParseEntity(filtro, DatabaseEngine.SQLite,
                                      PersistenceAction.Query, filtro);
```

> Requer as annotations de `Rochas.Data.Specification` (`[Table]`, `[Key]`, `[Column]` opcional — sem `[Table]` usa o nome da classe, `[Key]` obrigatório).

---

## 📌 Qualidade e cobertura de testes

A suite de testes unitários (xUnit) cobre `Rochas.SqlWrapper.Test`, validando os quatro dialetos, projeções, `groupAttributes`, agregações, relações, `RangeFilter` e ocorrências de `LIKE` parametrizado.

| Métrica | Valor |
|---------|-------|
| Total de testes | **91** |
| Cobertura de linha — `Rochas.SqlWrapper` | **99,7%** |
| Linhas cobertas | 981 / 984 |

![line coverage](https://img.shields.io/badge/coverage-line-99.7%25-brightgreen)

Cobertura por classe (`Rochas.SqlWrapper`):

| Classe | Cobertura de linha |
|--------|--------------------|
| `EntitySqlParser` | 99,6% |
| `EntityReflector` | 99,7% |
| `Helpers.SQL.SQLStatements` | 100% |
| `Exceptions.PropertyNotListableException` | 100% |

## 📌 Suporte a múltiplos bancos

| Recurso              | MySQL | SQL Server | PostgreSQL | SQLite |
|----------------------|-------|------------|------------|--------|
| `LIMIT`/pagination    | ✔     | `OFFSET FETCH`/`TOP` | ✔ | ✔ |
| Booleanos            | 1/0   | 1/0        | TRUE/FALSE | 1/0 |
| Quote de identificador| —    | —          | `"coluna"` | — |

---