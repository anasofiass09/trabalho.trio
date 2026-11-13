using Microsoft.EntityFrameworkCore;
using sistemaagendamento.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. LER A STRING DE CONEXÃO
var conexao = builder.Configuration.GetConnectionString("ConexaoMySQL");

// 2. CONFIGURAR O DBCONTEXT COM MYSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conexao, ServerVersion.AutoDetect(conexao))
);

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

app.UseAuthorization();

// ROTA PADRÃO MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();