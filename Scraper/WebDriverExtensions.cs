using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScout24
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutSec)
        {
            try
            {
                if (timeoutSec > 0)
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSec));
                    return wait.Until(drv => drv.FindElement(by));
                }
                return driver.FindElement(by);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static void WaitForDucumentReady(this IWebDriver driver, int timeoutSec = 30)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, timeoutSec));
            wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
        }
    }
}
