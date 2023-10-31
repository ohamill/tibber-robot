.PHONY: build up sudo-up down sudo-down test

build:
	dotnet build

up: build
	cd RobotCleaner && docker compose up -d --build

sudo-up: build
	cd RobotCleaner && sudo docker compose up -d --build

down:
	cd RobotCleaner && docker compose down

sudo-down: build
	cd RobotCleaner && sudo docker compose down
	
test: build
	cd RobotCleaner.Test && dotnet test