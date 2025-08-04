using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using LoveForTennis.BlazorServer.Components;
using LoveForTennis.BlazorServer.Components.Account;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Enums;
using LoveForTennis.Infrastructure.Data;
using LoveForTennis.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Use our existing database configuration
builder.Services.AddDatabase(builder.Configuration, "BlazorServer");
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Seed the database with test data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedDatabase(context);
}

app.Run();

static async Task SeedDatabase(ApplicationDbContext context)
{
    // Ensure database is created
    await context.Database.EnsureCreatedAsync();
    
    // Only seed if no courts exist
    if (!context.Courts.Any())
    {
        var courts = new[]
        {
            new Court
            {
                Name = "Court 1",
                Description = "Professional clay court with excellent drainage",
                SurfaceType = CourtSurfaceType.Clay,
                AllowedBookingTimeType = BookingTimeType.Hour,
                InOrOutdoorType = InOrOutdoorType.Outdoor,
                BookingAllowedFrom = new TimeOnly(7, 0),
                BookingAllowedTill = new TimeOnly(22, 0),
                BookingsOpenForNumberOfDaysIntoTheFuture = 14
            },
            new Court
            {
                Name = "Court 2", 
                Description = "Indoor hard court with climate control",
                SurfaceType = CourtSurfaceType.Hard,
                AllowedBookingTimeType = BookingTimeType.HalfHour,
                InOrOutdoorType = InOrOutdoorType.Indoor,
                BookingAllowedFrom = new TimeOnly(6, 0),
                BookingAllowedTill = new TimeOnly(23, 0),
                BookingsOpenForNumberOfDaysIntoTheFuture = 21
            },
            new Court
            {
                Name = "Court 3",
                Description = "Outdoor grass court for traditional play",
                SurfaceType = CourtSurfaceType.Grass,
                AllowedBookingTimeType = BookingTimeType.Hour,
                InOrOutdoorType = InOrOutdoorType.Outdoor,
                BookingAllowedFrom = new TimeOnly(8, 0),
                BookingAllowedTill = new TimeOnly(20, 0),
                BookingsOpenForNumberOfDaysIntoTheFuture = 7
            }
        };
        
        context.Courts.AddRange(courts);
        await context.SaveChangesAsync();
    }
}
