CREATE PROCEDURE reg.UpdateEmployee
    @Id int, @FirstName nvarchar(100), @LastName nvarchar(100), @Birthdate date, @OfficeId smallint
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Id IS NULL OR @FirstName IS NULL OR @LastName IS NULL OR @Birthdate IS NULL OR @OfficeId IS NULL
    BEGIN
        RAISERROR('All parameters are required to be not null', 16, 1);
    END
    
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    
    IF (SELECT COUNT(*) FROM reg.Employees e WHERE e.OfficeId = @OfficeId AND e.Id != @Id) < (SELECT o.MaxOccupancy FROM reg.Offices o WHERE o.Id = @OfficeId)
    BEGIN
        UPDATE reg.Employees
        SET FirstName = @FirstName, LastName = @LastName, Birthdate = @Birthdate, OfficeId = @OfficeId
        WHERE Id = @Id
        
        SELECT @@ROWCOUNT AS RowsAffected
    END
    ELSE
    BEGIN
        RAISERROR('Failed to update employee. Bad data or max occupancy already reached for office with id %i.', 11, -1, @OfficeId)
	END
    
    RETURN 0
END
