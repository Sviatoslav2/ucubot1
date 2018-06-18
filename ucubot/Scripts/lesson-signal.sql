use ucubot;
CREATE TABLE lesson_signal (
    id int NOT NULL AUTO_INCREMENT primary key,
    time_stamp DATETIME,
    signal_type int NOT NULL,
    user_id VARCHAR(255) NOT NULL
);