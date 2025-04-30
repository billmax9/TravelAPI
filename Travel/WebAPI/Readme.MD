### Start docker mssql container:
```shell
docker run -e 'ACCEPT_EULA=Y' \
           -e 'MSSQL_SA_PASSWORD=YourStrongPassword123!' \
           -p 1433:1433 \
           --name mssql-dev \
           -d mcr.microsoft.com/mssql/server:2022-latest
```

- 200 OK
- 201 Created
- 400 Bad Request
- 404 Not Found
- 409 Conflict
- 500 Internal Server Error
- 501 Not Implemented