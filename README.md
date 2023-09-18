# serverAuth

1. в корне проекта создайте файл .
вот с таким содержанием:

```c#
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "Config:ConnectionString" : "Host=127.0.0.1;Username=postgres;Password=mysecretpassword;Database=database"
    }
  }
```
2. потом запустите docker и вставьте поднятие постгри
``` bash
docker run --name postgres_container -e POSTGRES_PASSWORD=mysecretpassword -d -p 5432:5432 -v postgres_data:/var/lib/postgresql/data postgres
```
3. сделайте базку 'database' и как угодно в базке исполните сие чудо
``` sql
create table if not exists user_info
(
    id bigserial primary key,
    user_name varchar(255),
    games_count bigint not null default 0,
    constraint cu_user_name unique (user_name)
    );

create index idx_username on user_info(user_name);

INSERT INTO public.user_info (id, user_name, games_count) VALUES (DEFAULT, 'jay'::varchar(255), DEFAULT);

create table if not exists game_state
(
    id int primary key,
    name varchar(255)
    );

insert  into game_state values (1, 'in_progress'), (2,'finished');

create table if not exists games_info
(
    id bigserial primary key,
    game_state int,
    user_id bigint not null,
    number bigint not null,
    foreign key (user_id) references user_info(id),
    foreign key (game_state) references game_state(id)
    );
create index idx_username on games_info(user_id);

```
4. дальше запустите конфигурацию по пути ./run/Server.Auth.xml
5. перейдите по юрл http://localhost:7071/api/swagger/ui

играйте только не забутье замочек с авторизацией на хендлере после логина:
![Снимок экрана 2023-09-18 в 14 18 01](https://github.com/kiravellic/serverAuth/assets/124602696/9039ddff-51b2-4045-8c64-6927314538b4)


![Снимок экрана 2023-09-18 в 14 18 19](https://github.com/kiravellic/serverAuth/assets/124602696/70478913-0165-418b-9323-8f8179c8adff)

![Снимок экрана 2023-09-18 в 14 18 28](https://github.com/kiravellic/serverAuth/assets/124602696/45466009-cac2-45b4-b928-413a61e1b037)
