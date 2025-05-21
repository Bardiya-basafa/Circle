using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Infrastructure.Persistence.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Configure the database 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// configure the services 
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IFavoritesService, FavoritesService>();
builder.Services.AddScoped<IUserService, UserService>();

// configure the identities 
builder.Services.AddIdentity<User, IdentityRole<int>>(options => {
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Authentication/Login";
    options.AccessDeniedPath = "/Authentication/AccessDenied";
}
);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(
    options => {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
        options.CallbackPath = "/signin-google";
    });

// first app
//      "ClientSecret": "dac24998d5a3c17943ad6a5650a3e51612293a79"

// second app
// "ClientSecret": "eed63351ee523f16936f802f3e94b0915cb6331f"

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGitHub(
    options => {
        options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"] ?? "";
        options.Scope.Add("user:email");
        options.CallbackPath = "/signin-github";
    });


builder.Services.AddAuthorization();

var app = builder.Build();


using (var scope = app.Services.CreateAsyncScope()){
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbInitializer.SeedAsync(dbContext);
    UserManager<User>? userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    RoleManager<IdentityRole<int>>? roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    await DbInitializer.SeedUserAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()){
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
"default",
"{controller=Home}/{action=Index}/{id?}");

app.Run();
