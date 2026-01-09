create table if not exists users_trainings (
    user_id bigint not null references users on delete cascade,
    training_id bigint not null references trainings on delete cascade,

    primary key (user_id, training_id)
);
