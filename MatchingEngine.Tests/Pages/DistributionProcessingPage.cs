using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MatchingEngine.Tests.Pages;

internal sealed class DistributionProcessingPage
{
    public const string SectionHeading = "All-in-one solution for scale";
    public const string SectionIntroduction =
        "Imagine an technology solution for collective management organisations that stays ahead of industry trends.";
    public const string SectionLead =
        "With Matching Engine's distribution processing solution, you can:";

    private static readonly string[] ExpectedBenefits =
    [
        "Distribute royalty payments quickly",
        "Provide full detail of music usage to members",
        "Reduce cost-to-distribution ratios."
    ];

    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public DistributionProcessingPage(IWebDriver driver, TimeSpan timeout)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, timeout);
    }

    public DistributionProcessingPage ScrollToAllInOneSection()
    {
        var heading = FindSectionHeading();
        ((IJavaScriptExecutor)_driver).ExecuteScript(
            "arguments[0].scrollIntoView({block: 'center'});",
            heading);
        _wait.Until(_ => heading.Displayed);
        return this;
    }

    public string GetSectionContent()
    {
        var heading = FindSectionHeading();
        return (string)((IJavaScriptExecutor)_driver).ExecuteScript(
            """
            const heading = arguments[0];
            return heading.closest('header')?.innerText ?? heading.innerText;
            """,
            heading);
    }

    public static IReadOnlyList<string> GetExpectedBenefits() => ExpectedBenefits;

    private IWebElement FindSectionHeading() => _wait.Until(driver =>
        driver.FindElements(By.XPath($"//h2[normalize-space()='{SectionHeading}']"))
            .SingleOrDefault(element => element.Displayed));
}
