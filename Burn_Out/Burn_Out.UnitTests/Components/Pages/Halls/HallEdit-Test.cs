using Burn_Out.Tests.Helpers;
using Core.Models;
using Infrastructure.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Burn_Out.Tests.EfCore;

[TestClass]
public class HallEdit_EfCoreTests
{
    private ApplicationDbContext _db = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = DbContextFactoryTests.Create();
    }

    [TestMethod]
    public void CreateHall_ShouldBeSavedInDatabase()
    {
        // Arrange
        var hall = new HallModel
        {
            HallName = "Main Hall",
            Capacity = 500,
            IsAvailable = true
        };

        // Act
        _db.Halls.Add(hall);
        _db.SaveChanges();

        // Assert
        var savedHall = _db.Halls.Single();
        Assert.AreEqual("Main Hall", savedHall.HallName);
        Assert.AreEqual(500, savedHall.Capacity);
        Assert.IsTrue(savedHall.IsAvailable);
    }

    [TestMethod]
    public void UpdateHall_ShouldPersistChanges()
    {
        // Arrange
        var hall = new HallModel
        {
            HallName = "Old Name",
            Capacity = 100
        };

        _db.Halls.Add(hall);
        _db.SaveChanges();

        // Act
        hall.HallName = "Updated Name";
        hall.Capacity = 300;
        _db.SaveChanges();

        // Assert
        var updatedHall = _db.Halls.Single();
        Assert.AreEqual("Updated Name", updatedHall.HallName);
        Assert.AreEqual(300, updatedHall.Capacity);
    }

    [TestMethod]
    public void DeleteHall_ShouldRemoveFromDatabase()
    {
        // Arrange
        var hall = new HallModel
        {
            HallName = "Hall To Delete",
            Capacity = 50
        };

        _db.Halls.Add(hall);
        _db.SaveChanges();

        // Act
        _db.Halls.Remove(hall);
        _db.SaveChanges();

        // Assert
        Assert.AreEqual(0, _db.Halls.Count());
    }

    [TestMethod]
    public void ReserveHall_ShouldSetReservationFields()
    {
        // Arrange
        var begin = DateTime.Now;
        var end = begin.AddHours(2);

        var hall = new HallModel
        {
            HallName = "Conference Room",
            Capacity = 20,
            IsAvailable = true
        };

        _db.Halls.Add(hall);
        _db.SaveChanges();

        // Act
        hall.IsAvailable = false;
        hall.ReservationBegin = begin;
        hall.ReservationEnd = end;
        hall.ReservedBy = "User123";
        _db.SaveChanges();

        // Assert
        var reservedHall = _db.Halls.Single();
        Assert.IsFalse(reservedHall.IsAvailable);
        Assert.AreEqual(begin, reservedHall.ReservationBegin);
        Assert.AreEqual(end, reservedHall.ReservationEnd);
        Assert.AreEqual("User123", reservedHall.ReservedBy);
        Assert.IsTrue(reservedHall.ReservationEnd > reservedHall.ReservationBegin);
    }
}
