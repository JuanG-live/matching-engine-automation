# Matching Engine UI Automation

Selenium WebDriver assessment solution for the Matching Engine website. The test runs in Google Chrome and covers the complete requested user journey.

## Covered scenario

1. Visit `https://www.matchingengine.com/`.
2. Expand **Solutions** in the header.
3. Assert the complete ordered list of displayed solutions.
4. Select **Distribution processing**.
5. Scroll to **All-in-one solution for scale**.
6. Assert the heading, introduction, lead text, and all three benefit items in that section.

## Technology

- .NET 8
- C#
- Selenium WebDriver
- NUnit
- Google Chrome
- GitHub Actions

The project uses a small Page Object Model to separate page interactions from test assertions. Selenium Manager resolves a compatible ChromeDriver automatically.

## Project structure

```text
MatchingEngine.Tests/
├── Driver/
│   └── WebDriverFactory.cs
├── Pages/
│   ├── DistributionProcessingPage.cs
│   └── HomePage.cs
└── Tests/
    └── DistributionProcessingTests.cs
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Google Chrome

## Run the tests

```bash
dotnet restore
dotnet test
```

Tests run headlessly by default. To watch the Chrome session locally:

```bash
HEADLESS=false dotnet test
```

PowerShell:

```powershell
$env:HEADLESS="false"
dotnet test
```

## Continuous integration

The `UI Tests` GitHub Actions workflow restores the project and runs the test suite in headless Chrome on every push and pull request to `main`. It can also be started manually from the Actions tab.

## Notes

- The test uses explicit waits rather than fixed sleeps.
- Locators favor semantic text, ARIA state, and scoped header elements.
- The cookie banner is dismissed when present and does not fail the scenario when absent.
- Assertions intentionally verify the current copy supplied by the live website, including its punctuation.
