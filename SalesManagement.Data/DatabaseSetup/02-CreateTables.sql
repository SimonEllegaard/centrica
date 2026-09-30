USE SalesManagement;

CREATE TABLE dbo.Salesperson
(
    SalespersonId INT IDENTITY(1, 1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    
    CONSTRAINT PK_Salesperson PRIMARY KEY (SalespersonId)
);

CREATE TABLE dbo.District
(
    DistrictId INT IDENTITY(1, 1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    
    CONSTRAINT PK_District PRIMARY KEY (DistrictId)
);

CREATE TABLE dbo.Store
(
    StoreId INT IDENTITY(1, 1) NOT NULL,
    DistrictId INT NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    
    CONSTRAINT PK_Store PRIMARY KEY (StoreId),
    CONSTRAINT FK_Store_District FOREIGN KEY (DistrictId) REFERENCES dbo.District (DistrictId)
);

CREATE TABLE dbo.DistrictSalesperson
(
    DistrictId INT NOT NULL,
    SalespersonId INT NOT NULL,
    Role VARCHAR(20) NOT NULL,

    CONSTRAINT PK_DistrictSalesperson PRIMARY KEY (DistrictId, SalespersonId),
    CONSTRAINT FK_DistrictSalesperson_District FOREIGN KEY (DistrictId) REFERENCES dbo.District (DistrictId),
    CONSTRAINT FK_DistrictSalesperson_Salesperson FOREIGN KEY (SalespersonId) REFERENCES dbo.Salesperson (SalespersonId),
    CONSTRAINT CK_DistrictSalesperson_Role CHECK (Role IN ('Primary', 'Secondary'))
);