# MealSync

MealSync is a comprehensive, web-based meal planning application designed to streamline your culinary experience. It empowers users to store personal recipes, manage ingredients, organize meals on an interactive calendar, and automatically generate combined grocery lists.

## Features

- **User Authentication:** Secure registration and login to keep your recipes and meal plans private.
- **Recipe Management:** Full CRUD (Create, Read, Update, Delete) functionality for recipes, including a dynamic interface to add, edit, and remove specific ingredients and their units.
- **Interactive Meal Planner:** A weekly calendar view where you can assign your saved recipes to specific days and meal times (Breakfast, Lunch, Dinner, Snack).
- **Automated Grocery Lists:** Instantly generate a consolidated shopping list based on the upcoming meals in your calendar. The generator automatically aggregates duplicate ingredients and combines their quantities.

## Architecture

- **Frontend:** Blazor Interactive Server
- **Backend:** ASP.NET Core 8+
- **Database:** PostgreSQL Neon
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity

## Local Setup & Installation

Follow these steps to run the application locally on your machine.

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or later)

### Instructions

1. **Clone the repository:**

   ```bash
   git clone <repository-url>
   cd src
   cd MealSync.web
   ```

2. **Run the Application:**
   Start the Blazor web server:

   ```bash
   dotnet watch
   ```

3. **View the App:**
   Open your web browser and navigate to `http://localhost:5027` (or the URL specified in the console output).
      [Production Site Link on Azure](https://mealsync-hvatc7b2h6hsh7df.eastus-01.azurewebsites.net)

## Team Members

- Tanya Trampp
- Brandon Adolf
- Manuel Zamalloa
- MF Dube

### Favorite Quotes

--Tanya's choice--
"Mistakes are a fact of life. It is the response to the error that counts." -- Nikki Ciovanni

--Brandon's quote--
"Every dead body on Mt. Everest was once a highly motivated person. So... maybe calm down." -- Slow down and enjoy life quote.

--Manuel's quote--
“First, we act in accordance with the teachings of the Savior. Then, we are blessed with His power.” — Elder David A. Bednar
