#!/bin/bash

# Wait for database
for i in {30..0}; do
  if sqlcmd -S localhost -U sa -P $SA_PASSWORD -Q 'SELECT 1;' &> /dev/null; then
    echo "$0: SQL Server started"
    break
  fi
  echo "$0: SQL Server startup in progress..."
  sleep 1
done

# Create database
echo "$0: Creating database"
sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -Q 'create database wyvern;' &> /dev/null
echo "$0: Database created"
