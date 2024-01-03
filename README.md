# Integration Testing on Dometrain

## Problems

- connection strings set to a db that doesn't exist, for user that doesn't exist
	- add to path `C:\Program Files\PostgreSQL\16\bin` for psl.exe
	- connect `psql -U postgres -h localhost -p 5432`
	- create `CREATE DATABASE mydb;
	- create `CREATE USER course WITH PASSWORD 'changeme';`
	- grant `GRANT ALL PRIVILEGES ON DATABASE mydb TO course;`
	- grant `GRANT USAGE ON SCHEMA public TO course;`
