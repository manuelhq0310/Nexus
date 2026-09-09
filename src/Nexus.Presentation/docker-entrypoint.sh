#!/bin/sh
# Genera wwwroot/appsettings.json a partir de la plantilla, sustituyendo
# __API_BASE_URL__ por la variable de entorno API_BASE_URL. Esto permite usar
# la MISMA imagen Docker en distintos entornos (local, staging, producción)
# sin tener que reconstruirla: solo cambia la variable de entorno del contenedor.
set -eu

API_BASE_URL="${API_BASE_URL:-http://localhost:5100}"

echo "Configurando ApiBaseUrl = ${API_BASE_URL}"

sed "s|__API_BASE_URL__|${API_BASE_URL}|g" \
  /usr/share/nginx/html/appsettings.template.json \
  > /usr/share/nginx/html/appsettings.json

exec nginx -g 'daemon off;'
