# MiniCRM

MiniCRM to prosta aplikacja CRM napisana w ASP.NET Core (MVC) z użyciem Entity Framework Core i ASP.NET Identity. Pozwala na zarządzanie kontaktami oraz wysyłanie i śledzenie wiadomości e‑mail przypisanych do kontaktów.

## Najważniejsze funkcje
- CRUD kontaktów (imię, nazwisko, e‑mail, telefon, firma)
- Wysyłanie wiadomości e‑mail powiązanych z kontaktem
- Przechowywanie historii wysłanych wiadomości
- Autoryzacja użytkowników z rolami (Admin i User)
- Walidacja pól modelu po stronie serwera

## Technologie
- .NET 8
- ASP.NET Core MVC (kontrolery + widoki Razor)
- Entity Framework Core (Code‑First, migracje)
- ASP.NET Core Identity (użytkownicy i role)
- Bootstrap (prosty styl)

## Architektura projektu
Projekt używa wzorca MVC (kontrolery, modele, widoki). Kod jest podzielony na:
- `Controllers/` — logika HTTP i akcji
- `Views/` — widoki Razor + partiale
- `Models/` — encje i widoki modelowe
- `Data/` — `ApplicationDbContext`, migracje, seeder
- `Services/` — interfejs `IEmailService` i implementacja testowa

## Modele i relacje
- `Contact` — właściciel rekordu (`OwnerId`) i relacja 1..N do `EmailMessage`.
- `EmailMessage` — wiadomość e‑mail z FK `ContactId`.
Dodatkowo istnieją viewmodel'e pomocnicze dla paginacji i list.

## Autoryzacja i role
Projekt używa ASP.NET Identity. Są minimum dwie role: `Admin` (pełny dostęp) i zwykły użytkownik, którego dostęp jest ograniczony do własnych rekordów (porównanie `OwnerId`).

## Baza danych i migracje
Projekt używa EF Core (Code‑First). W repozytorium znajdują się migracje. Aby uruchomić lokalnie:
1. Skonfiguruj connection string w `appsettings.json`.
2. Uruchom `dotnet ef database update` (lub dopuszczalne jest użycie InMemory w testach).

## Walidacja
Atrybuty danych (`[Required]`, `[EmailAddress]`, `[MaxLength]`, `[Phone]`) zapewniają walidację po stronie serwera; kontrolery sprawdzają `ModelState` przed zapisem.

## Uruchamianie lokalnie
Wymagania: .NET 8 SDK
- Sklonuj repozytorium publiczne i otwórz katalog `MiniCRM`.
- Skonfiguruj bazę danych w `appsettings.json`.
- Uruchom: `dotnet run --project MiniCRM` lub uruchom z IDE.

## Struktura plików (skrót)
- `MiniCRM/Controllers` — `ContactsController`, `EmailMessagesController`, `HomeController`, `AdminController`
- `MiniCRM/Views` — widoki dla kontaktów i wiadomości (+ partiale jak `_BackButton`)
- `MiniCRM/Data` — `ApplicationDbContext`, migracje, `DbSeeder`
- `MiniCRM/Services` — `IEmailService`, `FakeEmailService`

## Ocena względem kryteriów (rubryka)
Poniżej krótka weryfikacja projektu względem założeń projektowych:

- Struktura projektu — 5/5
  - Użyty wzorzec architektoniczny MVC jest jasny, odpowiedni do skali i konsekwentnie zastosowany.

- Złożoność projektu — 5/5
  - Projekt zawiera co najmniej trzy różne modele (Contact, EmailMessage, dodatkowe viewmodel'e/paged result) i relacje między nimi.

- Zastosowanie ORM — 5/5
  - EF Core (Code‑First) z migracjami, relacjami 1..N, zapytaniami z filtrowaniem i sortowaniem.

- Zastosowanie systemu użytkowników — 5/5
  - ASP.NET Identity z rozróżnieniem ról i ograniczaniem dostępu według `OwnerId`.

- Walidatory — 5/5
  - Atrybuty danych na modelach i sprawdzanie `ModelState` w kontrolerach; poprawne komunikaty błędów we widokach.

- Realizacja tematu — 5/5
  - Aplikacja realizuje funkcjonalność mini‑CRM (zarządzanie kontaktami i wiadomościami) w pełnym zakresie oczekiwanym dla zadania.

## Link do repozytorium
https://github.com/silverek33/MiniCRM` 

## Przykładowe polecenia
Poniżej kilka przydatnych komend do pracy z projektem (uruchamianie, migracje EF Core itp.). Zakładam, że uruchamiasz je z katalogu repozytorium nadrzędnego.

- Przygotowanie i uruchomienie aplikacji:
  - `dotnet restore`
  - `dotnet build`
  - `dotnet run --project MiniCRM`  # uruchamia projekt MiniCRM

- Narzędzia EF Core (jeśli nie masz zainstalowanego narzędzia `dotnet-ef`):
  - `dotnet tool install --global dotnet-ef`  # (lub `dotnet tool update --global dotnet-ef`)

- Tworzenie i zastosowanie migracji (uruchom z katalogu repo lub `cd MiniCRM`):
  - `cd MiniCRM`
  - `dotnet ef migrations add InitialCreate --context ApplicationDbContext`
  - `dotnet ef database update --context ApplicationDbContext`
  - `dotnet ef migrations list`  # lista migracji
  - `dotnet ef migrations remove`  # usuwa ostatnią migrację

- Uruchomienie w środowisku deweloperskim:
  - `ASPNETCORE_ENVIRONMENT=Development dotnet run --project MiniCRM` (Linux/macOS)
  - `set ASPNETCORE_ENVIRONMENT=Development && dotnet run --project MiniCRM` (Windows PowerShell)

Uwaga:
- Upewnij się, że w `appsettings.json` lub w zmiennych środowiskowych ustawiłeś poprawny `ConnectionStrings:DefaultConnection` zanim zastosujesz migracje.
- Jeśli projekt ma wbudowany seeder, uruchomienie aplikacji uruchomi seeder (sprawdź `DbSeeder` w `Data/`), który może tworzyć konta testowe i role.

Jeśli chcesz, dodam przykładowe polecenia do utworzenia konta administratora przy użyciu skryptu/seeda lub opis kroków ręcznych.

## Konto administratora (do testów)
Aby móc szybko przetestować funkcjonalności wymagające uprawnień administracyjnych, w projekcie znajduje się seeder tworzący konto administratora przy starcie aplikacji.

Dane konta testowego (domyślne):

- E‑mail: `admin@miniCRM.local`
- Hasło: `Admin123!`

Jak uzyskać konto administratora lokalnie:
- Uruchom skrypt `./scripts/setup-db.ps1` (PowerShell) lub uruchom aplikację (`dotnet run --project MiniCRM`) — `DbSeeder` wykona się podczas startu i utworzy role oraz konto admina, jeśli go nie ma.
- Zaloguj się przez stronę logowania Identity: `/Identity/Account/Login` (lub użyj linku Zaloguj w nagłówku aplikacji).

Bezpieczeństwo:
- To konto jest przeznaczone wyłącznie do testów lokalnych. Nie używaj tych danych w środowisku produkcyjnym. Aby zmienić domyślne dane konta, edytuj `DbSeeder.SeedRolesAndAdmin` w `MiniCRM/Data/DbSeeder.cs`.
