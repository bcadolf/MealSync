# MealSync Project Specification

## 1. Improved Requirements
**Project Vision:** MealSync is a comprehensive, web-based meal planning platform designed to streamline the culinary experience. It empowers users to manage custom recipes, organize their culinary schedule via an intuitive calendar interface, and automatically compile intelligent grocery lists based on planned meals.

**Core Features & User Stories:**
*   **User Authentication & Authorization:**
    *   *As a user, I want to securely register, log in, and manage my profile so that my recipes and meal plans remain private and secure.*
*   **Recipe Management (CRUD):**
    *   *As a user, I want to create, read, update, and delete my own recipes so that I can maintain a personalized digital cookbook.*
*   **Ingredient & Inventory Management:**
    *   *As a user, I want to add ingredients with specific quantities and standardized units to my recipes to ensure accurate measurements.*
*   **Meal Planner Calendar:**
    *   *As a user, I want to assign specific recipes to days on a weekly or monthly calendar view to plan my meals in advance.*
*   **Automated Grocery List Generation:**
    *   *As a user, I want the system to automatically generate a grocery list based on my meal plan, aggregating duplicate ingredients and converting units where possible, to simplify shopping.*
*   **External API Integration (Enhancement):**
    *   *As a user, I want to import recipes directly from public recipe APIs (e.g., Spoonacular, Edamam) to easily expand my recipe collection.*
*   **Shared Recipe Database (Enhancement):**
    *   *As a user, I want to search and browse a shared internal database of recipes contributed by other users to discover new meal ideas.*

## 2. Architecture Improvements
*   **Current:** Blazor (Frontend), ASP.NET Core (Backend), SQLite, EF Core.
*   **Suggestions:**
    *   **Blazor Web App (Auto/Interactive Server/WebAssembly):** Since .NET 8, Blazor Web App provides a unified hosting model. For a highly interactive meal calendar, consider using Interactive WebAssembly for the calendar component if offline capabilities or reduced server load are desired, while keeping Interactive Server or static SSR for simpler CRUD pages.
    *   **Database:** SQLite is excellent for development and light usage. However, if the "Shared internal recipe database" grows significantly or concurrent user access increases, migrating to **PostgreSQL** or **SQL Server** for production would provide better concurrency, robust backups, and advanced JSON/search features. EF Core makes this transition relatively seamless.
    *   **API Layer:** If you plan to build a mobile app in the future, decouple the backend from the Blazor frontend by exposing a RESTful API or GraphQL endpoint. This also allows the Blazor WebAssembly client (if used) to consume the API securely.

## 3. Missing Requirements
*   **Unit Conversion:** When combining ingredients for the grocery list (e.g., combining 1 cup of flour and 2 tablespoons of flour), the system needs a standard unit conversion logic.
*   **Scaling Recipes:** Users might want to scale a recipe up or down (e.g., from 4 servings to 6). The ingredient quantities should adjust accordingly.
*   **Dietary Restrictions & Tags:** Filtering recipes by tags (Vegan, Gluten-Free, Keto) or allergens.
*   **User Roles:** Differentiating between standard users and administrators (who might moderate the shared recipe database).
*   **Password Reset & Email Verification:** Basic security features for user account management.

## 4. Database Tables and Relationships
*   **Users**
    *   `UserId` (PK), `Username`, `Email`, `PasswordHash`, `CreatedAt`
*   **Recipes**
    *   `RecipeId` (PK), `UserId` (FK), `Title`, `Description`, `Instructions`, `PrepTime`, `CookTime`, `Servings`, `IsPublic` (for shared DB)
*   **Ingredients** (Master list of standardized ingredients)
    *   `IngredientId` (PK), `Name`, `Category` (e.g., Dairy, Produce)
*   **RecipeIngredients** (Join table)
    *   `RecipeId` (FK), `IngredientId` (FK), `Quantity`, `Unit` (e.g., grams, cups)
    *   *Composite PK: `RecipeId`, `IngredientId`*
*   **MealPlans**
    *   `MealPlanId` (PK), `UserId` (FK), `RecipeId` (FK), `Date`, `MealType` (Breakfast, Lunch, Dinner, Snack)
