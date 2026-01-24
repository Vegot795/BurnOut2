using System.Linq;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Burn_Out.AcceptanceTests.StepDefinitions;

[Binding]
public class HallSteps
{
    private ApplicationDbContext _db = default!;

    [Given(@"I have an empty database")]
    public void GivenIHaveAnEmptyDatabase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _db = new ApplicationDbContext(options);
        _db.Database.EnsureDeleted();
        _db.Database.EnsureCreated();
    }

    [When(@"I create a hall with name ""(.*)"" and capacity (.*)")]
    public void WhenICreateAHallWithNameAndCapacity(string hallName, int capacity)
    {
        var hall = new HallModel { HallName = hallName, Capacity = capacity, IsAvailable = true };
        _db.Halls.Add(hall);
        _db.SaveChanges();
    }

    [Then(@"the hall ""(.*)"" with capacity (.*) should exist")]
    public void ThenTheHallWithCapacityShouldExist(string hallName, int capacity)
    {
        var hall = _db.Halls.FirstOrDefault(h => h.HallName == hallName && h.Capacity == capacity);
        Assert.IsNotNull(hall, $"Hall {hallName} with capacity {capacity} should exist");
    }
}