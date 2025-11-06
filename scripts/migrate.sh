#!/usr/bin/env bash

# Runs EF Core migrations after ensuring the postgres container is up.
# Requires docker compose and dotnet CLI to be available.

set -euo pipefail

PROJECT_INFRA="Scheduler.Infrastructure"
PROJECT_API="Scheduler.Api"
DB_USER="scheduler"
DB_NAME="schedulerdb"
DB_PASSWORD="schedulerpass"
DB_HOST="localhost"
DB_PORT="5432"
CONNECTION_STRING="Host=$DB_HOST;Port=$DB_PORT;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD"

echo "[1/5] Subindo container do Postgres (se ainda não estiver)..."
docker compose up -d postgres

echo "[2/5] Aguardando Postgres aceitar conexões..."
for i in {1..20}; do
  if docker exec scheduler.postgres pg_isready -U "$DB_USER" -d "$DB_NAME" >/dev/null 2>&1; then
    echo "Postgres OK"
    break
  fi

  echo "Tentativa $i..."
  sleep 1
done

echo "[3/5] Derrubando banco atual (drop database)..."
ASPNETCORE_ENVIRONMENT=Development \
ConnectionStrings__DefaultConnection="$CONNECTION_STRING" \
dotnet ef database drop \
  --force \
  --context AppDbContext \
  --project "$PROJECT_INFRA" \
  --startup-project "$PROJECT_API"

echo "[4/5] Aplicando migrations (database update)..."
ASPNETCORE_ENVIRONMENT=Development \
ConnectionStrings__DefaultConnection="$CONNECTION_STRING" \
dotnet ef database update \
  --context AppDbContext \
  --project "$PROJECT_INFRA" \
  --startup-project "$PROJECT_API"

echo "[5/5] Finalizado."
