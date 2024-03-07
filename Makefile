.PHONY: build infrastructure run clean

build:
	@docker build --no-cache . -f .\src\Rinha.Backend\Dockerfile -t geovanebelisario/rinha-de-backend-2024-q1:latest

publish:
	@docker buildx build --no-cache --platform linux/amd64 . -f .\src\Rinha.Backend\Dockerfile -t geovanebelisario/rinha-de-backend-2024-q1:latest	

push:
	@docker push geovanebelisario/rinha-de-backend-2024-q1:latest

infrastructure:
	@docker-compose up -d db
	@docker-compose up -d nginx

run: infrastructure
	@docker-compose up -d api01
	@docker-compose up -d api02

clean:
	@docker-compose down