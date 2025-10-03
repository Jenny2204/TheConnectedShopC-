using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using TheConnectedShop.Pages;

namespace TheConnectedShop.Tests
{
    [TestFixture]
    public class ProductPageTest
    {
        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private ProductPage _productPage = null!;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            _productPage = new ProductPage(_driver, TimeSpan.FromSeconds(15));
        }

        [TearDown]
        public void TearDown()
        {
            try { _driver?.Quit(); } catch { }
        }

        [Test]
        public void CheckPriceProductPage()
        {
            // נווט לעמוד המוצר
            _productPage.GoToProductPage("https://theconnectedshop.com/products/smart-door-lock-slim");

            // (אופציונלי) הדפסת הטקסט המקורי מהעמוד אם הוספת GetPriceText()
            // var priceText = _productPage.GetPriceText();
            // Console.WriteLine($"Price (text): {priceText}");

            // קבלת המחיר כמספר
            var price = _productPage.GetPrice();
            Console.WriteLine($"Price (decimal): {price}");
            Console.WriteLine($"Price (formatted): {price:F2}");

            // אימות: המחיר חייב להיות חיובי
            Assert.That(price, Is.GreaterThan(0), "Price should be > 0");
        }
    }
}
