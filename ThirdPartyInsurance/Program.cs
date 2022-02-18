using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThirdPartyInsurance.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowSpecificOrigins,
//                      builder =>
//                      {
//                          builder.WithOrigins("http://localhost:4200",
//                                              "http://app.interdebglobal.com",
//                                              "http://test.interdebglobal.com",
//                                              "http://api.interdebglobal.com",
//                                              "http://testapi.interdebglobal.com")
//                          .AllowAnyOrigin()
//                          .WithExposedHeaders()
//                          .AllowAnyHeader()
//                          .AllowAnyMethod();
//                      });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.UseCors(x => x
           .AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders());

app.Run();
