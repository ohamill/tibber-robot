.PHONY: build up down test

build:
	dotnet build

up: build
	cd RobotCleaner && sudo docker compose up -d --build

	
down:
	cd RobotCleaner && sudo docker compose down
	
test: build
	cd RobotCleaner.Test && dotnet test