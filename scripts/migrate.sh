#!/usr/bin/env bash
set -euo pipefail

PROJECT_INFRA="Scheduler.Infrastructure"
PROJECT_API="Scheduler.Api"

echo "[1/4] Subindo container do Postgres (se ainda não estiver)..."
docker compose up -d postgres

# Aguarda Postgres responder
echo "[2/4] Aguardando Postgres aceitar conexões..."
for i in {1..20}; do
  if docker exec scheduler.postgres pg_isready -U scheduler -d schedulerdb >/dev/null 2>&1; then
    echo "Postgres OK"; break; fi
  echo "Tentativa $i..."; sleep 1
done

echo "[3/4] Aplicando migrations (database update)..."
dotnet ef database update \
  --project "$PROJECT_INFRA" \
  --startup-project "$PROJECT_API"

echo "[4/4] Finalizado."
