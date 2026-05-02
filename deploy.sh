#!/usr/bin/env bash
set -euo pipefail

REPO_DIR="/home/ulquaza/HikeConnect"
cd "$REPO_DIR"

echo "==> Pull latest"
git pull --ff-only

echo "==> Ensure shared docker network"
docker network inspect hike_net >/dev/null 2>&1 || docker network create hike_net

echo "==> Recreate HikeConnect stack"
docker compose up -d --build --remove-orphans

echo "==> Ensure Twenty .env exists"
if [ ! -f deploy/twenty/.env ]; then
  cp deploy/twenty/.env.example deploy/twenty/.env
  echo "deploy/twenty/.env created. Fill secrets and rerun script."
  exit 1
fi

echo "==> Start/Update Twenty stack"
docker compose -f deploy/twenty/docker-compose.yml --env-file deploy/twenty/.env up -d

echo "==> Health checks"
curl -fsS http://127.0.0.1:3000/healthz >/dev/null && echo "Twenty host health: OK"

# Проверка доступности Twenty из сети hike_net
docker run --rm --network hike_net curlimages/curl:8.8.0 -fsS http://server:3000/healthz >/dev/null \
  && echo "Twenty internal health (hike_net): OK"

echo "Done."
