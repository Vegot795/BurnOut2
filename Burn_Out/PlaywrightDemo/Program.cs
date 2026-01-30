using Microsoft.Playwright;

class Program
{
    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();

        // Uruchamiamy Chromium (Chrome/Edge)
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false // false = widać przeglądarkę
        });

        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://example.com");

        // Pobranie tytułu strony
        var title = await page.TitleAsync();
        Console.WriteLine($"Tytuł strony: {title}");

        // Kliknięcie elementu (przykład)
        // await page.ClickAsync("selector_css");

        // Zamknięcie przeglądarki
        await browser.CloseAsync();
    }
}
