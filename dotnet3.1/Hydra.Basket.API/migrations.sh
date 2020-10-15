
#!/bin/sh
# This is script will run the Database migration for this project
# Scripts created by Alexandre Yembo
echo "STARTING DATABASE MIGRATION"

echo "processing migration for BasketContext"
dotnet ef migrations add BasketDb -c BasketContext

echo "updating database"
dotnet ef database update
echo "migration finished!"