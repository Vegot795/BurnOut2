using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public static class WaitHelpers
{
    public static IWebElement WaitForElement(
        IWebDriver driver, By locator, int seconds = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
        return wait.Until(d => d.FindElement(locator));
    }
}
