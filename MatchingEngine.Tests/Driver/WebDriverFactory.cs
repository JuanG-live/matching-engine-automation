using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MatchingEngine.Tests.Driver;

internal static class WebDriverFactory
{
    public static IWebDriver CreateChromeDriver()
    {
        var options = new ChromeOptions();

        if (IsHeadlessEnabled())
        {
            options.AddArgument("--headless=new");
        }

        options.AddArguments(
            "--window-size=1440,1000",
            "--disable-dev-shm-usage",
            "--no-sandbox");

        return new ChromeDriver(options);
    }

    private static bool IsHeadlessEnabled()
    {
        var value = Environment.GetEnvironmentVariable("HEADLESS");
        return !string.Equals(value, "false", StringComparison.OrdinalIgnoreCase);
    }
}
