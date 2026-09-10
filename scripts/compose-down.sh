#!/bin/sh
set -eu
docker compose -p "${HOTJOES_COMPOSE_PROJECT:?HOTJOES_COMPOSE_PROJECT is required}" down --volumes --remove-orphans
