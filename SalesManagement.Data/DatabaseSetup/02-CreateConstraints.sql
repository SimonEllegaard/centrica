CREATE UNIQUE INDEX UX_DistrictSalesperson_OnePrimary
    ON dbo.DistrictSalesperson (DistrictId)
    WHERE Role = 'Primary';

ALTER TABLE dbo.District
    ADD CONSTRAINT UQ_District_Name UNIQUE (Name);