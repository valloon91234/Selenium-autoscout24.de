//using DbHelper.DbBase;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAuto;
using Utils;
using OpenQA.Selenium.Support.Events;

namespace WebHelper
{
    public partial class IWebHelper
    {
        public bool m_must_terminate;
        public string m_str_proxy;

        public ChromeDriver WebDriver;
        //public EventFiringWebDriver m_firingDriver;
        public IJavaScriptExecutor m_js;
        public CookieContainer m_cookies;
        public List<string> m_lst_cookies;
        public int m_ID;
        public bool m_incognito = true;
        public bool m_dis_webrtc = false;
        public bool m_dis_cache = false;
        public bool m_dis_js = false;
        public object m_locker;
        public string m_chr_user_data_dir;

        public int m_useragent_id;
        public string m_resolution;

        public System.Drawing.Point m_location = new System.Drawing.Point(0, 0);
        public System.Drawing.Size m_size = new System.Drawing.Size(0, 0);

        public string m_chr_extension_dir = Path.Combine(MainApp.g_working_directory, "ChromeExtension");
        public string m_creat_time;
        
        public object m_chr_data_dir = new object();
        public object m_selen_locker = new object();

        public string m_err_str = "##$$##$$";

        public Guid m_guid;

        public IWebHelper()
        {
            WebDriver = null;
            m_locker = new object();
            m_chr_user_data_dir = "";
            m_resolution = "";
            m_str_proxy = string.Empty;
            m_must_terminate = false;
        }

