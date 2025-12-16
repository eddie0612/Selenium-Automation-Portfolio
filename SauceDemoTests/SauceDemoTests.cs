using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SauceDemoTests
{
	public class SauceDemo
	{
		IWebDriver driver;

		[SetUp]
		public void Setup()
		{
			// 設定瀏覽器
			ChromeOptions options = new ChromeOptions();
			options.AddArgument("--incognito"); // 無痕模式

			driver = new ChromeDriver(options);
			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
		}

		[Test]
		public void Test_BuyProduct_Success()
		{
			// --- 動作 1: 登入 ---
			driver.Navigate().GoToUrl("https://www.saucedemo.com/");
			driver.FindElement(By.Id("user-name")).SendKeys("standard_user");
			driver.FindElement(By.Id("password")).SendKeys("secret_sauce");
			driver.FindElement(By.Id("login-button")).Click();

			// --- 動作 2: 購買商品 ---
			string targetName = "Sauce Labs Onesie";
			var productList = driver.FindElements(By.ClassName("inventory_item_name"));
			bool isFound = false;

			foreach (var item in productList)
			{
				if (item.Text == targetName)
				{
					item.Click();
					isFound = true;
					break;
				}
			}

			Assert.That(isFound, Is.True, $"找不到商品: {targetName}");

			// 等待換頁
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			wait.Until(d => d.Url.Contains("inventory-item"));

			// 加入購物車
			driver.FindElement(By.Name("add-to-cart")).Click();

			// 驗證購物車
			var cartBadge = driver.FindElement(By.ClassName("shopping_cart_badge"));

			// 驗證數量是否為 "1"
			Assert.That(cartBadge.Text, Is.EqualTo("1"), "購物車數量不正確！");
		}

		[TearDown]
		public void TearDown()
		{
			if (driver != null)
			{
				driver.Quit();   // 關閉瀏覽器
				driver.Dispose(); // 釋放記憶體
			}
		}
	}
}