*   **GroceryLists**
    *   `ListId` (PK), `UserId` (FK), `CreatedAt`, `IsCompleted`
*   **GroceryListItems**
    *   `ListItemId` (PK), `ListId` (FK), `IngredientId` (FK), `Quantity`, `Unit`, `IsChecked`

**Relationships:**
*   User 1:N Recipes
*   User 1:N MealPlans
*   User 1:N GroceryLists
*   Recipe 1:N RecipeIngredients
*   Ingredient 1:N RecipeIngredients
*   MealPlan N:1 Recipe
*   GroceryList 1:N GroceryListItems

## 5. API Endpoints / Service Structure
Even if using Blazor Server (where services are called directly), structuring as distinct services or endpoints is crucial for clean architecture.

**Services / Interfaces:**
*   `IAuthService`: Login, Register, Logout.
*   `IRecipeService`: GetById, GetAllForUser, SearchShared, Create, Update, Delete, ImportFromApi.
*   `IMealPlanService`: GetPlanForDateRange, AddRecipeToPlan, RemoveRecipeFromPlan.
*   `IGroceryListService`: GenerateListFromDateRange, GetActiveList, MarkItemPurchased.
*   `IIngredientService`: SearchIngredients, AddIngredient.

**REST API Endpoints (if decoupled):**
*   **Auth:** `POST /api/auth/register`, `POST /api/auth/login`
*   **Recipes:** `GET /api/recipes`, `GET /api/recipes/{id}`, `POST /api/recipes`, `PUT /api/recipes/{id}`, `DELETE /api/recipes/{id}`, `GET /api/recipes/search`
*   **Meal Plans:** `GET /api/mealplans?startDate={date}&endDate={date}`, `POST /api/mealplans`, `DELETE /api/mealplans/{id}`
*   **Grocery Lists:** `POST /api/grocerylists/generate`, `GET /api/grocerylists/{id}`, `PUT /api/grocerylists/{listId}/items/{itemId}`

## 6. Project Folder Structure (Clean Architecture)
```text
MealSync.sln
├── src/
│   ├── MealSync.Core/                (Domain Entities, Interfaces, Exceptions)
│   │   ├── Entities/
│   │   ├── Interfaces/
│   │   └── Exceptions/
│   ├── MealSync.Infrastructure/      (EF Core DbContext, Migrations, External API Clients)
│   │   ├── Data/
│   │   ├── Repositories/
│   │   └── Services/               (e.g., SpoonacularApiClient)
│   └── MealSync.Web/                 (Blazor UI, Controllers/Minimal APIs, Auth setup)
│       ├── Components/
│       │   ├── Layout/
│       │   └── Pages/
│       ├── Services/               (UI-specific state or wrappers)
│       ├── Program.cs
│       └── appsettings.json
├── tests/
│   ├── MealSync.Core.Tests/
│   ├── MealSync.Infrastructure.Tests/
│   └── MealSync.Web.Tests/
```

## 7. Potential Technical Risks
1.  **Complex Ingredient Aggregation:** Parsing, normalizing, and combining different units (e.g., "1 cup" + "4 oz" + "1 pinch") for the grocery list is algorithmically complex. *Mitigation: Standardize units on input or use a reliable unit conversion library.*
2.  **External API Rate Limits:** Public recipe APIs like Spoonacular have strict rate limits on free tiers. *Mitigation: Implement aggressive caching in the database or Redis to avoid hitting the API for every request.*
3.  **Blazor Server State Management & Scalability:** Blazor Server maintains an active SignalR connection for each client. High latency or network instability can degrade UX. If user count scales, memory usage on the server will increase. *Mitigation: Design stateless components where possible, or consider Blazor WebAssembly for offline/client-side heavy processing.*
4.  **Date/Time Zone Handling:** A meal planned for "Dinner on Tuesday" might appear on Wednesday for a user traveling across time zones. *Mitigation: Store all meal plan dates in UTC and convert to the user's local timezone in the UI layer.*
5.  **Concurrency in Shared Recipes:** If multiple users attempt to edit a shared recipe simultaneously, data loss could occur. *Mitigation: Implement optimistic concurrency control (e.g., RowVersion in EF Core).*
