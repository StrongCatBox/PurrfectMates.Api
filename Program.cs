using Microsoft.AspNetCore.Authentication.JwtBearer;   // Pour activer l’authentification via token JWT
using Microsoft.AspNetCore.Identity;                  // Pour gérer les utilisateurs/roles avec Identity
using Microsoft.EntityFrameworkCore;                  // Pour utiliser Entity Framework Core (accès SQL)
using Microsoft.IdentityModel.Tokens;                 // Pour configurer la validation des tokens JWT
using PurrfectMates.Api.Data;                         // Pour accéder à AppDbContext et ApplicationUser
using System.Text;                                    // Pour encoder la clé secrète JWT

var builder = WebApplication.CreateBuilder(args);

// ============= Configuration des services (injection de dépendances) =============

// Active les Controllers (API REST)
builder.Services.AddControllers();

// Swagger permet de tester l’API dans un navigateur (interface interactive)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Politique CORS pour autoriser les appels depuis mon front 
builder.Services.AddCors(o => o.AddPolicy("frontend", b => b
    .WithOrigins("http://localhost:5173", "http://localhost:3000") // Front autorisé
    .AllowAnyHeader()   // Autorise tous les headers (ex: Authorization)
    .AllowAnyMethod()   // Autorise toutes les méthodes (GET, POST, PUT, DELETE)
));

// Récupération de la chaîne de connexion SQL Server définie dans appsettings.json
var cs = builder.Configuration.GetConnectionString("SqlServer");

// Injection du DbContext pour Entity Framework (base de données + Identity)
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));

// Configuration d’ASP.NET Identity (gestion des comptes et rôles utilisateurs)
builder.Services
    .AddIdentityCore<ApplicationUser>(opt =>
    {
        // Je définis mes règles de sécurité pour les mots de passe
        opt.Password.RequiredLength = 6;     // Minimum 6 caractères
        opt.User.RequireUniqueEmail = true;  // Chaque email doit être unique
    })
    .AddRoles<IdentityRole>()                // Ajout de la gestion des rôles (Adoptant, proprio)
    .AddEntityFrameworkStores<AppDbContext>() // Sauvegarde des comptes dans ma base SQL Server
    .AddDefaultTokenProviders();             // Permet la génération de tokens (réinitialisation mot de passe, etc.)

// Configuration de l’authentification avec JWT (tokens pour se connecter au front)
var jwt = builder.Configuration.GetSection("Jwt"); // Récupère la config du fichier appsettings.json
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)); // Clé secrète encodée

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        // Règles de validation d’un token JWT
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,               // Vérifie l’émetteur du token
            ValidateAudience = true,             // Vérifie l’audience du token
            ValidateLifetime = true,             // Vérifie que le token n’a pas expiré
            ValidateIssuerSigningKey = true,     // Vérifie que la signature du token est correcte
            ValidIssuer = jwt["Issuer"],         // Émetteur attendu (défini dans appsettings.json)
            ValidAudience = jwt["Audience"],     // Audience attendue (définie dans appsettings.json)
            IssuerSigningKey = key               // Clé utilisée pour signer le token
        };
    });

// Ajoute le système d’autorisation (vérifie les rôles, [Authorize] sur les contrôleurs, etc.)
builder.Services.AddAuthorization();

var app = builder.Build();

// Pipeline des middlewares (ordre d’exécution des requêtes) 

// Active Swagger uniquement en mode développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Force HTTPS (meilleure sécurité que HTTP)
app.UseHttpsRedirection();

// Active CORS (autorise mon front à appeler mon API)
app.UseCors("frontend");

// ⚠️ Authentication AVANT Authorization
// Vérifie que la requête contient un token JWT valide
app.UseAuthentication();

// Vérifie que l’utilisateur a bien les droits (rôle, policies, etc.)
app.UseAuthorization();

// Lie automatiquement mes controllers (routes REST)
app.MapControllers();

// Endpoint simple de test → si ça répond "API OK", l’API tourne
app.MapGet("/", () => "API OK");

app.Run(); // Lance l’application
