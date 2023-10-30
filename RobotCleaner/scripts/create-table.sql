CREATE TABLE IF NOT EXISTS executions (
    ID SERIAL PRIMARY KEY,
    Timestamp timestamp,
    Commands integer,
    Result integer,
    Duration double precision
)