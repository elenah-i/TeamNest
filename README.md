# TeamNest – Multi-tenant project management (ASP.NET Core MVC)

Phase 1 (about 25% of the project): layouts, Identity customization, Dapper repositories,
AJAX features, organizations, projects and team pages.

## Run it in Visual Studio

1. Open `TeamNest.sln`. NuGet packages restore automatically.
2. Open **Tools → NuGet Package Manager → Package Manager Console** and run:
   ```
   Add-Migration InitialCreate
   Update-Database
   ```
   This creates the `TeamNestDb` database in SQL Server LocalDB.
3. Press **F5** and click **Sign up free**.

To test roles: sign up once with "Create a new one" (you become Owner), copy the invite code
from the dashboard, then sign up in a private browser window with "Join with a code" (Member).

## Where things are

| Folder | What it holds |
| --- | --- |
| `Controllers` | Home, Account (login/register), Dashboard, Projects, Team |
| `Data` | EF Core context (Identity + migrations), Dapper connection factory, role seeding |
| `Identity` | Custom claims factory that adds OrganizationId to the login cookie |
| `Models/Entities` | Database tables: ApplicationUser, Organization, Project |
| `Models/ViewModels` | Form and page models with validation rules |
| `Models/Dtos` | Query results filled by Dapper |
| `Repositories` | All SQL, written with Dapper, always filtered by organization |
| `Services` | CurrentUserService: reads the signed-in user and tenant from claims |
| `TagHelpers` | `<project-status>` custom tag helper |
| `ViewComponents` | Sidebar workspace card |
| `Views/Shared` | `_Layout` (base) + `_PublicLayout`, `_AuthLayout`, `_AppLayout` (nested) |
| `wwwroot/js` | `site.js` (AJAX helpers, toasts), `projects.js`, `register.js` |

## Connection string

`appsettings.json` uses LocalDB, which comes with Visual Studio. To use a full SQL Server,
change `DefaultConnection`, for example:
`Server=.;Database=TeamNestDb;Trusted_Connection=True;TrustServerCertificate=True`
