using Bunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Diagnostics;

namespace Burn_Out.FunctionalTests
{
    [TestClass]
    public class SeleniumTests
    {
        private ChromeDriver? _driver;
        private Process? _appProcess;
        private const string BaseUrl = "http://localhost:5230";
        private WebDriverWait? _wait;

        [TestInitialize]
        public void Setup()
        {
            StartApplication(BaseUrl);

            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--ignore-certificate-errors");

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var path in Directory.GetFiles(baseDir, "chromedriver*.exe", SearchOption.TopDirectoryOnly))
            {
                try { File.Delete(path); } catch { }
            }

            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        private void StartApplication(string baseUrl)
        {
            if (IsResponding(baseUrl, out _)) return;

            var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            var projectPath = Path.Combine(root, "Burn_Out", "Burn_Out.csproj");

            if (!File.Exists(projectPath))
                Assert.Inconclusive($"Server project not found at {projectPath}");

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" --urls {baseUrl}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            psi.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
            psi.Environment["ASPNETCORE_URLS"] = baseUrl;
            psi.Environment["E2E_TEST_USE_INMEMORY"] = "1";

            _appProcess = Process.Start(psi);
            if (_appProcess == null || _appProcess.HasExited)
                Assert.Inconclusive("Failed to start application for functional tests.");

            var started = SpinWait.SpinUntil(() => IsResponding(baseUrl, out _), TimeSpan.FromSeconds(45));
            if (!started)
            {
                try { _appProcess.Kill(entireProcessTree: true); } catch { }
                Assert.Inconclusive($"Application did not start listening at {baseUrl} within timeout.");
            }

            AppDomain.CurrentDomain.ProcessExit += (_, __) => { try { if (!_appProcess.HasExited) _appProcess.Kill(entireProcessTree: true); } catch { } };
        }

        private static bool IsResponding(string baseUrl, out string? body)
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
                var probe = new Uri(new Uri(baseUrl), "/api/auth/check");
                var resp = client.GetAsync(probe).GetAwaiter().GetResult();
                body = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return resp.IsSuccessStatusCode || (int)resp.StatusCode < 500;
            }
            catch
            {
                body = null;
                return false;
            }
        }

        private void Login(string email, string password)
        {
            _driver!.Navigate().GoToUrl($"{BaseUrl}/login");

            _driver.FindElement(By.Id("email")).SendKeys(email);
            _driver.FindElement(By.Id("password")).SendKeys(password);

            _driver.FindElement(By.Id("loginButton")).Click();


        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                _driver?.Quit();
                _driver?.Dispose();
            }
            catch { }

            try
            {
                if (_appProcess != null && !_appProcess.HasExited)
                    _appProcess.Kill(entireProcessTree: true);
            }
            catch { }
        }

        [TestMethod]
        public void SignIn_WithValidCredentials_RedirectsToLoginError()
        {
            Login("client@example.com", "Pass!23");

            _wait!.Until(d => d.Url.Contains("/user-profile"));

            Assert.IsTrue(_driver!.Url.Contains("/user-profile"), "Expected to be on user profile page after successful login");

            var userInfo = _driver.FindElement(By.Id("userInfo"));
            Assert.IsTrue(userInfo.Displayed);
        }

        [TestMethod]
        public void SignIn_WithInvalidCredentials_ShouldShowError()
        {
            Login("badclient@example.com", "Pass!23");

            var error = _wait!.Until(d => d.FindElement(By.ClassName("mud-alert")));

            StringAssert.Contains(error.Text, "Nieprawid³owe has³o");
        }

        [TestMethod]
        public void Reservation_AfterLogin_ShouldSucceed()
        {
            Login("admin@example.com", "Pass!23");

            var reservationUrl = $"{BaseUrl}/hall-list";
            var hallEditUrl = $"{BaseUrl}/hall-editt";

            _driver!.Navigate().GoToUrl(reservationUrl);
            _wait!.Until(b => b.Url.Contains(reservationUrl));

            _driver!.Navigate().GoToUrl(hallEditUrl);

            _wait!.Until(d => d.Url.Contains("/hall-edit"));
            _driver.FindElement(By.Id("HallName")).SendKeys("Test-Hall");
            _driver.FindElement(By.Id("capacity")).SendKeys("100");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            _wait!.Until(d => d.Url.Contains("/hall-list"));

            var subButton = _wait!.Until(ExpectedConditions.ElementToBeClickable(By.Id("res-Test-Hall")));
            subButton.Click();
            _wait!.Until(d => d.Url.Contains("/hall-reservation/"));



            // Fill in reservation details
            _driver.FindElement(By.Id("startDate")).SendKeys("31.01.2026 15:00:00");
            _driver.FindElement(By.Id("endDate")).SendKeys("31.01.2026 17:00:00");
            var subResButton = _wait!.Until(ExpectedConditions.ElementToBeClickable(By.Id("res-Test-Hall")));
            subResButton.Click();

            _wait!.Until(d => d.Url.Contains("/hall-list"));

            // Assert reservation success

            
            var successMessage = _wait!.Until(d => d.FindElement(By.ClassName("mud-alert")));
            StringAssert.Contains(successMessage.Text, "Rezerwacja zosta³a pomyœlnie utworzona");
        }
    }
}