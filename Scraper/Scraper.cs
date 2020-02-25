using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AutoScout24
{
    class Scraper : IDisposable
    {
        public static void Print(String text = null, ConsoleColor? color = null)
        {
            if (text == null)
            {
                Console.WriteLine();
            }
            else if (color == null)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.ForegroundColor = (ConsoleColor)color;
                Console.WriteLine(text);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static String GetNumbers(String input)
        {
            return new String(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public static String GetNowDateTimeString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
        }

        public static Func<IWebDriver, bool> UrlContains(string fraction)
        {
            return (driver) => { return driver.Url.ToLowerInvariant().Contains(fraction.ToLowerInvariant()); };
        }

        private const int DEFAULT_TIMEOUT_PAGELOAD = 180;
        private const String SUFFIX_AUTO = "    (AUTO)";
        public const String LOG_FILENAME = "log.txt";
        private RandomGenerator Random = new RandomGenerator();
        private IWebDriver ChromeDriver;
        private WebDriverWait Wait;
        private IJavaScriptExecutor JSE;
        private String ProxyType;
        private String ProxyAddress;
        private String ProxyUsername;
        private String ProxyPassword;

        public Scraper(String ProxyType = null, String ProxyAddress = null, String ProxyUsername = null, String ProxyPassword = null)
        {
            this.ProxyType = ProxyType;
            this.ProxyAddress = ProxyAddress;
            this.ProxyUsername = ProxyUsername;
            this.ProxyPassword = ProxyPassword;
            ChromeOptions options = new ChromeOptions();
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            //String username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //if (username == @"DESKTOP-2KTBPSE\Valloon")
            //{
            //    var proxy = new Proxy();
            //    proxy.Kind = ProxyKind.Manual;
            //    proxy.IsAutoDetect = false;
            //    proxy.HttpProxy = proxy.SslProxy = "81.177.48.86:80";
            //    options.Proxy = proxy;
            //}

            //options.AddArgument("--start-maximized");
            //options.AddArgument("--auth-server-whitelist");
            //options.AddArguments("--disable-extensions");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");
            options.AddArgument("--system-developer-mode");
            options.AddArgument("--no-first-run");
            options.SetLoggingPreference(LogType.Driver, LogLevel.All);
            options.AddAdditionalCapability("useAutomationExtension", false);
            //chromeOptions.AddArguments("--disk-cache-size=0");
            //options.AddArgument("--user-data-dir=" + m_chr_user_data_dir);
#if !DEBUG
            options.AddArguments("--headless");
            options.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            options.AddArguments("--disable-plugins-discovery");
            //options.AddArguments("--profile-directory=Default");
            //options.AddArguments("--no-sandbox");
            //options.AddArguments("--incognito");
            //options.AddArguments("--disable-gpu");
            //options.AddArguments("--no-first-run");
            //options.AddArguments("--ignore-certificate-errors");
            //options.AddArguments("--start-maximized");
            //options.AddArguments("disable-infobars");

            //options.AddAdditionalCapability("acceptInsecureCerts", true, true);
#endif
            if (ProxyType != null)
            {
                String m_chr_extension_dir = Environment.CurrentDirectory + "\\ChromeExtension";
                options.AddArgument("--load-extension=" + m_chr_extension_dir + "\\Proxy-SwitchyOmega_v2.5.20");
                //options.AddExtension(m_chr_extension_dir + "\\Proxy-SwitchyOmega_v2.5.20.crx");
            }
            ChromeDriver = new ChromeDriver(chromeDriverService, options, TimeSpan.FromSeconds(DEFAULT_TIMEOUT_PAGELOAD));
            ChromeDriver.Manage().Window.Position = new Point(0, 0);
            ChromeDriver.Manage().Window.Size = new Size(1200, 900);
            ChromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            JSE = (IJavaScriptExecutor)ChromeDriver;
            Wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(180));
            Wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            if (ProxyType != null)
            {
                ChromeDriver.Navigate().GoToUrl("chrome-extension://padekgcemlokbadohgkifijomclgjgif/options.html#!/profile/proxy");
                String ip = ProxyAddress.Split(':')[0];
                String port = ProxyAddress.Split(':')[1];
                var selectProtocol = Wait.Until(drv => drv.FindElement(By.CssSelector("table.fixed-servers>tbody>tr>td:nth-of-type(2)>select")));
                new SelectElement(selectProtocol).SelectByText(ProxyType);
                var inputServer = ChromeDriver.FindElement(By.CssSelector("table.fixed-servers>tbody>tr>td:nth-of-type(3)>input"));
                inputServer.Clear();
                inputServer.SendKeys(ip);
                var inputPort = ChromeDriver.FindElement(By.CssSelector("table.fixed-servers>tbody>tr>td:nth-of-type(4)>input"));
                inputPort.Clear();
                inputPort.SendKeys(port);
                JSE.ExecuteScript($"document.querySelector(\"a[ng-click=\'applyOptions()\']\").click();");
                try
                {
                    ChromeDriver.SwitchTo().Alert().Accept();
                }
                catch { }
            }
        }

        public void StartPublishingThread(String password, String rootDirectoryPath, String dataFilename)
        {
            Thread thread = new Thread(() => StartPublishing(password, rootDirectoryPath, dataFilename));
            thread.Start();
        }

        public void StartPublishing(String Password, String RootDirectoryPath, String dataFilename)
        {
            Print($"Root directory selected : \"{RootDirectoryPath}\"\r\n");
            Print($"Reading data from \"{dataFilename}\"\r\n", ConsoleColor.Green);
            Car[] cars = Car.ReadData(dataFilename);
            String[] words = File.ReadAllText("words.txt").Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            String[] firsNames = File.ReadAllText("firstname.txt").Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            String[] lastNames = File.ReadAllText("lastname.txt").Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int carCount = cars.Length;
            int carCountSuccess = 0, carCountFailed = 0;
            for (int carIndex = 0; carIndex < carCount; carIndex++)
            {
                try
                {
                    Car car = cars[carIndex];
                    String directoryName = car.DirectoryName;
                    Print($"( {carIndex + 1} / {carCount} )    {directoryName}    [{GetNowDateTimeString()}]", ConsoleColor.Green);
                    String directoryPath = Path.Combine(RootDirectoryPath, directoryName);
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                    if (!directoryInfo.Exists)
                    {
                        Print($"Directory not found : {directoryPath}", ConsoleColor.Red);
                        continue;
                    }
                    ChromeDriver.Navigate().GoToUrl("https://www.autoscout24.de/account/register");
                    DateTime now = DateTime.Now;
                    int days = (now - new DateTime(2020, 1, 1)).Days;
                    int seconds = now.Hour * 3600 + now.Minute * 60 + now.Second;
                    String email = words[days] + words[seconds] + "@laserdog.nl";
                    Print($"\tRegistering : email = {email}");
                    do
                    {
                        var username = ChromeDriver.FindElement(By.Name("Credentials.Username"));
                        Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(username)).Click();
                        username.SendKeys(email);
                        var password = ChromeDriver.FindElement(By.Name("Credentials.Password"));
                        password.SendKeys(Password);
                        //var passwordIndicator = ChromeDriver.FindElement(By.Id("password-indicator"));
                        //wait.Until(ExpectedConditions.TextToBePresentInElement(passwordIndicator, "Passwortstärke: gering"));
                        //wait.Until(d => d.FindElement(By.Id("password-indicator")).Text.Contains("gering"));
                        var by = By.CssSelector("input[type=submit]");
                        Wait.Until(d => !d.FindElement(by).GetAttribute("class").Contains("disabled"));
                        Thread.Sleep(3000);
                        var submit = ChromeDriver.FindElement(by);
                        JSE.ExecuteScript("arguments[0].click();", submit);
                        Print($"\tOpening...");
                        ChromeDriver.Navigate().GoToUrl("https://angebot.autoscout24.de/Listing");
                        if (!ChromeDriver.Url.Contains("/Listing")) continue;
                    } while (false);
                    {
                        var el = ChromeDriver.FindElement(By.Name("makeList"));
                        var selectElement = new SelectElement(el);
                        var optionArray = selectElement.Options;
                        int optionCount = optionArray.Count;
                        String suffix = null;
                        if (car.Make == null)
                            for (int optionIndex = optionCount - 1; optionIndex >= 0; optionIndex--)
                            {
                                var option = optionArray[optionIndex];
                                String text = option.Text.Trim();
                                if (directoryName.Contains(text))
                                {
                                    car.Make = text;
                                    suffix = SUFFIX_AUTO;
                                    break;
                                }
                            }
                        selectElement.SelectByText(car.Make);
                        Print($"\t\tMake = {car.Make}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("modelList"));
                        var selectElement = new SelectElement(el);
                        var optionArray = selectElement.Options;
                        int optionCount = optionArray.Count;
                        String suffix = null;
                        if (car.Model == null)
                            for (int optionIndex = optionCount - 1; optionIndex >= 0; optionIndex--)
                            {
                                var option = optionArray[optionIndex];
                                String text = option.Text.Trim();
                                if (directoryName.Contains(text))
                                {
                                    car.Model = text;
                                    suffix = SUFFIX_AUTO;
                                    break;
                                }
                            }
                        try
                        {
                            selectElement.SelectByText(car.Model);
                        }
                        catch
                        {
                            selectElement.SelectByText("    " + car.Model);
                        }
                        Print($"\t\tModel = {car.Model}{suffix}");
                    }
                    if (car.Type != null)
                    {
                        var el = ChromeDriver.FindElement(By.Name("offerTypeId"));
                        var selectElement = new SelectElement(el);
                        selectElement.SelectByText(car.Type);
                        Print($"\t\tType = {car.Type}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("firstRegistrationMonth"));
                        var selectElement = new SelectElement(el);
                        if (car.Month.Length < 2) car.Month = "0" + car.Month;
                        selectElement.SelectByText(car.Month);
                        Print($"\t\tMonth = {car.Month}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("firstRegistrationYear"));
                        var selectElement = new SelectElement(el);
                        var optionArray = selectElement.Options;
                        int optionCount = optionArray.Count;
                        String suffix = null;
                        if (car.Year == null)
                            for (int optionIndex = optionCount - 1; optionIndex >= 0; optionIndex--)
                            {
                                var option = optionArray[optionIndex];
                                String text = option.Text;
                                if (directoryName.Contains(text))
                                {
                                    car.Year = option.Text;
                                    suffix = SUFFIX_AUTO;
                                    break;
                                }
                            }
                        selectElement.SelectByText(car.Year);
                        //jse.ExecuteScript("arguments[0].click();", el);
                        //el.SendKeys(car.Year);
                        //jse.ExecuteScript("arguments[0].click();", el);
                        Print($"\t\tYear = {car.Year}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("powerKw"));
                        el.SendKeys(car.KW);
                        Print($"\t\tPower kW = {car.KW}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("fuelTypeId"));
                        var selectElement = new SelectElement(el);
                        selectElement.SelectByText(car.Fuel);
                        Print($"\t\tFuel = {car.Fuel}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("colorList"));
                        var selectElement = new SelectElement(el);
                        selectElement.SelectByText(car.Color);
                        Print($"\t\tBody Color = {car.Color}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("mileageInput"));
                        String suffix = null;
                        if (car.Mileage == null)
                        {
                            car.Mileage = Random.Next(45000, 99000).ToString();
                            suffix = SUFFIX_AUTO;
                        }
                        el.SendKeys(car.Mileage);
                        Print($"\t\tMileage = {car.Mileage}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("priceInput"));
                        String suffix = null;
                        if (car.PriceInEuro.Length < 4)
                        {
                            car.PriceInEuro += Random.Next(0, 999).ToString("D3");
                            suffix = SUFFIX_AUTO;
                        }
                        el.SendKeys(car.PriceInEuro);
                        Print($"\t\tPrice in Euro = {car.PriceInEuro}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Id("descriptionText"));
                        el.SendKeys(car.Description);
                        //jse.ExecuteScript($"arguments[0].value=`{car.Description}`;", el);
                        Print($"\t\tDescription = (...)");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("selectImagesInput"));
                        String[] filenames = car.Pictures.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        String fullValue = "";
                        int pictureCount = 0;
                        foreach (var filename in filenames)
                        {
                            String jpgFilename = filename.Trim() + ".jpg";
                            String fullName = Path.Combine(directoryPath, jpgFilename);
                            if (new FileInfo(fullName).Exists)
                            {
                                fullValue += fullName + " \n ";
                                pictureCount++;
                            }
                            else
                            {
                                Print($"\t\t\tPicture not found : {fullName}", ConsoleColor.Red);
                            }
                        }
                        el.SendKeys(fullValue.Trim());
                        var to = ChromeDriver.FindElement(By.CssSelector(@"div[data-ng-click='model.selectImages()"));
                        JSE.ExecuteScript("arguments[0].scrollIntoView(true);", to);
                        Print($"\t\tPictures = {car.Pictures}    ({pictureCount} pictures added.)");
                        var by = By.CssSelector(@"div[ui-sortable='model.sortableOptions']>div:first-of-type img");
                        Wait.Until(drv => drv.FindElement(by));
                        var from = ChromeDriver.FindElement(by);
                        Actions action = new Actions(ChromeDriver);
                        action.DragAndDrop(from, to).Build().Perform();
                        action.DragAndDrop(from, to).Build().Perform();
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("firstName"));
                        String suffix = null;
                        if (car.FirstName == null)
                        {
                            car.FirstName = firsNames[(days * 100 + now.Millisecond % 100) % firsNames.Length].Trim();
                            suffix = SUFFIX_AUTO;
                        }
                        el.SendKeys(car.FirstName);
                        Print($"\t\tFirstName = {car.FirstName}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("lastName"));
                        String suffix = null;
                        if (car.LastName == null)
                        {
                            car.LastName = lastNames[seconds].Trim();
                            suffix = SUFFIX_AUTO;
                        }
                        el.SendKeys(car.LastName);
                        Print($"\t\tLastName = {car.LastName}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("street"));
                        String suffix = null;
                        if (car.Street == null)
                        {
                            car.Street = Random.Next(1, 500).ToString();
                            suffix = SUFFIX_AUTO;
                        }
                        el.SendKeys(car.Street);
                        Print($"\t\tStreet = {car.Street}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("zip"));
                        String suffix = null;
                        if (car.ZIP == null)
                        {
                            car.ZIP = Random.Next(10000, 19000).ToString();
                            suffix = SUFFIX_AUTO;
                        }
                        el.SendKeys(car.ZIP);
                        Print($"\t\tZIP code = {car.ZIP}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("city"));
                        el.SendKeys(car.City);
                        Print($"\t\tCity = {car.City}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("areacode"));
                        String suffix = null;
                        if (car.Area == null)
                        {
                            car.Area = "04";
                            suffix = SUFFIX_AUTO;
                        }
                        el.SendKeys(car.Area);
                        Print($"\t\tArea code = {car.Area}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Name("phone"));
                        String suffix = null;
                        if (car.Tel == null)
                        {
                            car.Tel = Random.Next(0, 99999999).ToString("D8");
                            suffix = SUFFIX_AUTO;
                        }
                        el.SendKeys(car.Tel);
                        Print($"\t\tTelephone number = {car.Tel}{suffix}");
                    }
                    {
                        var el = ChromeDriver.FindElement(By.Id("hidephone"));
                        JSE.ExecuteScript("arguments[0].click();", el);
                        JSE.ExecuteScript("arguments[0].scrollIntoView(true);", el);
                        Print($"\t\tHide phone number checked.");
                    }
                    {
                        Print("\tPublishing...");
                        var publish = ChromeDriver.FindElement(By.CssSelector(@"div[data-ng-show='!isEdit'] button"));
                        JSE.ExecuteScript("arguments[0].click();", publish);
                    }
                    Wait.Until(UrlContains("/TierSelection"));
                    if (carIndex == 0 && new FileInfo(LOG_FILENAME).Exists) File.Move(LOG_FILENAME, LOG_FILENAME + "." + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss"));
                    using (StreamWriter file = new StreamWriter(LOG_FILENAME, true, Encoding.UTF8))
                    {
                        file.WriteLine(email + " - " + directoryName);
                    }
                    {
                        Print($"\tLogout : email = {email}");
                        carCountSuccess++;
                        //NavigatePageWithTimeout("https://www.autoscout24.de/Logout");
                        //ChromeDriver.Navigate().GoToUrl("https://www.autoscout24.de/Logout");
                        //                        String js = @"var xhr = new XMLHttpRequest();
                        //xhr.open('GET', 'https://www.autoscout24.de/Logout', true);
                        //xhr.send(null);";
                        //                        jse.ExecuteScript(js);
                        //String js = @"window.location.href='https://www.autoscout24.de/Logout';";
                        //jse.ExecuteScript(js);
                        //Thread.Sleep(3000);
                    }
                }
                catch (Exception ex)
                {
                    Print(ex.ToString(), ConsoleColor.Red);
                    carCountFailed++;
                    //ChromeDriver.Navigate().GoToUrl("https://www.autoscout24.de/Logout");
                    //try
                    //{
                    //    IAlert alert = ChromeDriver.SwitchTo().Alert();
                    //    alert.Accept();
                    //}
                    //catch { }
                }
                ClearCache();
                Print();
            }
            Print($"\r\nAll completed : {carCount} cars\r\n{carCountSuccess} succeed, {carCountFailed} failed.\r\n[{GetNowDateTimeString()}]\r\n\r\n\r\n", ConsoleColor.Green);
            ChromeDriver.Navigate().GoToUrl("https://www.autoscout24.de/account/register");
        }

        public void StartCheckingThread(String password)
        {
            Thread thread = new Thread(() => StartChecking(password));
            thread.Start();
        }

        public void StartChecking(String password)
        {
            Print($"Reading data from \"{LOG_FILENAME}\"\r\n", ConsoleColor.Green);
            String[] lines = File.ReadAllText(LOG_FILENAME, Encoding.UTF8).Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int lineCount = lines.Length;
            for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
            {
                try
                {
                    String[] words = lines[lineIndex].Split(new char[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    String email = words[0].Trim();
                    String directoryName = null;
                    if (words.Length > 1) directoryName = words[1].Trim();
                    ChromeDriver.Navigate().GoToUrl("https://angebot.autoscout24.de/ListingOverview");
                    Print($"( {lineIndex + 1} / {lineCount} )    {email}    {directoryName}    [{GetNowDateTimeString()}]");
                    if (ChromeDriver.Url.Contains("/login"))
                    {
                        var inputUsername = ChromeDriver.FindElement(By.Id("Username"));
                        Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(inputUsername)).Click();
                        inputUsername.Clear();
                        inputUsername.SendKeys(email);
                        var inputPassword = ChromeDriver.FindElement(By.Id("Password"));
                        inputPassword.Clear();
                        inputPassword.SendKeys(password);
                        var enableRecaptcha = ChromeDriver.FindElements(By.Id("EnableRecaptcha"));
                        if (enableRecaptcha.Count < 1)
                        {
                            var submit = ChromeDriver.FindElement(By.Id("Login"));
                            JSE.ExecuteScript("arguments[0].click();", submit);
                        }
                        else
                        {
                            Print("Plese solve the captcha and click login button.", ConsoleColor.Cyan);
                            Wait.Until(UrlContains("/ListingOverview"));
                        }
                    }
                    if (!ChromeDriver.Url.Contains("/ListingOverview"))
                    {
                        Print("\tFailed to login", ConsoleColor.Red);
                        continue;
                    }
                    Thread.Sleep(3000);
                }
                catch (Exception ex)
                {
                    Print(ex.ToString(), ConsoleColor.Red);
                }
                ClearCache();
            }
            Print($"\r\nAll completed : {lineCount} emails.    [{GetNowDateTimeString()}]\r\n\r\n\r\n", ConsoleColor.Green);
        }

        private void NavigatePageWithTimeout(String url, int timeoutSeconds = 5)
        {
            var timeout = ChromeDriver.Manage().Timeouts();
            timeout.PageLoad = TimeSpan.FromSeconds(timeoutSeconds);
            timeout.AsynchronousJavaScript = TimeSpan.FromSeconds(timeoutSeconds);
            try
            {
                ChromeDriver.Navigate().GoToUrl(url);
            }
            catch { }
            timeout.PageLoad = TimeSpan.FromSeconds(DEFAULT_TIMEOUT_PAGELOAD);
            timeout.AsynchronousJavaScript = TimeSpan.FromSeconds(DEFAULT_TIMEOUT_PAGELOAD);
        }

        private void ClearCache()
        {
            ChromeDriver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
            try
            {
                IAlert alert = ChromeDriver.SwitchTo().Alert();
                alert.Accept();
                ChromeDriver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
            }
            catch { }
            IWebElement root1 = ChromeDriver.FindElement(By.CssSelector("settings-ui"));
            IWebElement shadowRoot1 = expandRootElement(root1);
            IWebElement root2 = shadowRoot1.FindElement(By.CssSelector("settings-main"));
            IWebElement shadowRoot2 = expandRootElement(root2);
            IWebElement root3 = shadowRoot2.FindElement(By.CssSelector("settings-basic-page"));
            IWebElement shadowRoot3 = expandRootElement(root3);
            IWebElement root4 = shadowRoot3.FindElement(By.CssSelector("settings-section > settings-privacy-page"));
            IWebElement shadowRoot4 = expandRootElement(root4);
            IWebElement root5 = shadowRoot4.FindElement(By.CssSelector("settings-clear-browsing-data-dialog"));
            IWebElement shadowRoot5 = expandRootElement(root5);
            IWebElement root6 = shadowRoot5.FindElement(By.CssSelector("#clearBrowsingDataDialog"));
            //IWebElement root7 = root6.FindElement(By.CssSelector("cr-tabs[role='tablist']"));
            //root7.Click();
            var cacheCheckboxBasic = root6.FindElement(By.Id("cacheCheckboxBasic"));
            var shadowRoot_cacheCheckboxBasic = expandRootElement(cacheCheckboxBasic);
            var cacheCheckboxBasicCheck = shadowRoot_cacheCheckboxBasic.FindElement(By.TagName("cr-checkbox"));
            var x = cacheCheckboxBasicCheck.GetAttribute("checked");
            if (x != null)
                cacheCheckboxBasicCheck.Click();
            IWebElement clearDataButton = root6.FindElement(By.Id("clearBrowsingDataConfirm"));
            clearDataButton.Click();
        }

        private IWebElement expandRootElement(IWebElement element)
        {
            return (IWebElement)((IJavaScriptExecutor)ChromeDriver).ExecuteScript("return arguments[0].shadowRoot", element);
        }

        private String GetStringFromCssSelector(String cssSelector, String defaultValue = null)
        {
            try
            {
                IWebElement e = ChromeDriver.FindElement(By.CssSelector(cssSelector));
                return e.Text;
            }
            catch { return defaultValue; }
        }

        private Decimal? GetDecimalFromCssSelector(String cssSelector, Decimal? defaultValue = null)
        {
            try
            {
                IWebElement e = ChromeDriver.FindElement(By.CssSelector(cssSelector));
                return Convert.ToDecimal(GetNumbers(GetStringFromCssSelector(cssSelector)));
            }
            catch { return defaultValue; }
        }

        private static String JoinForCSV(IEnumerable<String> values)
        {
            List<String> list = new List<string>();
            foreach (String s in values)
            {
                list.Add("\"" + s + "\"");
            }
            return String.Join(",", list);
        }

        public void Dispose()
        {
            ChromeDriver.Close();
            ChromeDriver.Quit();
        }
    }
}
