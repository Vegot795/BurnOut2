using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using System;
using System.Text.RegularExpressions;
using Xunit;


namespace PlaywrightDemo;

public class PlaywrightTests : PageTest
{

    [Fact]
    public async Task Login_ClientShouldShowUserPage()
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

    [Fact]
    public async Task Login_AdminShouldShowUserPage()
    {
        await Page.PauseAsync();
        await Page.GotoAsync("localhost:5230/login");

        // Wpisanie loginu i hasła
        await Page.FillAsync("#email", "admin@example.com");
        await Page.FillAsync("#password", "Pass!23");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        await Expect(Page).ToHaveURLAsync("http://localhost:5230/user-profile");

        await Page.Context.StorageStateAsync(
            new() { Path = "loggedClient.json" });
    }

    [Fact]
     public async Task Measurements_ShouldAddMeasurementPosition()
     {
         await Login_ClientShouldShowUserPage();

         await Page.PauseAsync();
         await Expect(Page).ToHaveURLAsync("http://localhost:5230/user-profile");

         await Page.GetByRole(AriaRole.Link, new() { Name = "Postęp treningu" }).ClickAsync();
         await Expect(Page).ToHaveURLAsync("http://localhost:5230/measure");

         await Page.FillAsync("#NBP", "100");
         await Page.FillAsync("#NS", "100");
         await Page.FillAsync("#NBW", "100");
         await Page.FillAsync("#NBC", "100");
         await Page.FillAsync("#NTC", "100");
         await Page.FillAsync("#NCC", "100");
         await Page.FillAsync("#NBC", "100");
         await Page.GetByRole(AriaRole.Button, new() { Name = "Dodaj" }).ClickAsync();

        var date = "2026-01-30";
        var row = Page.Locator($"tr:has(td:text-is(\"{date}\"))").First;
        await row.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        var benchCell = row.Locator("td[data-label='Bench Press']");
        await benchCell.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        var benchText = (await benchCell.InnerTextAsync()).Trim();
        Assert.Equal("100", benchText);
        var squatCell = row.Locator("td[data-label='Squat']");
        var squatText = (await squatCell.InnerTextAsync()).Trim();
        Assert.Equal("100", squatText);
        var dateCell = row.Locator("td[data-label='Date']");
        var dateText = (await dateCell.InnerTextAsync()).Trim();
        Assert.Equal(date, dateText);

        await Expect(row).ToBeVisibleAsync();

    }

    [Fact]
    public async Task ActiveHistory_ShouldAddPositionToActiveHistoryTable()
    {
        var baseUrl = "http://localhost:5230";

        await Login_ClientShouldShowUserPage();
        await Page.PauseAsync();
        await Expect(Page).ToHaveURLAsync($"{baseUrl}/user-profile");

        var targetTable = Page.Locator("div.mud-table")
                .Filter(new() { HasText = "Ostatnia aktywność" });

        int rowOldCount = await targetTable.Locator("tbody tr").CountAsync();
        

        await Page.GotoAsync($"{baseUrl}/");
        await Page.PauseAsync();
        await Expect(Page).ToHaveURLAsync($"{baseUrl}/");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Potwierdź" }).ClickAsync();

        await Page.GotoAsync($"{baseUrl}/user-profile");
        await Page.PauseAsync();
        await Expect(Page).ToHaveURLAsync($"{baseUrl}/user-profile");

        int rowNewCount = await targetTable.Locator("tbody tr").CountAsync();

        Console.WriteLine($"RowOldCount: {rowOldCount}");
        Console.WriteLine($"RowNewCount: {rowNewCount}");

        var difference = rowNewCount - rowOldCount;

        Assert.Equal(1, difference);

    }

    [Fact]
    public async Task HallReservation_SuccessfullyCreateAHall()
    {
        var baseUrl = "http://localhost:5230";

        await Login_AdminShouldShowUserPage();

        await Expect(Page).ToHaveURLAsync($"{baseUrl}/user-profile");
        await Page.GotoAsync($"{baseUrl}/hall-list");
        await Expect(Page).ToHaveURLAsync($"{baseUrl}/hall-list");

        await Page.GetByRole(AriaRole.Link, new() { Name = "DODAJ SALĘ" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{baseUrl}/hall-edit");

        await Page.FillAsync("#HallName", "Sala Testowa");
        await Page.Locator("input#Capacity").WaitForAsync();
        await Page.FillAsync("input#Capacity", "100");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Dodaj salę" }).ClickAsync();

        await Expect(Page).ToHaveURLAsync($"{baseUrl}/hall-list");
        await Page.GetByRole(AriaRole.Cell, new() { Name = "Sala Testowa" }).IsVisibleAsync();

    }
}
