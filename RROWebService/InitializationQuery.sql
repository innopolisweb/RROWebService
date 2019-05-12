insert into Categories(CategoryId, FormPath) values
('1', 'EMPTY STRING');

insert into Teams(Id, Tour, Polygon, CategoryId) values
('0', 'kv', 1, '1'),
('1', 'kv', 1, '1'),
('2', 'fi', 2, '1'),
('3', 'kv', 3, '1'),
('4', 'kv', 2, '1'),
('5', 'fi', 1, '1'),
('6', 'kv', 7, '1'),
('7', 'fi', 7, '1'),
('8', 'kv', 3, '1'),
('9', 'fi', 2, '1'),
('10', 'kv', 5, '1');

insert into Judges(Id, Tour, Name, Polygon) values
('0', 'kv', 'Egor', 1),
('1', 'kv', 'Gubanov', 1),
('2', 'fi', 'Tolchkorojdenniy', 2),
('3', 'kv', 'Otec', 3),
('4', 'kv', 'Tolchkov', 2),
('5', 'fi', 'Povelitel', 1),
('6', 'kv', 'Pizzi', 7),
('7', 'fi', 'Andd', 7),
('8', 'kv', 'Tualetnoy', 3),
('9', 'fi', 'Bumagi', 2),
('10', 'kv', 'TolchkuPizdaAveTolchek', 5);