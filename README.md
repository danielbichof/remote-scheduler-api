# Remote Scheduler API

API para gerenciamento de escalas, usuários, papéis, permissões e registros de planejamento de trabalho (DDD + EF Core + PostgreSQL).

## Sumário

1. Requisitos
2. Estrutura do Projeto
3. Setup Rápido
4. Rodando com Docker
5. Rodando Local (sem Docker para API)
6. Migrações (Entity Framework Core)
7. Scripts de Automação
8. Swagger / Testes de Endpoints
9. Variáveis de Ambiente / Connection Strings
10. Troubleshooting
11. Próximos Passos

## 1. Requisitos

- .NET 9 SDK
- Docker + Docker Compose
- `dotnet-ef` instalado globalmente (ou usar `dotnet tool restore` se configurado)

```bash
dotnet tool install --global dotnet-ef
```

## 2. Estrutura do Projeto

```sh
Scheduler.Api/              -> Camada de apresentação (controllers, Program.cs)
Scheduler.Application/      -> Casos de uso / serviços de aplicação (futuro)
Scheduler.Domain/           -> Entidades de domínio, regras e interfaces
Scheduler.Infrastructure/   -> Persistência (EF Core), Repositórios, Migrations
scripts/                    -> Scripts utilitários (migrations, run dev)
docker-compose.yml          -> Orquestração Postgres + API
```

## 3. Setup Rápido

```bash
# Clonar repositório (exemplo)
git clone <url>
cd remote-scheduler-api

# Subir Postgres e aplicar migrations
./scripts/migrate.sh

# Rodar API em modo desenvolvimento (watch)
./scripts/run-dev.sh
```

Depois acesse: http://localhost:8080/swagger

## 4. Rodando com Docker (API dentro de container)

```bash
# Build e subir serviços
docker compose up -d --build

# Ver logs da API
docker compose logs -f scheduler.api
```

Swagger: http://localhost:8080/swagger

## 5. Rodando Local (somente banco em container)

```bash
docker compose up -d postgres
dotnet run --project Scheduler.Api
```

A API usará a connection string de `appsettings.Development.json` (Host=localhost).

## 6. Migrações (Entity Framework Core)

Criar nova migração:

```bash
dotnet ef migrations add NomeDaMigracao \
  --project Scheduler.Infrastructure \
  --startup-project Scheduler.Api \
  --output-dir Migrations
```

Aplicar migrations:

```bash
dotnet ef database update \
  --project Scheduler.Infrastructure \
  --startup-project Scheduler.Api
```

Remover última (se ainda não aplicada):

```bash
dotnet ef migrations remove \
  --project Scheduler.Infrastructure \
  --startup-project Scheduler.Api
```

## 7. Scripts de Automação

`./scripts/migrate.sh`:

- Sobe Postgres (se não estiver rodando)
- Aguarda disponibilidade
- Aplica migrations

`./scripts/run-dev.sh`:

- Executa `migrate.sh`
- Inicia API com `dotnet watch run`

## 8. Swagger / Testes de Endpoints

- UI: http://localhost:8080/swagger
- Arquivo `Scheduler.Api/Scheduler.Api.http` pode ser usado no VS Code (REST Client) para testar rotas manualmente.

## 9. Variáveis de Ambiente / Connection Strings

No `docker-compose.yml` (API dentro de container):

```yml
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=schedulerdb;Username=scheduler;Password=schedulerpass
```

No desenvolvimento local (`appsettings.Development.json`):

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=schedulerdb;Username=scheduler;Password=schedulerpass"
```

## 10. Troubleshooting

| Problema | Possível causa | Ação |
|----------|----------------|------|
| `password authentication failed` | Usuário/senha divergente | Ver env do compose e appsettings |
| `relation "__EFMigrationsHistory" does not exist` | Banco vazio sem migrations | Rodar `./scripts/migrate.sh` |
| `timeout connecting` | Postgres ainda subindo | Aguardar ou aumentar loop no script |
| `package downgrade` | Versões EF diferentes entre projetos | Alinhar versões nos `.csproj` |

Ver logs Postgres:

```bash
docker compose logs -f postgres
```

Entrar no banco:

```bash
docker exec -it scheduler.postgres psql -U scheduler -d schedulerdb
```

Listar tabelas:

```sql
\dt
```

## 11. Próximos Passos (Sugestões)

- Adicionar seeds (roles padrões, weekdays)
- Implementar autenticação/autorização
- Adicionar testes unitários e integração
- Pipeline CI (lint, build, migrations dry-run)

---
Qualquer dúvida, abra uma issue ou continue a conversa.
