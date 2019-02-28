#!/bin/bash

/app/migrate.sh &
exec /opt/mssql/bin/sqlservr
