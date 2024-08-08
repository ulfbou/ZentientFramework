using LMS.Components;
using LMS.Components.Account;
using LMS.Core.Extensions;
using LMS.Core.Identity;
using LMS.Core.Services;
using LMS.Persistence.Data;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

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
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.RequireAdminRole, policy => policy.RequireRole(Role.Admin));
    options.AddPolicy(Policy.RequireTeacherRole, policy => policy.RequireRole(Role.Teacher));
    options.AddPolicy(Policy.RequireStudentRole, policy => policy.RequireRole(Role.Student));
    options.AddPolicy(Policy.RequireStudentOrTeacherRole, policy => policy.RequireRole(Role.Student, Role.Teacher));
    options.AddPolicy(Policy.RequireAnyRole, policy => policy.RequireAuthenticatedUser());
});

var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext")
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, Role>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddRequestService<IActivityRequestService, ActivityRequestService>(configuration);
builder.Services.AddRequestService<IApplicationUserRequestService, ApplicationUserRequestService>(configuration);
builder.Services.AddRequestService<IActivityRequestService, ActivityRequestService>(configuration);
builder.Services.AddRequestService<IApplicationUserRequestService, ApplicationUserRequestService>(configuration);
builder.Services.AddRequestService<ICourseRequestService, CourseRequestService>(configuration);
builder.Services.AddRequestService<IDocumentRequestService, DocumentRequestService>(configuration);
builder.Services.AddRequestService<IModuleRequestService, ModuleRequestService>(configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();

    await context.Database.MigrateAsync();

    await SeedData.EnsureRolesAsync(roleManager);

    var roles = new List<string> { Role.Admin, Role.Teacher, Role.Student };

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new Role(roleName));
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseWebAssemblyDebugging();
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

app.Run();
