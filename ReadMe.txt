Instalacja projektu Burn Out

1. Sklonuj repozytorium https://github.com/Vegot795/BurnOut2.git
2. Po otworzeniu projektu w edytorze tekstowym, należy dokonać zmian w następujących plikach
	global.json :
"sdk": {
  "version": (becna wersja SDK)
}

	appsetings.json:"DefaultConnection": "Server=(link do połączenia z serwerem);Database=(nazwa bazy danych);Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"

3. Należy zainstalowań następujące NuGet'y:
	-Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
	-Aspnet.Identity.EntityFramework.Core
	-MudBlazor
	-Microsoft.EntityFrameworkCore.Tools
	-Microsoft.EntityFrameworkCore.SqlServe
	-Microsoft.Data.SqlClient
	-Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
	-Playwright
	-Microsoft.Extensions.Identity.Core
	-Microsoft.EntityFrameworkCore.InMemory
	-xunit
	-xunit.runner.visualstudio
	-Microsoft.NET.Test.Sdk
	-Selenium
	-MSTest
3. Po zainstalowaniu NuGet'ów należy skompilować projekt
4. Po zakończeniu kompilacji należy wpisać poniższe komendy w Developer PowerShell

dotnet clean
dotnet restore
dotnet build

5. Instalacja dobiegła końca

Autorzy:
Davies Ogunruku 52753
Paweł Nizio 52749



	