        public async Task<bool> Start()
        {
            try
            {
                lock (m_chr_data_dir)
                {
                    m_guid = Guid.NewGuid();
                    //m_chr_user_data_dir = $"ChromeData\\selenium_{Thread.CurrentThread.ManagedThreadId}" + m_guid.ToString();
                    m_chr_user_data_dir = Path.Combine("ChromeData", $"selenium_{Thread.CurrentThread.ManagedThreadId}" + m_guid.ToString());
                    Directory.CreateDirectory(m_chr_user_data_dir);
                }

                //MainApp.log_error($"#{m_ID} - Start...");
                try
                {
                    ChromeDriverService defaultService = ChromeDriverService.CreateDefaultService();
                    defaultService.HideCommandPromptWindow = true;
                    ChromeOptions chromeOptions = new ChromeOptions();
                    if (m_incognito)
                    {
                        chromeOptions.AddArguments("--incognito");
                    }

                    chromeOptions.AddArgument("--start-maximized");
                    //chromeOptions.AddArgument("--auth-server-whitelist");
                    chromeOptions.AddArgument("--ignore-certificate-errors");
                    chromeOptions.AddArgument("--ignore-ssl-errors");
                    chromeOptions.AddArgument("--system-developer-mode");
                    chromeOptions.AddArgument("--no-first-run");
                    //chromeOptions.AddArguments("--disk-cache-size=0");
                    chromeOptions.AddArgument("--load-extension=" + Path.Combine(m_chr_extension_dir, "proxy helper"));
                    chromeOptions.AddArgument("--user-data-dir=" + m_chr_user_data_dir);

                    chromeOptions.AddExcludedArgument("enable-automation");
                    chromeOptions.AddArguments("--disable-infobars");
                    chromeOptions.AddArguments("--disable-popup-blocking");
                    chromeOptions.AddArgument("--disable-gpu");

                    chromeOptions.AddArgument("--lang=en-ca");

                    /*chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-dev-shm-usage");
                    chromeOptions.AddArgument("--headless");
                    chromeOptions.AddArgument("--disable-gpu");*/

                    if (m_dis_webrtc)
                        chromeOptions.AddExtension(Path.Combine(m_chr_extension_dir, "WebRTC Protect.crx"));
                    if (m_dis_cache)
                        chromeOptions.AddExtension(Path.Combine(m_chr_extension_dir, "CacheKiller.crx"));

                    if (m_dis_js)
                        chromeOptions.AddArgument("--load-extension=" + Path.Combine(m_chr_extension_dir, "jsoff-master"));

                    string randomUserAgent = Str_Utils.GetRandomUserAgent();
                    chromeOptions.AddArgument(string.Format("--user-agent={0}", (object)randomUserAgent));

                    chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.All);
                    chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
                    //chromeOptions.AddUserProfilePreference("profile.managed_default_content_settings.images", 2);

                    string chr_path = "";

                    string reg = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
                    RegistryKey registryKey;
                    using (registryKey = Registry.LocalMachine.OpenSubKey(reg))
                    {
                        if (registryKey != null)
                            chr_path = registryKey.GetValue("Path").ToString() + @"\chrome.exe";
                    }
                    if (chr_path == "")
                    {
                        if (Environment.Is64BitOperatingSystem)
                            chr_path = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
                        else
                            chr_path = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

                        if (!System.IO.File.Exists(chr_path))
                        {
                            chr_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\Application\chrome.exe";
                        }
                    }

                    if (!System.IO.File.Exists(chr_path))
                    {
                        MainApp.log_error($"#{m_ID} - chrome.exe Not found. Perhaps the Google Chrome browser is not installed on this computer.");
                        return false;
                    }
                    chromeOptions.BinaryLocation = chr_path;

                    try
                    {
                        WebDriver = new ChromeDriver(defaultService, chromeOptions);
                    }
                    catch (Exception ex)
                    {
                        MainApp.log_error($"#{m_ID} - Fail to start chrome.exe.{ex.Message}");
                        return false;
                    }
                    m_js = (IJavaScriptExecutor)WebDriver;

                    if (m_str_proxy != "" && !m_incognito && m_str_proxy.Split(':').Length == 4) // regular proxy setting
                    {
                        string ip = "";
                        string port = "";
                        string type = ConstEnv.PROXY_TYPE_HTTP;
                        string login = "";
                        string password = "";

                        //type = m_str_proxy.Split(':')[0];
                        ip = m_str_proxy.Split(':')[0];
                        port = m_str_proxy.Split(':')[1];
                        login = m_str_proxy.Split(':')[2];
                        password = m_str_proxy.Split(':')[3];

                        await Navigate("chrome-extension://mnloefcpaepkpmhaoipjkpikbnkmbnic/options.html");
                        m_js.ExecuteScript("$('#http-host').val(\"" + ip + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#http-port').val(\"" + port + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#https-host').val(\"" + ip + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#https-port').val(\"" + port + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#socks-host').val(\"" + ip + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#socks-port').val(\"" + port + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#username').val(\"" + login + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#password').val(\"" + password + "\")", Array.Empty<object>());
                        
                        if (type == ConstEnv.PROXY_TYPE_SOCKS5)
                        {
                            m_js.ExecuteScript("var a = document.getElementById(\"socks5\"); a.click();", Array.Empty<object>());
                            Console.WriteLine("Socks5 proxy is set.");
                        }
                        else
                        {
                            m_js.ExecuteScript("var a = document.getElementById(\"socks4\"); a.click();", Array.Empty<object>());
                            Console.WriteLine("Socks4 proxy is set.");
                        }
                        m_js.ExecuteScript("$('#proxy-rule').val(\"singleProxy\");", Array.Empty<object>());
                        m_js.ExecuteScript("save();", Array.Empty<object>());

                        bool is_success = false;
                        while (!is_success)
                        {
                            try
                            {
                                WebDriver.Navigate().GoToUrl("chrome-extension://mnloefcpaepkpmhaoipjkpikbnkmbnic/popup.html");

                                if (type == ConstEnv.PROXY_TYPE_SOCKS4 || type == ConstEnv.PROXY_TYPE_SOCKS5)
                                    m_js.ExecuteScript("socks5Proxy();", Array.Empty<object>());
                                else if (type == ConstEnv.PROXY_TYPE_HTTP)
                                    m_js.ExecuteScript("httpProxy();", Array.Empty<object>());

                                is_success = true;
                            }
                            catch (Exception ex)
                            {
                                is_success = false;
                                await TaskDelay(100);
                            }
                        }
                    }

                    MainApp.log_info($"Proxy set success - {m_str_proxy}");

                    //Driver.Navigate().Refresh();
                    //if(m_dis_cache)
                    //{
                    //    await Navigate("chrome-extension://kkmknnnjliniefekpicbaaobdnjjikfp/options.html");
                    //}

                    //if (!m_incognito && m_dis_js) // regular proxy setting
                    //{
                    //    await Navigate("chrome-extension://jfpdlihdedhlmhlbgooailmfhahieoem/options.html");
                    //}

                    //if (m_incognito == false)
                    //    await remove_all_cookies(); //<- not necessary in incogneto mode

                    MainApp.log_info($"#{m_ID} - Browser successfully started.");
                    return true;
                }
                catch (Exception exception)
                {
                    MainApp.log_error($"#{m_ID} - Failed to start. Exception:{exception.Message}\n{exception.StackTrace}");
                    try
                    {
                        WebDriver.Quit();
                    }
                    catch(Exception ex)
                    {
                        MainApp.log_error($"#{m_ID} - Failed to quit driver. Exception:{ex.Message}");
                    }
                    return false;
                }
            }
            catch (Exception exception)
            {
                MainApp.log_error($"#{m_ID} - Exception occured while trying to start chrome driver. Exception:{exception.Message}");
            }
            return false;
        }

