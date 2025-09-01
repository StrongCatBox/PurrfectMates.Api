using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PurrfectMates.Api.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//  Ici j’ajoute le support des contrôleurs et de Swagger (documentation interactive de mon API)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Je configure CORS pour autoriser uniquement mon front-end (React, Vue, etc.) à appeler l’API
builder.Services.AddCors(o => o.AddPolicy("frontend", b => b
    .WithOrigins("http://localhost:5173", "http://localhost:3000") // URL du front
    .AllowAnyHeader()  // j’autorise tous les en-têtes (Content-Type, Authorization, etc.)
    .AllowAnyMethod()  // j’autorise toutes les méthodes HTTP (GET, POST, PUT, DELETE)
));

//  Je récupère ma chaîne de connexion à SQL Server depuis appsettings.json
var cs = builder.Configuration.GetConnectionString("SqlServer");

//  J’ajoute Entity Framework Core pour accéder à ma base de données
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));

//  Ici je configure l’authentification JWT (token d’accès sécurisé pour les utilisateurs)
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //  Vérifications que le token est bien valide (signature, durée de vie, émetteur, audience)
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,  // je vérifie que l’émetteur correspond
            ValidateAudience = true, // je vérifie que l’audience correspond
            ValidateLifetime = true, // je vérifie que le token n’est pas expiré
            ValidateIssuerSigningKey = true, // je vérifie la clé de signature
            ValidIssuer = jwt["Issuer"],     // valeur que j’ai mise dans appsettings.json
            ValidAudience = jwt["Audience"], // valeur que j’ai mise dans appsettings.json
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)) // ma clé secrète
        };
    });

var app = builder.Build();

// 👉 En mode développement, j’active Swagger pour tester mon API facilement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // 👉 je force les appels à passer en HTTPS pour plus de sécurité

app.UseCors("frontend"); // 👉 j’applique la règle CORS définie plus haut

app.UseAuthentication(); // 👉 je vérifie les tokens JWT (doit être placé avant UseAuthorization)
app.UseAuthorization();  // 👉 je vérifie si l’utilisateur a le droit d’accéder à la ressource

//  Je mappe mes contrôleurs pour qu’ils répondent aux routes API
app.MapControllers();

//  Je lance mon application
app.Run();
