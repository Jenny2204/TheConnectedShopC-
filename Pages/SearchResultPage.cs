using OpenQA.Selenium;            // Selenium main library (WebDriver, IWebElement, By)
using OpenQA.Selenium.Support.UI; // WebDriverWait support
using System;                     // Basic C# utilities (TimeSpan, etc.)
using TheConnectedShop.Pages;     // Namespace for your Page Objects (not strictly needed here)
using System.Linq;                // For LINQ methods like .First()

namespace TheConnectedShop.Pages   // Namespace for Page Objects
{
    public class SearchResultPage  // Page Object for the "Search Results" page
    {
        private readonly IWebDriver _driver;   // Browser controller (Selenium WebDriver)
        private readonly WebDriverWait _wait;  // Explicit wait (waits until condition is true)
 
        public SearchResultPage(IWebDriver driver, WebDriverWait wait)
        {
            _driver = driver;  // Inject WebDriver from the test
            _wait = wait;      // Inject WebDriverWait from the test
        }
 
        // CSS locator for all result items on the search page
        private By ResultItems => By.CssSelector(".search-results__item");
 
        /// <summary>
        /// Returns the first search result element (IWebElement).
        /// </summary>
        public IWebElement FirstResult()
        {
            // Wait until there is at least one result item on the page
            var results = _wait.Until(d =>
            {
                var items = d.FindElements(ResultItems);  // Find all elements that match the locator
                return items.Count > 0 ? items : null;    // If list is not empty → return it, else keep waiting
            });
 
            return results.First(); // Take the first element from the list
        }
 
        /// <summary>
        /// Clicks the first result element.
        /// </summary>
        public void ClickFirstResult() => FirstResult().Click();
 
        /// <summary>
        /// Returns the total number of search results found.
        /// </summary>
        public int GetResultCount() => _driver.FindElements(ResultItems).Count;
    }
}
