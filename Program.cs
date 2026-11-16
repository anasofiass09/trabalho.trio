using Microsoft.EntityFrameworkCore;
using sistemaagendamento.Data;

var builder = WebApplication.CreateBuilder(args);

// Railway usa a variável PORT para expor a aplicação.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();

// Configure a variável ConnectionStrings__ConexaoPostgres no Railway.
var conexao = builder.Configuration.GetConnectionString("ConexaoPostgres");
if (string.IsNullOrWhiteSpace(conexao))
    throw new InvalidOperationException(
        "Configure ConnectionStrings__ConexaoPostgres no Railway com a conexão PostgreSQL do Supabase.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(conexao, postgres => postgres.EnableRetryOnFailure(3)));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS é finalizado pelo proxy do Railway.
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
