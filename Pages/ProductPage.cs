using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace TheConnectedShop.Pages
{
    public class ProductPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public ProductPage(IWebDriver driver, TimeSpan timeout)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, timeout);
        }

        private By ProductTitle  => By.CssSelector("div.product__title h1");
        private By RatingBlock   => By.CssSelector(".rating.rating--in-main");
        private By RatingCount   => By.CssSelector(".rating-count span");
        private By PriceRegular  => By.CssSelector(".price__regular .price-item--regular, .price__regular .price-item");
        private By PriceSale     => By.CssSelector(".price__sale .price-item--sale, .price__sale .price-item");

        private By[] AddToCartSelectors => new[]
        {
            By.CssSelector("button[id*='cart-submit'], button[id*='card-submit-button']"),
            By.CssSelector("button[name='add'], button[type='submit'][name='add']"),
            By.CssSelector("button.add-to-cart, form[action*='cart'] button[type='submit']")
        };

        // NEW: ניווט לדף מוצר
        public void GoToProductPage(string url)
        {
            _driver.Navigate().GoToUrl(url);
            _wait.Until(ExpectedConditions.ElementIsVisible(ProductTitle));
        }

        public string GetProductTitle()
        {
            var titleElement = _wait.Until(ExpectedConditions.ElementIsVisible(ProductTitle));
            return titleElement.Text.Trim();
        }

        public double GetRatingValue()
        {
            var ratingElement = _wait.Until(ExpectedConditions.ElementIsVisible(RatingBlock));
            var ariaLabel = ratingElement.GetAttribute("aria-label");
            if (string.IsNullOrWhiteSpace(ariaLabel))
                throw new Exception("Rating aria-label not found.");

            var valueStr = ariaLabel.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
            return double.Parse(valueStr, CultureInfo.InvariantCulture);
        }

        public int GetReviewCount()
        {
            var countEl = _wait.Until(ExpectedConditions.ElementIsVisible(RatingCount));
            var digits = new string(countEl.Text.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out var n) ? n : 0;
        }

        // משופר: מחזיר decimal; מחכה ל-sale ואם אין – ל-regular; מנקה סימנים לא-מספריים
        public decimal GetPrice()
        {
            string priceText;
            try
            {
                priceText = _wait.Until(ExpectedConditions.ElementIsVisible(PriceSale)).Text;
                if (string.IsNullOrWhiteSpace(priceText))
                    throw new WebDriverTimeoutException();
            }
            catch (WebDriverTimeoutException)
            {
                priceText = _wait.Until(ExpectedConditions.ElementIsVisible(PriceRegular)).Text;
            }

            // ניקוי כל מה שלא ספרות/., או , → ל־. ואח"כ Parse Invariant
            var numeric = Regex.Replace(priceText, @"[^\d.,]", "").Trim();
            numeric = numeric.Count(c => c == ',') == 1 && numeric.Count(c => c == '.') == 0
                      ? numeric.Replace(',', '.')
                      : numeric.Replace(",", "");

            return decimal.Parse(numeric, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        private IWebElement FindAddToCartButton()
        {
            foreach (var by in AddToCartSelectors)
            {
                try
                {
                    var el = _wait.Until(ExpectedConditions.ElementToBeClickable(by));
                    if (el.Displayed && el.Enabled) return el;
                }
                catch (WebDriverTimeoutException) { /* try next */ }
            }
            throw new NoSuchElementException("Add to Cart button not found with any known selectors.");
        }

        public void AddToCart()
        {
            var addBtn = FindAddToCartButton();
            addBtn.Click();

            _wait.Until(d =>
                d.FindElements(By.CssSelector(".cart-drawer,[id*='CartDrawer'],[data-cart-drawer]")).Any()
                || d.Url.Contains("/cart", StringComparison.OrdinalIgnoreCase));
        }
    }
}
