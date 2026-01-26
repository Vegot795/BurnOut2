using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace LoginPageTests;
public class LoginPageTests : TestBase
{
    [Fact]
    public void Login_Page_Should_Load()
    {
        StartDriver();
        Driver.Navigate().GoToUrl("https://localhost:5230/login");

        var emailInput = Driver.FindElement(By.Id("email-input"));
        var passwordInput = Driver.FindElement(By.Id("password-input"));
        var loginButton = Driver.FindElement(By.Id("login-button"));

        Assert.True(emailInput.Displayed);
        Assert.True(passwordInput.Displayed);
        Assert.True(loginButton.Displayed);
    }

    [Fact]
    public void Empty_Form_Should_Show_Error_Message()
    {
        StartDriver();
        Driver.Navigate().GoToUrl("https://localhost:5230/login");

        Driver.FindElement(By.Id("login-button")).Click();

        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));

        var alert = wait.Until(d =>
            d.FindElement(By.CssSelector("[role='alert']"))
        );

        Assert.Contains("Email and password are required", alert.Text);
    }

    [Fact]
    public void Invalid_Password_Should_Show_Error()
    {
        StartDriver();
        Driver.Navigate().GoToUrl("https://localhost:5230/login");

        Driver.FindElement(By.Id("email-input"))
              .SendKeys("test@test.com");

        Driver.FindElement(By.Id("password-input"))
              .SendKeys("wrongpassword");

        Driver.FindElement(By.Id("login-button")).Click();

        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));

        var alert = wait.Until(d =>
            d.FindElement(By.CssSelector("[role='alert']"))
        );

        Assert.Contains("Nieprawidłowe hasło", alert.Text);
    }

    [Fact]
    public void Valid_Login_Should_Redirect()
    {
        StartDriver();
        Driver.Navigate().GoToUrl("https://localhost:5230/login");

        Driver.FindElement(By.Id("email-input"))
              .SendKeys("admin@test.com");

        Driver.FindElement(By.Id("password-input"))
              .SendKeys("CorrectPassword123");

        Driver.FindElement(By.Id("login-button")).Click();

        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));

        wait.Until(d => d.Url.Contains("/auth/login"));

        Assert.Contains("/auth/login", Driver.Url);
    }



}
