using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using System;
using System.Text.RegularExpressions;
using Xunit;


namespace PlaywrightDemo;

public class LoginTests : PageTest
{
    [Fact]
    public async Task SuccessfulLogin_ShouldShowUserPage()
    {
        await Page.PauseAsync();
        await Page.GotoAsync("localhost:5230/login");

        // Wpisanie loginu i hasła
        await Page.FillAsync("#email", "client@example.com");
        await Page.FillAsync("#password", "Pass!23");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Login"}).ClickAsync();

        await Expect(Page).ToHaveURLAsync("http://localhost:5230/user-profile");

        await Page.Context.StorageStateAsync(
            new() { Path = "loggedClient.json" });

        
    }

    /* [Fact]
     public async Task SuccesfulMeasurements_ShouldAddMeasurementPosition()
     {
         await SuccessfulLogin_ShouldShowUserPage();

         await Page.PauseAsync();
         await Expect(Page).ToHaveURLAsync("http://localhost:5230/user-profile");

         await Page.GetByRole(AriaRole.Link, new() { Name = "Postęp treningu" }).ClickAsync();
         await Expect(Page).ToHaveURLAsync("http://localhost:5230/measure");

         await Page.GetByText("Martwy ciąg").
         await Page.FillAsync("#NBP", "100");
         await Page.FillAsync("#NS", "100");
         await Page.FillAsync("#NBW", "100");
         await Page.FillAsync("#NBC", "100");
         await Page.FillAsync("#NTC", "100");
         await Page.FillAsync("#NCC", "100");
         await Page.FillAsync("#NBC", "100");
         await Page.GetByRole(AriaRole.Button, new() { Name = "Dodaj" }).ClickAsync();

         await Expect(Page.GetByRole(AriaRole.Cell)).ToHaveTextAsync("100");

     }*/

    [Fact]
    public async Task SuccessfulActiveHistory_ShouldAddPositionToActiveHistoryTable()
    {
        await SuccessfulLogin_ShouldShowUserPage();
        await Page.PauseAsync();
        await Expect(Page).ToHaveURLAsync("http://localhost:5230/user-profile");

        var targetTable = Page.Locator("div.mud-table")
                .Filter(new() { HasText = "Ostatnia aktywność" });

        int rowOldCount = await targetTable.Locator("tbody tr").CountAsync();
        

        await Page.GotoAsync("http://localhost:5230/");
        await Page.PauseAsync();
        await Expect(Page).ToHaveURLAsync("http://localhost:5230/");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Potwierdź" }).ClickAsync();

        await Page.GotoAsync("http://localhost:5230/user-profile");
        await Page.PauseAsync();
        await Expect(Page).ToHaveURLAsync("http://localhost:5230/user-profile");

        int rowNewCount = await targetTable.Locator("tbody tr").CountAsync();

        Console.WriteLine($"RowOldCount: {rowOldCount}");
        Console.WriteLine($"RowNewCount: {rowNewCount}");

        var difference = rowNewCount - rowOldCount;

        Assert.Equal(1, difference);

    }
}
