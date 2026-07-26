using TaskFlow.Business.Domain;
using TaskFlow.Data;
using TaskFlow.Data.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Encodings.Web;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskFlowContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<StatusRepository>();
builder.Services.AddScoped<StatusDomain>();
builder.Services.AddScoped<UserRoleRepository>();
builder.Services.AddScoped<UserRoleDomain>();
builder.Services.AddScoped<SystemUserRepository>();
builder.Services.AddScoped<SystemUserDomain>();
builder.Services.AddScoped<SystemLogRepository>();
builder.Services.AddScoped<SystemLogDomain>();
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<ProjectDomain>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<CategoryDomain>();
builder.Services.AddScoped<TodoItemRepository>();
builder.Services.AddScoped<TodoItemDomain>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Login/";
        options.Cookie.Name = ".TaskFlow.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = ctx =>
            {
                var ret = ctx.Request.Path + ctx.Request.QueryString;
                ctx.Response.Redirect($"/Auth/Login?denied=1&ReturnUrl={UrlEncoder.Default.Encode(ret)}");
                return Task.CompletedTask;
            },
            OnRedirectToLogin = ctx =>
            {
                var ret = ctx.Request.Path + ctx.Request.QueryString;
                ctx.Response.Redirect($"/Auth/Login?required=1&ReturnUrl={UrlEncoder.Default.Encode(ret)}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

var cultures = new[] { "en", "ar" }.Select(c => new CultureInfo(c)).ToList();

var app = builder.Build();

// Auto-create database on startup (development)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskFlowContext>();
    db.Database.EnsureCreated();

    if (!db.UserRole.Any(r => r.RoleName == "User" || r.RoleName == "Admin"))
    {
        db.UserRole.AddRange(
            new TaskFlow.Data.Models.tblUserRole { guid = Guid.NewGuid(), RoleName = "User", RoleNameAr = "مستخدم", IsActive = true, CreationDate = DateTime.Now },
            new TaskFlow.Data.Models.tblUserRole { guid = Guid.NewGuid(), RoleName = "Admin", RoleNameAr = "مدير", IsActive = true, CreationDate = DateTime.Now }
        );
        db.SaveChanges();
    }

    if (!db.Status.Any())
    {
        db.Status.AddRange(
            new TaskFlow.Data.Models.tblStatus { guid = Guid.NewGuid(), StatusName = "Pending", StatusNameAr = "قيد الانتظار", IsActive = true, CreationDate = DateTime.Now },
            new TaskFlow.Data.Models.tblStatus { guid = Guid.NewGuid(), StatusName = "In Progress", StatusNameAr = "قيد التنفيذ", IsActive = true, CreationDate = DateTime.Now },
            new TaskFlow.Data.Models.tblStatus { guid = Guid.NewGuid(), StatusName = "Completed", StatusNameAr = "مكتمل", IsActive = true, CreationDate = DateTime.Now },
            new TaskFlow.Data.Models.tblStatus { guid = Guid.NewGuid(), StatusName = "On Hold", StatusNameAr = "معلق", IsActive = true, CreationDate = DateTime.Now }
        );
        db.SaveChanges();
    }

    var sysRepo = scope.ServiceProvider.GetRequiredService<SystemUserRepository>();

    // Ensure roles exist
    foreach (var r in new[] { ("Admin", "مدير"), ("User", "مستخدم") })
    {
        if (!db.UserRole.Any(x => x.RoleName == r.Item1))
        {
            db.UserRole.Add(new TaskFlow.Data.Models.tblUserRole { guid = Guid.NewGuid(), RoleName = r.Item1, RoleNameAr = r.Item2, IsActive = true, CreationDate = DateTime.Now });
            db.SaveChanges();
        }
    }

    var adminRoleId = db.UserRole.First(r => r.RoleName == "Admin").Id;
    var userRoleId = db.UserRole.First(r => r.RoleName == "User").Id;

    // Always reset admin password to ensure it works
    var adminUser = db.SystemUser.FirstOrDefault(u => u.Email == "admin@taskflow.local");
    if (adminUser != null)
    {
        adminUser.Password = sysRepo.HashPassword("Admin@123");
        db.SaveChanges();
    }
    else
    {
        db.SystemUser.Add(new TaskFlow.Data.Models.tblSystemUser
        {
            guid = Guid.NewGuid(), Name = "Admin", NameAr = "مدير",
            Email = "admin@taskflow.local",
            Password = sysRepo.HashPassword("Admin@123"),
            UserRoleId = adminRoleId, IsActive = true, CreationDate = DateTime.Now
        });
        db.SaveChanges();
    }

    var userUser = db.SystemUser.FirstOrDefault(u => u.Email == "user@taskflow.local");
    if (userUser == null)
    {
        db.SystemUser.Add(new TaskFlow.Data.Models.tblSystemUser
        {
            guid = Guid.NewGuid(), Name = "Nasser", NameAr = "ناصر",
            Email = "user@taskflow.local",
            Password = sysRepo.HashPassword("User@123"),
            UserRoleId = userRoleId, IsActive = true,
            CreationDate = DateTime.Now.AddDays(-10)
        });
        db.SaveChanges();

        var userId = db.SystemUser.First(u => u.Email == "user@taskflow.local").Id;
        var pendingId = db.Status.First(s => s.StatusName == "Pending").Id;
        var inProgressId = db.Status.First(s => s.StatusName == "In Progress").Id;
        var completedId = db.Status.First(s => s.StatusName == "Completed").Id;

        var proj1Guid = Guid.NewGuid();
        var proj2Guid = Guid.NewGuid();
        db.Project.AddRange(
            new TaskFlow.Data.Models.tblProject { guid = proj1Guid, Name = "Website Redesign", Description = "Redesign company website with modern UI", Color = "#0f7179", UserId = userId, IsActive = true, CreationDate = DateTime.Now.AddDays(-20) },
            new TaskFlow.Data.Models.tblProject { guid = proj2Guid, Name = "Mobile App", Description = "Cross-platform mobile app", Color = "#6d28d9", UserId = userId, IsActive = true, CreationDate = DateTime.Now.AddDays(-10) }
        );
        db.SaveChanges();

        var cat1Guid = Guid.NewGuid();
        var cat2Guid = Guid.NewGuid();
        var cat3Guid = Guid.NewGuid();
        db.Category.AddRange(
            new TaskFlow.Data.Models.tblCategory { guid = cat1Guid, Name = "Design", Color = "#0f7179", UserId = userId, IsActive = true, CreationDate = DateTime.Now.AddDays(-15) },
            new TaskFlow.Data.Models.tblCategory { guid = cat2Guid, Name = "Development", Color = "#6d28d9", UserId = userId, IsActive = true, CreationDate = DateTime.Now.AddDays(-15) },
            new TaskFlow.Data.Models.tblCategory { guid = cat3Guid, Name = "Meeting", Color = "#059669", UserId = userId, IsActive = true, CreationDate = DateTime.Now.AddDays(-15) }
        );
        db.SaveChanges();

        db.TodoItem.AddRange(
            new TaskFlow.Data.Models.tblTodoItem { guid = Guid.NewGuid(), Title = "Design homepage", Description = "Create wireframes for homepage", DueDate = DateTime.Now.AddDays(5), Priority = 1, ProjectId = db.Project.First(p => p.guid == proj1Guid).Id, CategoryId = db.Category.First(c => c.guid == cat1Guid).Id, UserId = userId, StatusId = inProgressId, IsActive = true, CreationDate = DateTime.Now.AddDays(-2) },
            new TaskFlow.Data.Models.tblTodoItem { guid = Guid.NewGuid(), Title = "Implement auth", Description = "Add login/register with JWT", DueDate = DateTime.Now.AddDays(10), Priority = 1, ProjectId = db.Project.First(p => p.guid == proj1Guid).Id, CategoryId = db.Category.First(c => c.guid == cat2Guid).Id, UserId = userId, StatusId = pendingId, IsActive = true, CreationDate = DateTime.Now.AddDays(-1) },
            new TaskFlow.Data.Models.tblTodoItem { guid = Guid.NewGuid(), Title = "Review competitors", Description = "Analyze competitor websites", DueDate = DateTime.Now.AddDays(-2), Priority = 3, ProjectId = db.Project.First(p => p.guid == proj1Guid).Id, CategoryId = db.Category.First(c => c.guid == cat1Guid).Id, UserId = userId, StatusId = completedId, IsActive = true, CreationDate = DateTime.Now.AddDays(-10) },
            new TaskFlow.Data.Models.tblTodoItem { guid = Guid.NewGuid(), Title = "Design DB schema", Description = "Create ERD for mobile app backend", DueDate = DateTime.Now.AddDays(4), Priority = 1, ProjectId = db.Project.First(p => p.guid == proj2Guid).Id, CategoryId = db.Category.First(c => c.guid == cat2Guid).Id, UserId = userId, StatusId = inProgressId, IsActive = true, CreationDate = DateTime.Now.AddDays(-4) },
            new TaskFlow.Data.Models.tblTodoItem { guid = Guid.NewGuid(), Title = "Sprint planning", Description = "Plan next sprint tasks", DueDate = DateTime.Now.AddDays(1), Priority = 2, ProjectId = null, CategoryId = db.Category.First(c => c.guid == cat3Guid).Id, UserId = userId, StatusId = pendingId, IsActive = true, CreationDate = DateTime.Now.AddDays(-1) }
        );
        db.SaveChanges();
    }
}

var locOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = cultures,
    SupportedUICultures = cultures,
    FallBackToParentCultures = true,
    FallBackToParentUICultures = true,
    RequestCultureProviders =
    [
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    ]
};

app.UseRequestLocalization(locOptions);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}");

app.Run();