        public async Task<bool> StartProxyNoExt(string proxytxt)
        {
            try
            {
                lock (m_chr_data_dir)
                {
                    m_guid = Guid.NewGuid();
                    //m_chr_user_data_dir = $"ChromeData\\selenium_{Thread.CurrentThread.ManagedThreadId}" + m_guid.ToString();
                    m_chr_user_data_dir = Path.Combine("ChromeData", $"selenium_{Thread.CurrentThread.ManagedThreadId}" + m_guid.ToString());
                    Directory.CreateDirectory(m_chr_user_data_dir);
                }

                //MainApp.log_error($"#{m_ID} - Start...");
                try
                {
                    ChromeDriverService defaultService = ChromeDriverService.CreateDefaultService();
                    defaultService.HideCommandPromptWindow = true;
                    ChromeOptions chromeOptions = new ChromeOptions();
                    if (m_incognito)
                    {
                        chromeOptions.AddArguments("--incognito");
                    }

                    chromeOptions.AddArgument("--start-maximized");
                    //chromeOptions.AddArgument("--auth-server-whitelist");
                    chromeOptions.AddArgument("--ignore-certificate-errors");
                    chromeOptions.AddArgument("--ignore-ssl-errors");
                    chromeOptions.AddArgument("--system-developer-mode");
                    chromeOptions.AddArgument("--no-first-run");
                    //chromeOptions.AddArguments("--disk-cache-size=0");
                    chromeOptions.AddArgument("--user-data-dir=" + m_chr_user_data_dir);

                    chromeOptions.AddExcludedArgument("enable-automation");
                    chromeOptions.AddArguments("--disable-infobars");
                    chromeOptions.AddArguments("--disable-popup-blocking");
                    chromeOptions.AddArgument("--disable-gpu");

                    chromeOptions.AddArgument("--lang=en-ca");

                    /*chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-dev-shm-usage");
                    chromeOptions.AddArgument("--headless");
                    chromeOptions.AddArgument("--disable-gpu");*/

                    if (m_dis_webrtc)
                        chromeOptions.AddExtension(Path.Combine(m_chr_extension_dir, "WebRTC Protect.crx"));
                    if (m_dis_cache)
                        chromeOptions.AddExtension(Path.Combine(m_chr_extension_dir, "CacheKiller.crx"));

                    if (m_dis_js)
                        chromeOptions.AddArgument("--load-extension=" + Path.Combine(m_chr_extension_dir, "jsoff-master"));

                    string randomUserAgent = Str_Utils.GetRandomUserAgent();
                    chromeOptions.AddArgument(string.Format("--user-agent={0}", (object)randomUserAgent));

                    chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.All);
                    chromeOptions.AddAdditionalCapability("useAutomationExtension", false);

                    //chromeOptions.AddUserProfilePreference("profile.managed_default_content_settings.images", 2);

                    string chr_path = "";

                    string reg = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
                    RegistryKey registryKey;
                    using (registryKey = Registry.LocalMachine.OpenSubKey(reg))
                    {
                        if (registryKey != null)
                            chr_path = registryKey.GetValue("Path").ToString() + @"\chrome.exe";
                    }
                    if (chr_path == "")
                    {
                        if (Environment.Is64BitOperatingSystem)
                            chr_path = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
                        else
                            chr_path = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

                        if (!System.IO.File.Exists(chr_path))
                        {
                            chr_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\Application\chrome.exe";
                        }
                    }

                    if (!System.IO.File.Exists(chr_path))
                    {
                        MainApp.log_error($"#{m_ID} - chrome.exe Not found. Perhaps the Google Chrome browser is not installed on this computer.");
                        return false;
                    }
                    chromeOptions.BinaryLocation = chr_path;
                    
                    try
                    {
                        WebDriver = new ChromeDriver(defaultService, chromeOptions);
                        //m_firingDriver = new EventFiringWebDriver(WebDriver);
                        //m_firingDriver.Navigated += M_firingDriver_Navigated;
                        
                    }
                    catch (Exception ex)
                    {
                        MainApp.log_error($"#{m_ID} - Fail to start chrome.exe.{ex.Message}");
                        return false;
                    }
                    m_js = (IJavaScriptExecutor)WebDriver;

                    //if (m_incognito == false)
                    //    await remove_all_cookies(); //<- not necessary in incogneto mode

                    MainApp.log_info($"#{m_ID} - Browser successfully started.");
                    return true;
                }
                catch (Exception exception)
                {
                    MainApp.log_error($"#{m_ID} - Failed to start. Exception:{exception.Message}\n{exception.StackTrace}");
                    try
                    {
                        WebDriver.Quit();
                    }
                    catch (Exception ex)
                    {
                        MainApp.log_error($"#{m_ID} - Failed to quit driver. Exception:{ex.Message}");
                    }
                    return false;
                }
            }
            catch (Exception exception)
            {
                MainApp.log_error($"#{m_ID} - Exception occured while trying to start chrome driver. Exception:{exception.Message}");
            }
            return false;
        }


