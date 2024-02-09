using System;
using NUnit.Framework;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace TodoMvc
{
    [TestFixture]
    public class TodoMvcTests
    {
        private const string Url = "https://todomvc.com";
        private const int WAIT_FOR_ELEMENT_TIMEOUT = 30;
        private WebDriverWait WebDriverWait;
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            WebDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(WAIT_FOR_ELEMENT_TIMEOUT));
            driver.Navigate().GoToUrl(Url);
        }

        [TearDown]
        public void Cleanup()
        {
            driver.Quit();
        }

        [Test]
        public void TestTodoListAndAddingItemsToLocalStorage()
        {
            WaitAndFindElement(By.XPath("//span[@class='link' and text()='Backbone.js']")).Click();

            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("window.localStorage.clear();");
            driver.Navigate().Refresh();

            for (int i = 0; i < 5; i++)
            {
                string taskId = Guid.NewGuid().ToString();
                string taskTitle = $"Task {i + 1}";

                jsExecutor.ExecuteScript($"localStorage.setItem('{taskTitle}', 'id: {taskId}');");

                var inputField = WaitAndFindElement(By.ClassName("new-todo"));
                inputField.SendKeys(taskTitle);
                inputField.SendKeys(Keys.Enter);
            }

            IReadOnlyCollection<IWebElement> taskElements = driver.FindElements(By.CssSelector("ul.todo-list li"));
            Assert.AreEqual(5, taskElements.Count);

            driver.Navigate().Refresh();

            long localStorageCount = (long)jsExecutor.ExecuteScript("return window.localStorage.length;");
            Assert.AreEqual(5, localStorageCount);
        }

        private IWebElement WaitAndFindElement(By locator)
        {
            return WebDriverWait.Until(ExpectedConditions.ElementExists(locator));
        }
    }
}
