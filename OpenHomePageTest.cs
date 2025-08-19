using System;
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
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            // Для CI при необходимости:
            // options.AddArgument("--headless=new");
            // options.AddArgument("--no-sandbox");
            // options.AddArgument("--disable-dev-shm-usage");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        [Test]
        public void OpenHomePage_ShouldLoadSuccessfully()
        {
            driver.Navigate().GoToUrl("https://theconnectedshop.com/");

            // Ждём подходящий заголовок
            wait.Until(d => (d.Title ?? string.Empty).Contains("The Connected Shop"));

            Assert.IsTrue(driver.Title.Contains("The Connected Shop"),
                $"Expected title to contain 'The Connected Shop' but was '{driver.Title}'");

            // Ждём появления header и проверяем, что он виден
            var header = wait.Until(d => d.FindElement(By.CssSelector("header")));
            Assert.IsTrue(header.Displayed, "Header is not displayed");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}
