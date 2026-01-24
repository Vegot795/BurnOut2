using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Burn_Out.Tests.Helpers;

public static class DbContextFactoryTests
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
