#!/usr/bin/env bash
set -euo pipefail

PROJECT_API="Scheduler.Api"
PROJECT_INFRA="Scheduler.Infrastructure"

./scripts/migrate.sh

echo "Iniciando API em modo desenvolvimento..."
dotnet watch --project "$PROJECT_API" run
