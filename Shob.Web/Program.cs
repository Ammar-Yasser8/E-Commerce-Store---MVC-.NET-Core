using Microsoft.EntityFrameworkCore;
using Mshop.DataAccess;
using Mshop.Entities.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Shop.Utilities;
using Stripe;
using Myshop.DataAccess.DbIntializer;
using Mshop.DataAccess.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();  
builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer
(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options => options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(4)
    ).AddDefaultTokenProviders().AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();    
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:Secretkey").Get<string>();
SeedDB();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Customer",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


app.Run();


void SeedDB()
{
    using (var scop = app.Services.CreateScope())
    {
        var dbinitialzer = scop.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbinitialzer.Initialize();
    }
}
                          