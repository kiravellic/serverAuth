# serverAuth

1. в корне проекта создайте файл .
вот с таким содержанием:

```c#
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "Config:ConnectionString" : "Host=127.0.0.1;Username=postgres;Password=mysecretpassword"
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
4. а дальше я сама хз
