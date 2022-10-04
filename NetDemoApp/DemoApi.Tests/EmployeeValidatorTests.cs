using DemoApi.Employee.Model;
using DemoApi.Employee.Validation;
using FluentAssertions;

namespace DemoApi.Tests;

public class EmployeeValidatorTests
{
    private EmployeeValidator sut;

    [SetUp]
    public void SetUp()
    {
        sut = new EmployeeValidator();
    }

    [Test]
    [TestCase("John", true)]
    [TestCase("John Smith", true)]
    [TestCase("", false)]
    [TestCase("  ", false)]
    public void ShouldValidateFirstName(string firstName, bool expectedResult)
    {
        var employee = new EmployeeModel(null, firstName, "Doe", new DateTime(1990, 1, 1), 1);

        var result = sut.Validate(employee);

        result.IsValid.Should().Be(expectedResult);
        result.Errors.Any(x => x.Contains("Firstname", StringComparison.InvariantCultureIgnoreCase)).Should().Be(!expectedResult);
    }

    [Test]
    [TestCase("Smith", true)]
    [TestCase("", false)]
    [TestCase("  ", false)]
    [TestCase("W. Bush", false)]
    public void ShouldValidateLastName(string lastName, bool expectedResult)
    {
        var employee = new EmployeeModel(null, "John", lastName, new DateTime(1990, 1, 1), 1);

        var result = sut.Validate(employee);

        result.IsValid.Should().Be(expectedResult);
        result.Errors.Any(x => x.Contains("Lastname", StringComparison.InvariantCultureIgnoreCase)).Should().Be(!expectedResult);
    }

    [Test]
    [TestCase(21, true)]
    [TestCase(62, true)]
    [TestCase(3, false)]
    [TestCase(150, false)]
    [TestCase(0, false)]
    [TestCase(-1, false)]
    public void ShouldValidateBirthDate(int yearsBack, bool expectedResult)
    {
        var employee = new EmployeeModel(null, "John", "Doe", DateTime.Now.AddYears(-yearsBack), 1);
        var result = sut.Validate(employee);
        result.IsValid.Should().Be(expectedResult);
        result.Errors.Any(x => x.Contains("Birthdate", StringComparison.InvariantCultureIgnoreCase)).Should().Be(!expectedResult);
    }
}
