using OpenQA.Selenium;

public class LoginPage : BasePage
{
    public LoginPage(IWebDriver driver) : base(driver) { }

    public void Open()
        => Driver.Navigate().GoToUrl($"{TestSettings.BaseUrl}/login");

    public void Login(string user, string pass)
    {
        Driver.FindElement(By.Id("username")).SendKeys(user);
        Driver.FindElement(By.Id("password")).SendKeys(pass);
        Driver.FindElement(By.Id("loginBtn")).Click();
    }
}
