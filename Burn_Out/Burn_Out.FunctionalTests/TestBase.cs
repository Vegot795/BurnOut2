using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

public abstract class TestBase : IDisposable
{
    protected IWebDriver Driver { get; private set; }

    protected void StartDriver()
    {
        if (Driver != null)
            return;

        var options = new ChromeOptions();
        options.AddArgument("start-maximized");             // otwórz pełny ekran
        options.AddArgument("--ignore-certificate-errors"); // ignoruj self-signed cert
        options.AddArgument("--disable-popup-blocking");    // wyłącz blokadę popupów

        Driver = new ChromeDriver(options);
    }

    public void Dispose()
    {
        if (Driver != null)
        {
            Driver.Quit();
            Driver.Dispose();
            Driver = null;
        }
    }
}
