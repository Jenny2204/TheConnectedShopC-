
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using TheConnectedShop.Pages;




namespace TheConnectedShop.Tests
{
    [TestFixture]

    public class OpenHomePageTest


    {

        private IWebDriver _driver = null!;

        private WebDriverWait _wait = null!;

        private HomePage _homePage = null!;

         private SearchResultPage _searchResult = null!;

        // private ProductPage _productPage = null!;

        // private CartDrawer _cartDrawer = null!;

        [SetUp]

        public void Setup()

        {

            var options = new ChromeOptions();

            options.AddArgument("--start-maximized");

            _driver = new ChromeDriver(options);

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            _homePage = new HomePage(_driver, _wait);

            _searchResult = new SearchResultPage(_driver, _wait);

            // _productPage = new ProductPage(_driver, _wait);

            // _cartDrawer = new CartDrawer(_driver, _wait);

        }

        [Test]

        public void OpenHomePage_ShouldLoadSuccessfully()

        {

            _homePage.GoToUrl("https://theconnectedshop.com/");

            _homePage.WaitForPageLoad();

            Assert.That(_driver.Title, Does.Contain("The Connected Shop"));

            Assert.That(_homePage.GetLogo().Displayed, Is.True);

        }

        [Test]

        public void CheckLogoNavigatesToHomePage()

        {

            _homePage.GoToUrl("https://theconnectedshop.com/search?q=Smart+Door+Lock");

            _homePage.GetLogo().Click();

            _wait.Until(d => d.Url.StartsWith("https://theconnectedshop.com/"));

            Assert.That(_driver.Url, Does.StartWith("https://theconnectedshop.com/"));

        }

        [Test]

        public void CheckSearchResult()

        {

            _homePage.GoToUrl("https://theconnectedshop.com/");

            _homePage.WaitForPageLoad();

            _homePage.SearchProduct("Smart Door Lock");

            var firstResult = _searchResults.FirstResult();

            Assert.That(firstResult.Text, Does.Contain("Smart Door Lock"));

            Console.WriteLine($"First result text: {firstResult.Text}");

        }
    }
}