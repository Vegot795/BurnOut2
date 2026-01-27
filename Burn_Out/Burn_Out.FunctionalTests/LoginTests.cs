using System;
using System.Threading;
using System.IO;
using System.Net.Http;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Burn_Out.FunctionalTests
{
    [TestClass]
    public class LoginTests
    {
        private ChromeDriver? _driver;
        private Process? _appProcess;
        private const string BaseUrl = "https://localhost:5230"; // keep aligned with app config

        [TestInitialize]
        public void Setup()
        {
            // Ensure the app is running for E2E navigation
            StartApplication(BaseUrl);

            var options = new ChromeOptions();
            // Headless by default for CI; remove to see browser during development
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--ignore-certificate-errors"); // tolerate local dev cert

            // Remove any stale local ChromeDriver so Selenium Manager can fetch a matching one
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var path in Directory.GetFiles(baseDir, "chromedriver*.exe", SearchOption.TopDirectoryOnly))
            {
                try { File.Delete(path); } catch { /* ignore */ }
            }

            // Create driver (let Selenium Manager resolve a matching ChromeDriver)
            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        private void StartApplication(string baseUrl)
        {
            // Only start if not already listening
            if (IsResponding(baseUrl, out _)) return;

            // Resolve server project path from test bin directory
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

            // Environment for test-friendly hosting and DB
            psi.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
            psi.Environment["ASPNETCORE_URLS"] = baseUrl;
            psi.Environment["E2E_TEST_USE_INMEMORY"] = "1";

            _appProcess = Process.Start(psi);
            if (_appProcess == null || _appProcess.HasExited)
                Assert.Inconclusive("Failed to start application for functional tests.");

            // Wait until the app responds
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
        public void SignIn_WithInvalidCredentials_RedirectsToLoginError()
        {
            var email = "nonexistent@example.com";
            var password = "BadPassword";
            var url = $"{BaseUrl}/auth/signin?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";

            // Act
            _driver!.Navigate().GoToUrl(url);

            // Small wait to allow redirect to complete
            Thread.Sleep(500);

            // Assert - signin endpoint redirects to /login?error=1 on failure
            var current = _driver.Url ?? string.Empty;
            StringAssert.Contains(current, "/login?error=1", "Expected to be redirected to login error page for invalid credentials");
        }
    }
}
