using MealSync.Web.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MealSync.Infrastructure.Data;
using MealSync.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<MealSyncDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<MealSyncDbContext>();

builder.Services.AddScoped<MealSync.Core.Interfaces.IRecipeService, MealSync.Infrastructure.Services.RecipeService>();
builder.Services.AddScoped<MealSync.Core.Interfaces.IMealPlanService, MealSync.Infrastructure.Services.MealPlanService>();
builder.Services.AddScoped<MealSync.Core.Interfaces.IGroceryListService, MealSync.Infrastructure.Services.GroceryListService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
