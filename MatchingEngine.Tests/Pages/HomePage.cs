using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MatchingEngine.Tests.Pages;

internal sealed class HomePage
{
    public const string Url = "https://www.matchingengine.com/";

    private static readonly string[] ExpectedSolutions =
    [
        "Music and copyright solutions",
        "Repertoire management",
        "Repertoire and usage matching",
        "Data ingestion and integration",
        "Distribution processing",
        "Member management",
        "Member self service"
    ];

    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public HomePage(IWebDriver driver, TimeSpan timeout)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, timeout);
    }

    public HomePage Open()
    {
        _driver.Navigate().GoToUrl(Url);
        _wait.Until(driver => driver.FindElement(By.CssSelector("header nav")));
        DismissCookieBannerIfPresent();
        return this;
    }

    public HomePage ExpandSolutionsMenu()
    {
        var mobileMenuButton = _driver.FindElements(By.CssSelector("button[aria-label='Open menu']"))
            .FirstOrDefault(element => element.Displayed);

        if (mobileMenuButton is not null)
        {
            mobileMenuButton.Click();
            _wait.Until(_ => mobileMenuButton.GetAttribute("aria-expanded") == "true");
        }

        var solutionsButton = _wait.Until(FindSolutionsMenuTrigger);

        ClickElement(solutionsButton!);
        _wait.Until(driver => GetVisibleSolutionLinks(driver).Count > 0);
        _wait.Until(driver => GetVisibleSolutionLinks(driver).Count == ExpectedSolutions.Length);
        return this;
    }

    public IReadOnlyList<string> GetDisplayedSolutions() =>
        GetVisibleSolutionLinks(_driver)
            .Select(element => element.Text.Trim())
            .ToList();

    public DistributionProcessingPage OpenDistributionProcessing()
    {
        var link = _wait.Until(driver => GetVisibleSolutionLinks(driver)
            .Single(element => element.Text.Trim().Equals("Distribution processing", StringComparison.OrdinalIgnoreCase)));

        link.Click();
        _wait.Until(driver => driver.Url.Contains("/Distribution-processing", StringComparison.OrdinalIgnoreCase));
        return new DistributionProcessingPage(_driver, _wait.Timeout);
    }

    public static IReadOnlyList<string> GetExpectedSolutions() => ExpectedSolutions;

    private void ClickElement(IWebElement element)
    {
        try
        {
            element.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
        }
    }

    private static IWebElement? FindSolutionsMenuTrigger(IWebDriver driver) =>
        driver.FindElements(By.CssSelector("header nav [aria-haspopup='menu']"))
            .Where(element => element.Displayed && element.Size.Width > 0)
            .FirstOrDefault(element =>
                string.Equals(element.GetAttribute("aria-label")?.Trim(), "Solutions", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(element.Text.Trim(), "Solutions", StringComparison.OrdinalIgnoreCase));

    private static IReadOnlyList<IWebElement> GetVisibleSolutionLinks(IWebDriver driver) =>
        driver.FindElements(By.CssSelector("[role='menu'] a, nav [aria-label*='submenu'] a"))
            .Where(element => element.Displayed && !string.IsNullOrWhiteSpace(element.Text))
            .ToList();

    private void DismissCookieBannerIfPresent()
    {
        var denyButton = _driver.FindElements(By.XPath("//button[normalize-space()='Deny']"))
            .FirstOrDefault(element => element.Displayed);
        denyButton?.Click();
    }
}
