# Database related thing
db-start: 
	docker run --name rosa-mysql \
		-e MYSQL_ROOT_PASSWORD=root \
		-e MYSQL_DATABASE=rosa_db \
		-p 3306:3306 \
		-d mysql:8

db-stop: 
	docker stop rosa-mysql

db-remove:
	docker rm rosa-mysql

db-restart:
	docker start rosa-mysql