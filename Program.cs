using Microsoft.EntityFrameworkCore;         
using PurrfectMates.Api.Data;              

var builder = WebApplication.CreateBuilder(args);

// 1) Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(o => o.AddPolicy("frontend", b => b
    .WithOrigins("http://localhost:5173", "http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
));

// 3) (à activer quand tu crées AppDbContext + appsettings.json)
 var cs = builder.Configuration.GetConnectionString("SqlServer");
 builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("frontend");     // autorise le front à appeler l’API
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "API OK"); // ping

app.Run();
