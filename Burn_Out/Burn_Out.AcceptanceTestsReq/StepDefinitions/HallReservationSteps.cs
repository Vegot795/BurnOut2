using Application.Services;
using Core.Models;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Burn_Out.AcceptanceTests.Steps
{
    [Binding]
    public class HallReservationSteps
    {
        private readonly TestWebApplicationFactory _factory;
        private bool _reservationResult;

        public HallReservationSteps(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Given(@"a hall exists with id (.*) named ""(.*)"" and capacity (.*)")]
        public async Task GivenAHallExists(int id, string name, int capacity)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var existing = await db.Halls.FindAsync(id);
            if (existing is not null)
            {
                db.Halls.Remove(existing);
                await db.SaveChangesAsync();
            }

            var hall = new HallModel
            {
                Id = id,
                HallName = name,
                Capacity = capacity,
                IsAvailable = true,
                ReservationBegin = null,
                ReservationEnd = null,
                ReservedBy = null
            };

            await db.Halls.AddAsync(hall);
            await db.SaveChangesAsync();
        }

        [Given(@"I reserve hall (.*) from ""(.*)"" to ""(.*)"" as ""(.*)""")]
        [When(@"I reserve hall (.*) from ""(.*)"" to ""(.*)"" as ""(.*)""")]
        public async Task WhenIReserveHall(int hallId, string start, string end, string email)
        {
            var startDt = DateTime.Parse(start, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var endDt = DateTime.Parse(end, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

            using var scope = _factory.Services.CreateScope();
            var provider = scope.ServiceProvider;

            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var svc = provider.GetRequiredService<HallReservationService>();

            var user = await userManager.FindByEmailAsync(email);
            user.Should().NotBeNull("the user must exist to reserve a hall");

            _reservationResult = await svc.ReserveHallAsync(hallId, user!.Id, startDt, endDt);
        }

        [Then(@"the reservation should succeed")]
        public void ThenReservationShouldSucceed()
        {
            _reservationResult.Should().BeTrue();
        }

        [Then(@"the reservation should fail")]
        public void ThenReservationShouldFail()
        {
            _reservationResult.Should().BeFalse();
        }

        [Then(@"hall (.*) should be unavailable")]
        public async Task ThenHallShouldBeUnavailable(int hallId)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var hall = await db.Halls.FindAsync(hallId);
            hall.Should().NotBeNull();
            hall!.IsAvailable.Should().BeFalse();

            var hasReservation = db.HallReservations.Any(r => r.HallId == hallId);
            hasReservation.Should().BeTrue("a reservation record should exist for this hall");
        }
    }
}