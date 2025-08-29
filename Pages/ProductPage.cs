using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace TheConnectedShop.Pages
{
    public class ProductPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public ProductPage(IWebDriver driver, WebDriverWait wait)
        {
            _driver = driver;
            _wait = wait;
        }

        // Try multiple common selectors for "Add to cart" to be theme-resilient
        private By[] AddToCartSelectors => new[]
        {
            By.CssSelector("button[id*='cart-submit'], button[id*='card-submit-button']"),
            By.CssSelector("button[name='add'], button[type='submit'][name='add']"),
            By.CssSelector("button.add-to-cart, form[action*='cart'] button[type='submit']")
        };

        private IWebElement FindAddToCartButton()
        {
            foreach (var by in AddToCartSelectors)
            {
                try
                {
                    var el = _wait.Until(ExpectedConditions.ElementToBeClickable(by));
                    if (el.Displayed && el.Enabled) return el;
                }
                catch (WebDriverTimeoutException) { /* try next selector */ }
            }
            throw new NoSuchElementException("Add to Cart button not found with known selectors.");
        }

        public void AddToCart()
        {
            var addBtn = FindAddToCartButton();
            addBtn.Click();
            // wait for cart drawer or mini-cart signal
            _wait.Until(d =>
                d.FindElements(By.CssSelector(".cart-drawer,[id*='CartDrawer'],[data-cart-drawer]")).Any()
                || d.Url.Contains("cart")); // fallback if it navigates to /cart
        }
    }

    public class CartDrawer
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public CartDrawer(IWebDriver driver, WebDriverWait wait)
        {
            _driver = driver;
            _wait = wait;
        }

        // Generic selectors for the drawer container and minus button near a given line item
        private By DrawerContainer => By.CssSelector(".cart-drawer,[id*='CartDrawer'],[data-cart-drawer]");
        private static By QuantityInputBy(string variantId) =>
            By.CssSelector($"input[data-quantity-variant-id='{variantId}']");

        public void WaitUntilOpen()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(DrawerContainer));
        }

        public IWebElement GetQuantityInput(string variantId)
        {
            if (string.IsNullOrWhiteSpace(variantId))
                throw new ArgumentException("variantId cannot be null or empty.", nameof(variantId));

            WaitUntilOpen();
            return _wait.Until(d => d.FindElement(QuantityInputBy(variantId)));
        }

        public void RemoveItem(string variantId)
        {
            WaitUntilOpen();

            // Find the row that contains the quantity input of this variant
            var qty = GetQuantityInput(variantId);

            // Find a nearby "minus" button within the same line item container
            // Prefer CSS when possible; many themes use name='minus' on the button
            var lineItem = qty.FindElement(By.XPath("./ancestor::*[contains(@class,'cart') or contains(@id,'Drawer')][1]"));
            var minusBtn = lineItem.FindElement(By.CssSelector("button[name='minus'],button[data-action='decrement']"));

            minusBtn.Click();

            // Wait until this variant's input disappears (item removed)
            _wait.Until(d => d.FindElements(QuantityInputBy(variantId)).Count == 0);
        }
    }
}
