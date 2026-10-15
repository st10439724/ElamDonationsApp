# Elam Igugu Leli web application

A small ASP.NET Core (C#, .NET 8) web app for the Elam Igugu Leli NPO.

- **Public:** landing page, sign up, sign in, career day webinars with a no-login RSVP.
- **Donor:** "Donate / Drop Off" pop-up with the NPO's Instagram and phone details, and a "My donations" table showing each donation's status.
- **Admin:** donors list, donations list with a status filter, and a **Verify** button that moves a donation from **Submitted** to **Received**.
- **Database:** SQL Server. Azure SQL in production, LocalDB on your own computer while developing.

> **Status: untested.** This code was written and reviewed by reading it, but it has not been compiled or run. Expect to fix a few small compile errors on first build. Send the exact error text back and they are usually one-line fixes.

---

## 1. What you need installed

| Tool | Why |
|---|---|
| .NET 8 SDK | builds and runs the app (`dotnet --version` should say 8.x) |
| Visual Studio 2022 (with the "ASP.NET and web development" workload) **or** VS Code with the C# Dev Kit | editing |
| SQL Server LocalDB (comes with Visual Studio on Windows) **or** an Azure SQL database | the database |
| `dotnet-ef` tool | creates the database tables: `dotnet tool install --global dotnet-ef` |

## 2. First run (local, using LocalDB)

Open a terminal in the `ElamIgugu.Web` folder.

```
dotnet restore

# 1. Choose the first admin account (kept out of the code, in "user secrets")
dotnet user-secrets set "Seed:AdminEmail" "admin@yourdomain.com"
dotnet user-secrets set "Seed:AdminPassword" "ChangeMe123"

# 2. Create the database tables (a "migration"). Do this ONCE, and again whenever a model class changes.
dotnet ef migrations add InitialCreate

# 3. Run. On start-up the app applies the migration, creates the roles and the admin, and adds 3 sample webinars.
dotnet run
```

**Already ran an earlier version?** Add one new migration for the new Description and Amount columns, then run:

```
dotnet ef migrations add AddDescriptionAndMoney
dotnet run
```

Open https://localhost:7080. If the browser warns about the certificate, run `dotnet dev-certs https --trust` once.

Admin password rules: at least 8 characters, with an uppercase letter, a lowercase letter and a number.

## 3. Moving the database to Azure SQL

1. In the Azure portal create an **SQL database** (it creates an **SQL server** with it). Note the server name, admin login and password.
2. On the SQL **server**, open *Networking* and: add your own IP address to the firewall, and tick *Allow Azure services and resources to access this server* (needed once the app is hosted in Azure).
3. Copy the connection string from the database page (*Connection strings > ADO.NET*) and put your password in it. Keep it out of the code:
   ```
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:YOURSERVER.database.windows.net,1433;Initial Catalog=YOURDB;User ID=YOURLOGIN;Password=YOURPASSWORD;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
   ```
4. `dotnet run` again. The app creates all the tables in Azure on start-up.
5. When you host the app on **Azure App Service**, add the same values under *Settings > Environment variables*: `ConnectionStrings__DefaultConnection`, `Seed__AdminEmail`, `Seed__AdminPassword` (double underscore replaces the colon).

Do not commit passwords. `.gitignore` already excludes `appsettings.Development.json` and secrets.

## 4. Test checklist (do these in order)

1. Landing page loads, with **Sign in** and **Sign up** side by side.
2. **Sign up** a donor. Try a weak password and a mismatched confirm password: you should see errors. A good one sends you to Sign in.
3. **Sign in** as that donor. You land on the Donor page.
4. Click **Donate / Drop Off**. The pop-up shows the Instagram handle and both phone numbers. Choose an item and a city, save. A row appears in My donations with status **Submitted**.
5. Try to open `/Admin`. You should get *Access denied*.
6. Sign out. Sign in as the **admin**. You land on the Admin dashboard.
7. **Donors list** shows your donor with 1 donation. **Donations** shows the row as Submitted; click **Verify**. It changes to **Received** and the counts update.
8. Sign back in as the donor. The same row now says **Received**.
9. Without signing in, open **Career day webinars**, click Register on one, enter a name and number, confirm. You see the success message. Register the same number again: you are told you are already registered.
10. Enter 5 wrong passwords for one account: the account is locked for 5 minutes.
11. **Money:** as the donor, click Donate / Drop Off and choose **Money (EFT)**. The pop-up swaps the contact details for the FNB bank details and shows an Amount box. Save: the page shows a reference like `EIL-0001` and the row shows the amount and reference with status Submitted. As admin, the same row shows the amount and reference; check them against the bank statement, click Verify, and **Money received (verified)** goes up.
12. The landing page and footer show the slogan and **REF: 260-053 NPO**.
13. Check the data in the database (Azure portal *Query editor*, or Visual Studio *SQL Server Object Explorer*): tables `AspNetUsers`, `AspNetRoles`, `Donations`, `Webinars`, `WebinarRsvps`. `Donations.Status` holds the words Submitted / Received.

## 5. What each file does

### Project root

| File | What it does |
|---|---|
| `ElamIgugu.Web.csproj` | The project definition: .NET 8, and the three packages (Identity, SQL Server, EF tools). Also the `UserSecretsId` that lets you keep passwords out of the code. |
| `Program.cs` | The start-up file. Connects to the database, turns on Identity (accounts, password rules, lockout), sets cookie security, loads the NPO contact details, turns on anti-forgery protection, runs the seeder, and sets up the request pipeline (HTTPS, static files, sign-in check, role check, routes). |
| `appsettings.json` | Settings: the database connection string, the NPO's Instagram handle and phone numbers (edit them here), and empty admin seed values. |
| `.gitignore` | Keeps build output, secrets and editor files out of GitHub. |
| `Properties/launchSettings.json` | Which web addresses and environment `dotnet run` uses (https://localhost:7080). |

### Models (the shape of the data)

| File | What it does |
|---|---|
| `Models/ApplicationUser.cs` | A person who can sign in (donor or admin). Adds `FullName` and `CreatedDate` to Identity's built-in user, which already stores email, phone number, password hash, lockout and two-factor fields. |
| `Models/Donation.cs` | One donation: donor, item type, city, date, **Status** (Submitted or Received), and who verified it and when. Also holds the two-value `DonationStatus` enum. |
| `Models/Webinar.cs` | A career day webinar: topic, start time, speaker. |
| `Models/WebinarRsvp.cs` | One webinar sign-up: attendee name, contact number, which webinar. No account needed. |
| `Models/AppRoles.cs` | The two role names ("Admin", "Donor") in one place. |
| `Models/DonationOptions.cs` | The fixed lists of item types and cities used by the pop-up and checked again on the server. |
| `Models/NpoOptions.cs` | Holds the Instagram handle, phone numbers, email, slogan, registration number and FNB bank details read from `appsettings.json` (section `Npo`). Change them there, not in the views. |
| `Models/ErrorInfo.cs` | Small helper for the error page. |
| `Models/ViewModels/AccountViewModels.cs` | The sign-up and sign-in form fields, with their validation messages and the POPIA consent rule. |
| `Models/ViewModels/DonorViewModels.cs` | The donor page's data, and the two values chosen in the Donate pop-up. |
| `Models/ViewModels/AdminViewModels.cs` | The admin page's data: counts, donor rows, donation rows. |
| `Models/ViewModels/WebinarViewModels.cs` | The fields posted when someone registers for a webinar. |

### Data (the database)

| File | What it does |
|---|---|
| `Data/ApplicationDbContext.cs` | The link between the C# classes and the SQL tables. Defines the relationships (one donor has many donations, one webinar has many RSVPs) and stores Status as readable text. |
| `Data/DbSeeder.cs` | Runs on every start-up and only adds what is missing: applies the migration, creates the Admin and Donor roles, creates the first admin from your secrets, and adds sample webinars. |
| `Migrations/` | **You create this** with `dotnet ef migrations add InitialCreate`. It is the recipe for the tables. |

### Controllers (the logic: one per area)

| File | What it does |
|---|---|
| `Controllers/HomeController.cs` | Landing page; `Dashboard` sends a signed-in user to the Admin or Donor page by role; error page. |
| `Controllers/AccountController.cs` | Register (always creates a **Donor**), Login (with lockout after failed attempts), Logout, Access denied. This is where the two-factor step will be added. |
| `Controllers/DonorController.cs` | Donors only. Shows their own donations, and saves a new donation as **Submitted** from the pop-up. The donor's identity comes from the sign-in, never from the form, so nobody can submit as someone else. |
| `Controllers/AdminController.cs` | Admins only. Donors list, donations list with status filter, and **Verify** (Submitted to Received, recording which admin and when). |
| `Controllers/WebinarsController.cs` | Public. Lists upcoming webinars and saves RSVPs (blocks duplicate number for the same webinar). |

### Views (the pages)

| File | What it does |
|---|---|
| `Views/_ViewImports.cshtml`, `Views/_ViewStart.cshtml` | Shared settings: which namespaces pages can use, and that every page uses the common layout. |
| `Views/Shared/_Layout.cshtml` | The frame around every page: header with Sign in / Sign up (or role badge and Sign out), success and error messages, footer. |
| `Views/Home/Index.cshtml` | The landing page. |
| `Views/Home/Error.cshtml` | Friendly error page. |
| `Views/Account/Register.cshtml`, `Login.cshtml`, `AccessDenied.cshtml` | Sign up, sign in, and "not allowed" pages. |
| `Views/Donor/Index.cshtml` | Donor page: welcome, **Donate / Drop Off** button, the pop-up, and My donations. |
| `Views/Admin/Index.cshtml` | Admin page: counts, Donors list tab, Donations tab with filter and Verify button. |
| `Views/Webinars/Index.cshtml` | Webinar list with an expandable Register form per webinar. |

### Other

| File | What it does |
|---|---|
| `Helpers/SaTime.cs` | Shows stored UTC dates in South African time. |
| `wwwroot/css/site.css` | The green theme on top of Bootstrap (Bootstrap itself loads from a CDN). |

## 6. How the security works

- **Passwords** are never stored: Identity stores a salted hash.
- **Roles:** `[Authorize(Roles = ...)]` on the Donor and Admin controllers. Sign up can only ever create a Donor; admins come from the seeder.
- **A donor sees only their own donations** (the query filters on their user id).
- **Forms** carry anti-forgery tokens, and the whole site requires them on POST.
- **Lockout:** 5 wrong passwords locks the account for 5 minutes. The login error message is the same for "wrong email" and "wrong password".
- **Cookies** are HTTP-only and HTTPS-only.

## 7. Activating the authenticator app (two-factor) later

Nothing needs to change in the database. Identity already has the `TwoFactorEnabled` flag, stores each user's authenticator key in `AspNetUserTokens`, and `Program.cs` already registers the authenticator token provider (`AddDefaultTokenProviders`). What you add:

1. **A QR code library.** Identity gives you the secret key, not a picture. Add the `QRCoder` NuGet package (or a small JavaScript QR script) to draw the code from an `otpauth://totp/...` link.
2. **"Set up authenticator" page** (signed-in users): get or reset the key with `UserManager.GetAuthenticatorKeyAsync` / `ResetAuthenticatorKeyAsync`, show the QR code and key, then check the first 6-digit code with `VerifyTwoFactorTokenAsync`. If it is correct, call `SetTwoFactorEnabledAsync(user, true)` and generate backup codes with `GenerateNewTwoFactorRecoveryCodesAsync`.
3. **"Enter your code" page** at sign-in. In `AccountController.Login`, the `result.RequiresTwoFactor` branch (currently a placeholder message) redirects here. Its POST calls `SignInManager.TwoFactorAuthenticatorSignInAsync`.
4. **"Use a recovery code" page** calling `TwoFactorRecoveryCodeSignInAsync`, so a lost phone does not lock someone out for good.
5. **A "show backup codes" page** displayed once after setup.
6. **Optional rule:** make it compulsory for admins by redirecting an admin without `TwoFactorEnabled` to the setup page after sign-in.
7. **Phones:** each person installs Google Authenticator or Microsoft Authenticator and scans the QR code once. The server clock must be correct (Azure's is).

That is 3 new pages plus about 2 controller actions each, roughly one day including testing. Do it after everything above passes the test checklist.

## 8. Known limits of this version (good "future enhancements" for your report)

- Webinars are loaded by the seeder, not managed by an admin screen. To change them, edit `DbSeeder.cs` before the first run, or edit the `Webinars` table in the Azure Query editor.
- No email: there is no password reset or email confirmation yet.
- Donation status has two values only (Submitted, Received), as agreed.
- Admin accounts can only be added through the seeder or directly in the database.
