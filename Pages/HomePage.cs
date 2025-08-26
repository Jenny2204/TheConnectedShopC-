using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
 using SeleniumExtras.WaitHelpers;
 
namespace TheConnectedShop
{
    public class HomePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
 
        public HomePage(IWebDriver driver, WebDriverWait wait)
        {
            _driver = driver;
            _wait = wait;
        }
 
        private By Logo => By.CssSelector("img.header__heading-logo");
        private By SearchBar => By.Id("Search-In-Inline");
 
        public void GoToUrl(string url) => _driver.Navigate().GoToUrl(url);
 
        public void WaitForPageLoad()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(Logo));
        }
 
        public void ClickLogo() => _wait.Until(ExpectedConditions.ElementToBeClickable(Logo)).Click();
 
        public void SearchProduct(string query)
        {
            var search = _wait.Until(ExpectedConditions.ElementIsVisible(SearchBar));
            search.Clear();
            search.SendKeys(query + Keys.Enter);
        }
 
        public IWebElement GetLogo() => _wait.Until(ExpectedConditions.ElementIsVisible(Logo));
    }
    }