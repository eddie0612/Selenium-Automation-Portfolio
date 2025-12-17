using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace PChomeTests
{
	/// <summary>
	/// PChome 24h 購物網站自動化測試
	/// 功能：批次商品價格查詢與驗證
	/// </summary>
	public class PChome
	{
		// 定義司機變數
		IWebDriver driver;

		[SetUp]
		public void Setup()
		{
			// 設定瀏覽器
			ChromeOptions options = new ChromeOptions();
			options.AddArgument("--incognito"); // 無痕模式
			options.AddArgument("--headless");

			// 啟動司機
			driver = new ChromeDriver(options);
			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
		}

		
		[Test]
		public void Test_PChome_GetPrice()
		{
			driver.Navigate().GoToUrl("https://24h.pchome.com.tw/");
			// 測試資料：包含存在的與不存在的商品
			string[] ShopList = { "iPhone 17 Pro", "PS5", "Airpod Pro15" };
			
			foreach (var product in ShopList)
			{
				Console.WriteLine($"正在搜尋:{product}");
				try
				{
					var serchBox = driver.FindElement(By.ClassName("c-search__input"));
					// 清除輸入框 (應對 React 框架的特殊處理)
					serchBox.SendKeys(Keys.Control + "a");
					serchBox.SendKeys(Keys.Backspace);
					//輸入關鍵字並搜尋
					serchBox.SendKeys(product + Keys.Enter);
					WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
					// 等待價格元素出現 (確保頁面載入完成)
					wait.Until(d => d.FindElement(By.CssSelector("[data-regression='store_prodPrice']")).Displayed);
					//抓取資料進行驗證
					var prodPrice = driver.FindElement(By.CssSelector("[data-regression='store_prodPrice']"));
					var prodName = driver.FindElement(By.CssSelector("[data-regression='store_prodName']"));
					string foundName = prodName.Text;
					string foundPrice = prodPrice.Text;
					//驗證名稱 (忽略大小寫)
					if (foundName.Contains(product, StringComparison.OrdinalIgnoreCase)){
						var priceElement = driver.FindElement(By.CssSelector("[data-regression='store_prodPrice']")).Text;
						Console.WriteLine($"成功!{product}的價格是:{foundPrice}");
						Thread.Sleep(2000);
					} 
					else {
						Console.WriteLine($"失敗!找不到{product}");
					}
				}
				catch (WebDriverTimeoutException)
				{
					Console.WriteLine($"失敗!找不到{product}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"發生意外錯誤:{ex.Message}");
				}
			}
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