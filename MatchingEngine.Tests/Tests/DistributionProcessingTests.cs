using MatchingEngine.Tests.Driver;
using MatchingEngine.Tests.Pages;
using NUnit.Framework;
using OpenQA.Selenium;

namespace MatchingEngine.Tests.Tests;

[TestFixture]
public sealed class DistributionProcessingTests
{
    private IWebDriver _driver = null!;

    [SetUp]
    public void SetUp()
    {
        _driver = WebDriverFactory.CreateChromeDriver();
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    [Test]
    public void SolutionsMenuNavigatesToDistributionProcessingAndDisplaysExpectedContent()
    {
        var homePage = new HomePage(_driver, TimeSpan.FromSeconds(15))
            .Open()
            .ExpandSolutionsMenu();

        Assert.That(
            homePage.GetDisplayedSolutions(),
            Is.EqualTo(HomePage.GetExpectedSolutions()),
            "The Solutions menu did not contain the expected ordered list.");

        var distributionPage = homePage
            .OpenDistributionProcessing()
            .ScrollToAllInOneSection();

        var sectionContent = distributionPage.GetSectionContent();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sectionContent, Does.Contain(DistributionProcessingPage.SectionHeading));
            Assert.That(sectionContent, Does.Contain(DistributionProcessingPage.SectionIntroduction));
            Assert.That(sectionContent, Does.Contain(DistributionProcessingPage.SectionLead));

            foreach (var benefit in DistributionProcessingPage.GetExpectedBenefits())
            {
                Assert.That(sectionContent, Does.Contain(benefit));
            }
        }
    }
}
