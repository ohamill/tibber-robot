# Tibber Robot

This project consists of two endpoints:

`POST /tibber-developer-test/enter-path`

* Accepts a path for a robot to follow, and returns a response detailing the robot's execution of that path, including locations visited and cleaned and the duration of the execution.

`GET /tibber-developer-test-enter-path/{id}`

* Gets an execution report matching the supplied ID

## How to Run
After cloning the GitHub repository, `cd` from the project's root directory to `RobotCleaner` and run `docker compose up --build` (You may need to run `sudo docker compose up --build`).

## How to Test
From the `RobotCleaner.Test` directory, run `dotnet test`.

## Tested On
MacOS 14.0 Sonoma<br>
Debian 11.3 ARM64