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

        _wait.Until(FindSolutionsMenuTrigger);
        ClickSolutionsMenuTrigger();
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

    private void ClickSolutionsMenuTrigger()
    {
        _wait.Until(driver =>
        {
            var solutionsTrigger = FindSolutionsMenuTrigger(driver);
            if (solutionsTrigger is null)
            {
                return false;
            }

            try
            {
                solutionsTrigger.Click();
            }
            catch (ElementClickInterceptedException)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", solutionsTrigger);
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }

            try
            {
                return string.Equals(
                           solutionsTrigger.GetAttribute("aria-expanded"),
                           "true",
                           StringComparison.OrdinalIgnoreCase) ||
                       GetVisibleSolutionLinks(driver).Count > 0;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }

    private static IWebElement? FindSolutionsMenuTrigger(IWebDriver driver)
    {
        foreach (var element in driver.FindElements(By.CssSelector("header nav [aria-haspopup='menu']")))
        {
            try
            {
                if (!element.Displayed || element.Size.Width <= 0)
                {
                    continue;
                }

                var ariaLabel = element.GetAttribute("aria-label")?.Trim();
                var text = element.Text.Trim();
                if (string.Equals(ariaLabel, "Solutions", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(text, "Solutions", StringComparison.OrdinalIgnoreCase))
                {
                    return element;
                }
            }
            catch (StaleElementReferenceException)
            {
                continue;
            }
        }

        return null;
    }

    private static IReadOnlyList<IWebElement> GetVisibleSolutionLinks(IWebDriver driver) =>
        GetVisibleLinksFromControlledSolutionsMenu(driver) ??
        driver.FindElements(By.CssSelector("[role='menu'] a, nav [aria-label*='submenu'] a"))
            .Where(element => element.Displayed && !string.IsNullOrWhiteSpace(element.Text))
            .Where(element => ExpectedSolutions.Contains(element.Text.Trim(), StringComparer.OrdinalIgnoreCase))
            .ToList();

    private static IReadOnlyList<IWebElement>? GetVisibleLinksFromControlledSolutionsMenu(IWebDriver driver)
    {
        var trigger = FindSolutionsMenuTrigger(driver);
        if (trigger is null)
        {
            return null;
        }

        try
        {
            var controlledMenuId = trigger.GetAttribute("aria-controls");
            if (string.IsNullOrWhiteSpace(controlledMenuId))
            {
                return null;
            }

            var links = driver.FindElements(By.Id(controlledMenuId))
                .SelectMany(menu => menu.FindElements(By.CssSelector("a")))
                .Where(element => element.Displayed && !string.IsNullOrWhiteSpace(element.Text))
                .ToList();

            return links.Count > 0 ? links : null;
        }
        catch (StaleElementReferenceException)
        {
            return null;
        }
    }

    private void DismissCookieBannerIfPresent()
    {
        var denyButton = _driver.FindElements(By.XPath("//button[normalize-space()='Deny']"))
            .FirstOrDefault(element => element.Displayed);
        denyButton?.Click();
    }
}
