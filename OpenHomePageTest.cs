// using System;
// using System.Linq; // Needed for First()
// using NUnit.Framework;
// using OpenQA.Selenium;
// using OpenQA.Selenium.Chrome;
// using OpenQA.Selenium.Support.UI;

// namespace TheConnectedShop
// {
//     [TestFixture]
//     public class OpenHomePageTest
//     {
//         private IWebDriver driver = null!;
//         private WebDriverWait wait = null!;

//         [SetUp]
//         public void Setup()
//         {
//             // Configure and launch Chrome with a maximized window
//             var options = new ChromeOptions();
//             options.AddArgument("--start-maximized");

//             driver = new ChromeDriver(options);

//             // Prefer explicit waits; avoid mixing with implicit waits to prevent compounded delays
//             driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

//             // Explicit wait used throughout the tests for robust synchronization
//             wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
//         }

//         [Test]
//         public void OpenHomePage_ShouldLoadSuccessfully()
//         {
//             // Navigate to the site under test
//             driver.Navigate().GoToUrl("https://theconnectedshop.com/");

//             // Wait until the page title contains the expected text (indicates page is loaded)
//             wait.Until(d => (d.Title ?? string.Empty).Contains("The Connected Shop"));

//             // Assert the title indeed contains the expected text
//             Assert.IsTrue(driver.Title.Contains("The Connected Shop"),
//                 $"Expected title to contain 'The Connected Shop', but was '{driver.Title}'");

//             // Wait for the <header> element to be present and assert it is visible
//             var header = wait.Until(d => d.FindElement(By.CssSelector("header")));
//             Assert.IsTrue(header.Displayed, "Header is not displayed");
//         }

//         [Test]
//         public void CheckLogoNavigatesToHomePage()
//         {
//             // Step 1: Navigate to a non-home page (search page for example)
//             driver.Navigate().GoToUrl("https://theconnectedshop.com/search?q=Smart+Door+Lock");
//             Console.WriteLine("Opened search page.");

//             // Step 2: Wait for the logo to be visible
//             var logo = wait.Until(d => d.FindElement(By.CssSelector("img.header__heading-logo")));
//             Assert.That(logo.Displayed, Is.True, "Logo is not displayed");

//             // Step 3: Click the logo
//             logo.Click();
//             Console.WriteLine("Clicked on logo.");

//             // Step 4: Wait until URL is (or starts with) the homepage URL
//             wait.Until(d => d.Url.StartsWith("https://theconnectedshop.com/"));
//             Console.WriteLine("Navigated back to home page.");

//             // Step 5: Assert that we are on the homepage
//             Assert.That(driver.Url, Does.StartWith("https://theconnectedshop.com/"),
//                 "Clicking logo did not navigate to the home page.");
//         }
//         // Start a search test
//         [Test]
//         public void CheckSearchResult()
//         {
//             // Navigate to the website
//             driver.Navigate().GoToUrl("https://theconnectedshop.com/");

//             // Wait until the logo is visible -> ensures the page is fully loaded
//             wait.Until(d => d.FindElement(By.CssSelector("img.header__heading-logo")));

//             // Find the search input by Id, clear it, type "Smart Door Lock" and press Enter
//             driver.FindElement(By.Id("Search-In-Inline"))
//                   .SendKeys("Smart Door Lock" + Keys.Enter);

//             // Assert: wait until product links appear and check that at least 1 result exists
//             Assert.That(
//                 wait.Until(d => d.FindElements(By.CssSelector("a[href*='/products/']"))).Count,
//                 Is.GreaterThan(0)  // Condition: number of results must be greater than 0
//             );
//         }

// // [Test]
// // public void AddThenRemoveItemFromCart_Short()
// // {
// //     // Open & search
// //     driver.Navigate().GoToUrl("https://theconnectedshop.com/");
// //     wait.Until(d => d.FindElement(By.CssSelector("img.header__heading-logo")));
// //     driver.FindElement(By.Id("Search-In-Inline")).SendKeys("Smart Door Lock" + Keys.Enter);
// //     wait.Until(d => d.Url.Contains("/search") || d.Url.Contains("q="));
// //     driver.FindElements(By.CssSelector("a[href*='/products/']")).First().Click();

// //     // Add to cart
// //     var addBtn = wait.Until(d =>
// //         d.FindElements(By.CssSelector("button[name='add'], button[type='submit'], [id*='card-submit-button']"))
// //          .FirstOrDefault(b => b.Displayed && b.Enabled));
// //     Assert.That(addBtn, Is.Not.Null, "Add to cart button not found.");
// //     addBtn.Click();

// //     // Wait for drawer or /cart
// //     wait.Until(d => d.Url.Contains("/cart") ||
// //                     d.FindElements(By.CssSelector(".cart-drawer, [id*='CartDrawer'], [data-cart-drawer]"))
// //                      .Any(e => e.Displayed));

// //     // Try drawer path
// //     var drawer = driver.FindElements(By.CssSelector(".cart-drawer, [id*='CartDrawer'], [data-cart-drawer]"))
// //                        .FirstOrDefault(e => e.Displayed);

// //     if (drawer != null)
// //     {
// //         // Remove inside drawer
// //         var removeBtn = drawer.FindElements(By.CssSelector(
// //                 "[id^='CartDrawer-Remove'], button[name='remove'], .cart-remove-button, button[name='minus'], a[href*='quantity=0']"))
// //             .FirstOrDefault(b => b.Displayed && b.Enabled)
// //             ?? drawer.FindElements(By.XPath("//a[contains(text(),'Remove')] | //button[contains(text(),'Remove')]"))
// //                      .FirstOrDefault();

// //         Assert.That(removeBtn, Is.Not.Null, "No remove button found in drawer.");
// //         removeBtn.Click();

// //         wait.Until(d => !drawer.FindElements(By.CssSelector(".cart-item, [data-cart-item], .cart-items > *"))
// //                                .Any(it => it.Displayed));
// //     }
// //     else
// //     {
// //         // Remove on /cart page
// //         wait.Until(d => d.Url.Contains("/cart"));
// //         var form = wait.Until(d => d.FindElement(By.CssSelector("form[action='/cart']")));

// //         var removeBtn = form.FindElements(By.CssSelector(
// //                 "[id^='CartDrawer-Remove'], button[name='remove'], .cart-remove-button, button[name='minus'], a[href*='quantity=0']"))
// //             .FirstOrDefault(b => b.Displayed && b.Enabled)
// //             ?? form.FindElements(By.XPath("//a[contains(text(),'Remove')] | //button[contains(text(),'Remove')]"))
// //                    .FirstOrDefault();

// //         Assert.That(removeBtn, Is.Not.Null, "No remove button found on cart page.");
// //         removeBtn.Click();

// //         wait.Until(d =>
// //             form.FindElements(By.CssSelector("[data-cart-item], .cart-item, .cart__row")).All(it => !it.Displayed) ||
// //             d.FindElements(By.CssSelector(".cart--empty, [data-empty-cart]")).Any(e => e.Displayed));
// //     }
// // }


//         [TearDown]
//         public void Cleanup()
//         {
//             if (driver != null)
//             {
//                 driver.Quit();
//                 driver = null;
//             }
//         }
    
//     }

//         }

    
