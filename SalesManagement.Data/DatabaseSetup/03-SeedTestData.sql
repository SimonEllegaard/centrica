INSERT INTO dbo.Salesperson (Name)
VALUES
    ('Alice Jensen'),
    ('Bob Hansen'),
    ('Charlie Nielsen'),
    ('Diana Larsen'),
    ('Erik Andersen');

INSERT INTO dbo.District (Name)
VALUES
    ('North Denmark'),
    ('Southern Denmark'),
    ('Central Denmark'),
    ('Eastern Denmark')

INSERT INTO dbo.DistrictSalesperson
(DistrictId, SalespersonId, Role)
VALUES
    -- North Denmark
    (1, 1, 'Primary'),
    (1, 2, 'Secondary'),

    -- Southern Denmark
    (2, 1, 'Primary'),
    (2, 3, 'Secondary'),

    -- Central Denmark
    (3, 4, 'Primary'),

    -- Eastern Denmark
    (4, 5, 'Primary'),
    (4, 2, 'Secondary');

INSERT INTO dbo.Store (DistrictId, Name)
VALUES
    -- North Denmark
    (1, 'Aalborg Store'),
    (1, 'Frederikshavn Store'),
    (1, 'Thisted Store'),

    -- Southern Denmark
    (2, 'Odense Store'),
    (2, 'Esbjerg Store'),
    (2, 'Kolding Store'),

    -- Central Denmark
    (3, 'Aarhus Store'),
    (3, 'Herning Store'),

    -- Eastern Denmark
    (4, 'Roskilde Store'),
    (4, 'Hvidovre Store');