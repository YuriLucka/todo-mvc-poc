# Todo MVC POC

POC de ASP.NET Core MVC (.NET 10) com EF Core + PostgreSQL. Lista de tarefas simples (criar, editar, concluir, excluir), banco no Supabase e deploy no Render via Docker.

## Stack

- ASP.NET Core MVC (Model-View-Controller, views Razor + Bootstrap)
- EF Core 10 + Npgsql (migrations aplicadas no startup)
- Supabase (Postgres gerenciado) em producao, Postgres via Docker Compose localmente
- Render (Web Service, runtime Docker, plano free)

## Rodando localmente

```bash
docker compose up -d                      # Postgres em localhost:5433
cd src/TodoMvc
dotnet run                                # http://localhost:5267
```

A connection string local fica em `appsettings.Development.json`. As migrations sao aplicadas automaticamente ao subir a aplicacao.

Health check: `GET /healthz` (verifica a conexao com o banco).

## Banco: Supabase

1. Crie um projeto em https://supabase.com.
2. Em **Connect**, copie a string do **Session pooler** (porta 5432). A conexao direta do Supabase e so IPv6, e o Render nao tem saida IPv6, entao use o pooler.
3. Converta para o formato do Npgsql:

```
Host=aws-0-<regiao>.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.<project-ref>;Password=<senha>;SSL Mode=Require;Trust Server Certificate=true
```

## Deploy: Render

O `render.yaml` define um Web Service Docker com health check em `/healthz`.

1. No Render: **New > Blueprint** e selecione este repositorio (ou `render services create` pela CLI).
2. Defina a variavel `ConnectionStrings__Default` com a string do Supabase.
3. O Render injeta `PORT`; a aplicacao escuta nela automaticamente.

No plano free o servico hiberna apos ~15 min sem acesso; a primeira requisicao depois disso demora alguns segundos.
