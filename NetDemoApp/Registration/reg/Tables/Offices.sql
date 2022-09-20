CREATE TABLE [reg].[Offices]
(
    [Id]            smallint NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    [Location]      nvarchar(50) NOT NULL,
    [MaxOccupancy]  smallint
);
