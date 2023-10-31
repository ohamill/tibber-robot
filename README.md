# Tibber Robot

This project consists of two endpoints:

`POST /tibber-developer-test/enter-path`

* Accepts a path for a robot to follow, and returns a response detailing the robot's execution of that path, including locations visited and cleaned and the duration of the execution.

`GET /tibber-developer-test/enter-path/{id}`

* Gets an execution report matching the supplied ID

## How to Run
After cloning the GitHub repository, start the Docker daemon and run `make up` from the solution's root directory

## How to Stop
From the solution's root directory, run `make down`

## How to Test
From the solution's root directory, run `make test`

## Tested On
MacOS 14.0 Sonoma
<br>
Debian 11.3 ARM64