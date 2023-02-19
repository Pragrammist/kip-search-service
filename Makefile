all:
	docker build -t kip-searchservice .
	docker pull docker.elastic.co/elasticsearch/elasticsearch:8.6.0
	docker compose up