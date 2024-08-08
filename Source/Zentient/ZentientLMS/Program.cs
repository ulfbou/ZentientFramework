using LexiconLMS.Components;
using LexiconLMS.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<LexiconLMSContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LexiconLMSDbContext") ??
        throw new InvalidOperationException("Could not find connection string 'LexiconLMSDbContext'.")));

builder.Services.AddScoped<LexiconLMSContext>(options =>
    options.GetService<LexiconLMSContext>() ?? throw new InvalidOperationException("Could not resolve LexiconLMSContext."));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.Run();
