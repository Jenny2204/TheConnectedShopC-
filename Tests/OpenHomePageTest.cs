using System;                     // basic C# utilities (like Console, TimeSpan)
using NUnit.Framework;            // NUnit framework for writing and running tests
using OpenQA.Selenium;            // Selenium main library (WebDriver interface)
using OpenQA.Selenium.Chrome;     // Selenium Chrome driver support
using OpenQA.Selenium.Support.UI; // Support for WebDriverWait and SelectElement
using SeleniumExtras.WaitHelpers; // Extra conditions for WebDriverWait
using TheConnectedShop.Pages;     // Your Page Object classes (HomePage, SearchResultPage)
using TheConnectedShop.Configs;   // Configuration classes (like SearchConfig)

namespace TheConnectedShop.Tests   // Namespace for all your test classes
{
    [TestFixture]                  // Marks this class as a test fixture (test container)
    public class OpenHomePageTests // Your test class
    {
        private IWebDriver _driver = null!;           // WebDriver (the browser controller)
        private WebDriverWait _wait = null!;          // Explicit wait object
        private HomePage _homePage = null!;           // Page Object for HomePage
        private SearchResultPage _searchResultPage = null!; // Page Object for SearchResultPage

        [SetUp] // Runs before each test
        public void Setup()
        {
            var options = new ChromeOptions();        // Chrome browser settings
            options.AddArgument("--start-maximized"); // Start browser maximized

            _driver = new ChromeDriver(options);      // Launch Chrome with the options

            // Don't mix implicit and explicit waits → better to use explicit only
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15)); // Create WebDriverWait (15 sec)

            // Initialize Page Objects with driver + wait
            _homePage = new HomePage(_driver, _wait);
            _searchResultPage = new SearchResultPage(_driver, _wait);
        }

        [TearDown] // Runs after each test
        public void TearDown()
        {
            try { _driver?.Quit(); }   // Close the browser after test
            catch { /* ignore */ }     // Ignore errors if already closed
        }

        [Test] // First test: check if homepage loads correctly
        public void OpenHomePage_ShouldLoadSuccessfully()
        {
            _homePage.GoToUrl("https://theconnectedshop.com/"); // Navigate to homepage
            _homePage.WaitForPageLoad();                        // Wait until logo is visible

            Assert.That(_driver.Title, Does.Contain("The Connected Shop")); // Check page title
            Assert.That(_homePage.GetLogo().Displayed, Is.True);            // Check that logo is displayed
        }

        [Test] // Second test: check if clicking logo goes back to homepage
        public void CheckLogoNavigatesToHomePage()
        {
            _homePage.GoToUrl("https://theconnectedshop.com/search?q=Smart+Door+Lock"); // Go directly to search page
            _homePage.GetLogo().Click();                                               // Click on logo

            _wait.Until(d => d.Url.StartsWith("https://theconnectedshop.com/"));       // Wait until URL starts with homepage
            Assert.That(_driver.Url, Does.StartWith("https://theconnectedshop.com/")); // Verify URL
        }

        [Test] // Third test: check search functionality
        public void CheckSearchResult()
        {
            _homePage.GoToUrl("https://theconnectedshop.com/"); // Navigate to homepage
            _homePage.WaitForPageLoad();                        // Wait until page loads
            var searchConfig = SearchConfig.LoadFromFile(@"C:\Users\velic\Downloads\Projectscsharp\theconnectedshop\bin\Debug\net8.0\searchData.json");
            if (searchConfig.SearchQueries == null || searchConfig.SearchQueries.Count == 0)
            {
                Assert.Fail("Search query list is empty or not loaded.");
            }

            var query = searchConfig.SearchQueries[0];

            _searchResultPage.SearchProduct(query);         // Perform a search for "Smart Door Lock"

            var firstResult = _searchResultPage.FirstResult();  // Get the first search result
            Assert.That(firstResult.Text, Does.Contain("Smart").Or.Contain("Lock")); // Verify it contains expected text

            Console.WriteLine($"First result text: {firstResult.Text}"); // Print result text for debugging
        }
          [Test] // Second test: check if clicking logo goes back to homepage
        public void ProductPage()
        {
            _homePage.GoToUrl("https://theconnectedshop.com/products/smart-door-lock-slim"); // Go directly to search page
            _homePage.GetLogo().Click();                                               // Click on logo

            // _wait.Until(d => d.Url.StartsWith("https://theconnectedshop.com/"));       // Wait until URL starts with homepage
            // Assert.That(_driver.Url, Does.StartWith("https://theconnectedshop.com/")); // Verify URL
        }

    }
}
