#!/bin/sh
set -e

echo "database-migrator: waiting for postgres..."
until pg_isready -h postgres -U hotelos; do
  sleep 1
done

echo "database-migrator: creating databases..."

for db in hotelos_room hotelos_reception hotelos_housekeeping hotelos_payments hotelos_users hotelos_maintenance hotelos_websocket hotelos_gateway; do
  psql -h postgres -U hotelos -tc "SELECT 1 FROM pg_database WHERE datname = '$db'" | grep -q 1 \
    || psql -h postgres -U hotelos -c "CREATE DATABASE $db"
  echo "database-migrator: $db OK"
done

echo "database-migrator: creating schemas..."
psql -h postgres -U hotelos -d hotelos_room -c "CREATE SCHEMA IF NOT EXISTS room_service"
psql -h postgres -U hotelos -d hotelos_reception -c "CREATE SCHEMA IF NOT EXISTS reception"
psql -h postgres -U hotelos -d hotelos_housekeeping -c "CREATE SCHEMA IF NOT EXISTS housekeeping"
psql -h postgres -U hotelos -d hotelos_payments -c "CREATE SCHEMA IF NOT EXISTS payments"
psql -h postgres -U hotelos -d hotelos_users -c "CREATE SCHEMA IF NOT EXISTS users"
psql -h postgres -U hotelos -d hotelos_maintenance -c "CREATE SCHEMA IF NOT EXISTS maintenance"
psql -h postgres -U hotelos -d hotelos_websocket -c "CREATE SCHEMA IF NOT EXISTS websocket"
psql -h postgres -U hotelos -d hotelos_gateway -c "CREATE SCHEMA IF NOT EXISTS audit"
psql -h postgres -U hotelos -d hotelos_gateway -c "CREATE SCHEMA IF NOT EXISTS users"
psql -h postgres -U hotelos -d hotelos -c "CREATE SCHEMA IF NOT EXISTS websocket"
psql -h postgres -U hotelos -d hotelos -c "CREATE SCHEMA IF NOT EXISTS audit"
psql -h postgres -U hotelos -d hotelos -c "CREATE SCHEMA IF NOT EXISTS users"

echo "database-migrator: done"
