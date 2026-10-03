# TodoApp POC

POC de lista de tarefas com front e back separados, tudo em .NET 10:

```
Blazor WebAssembly (Render Static Site, CDN)  ->  ASP.NET Core API (Render Web Service)  ->  Supabase (Postgres)
```

| Projeto | Papel |
|---|---|
| `src/TodoApp.Client` | Blazor WebAssembly standalone. Chama a API via `HttpClient`; URL em `wwwroot/appsettings*.json` |
| `src/TodoApp.Api` | Minimal API REST (`/api/todos`), EF Core + Npgsql, migrations no startup, CORS, health check `/healthz` |
| `src/TodoApp.Shared` | DTOs e validacao compartilhados entre front e API |

## Endpoints

| Metodo | Rota | Descricao |
|---|---|---|
| GET | `/api/todos` | Lista (pendentes primeiro) |
| GET | `/api/todos/{id}` | Busca uma tarefa |
| POST | `/api/todos` | Cria (`{ "title": "..." }`) |
| PUT | `/api/todos/{id}` | Atualiza titulo e status |
| PATCH | `/api/todos/{id}/toggle` | Alterna concluida/pendente |
| DELETE | `/api/todos/{id}` | Exclui |

## Rodando localmente

```bash
docker compose up -d                                    # Postgres em localhost:5433
dotnet run --project src/TodoApp.Api                    # http://localhost:5116
dotnet run --project src/TodoApp.Client                 # http://localhost:5093
```

As origens permitidas no CORS ficam em `Cors:Origins` (lista separada por virgula).

## Banco: Supabase

Use a string do **Session pooler** (a conexao direta do Supabase e so IPv6; o Render nao tem saida IPv6):

```
Host=aws-0-<regiao>.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.<project-ref>;Password=<senha>;SSL Mode=Require;GSS Encryption Mode=Disable
```

## Deploy: Render

O `render.yaml` descreve os dois servicos:

- **API** (`todo-mvc-poc`): Web Service Docker, plano free, health check `/healthz`. Variaveis: `ConnectionStrings__Default` (secreta) e `Cors__Origins`.
- **Front** (`todo-app-client-poc`): Static Site. O build instala o .NET SDK via `dotnet-install.sh` e publica `out/wwwroot`. Regra de rewrite `/* -> /index.html` para as rotas do Blazor.

Cada servico so rebuilda quando seus arquivos mudam (`buildFilter`).

No plano free a API hiberna apos ~15 min sem acesso. O front (estatico) abre na hora e mostra um aviso enquanto a API acorda.
