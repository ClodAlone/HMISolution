using Microsoft.Playwright;

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Channel = "msedge", Headless = true });
var page = await browser.NewPageAsync();
var logs = new List<string>();
page.Console += (_, msg) => logs.Add(msg.Text);
await page.GotoAsync("http://localhost:15500/screen/Alarms");
await page.WaitForTimeoutAsync(8000);
var html = await page.ContentAsync();
System.IO.File.WriteAllText("page.html", html);
Console.WriteLine("done");
