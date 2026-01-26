using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

public static class WebDriverFactory
{
    public static IWebDriver Create()
    {
        var options = new EdgeOptions();

        if (TestSettings.Headless)
            options.AddArgument("--headless=new");

        options.AddArgument("start-maximized");

        return new EdgeDriver(options);
    }
}
