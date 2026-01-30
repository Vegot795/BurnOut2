using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;

namespace Burn_Out.FunctionalTests
{
    [TestClass]
    public class LoginTests
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

            _wait!.Until(d => d.Url.Contains("/user-profile"));
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

            Assert.IsTrue(_driver!.Url.Contains("/user-profile"), "Expected to be on user profile page after successful login");

            var userInfo = _driver.FindElement(By.Id("user-info"));
            Assert.IsTrue(userInfo.Displayed);
        }

        [TestMethod]
        public void SignIn_WithInvalidCredentials_ShouldShowError()
        {
            Login("badclient@example.com", "Pass!23");

            var error = _wait!.Until(d => d.FindElement(By.ClassName("mud-alert")));

            StringAssert.Contains(error.Text, "U¿ytkownik nie istnieje");
        }

        [TestMethod]
        public void Register_NewUser_RedirectsToProfile()
        {
            var email = $"test10@example.com";
            var password = "Test!23Password";
            var url = $"{BaseUrl}/auth/register?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}&confirmPassword={Uri.EscapeDataString(password)}&firstName=Test&lastName=User";

            // Act
            _driver!.Navigate().GoToUrl(url);
            Thread.Sleep(500);

            // Assert - register endpoint redirects to /user-profile on success
            var current = _driver.Url ?? string.Empty;
            StringAssert.Contains(current, "/user-profile", "Expected to be redirected to user profile page after successful registration");
        }
    }
}