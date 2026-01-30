using Burn_Out.Tests.Helpers;
using Infrastructure.Data;
using Infrastructure.Models;

namespace Burn_Out.Test.Components.Pages;

[TestClass]
public class Register
{
    private ApplicationDbContext _db = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = DbContextFactoryTests.Create();
    }

    [TestMethod]
    public void CreateUser_ShouldBeSavedInDatabase()
    {
        //Arrange
        var newUser = new ApplicationUser
        {
            FirstName = "Lugi",
            LastName = "Kowalski",
            Email = "lugi.kowalski@example.com",
            PasswordHash = "SecurePassword123",
            DateOfBirth = new DateTime(1990, 5, 15),
            CreatedAt = DateTime.UtcNow,
            PhoneNumber = 1234567890
        };

        //Act
        _db.Users.Add(newUser);
        _db.SaveChanges();

        //Assert
        var savedUser = _db.Users.FirstOrDefault(u => u.Email == "lugi.kowalski@example.com");
        Assert.IsNotNull(savedUser);
        Assert.AreEqual("Lugi", savedUser!.FirstName);
        Assert.AreEqual("Kowalski", savedUser.LastName);
        Assert.AreEqual(new DateTime(1990, 5, 15), savedUser.DateOfBirth);
        Assert.AreEqual(1234567890, savedUser.PhoneNumber);
    }
}