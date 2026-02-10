SELECT 'CREATE DATABASE keycloakdb'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'keycloakdb')\gexec