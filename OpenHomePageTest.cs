using System;
using System.Linq; // Needed for First()
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace TheConnectedShop
{
    [TestFixture]
    public class OpenHomePageTest
    {
        private IWebDriver driver = null!;
        private WebDriverWait wait = null!;

        [SetUp]
        public void Setup()
        {
            // Configure and launch Chrome with a maximized window
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);

            // Prefer explicit waits; avoid mixing with implicit waits to prevent compounded delays
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

            // Explicit wait used throughout the tests for robust synchronization
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        [Test]
        public void OpenHomePage_ShouldLoadSuccessfully()
        {
            // Navigate to the site under test
            driver.Navigate().GoToUrl("https://theconnectedshop.com/");

            // Wait until the page title contains the expected text (indicates page is loaded)
            wait.Until(d => (d.Title ?? string.Empty).Contains("The Connected Shop"));

            // Assert the title indeed contains the expected text
            Assert.IsTrue(driver.Title.Contains("The Connected Shop"),
                $"Expected title to contain 'The Connected Shop', but was '{driver.Title}'");

            // Wait for the <header> element to be present and assert it is visible
            var header = wait.Until(d => d.FindElement(By.CssSelector("header")));
            Assert.IsTrue(header.Displayed, "Header is not displayed");
        }

        [Test]
        public void CheckLogoNavigatesToHomePage()
        {
            // Step 1: Navigate to a non-home page (search page for example)
            driver.Navigate().GoToUrl("https://theconnectedshop.com/search?q=Smart+Door+Lock");
            Console.WriteLine("Opened search page.");

            // Step 2: Wait for the logo to be visible
            var logo = wait.Until(d => d.FindElement(By.CssSelector("img.header__heading-logo")));
            Assert.That(logo.Displayed, Is.True, "Logo is not displayed");

            // Step 3: Click the logo
            logo.Click();
            Console.WriteLine("Clicked on logo.");

            // Step 4: Wait until URL is (or starts with) the homepage URL
            wait.Until(d => d.Url.StartsWith("https://theconnectedshop.com/"));
            Console.WriteLine("Navigated back to home page.");

            // Step 5: Assert that we are on the homepage
            Assert.That(driver.Url, Does.StartWith("https://theconnectedshop.com/"),
                "Clicking logo did not navigate to the home page.");
        }

        [Test]
        public void CheckSearchResult()
        {
            // Navigate to the site under test
            driver.Navigate().GoToUrl("https://theconnectedshop.com/");
            Console.WriteLine("Navigated to the site.");

            // Ensure the page is loaded by waiting for the logo to appear
            wait.Until(d => d.FindElement(By.CssSelector("img.header__heading-logo")));
            Console.WriteLine("Logo appeared, page loaded.");

            // Locate the search input, clear it (in case of prefilled text), and submit the query with Enter
            var searchBar = driver.FindElement(By.Id("Search-In-Inline"));
            searchBar.Clear();
            searchBar.SendKeys("Smart Door Lock" + Keys.Enter);
            Console.WriteLine("Entered search query: Smart Door Lock");

            // Wait until at least one search result item exists, then return the collection
            var searchResults = wait.Until(d =>
            {
                var items = d.FindElements(By.CssSelector(".search-results__item"));
                return items.Count > 0 ? items : null; // Returning null keeps waiting
            });

            Console.WriteLine($"Number of results found: {searchResults.Count}");

            // Validate there is at least one search result
            Assert.That(searchResults.Count, Is.GreaterThan(0), "No search results found.");

            // Take the first result and verify it contains the expected text
            var firstResult = searchResults.First();
            Console.WriteLine($"First result text: {firstResult.Text}");

            Assert.That(firstResult.Text, Does.Contain("Smart Door Lock"),
                "The first search result does not contain 'Smart Door Lock'.");
        }

        // [Test]
        // public void CheckAllSearchResults()
        // {
        //     driver.Navigate().GoToUrl("https://theconnectedshop.com/");
        //     Console.WriteLine("Navigated to the site.");

        //     // Ensure the page is loaded by waiting for the logo
        //     wait.Until(d => d.FindElement(By.CssSelector("img.header__heading-logo")));
        //     Console.WriteLine("Logo appeared, page loaded.");

        //     // Find the search bar, clear it and type the search term
        //     var searchBar = driver.FindElement(By.Id("Search-In-Inline"));
        //     searchBar.Clear();
        //     searchBar.SendKeys("Smart Door Lock" + Keys.Enter);
        //     Console.WriteLine("Entered search query: Smart Door Lock");

        //     // Wait until results appear
        //     var searchResults = wait.Until(d =>
        //     {
        //         var items = d.FindElements(By.CssSelector(".search-results__item"));
        //         return items.Count > 0 ? items : null;
        //     });

        //     Console.WriteLine($"Total results found: {searchResults.Count}");

        //     // Assert at least one result exists
        //     Assert.That(searchResults.Count, Is.GreaterThan(0), "No search results found.");

        //     // Print all product names
        //     Console.WriteLine("=== Search Results ===");
        //     foreach (var result in searchResults)
        //     {
        //         Console.WriteLine(result.Text);
        //     }
        // }

        [Test]
        public void SearchAndAddToCart_VerifyItemInCart()
        {
            // Step 1: Navigate to site
            driver.Navigate().GoToUrl("https://theconnectedshop.com/");
            Console.WriteLine("Opened the site.");

            // Step 2: Wait for logo (page loaded)
            wait.Until(d => d.FindElement(By.CssSelector("img.header__heading-logo")));

            // Step 3: Enter search query
            var searchBar = driver.FindElement(By.Id("Search-In-Inline"));
            searchBar.Clear();
            searchBar.SendKeys("Smart Door Lock" + Keys.Enter);

            // Step 4: Wait for search results
            wait.Until(d => d.FindElements(By.CssSelector(".search-results__item")).Count > 0);

            // Step 5: Click on specific product by ID (adjust if ID changes)
            var productLink = wait.Until(d => d.FindElement(By.Id("title-template--19508649459953__main-7637535391985")));
            productLink.Click();
            Console.WriteLine("Clicked on product link, navigating to PDP.");

            // Step 6: Wait for Add to Cart button and click (adjust if ID changes)
            var addToCartBtn = wait.Until(d => d.FindElement(By.Id("card-submit-button-template--19567236284657__main")));
            Assert.IsTrue(addToCartBtn.Displayed, "Add to Cart button is not visible.");
            addToCartBtn.Click();
            Console.WriteLine("Clicked Add to Cart.");

            // Step 7: Wait for the cart drawer to show the quantity input of the added item
            var quantityInput = wait.Until(d =>
                d.FindElement(By.CssSelector("input[data-quantity-variant-id='43738169409777']"))
            );

            // Step 8: Assert that the quantity input value is "1"
            string value = quantityInput.GetAttribute("value");
            Console.WriteLine($"Cart item quantity: {value}");
            Assert.That(value, Is.EqualTo("1"), "Item was not added to the cart correctly.");
        }
        [Test]
public void AddThenRemoveItemFromCart_Short()
{
    // Add
    driver.Navigate().GoToUrl("https://theconnectedshop.com/");
    wait.Until(d => d.FindElement(By.CssSelector("img.header__heading-logo")));
    driver.FindElement(By.Id("Search-In-Inline")).SendKeys("Smart Door Lock" + Keys.Enter);
    wait.Until(d => d.FindElements(By.CssSelector(".search-results__item")).Count > 0);
    wait.Until(d => d.FindElement(By.Id("title-template--19508649459953__main-7637535391985"))).Click();
    wait.Until(d => d.FindElement(By.Id("card-submit-button-template--19567236284657__main"))).Click();

    // Verify in cart drawer
    var variantId = "43738169409777";
    var qtySel = By.CssSelector($"input[data-quantity-variant-id='{variantId}']");
    wait.Until(d => d.FindElement(qtySel)); // value should be "1" כבר

    // Remove (minus on same line)
    var minusBtn = driver.FindElement(By.XPath(
        $"//input[@data-quantity-variant-id='{variantId}']/ancestor::*[contains(@class,'cart') or contains(@id,'Drawer')][1]//button[@name='minus']"));
    minusBtn.Click();

    // Assert removed
    wait.Until(d => d.FindElements(qtySel).Count == 0);
    Assert.That(driver.FindElements(qtySel).Count, Is.EqualTo(0), "Line item still exists after removal.");
}


        [TearDown]
        public void TearDown()
        {
            // Always quit and dispose the driver to release resources
            driver.Quit();
            driver.Dispose();
        }
    }
}
