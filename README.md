# NetCrudExample

Minimal example of a simple Crud rest Api in C#

## Considerations and shortcomings

* Repository:
  * Consider a Maybe/Option type return from the Get operation instead of a nullable
  * Consider using mutable Employee class and setting it's Id on insert rather than just returning the Id
  * Could use interface with more generic data-language (save/load instead of insert/update/get) to support different storage mechanisms behind repo interface - in practice sql is is standard for such repos.
  * Orm use would make repo code easier to read
  * Soft deletes would probably make more sense
  * In practice might not need to care about race conditions in slow moving api, but they are definitely possible with the MaxOccupation condition

* Tests
  * Integration vs. unit tests. I personally prefer integration/end-to-end/functional over unit tests, but understand others prefer differently
  * Testing on a local database is faster (especially if no schema changes) but docker can be easier to integrate in pipelines
  * Consider Autofixture or similar for testdata

* System guarantees
  * Guarantees can be made on the database but require triggers for the MaxOccupation one - or denying access on object and only having that on a stored proc. A more pragmatic approach is probably to loosen guarantees a bit and just have interactions through the Api with only very restricted access to direct table interactions
  * When should birthday checks be made? On insert/update or scheduled in order to check "vampire" criteria - a pragmatic approach is imo better than guarantees again.

* Performance
  * Outside of using Async no big considerations for this simple api.
  * On the database a nonclustered index on reg.Employee(OfficeId) could make sense for queries ala SELECT FirstName, LastName FROM reg.Employees e INNER JOIN reg.Offices o ON o.Id=e.OfficeId WHERE o.Location='Silkeborg'. But I'd much rather make that assesment when seeing real usage. Also given the size of these tables it's no issue imo.

* Configs/Environments/Security
  * Ideally sql connections are made with ad-users but other sensitive info should be stored in Azure Keyvault or similar. Otherwise at least make sure not to commit sensitive info to source control.

* Philosophical:
  * Primitive obsession
