CREATE PROCEDURE reg.InsertEmployee
    @FirstName nvarchar(100), @LastName nvarchar(100), @Birthdate date, @OfficeId smallint
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @FirstName IS NULL OR @LastName IS NULL OR @Birthdate IS NULL OR @OfficeId IS NULL
    BEGIN
        RAISERROR('All parameters are required to be not null', 16, 1);
    END
    
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    
    IF (SELECT COUNT(*) FROM reg.Employees e WHERE e.OfficeId = @OfficeId) < (SELECT o.MaxOccupancy FROM reg.Offices o WHERE o.Id = @OfficeId)
    BEGIN
        INSERT INTO reg.Employees (FirstName, LastName, Birthdate, OfficeId)
        VALUES (@FirstName, @LastName, @Birthdate, @OfficeId)
        
        SELECT SCOPE_IDENTITY() AS Id
    END
    ELSE
    BEGIN
        RAISERROR('Failed to insert employee. Either bad data or max occupancy already reached for office with id %i', 11, -1, @OfficeId)
	END
    
    RETURN 0
END
