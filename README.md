# Tibber Robot

This project consists of two endpoints:

`POST /tibber-developer-test/enter-path`

* Accepts a path for a robot to follow, and returns a response detailing the robot's execution of that path, including locations visited and cleaned and the duration of the execution.

`GET /tibber-developer-test/enter-path/{id}`

* Gets an execution report matching the supplied ID

## Prerequisites
The following should be installed before running the application:
* Docker
* .NET SDK (including the `dotnet` CLI tool)

## How to Run
From the solution's root directory, run `make up`. If needing to run Docker Compose with `sudo` privileges, you can instead run `make sudo-up`. Alternatively, you can navigate to the `RobotCleaner` directory and run `docker compose up` directly.
Once started, the application listens on port `5000` of the local host. So, for example, you can access the application's `POST` endpoint at `http://localhost:5000/tibber-developer-test/enter-path`.

## How to Stop
From the solution's root directory, run `make down`. Alternatively, you can run `docker compose down` from the `RobotCleaner` directory.

## How to Test
From the solution's root directory, run `make test`. Alternatively, you can run `dotnet test` from the `RobotCleaner.Test` directory.

## Tested On
MacOS 14.0 Sonoma
<br>
Debian 11.3 ARM64

## Database Configuration
The `docker-compose.yml` file provides default values for the Postgres database, but these can be overriden by setting environment variables on your local machine.
* `POSTGRES_USER` controls the username
* `POSTGRES_DB` controls the name of the database
* `HOSTNAME` controls the hostname