        public async Task<bool> start_headless()
        {
            try
            {
                lock (m_chr_data_dir)
                {
                    m_guid = Guid.NewGuid();
                    //m_chr_user_data_dir = $"ChromeData\\selenium_{Thread.CurrentThread.ManagedThreadId}" + m_guid.ToString();
                    m_chr_user_data_dir = Path.Combine("ChromeData", $"selenium_{Thread.CurrentThread.ManagedThreadId}" + m_guid.ToString());
                    Directory.CreateDirectory(m_chr_user_data_dir);
                }

                try
                {
                    ChromeDriverService defaultService = ChromeDriverService.CreateDefaultService();
                    defaultService.HideCommandPromptWindow = true;
                    ChromeOptions chromeOptions = new ChromeOptions();

                    chromeOptions.AddArgument("--start-maximized");
                    chromeOptions.AddArgument("--ignore-certificate-errors");
                    chromeOptions.AddArgument("--ignore-ssl-errors");
                    chromeOptions.AddArgument("--system-developer-mode");
                    chromeOptions.AddArgument("--no-first-run");

                    chromeOptions.AddArgument("--load-extension=" + Path.Combine(m_chr_extension_dir, "proxy helper"));
                    chromeOptions.AddArgument("--user-data-dir=" + m_chr_user_data_dir);

                    chromeOptions.AddExcludedArgument("enable-automation");
                    chromeOptions.AddArguments("--disable-infobars");
                    chromeOptions.AddArguments("--disable-popup-blocking");

                    string randomUserAgent = Str_Utils.GetRandomUserAgent();
                    chromeOptions.AddArgument(string.Format("--user-agent={0}", (object)randomUserAgent));

                    //chromeOptions.AddArgument("--lang=en-ca");

                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-dev-shm-usage");
                    chromeOptions.AddArgument("--headless");
                    chromeOptions.AddArgument("--disable-gpu");

                    chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.All);
                    chromeOptions.AddAdditionalCapability("useAutomationExtension", false);

                    /*string chr_path = Path.Combine(Path.Combine("usr", "bin"), )
                    if (!System.IO.File.Exists(chr_path))
                    {
                        Console.WriteLine($"#{m_ID} - chrome.exe Not found. Perhaps the Google Chrome browser is not installed on this computer.");
                        return false;
                    }
                    chromeOptions.BinaryLocation = chr_path;*/
                    //chromeOptions.BinaryLocation = Path.Combine("usr", Path.Combine("bin", "chromedriver"));

                    try
                    {
                        WebDriver = new ChromeDriver(defaultService, chromeOptions);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"#{m_ID} - Fail to start chrome.exe. Please make sure any other chrome windows are not opened.\n{ex.Message}");
                        return false;
                    }

                    m_js = (IJavaScriptExecutor)WebDriver;

                    /*if (m_proxy != "" && !m_incognito && m_proxy.Split(':').Length == 3) // regular proxy setting
                    {
                        string ip = "";
                        string port = "";
                        string type = "";
                        //string login = "";
                        //string password = "";

                        type = m_proxy.Split(':')[0];
                        ip = m_proxy.Split(':')[1];
                        port = m_proxy.Split(':')[2];
                        //login = m_proxy.Split(':')[2];
                        //password = m_proxy.Split(':')[3];


                        await Navigate("chrome-extension://mnloefcpaepkpmhaoipjkpikbnkmbnic/options.html");
                        m_js.ExecuteScript("$('#http-host').val(\"" + ip + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#http-port').val(\"" + port + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#https-host').val(\"" + ip + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#https-port').val(\"" + port + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#socks-host').val(\"" + ip + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#socks-port').val(\"" + port + "\")", Array.Empty<object>());
                        //m_js.ExecuteScript("$('#username').val(\"" + login + "\")", Array.Empty<object>());
                        //m_js.ExecuteScript("$('#password').val(\"" + password + "\")", Array.Empty<object>());
                        if (type == ConstEnv.PROXY_TYPE_SOCKS5)
                        {
                            m_js.ExecuteScript("var a = document.getElementById(\"socks5\"); a.click();", Array.Empty<object>());
                            Console.WriteLine("Socks5 proxy is set.");
                        }
                        else
                        {
                            m_js.ExecuteScript("var a = document.getElementById(\"socks4\"); a.click();", Array.Empty<object>());
                            Console.WriteLine("Socks4 proxy is set.");
                        }
                        m_js.ExecuteScript("$('#proxy-rule').val(\"singleProxy\");", Array.Empty<object>());
                        m_js.ExecuteScript("save();", Array.Empty<object>());

                        bool is_success = false;
                        while (!is_success)
                        {
                            try
                            {
                                WebDriver.Navigate().GoToUrl("chrome-extension://mnloefcpaepkpmhaoipjkpikbnkmbnic/popup.html");

                                if (type == ConstEnv.PROXY_TYPE_SOCKS4 || type == ConstEnv.PROXY_TYPE_SOCKS5)
                                    m_js.ExecuteScript("socks5Proxy();", Array.Empty<object>());
                                else if (type == ConstEnv.PROXY_TYPE_HTTP)
                                    m_js.ExecuteScript("httpProxy();", Array.Empty<object>());

                                is_success = true;
                            }
                            catch (Exception ex)
                            {
                                is_success = false;
                                await TaskDelay(100);
                            }
                        }
                    }*/

                    return true;
                }
                catch (Exception exception)
                {
                    MainApp.log_error($"#{m_ID} - Failed to start. Exception:{exception.Message}\n{exception.StackTrace}");
                    try
                    {
                        WebDriver.Quit();
                    }
                    catch (Exception ex)
                    {
                        MainApp.log_error($"#{m_ID} - Failed to quit driver. Exception:{ex.Message}");
                    }
                    return false;
                }                
            }
            catch(Exception exception)
            {
                MainApp.log_error($"#{m_ID} - Exception occured while trying to start chrome driver. Exception:{exception.Message}");
            }
            return false;

            
        }
    }
}
