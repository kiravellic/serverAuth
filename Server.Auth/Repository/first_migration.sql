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
