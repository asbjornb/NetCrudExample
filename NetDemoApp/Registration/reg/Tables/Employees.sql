CREATE TABLE [reg].[Employees]
(
    [Id]            smallint NOT NULL IDENTITY(1,1),
    [FirstName]     nvarchar(100) NOT NULL,
    [LastName]      nvarchar(100) NOT NULL,
    [Birthdate]     date NOT NULL,
    [OfficeId]      smallint NOT NULL,
    CONSTRAINT PK_Employees PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT FK_EmployeeOffice FOREIGN KEY (OfficeId) REFERENCES [reg].[Offices]([Id]),
);
