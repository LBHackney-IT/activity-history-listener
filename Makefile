.PHONY: test
test:
	docker compose build activity-listener-test && \ 
	docker compose run activity-listener-test
