
CREATE TABLE IF NOT EXISTS table_from_resource (
  id INTEGER PRIMARY KEY ASC, 
  value TEXT NOT NULL);

INSERT INTO table_from_resource (id, value) VALUES (@id, @value);