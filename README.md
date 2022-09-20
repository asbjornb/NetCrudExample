# NetCrudExample

Minimal example of a simple Crud rest Api in C#

## Considerations and shortcomings

* Repository:
  * Consider a Maybe/Option type return from the Get operation instead of a nullable
  * Consider using mutable Employee class and setting it's Id on insert rather than just returning the Id
  * Could use interface with more generic data-language (save/load instead of insert/update/get) to support different storage mechanisms behind repo interface - in practice sql is is standard for such repos.

* Tests
  * Integration vs. unit tests
  * Testing vs a local database is faster (especially if no schema changes) but docker can be easier to integrate in pipelines

* Philosophical:
  * Primitive obsession
