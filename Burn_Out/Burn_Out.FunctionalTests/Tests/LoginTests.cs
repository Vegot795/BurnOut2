namespace LoginTests;

public class LoginTests : TestBase
{
    [Fact]
    public void Valid_User_Should_Login()
    {
        StartDriver();
        var login = new LoginPage(Driver);
        login.Open();
        login.Login("admin", "password123");

        Assert.True(Driver.Url.Contains("dashboard"));
    }
}
