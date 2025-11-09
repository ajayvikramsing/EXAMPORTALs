//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run(); 

using EXAMPORTAL.Data;
using EXAMPORTAL.Models;
using EXAMPORTAL.Repositery;
using EXAMPORTAL.Repositery.Interface;
using EXAMPORTAL.Repositery.Serviceses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// DbContext
services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Identity
services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT Auth
var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

// MVC + Razor pages for frontend
services.AddControllersWithViews();
services.AddRazorPages();

// DI for our services
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IFormService, FormService>();
services.AddScoped<ISubmissionService, SubmissionService>();
services.AddScoped<IPaymentService, PaymentService>();
services.AddScoped<JwtTokenGenerator>();

// Swagger for API docs
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// CORS
services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// apply migrations at startup (optional for dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // seed roles and admin user if missing
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    string[] roles = new[] { "Admin", "User" };
    foreach (var r in roles) if (!roleManager.RoleExistsAsync(r).Result) roleManager.CreateAsync(new IdentityRole(r)).Wait();

    var adminEmail = configuration["AdminUser:Email"];
    if (!string.IsNullOrEmpty(adminEmail))
    {
        var admin = userManager.FindByEmailAsync(adminEmail).Result;
        if (admin == null)
        {
            admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail, FullName = "Administrator" };
            var pw = configuration["AdminUser:Password"] ?? "Admin123!";
            userManager.CreateAsync(admin, pw).Wait();
            userManager.AddToRoleAsync(admin, "Admin").Wait();
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();

