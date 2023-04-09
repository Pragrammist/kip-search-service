all:
	docker build -t kip-searchservice .
	docker compose up