using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; //  nécessaire pour Swagger + sécurité JWT
using PurrfectMates.Api.Data;
using PurrfectMates.Api.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//  Ici j’ajoute le support des contrôleurs et de Swagger (documentation interactive de mon API)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });  //ici je convertis les string en json
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    //  Je décris mon API (titre et version)
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PurrfectMates API",
        Version = "v1"
    });

    //  J’ajoute la possibilité d’utiliser un token JWT dans Swagger (bouton Authorize )
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = " Entrer 'Bearer' + espace + votre token JWT.\n\nExemple : 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
    });

    //  J’impose que Swagger envoie le token sur les routes protégées par [Authorize]
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//  Je configure CORS pour autoriser uniquement mon front-end à appeler l’API
builder.Services.AddCors(o => o.AddPolicy("frontend", b => b
    .WithOrigins("http://localhost:5173", "http://localhost:3000") // URL du front
    .AllowAnyHeader()  // j’autorise tous les en-têtes (Content-Type, Authorization, etc.)
    .AllowAnyMethod()  // j’autorise toutes les méthodes HTTP (GET, POST, PUT, DELETE)
));

//  Je récupère ma chaîne de connexion SQL Server depuis appsettings.json
var cs = builder.Configuration.GetConnectionString("SqlServer");

//  J’ajoute Entity Framework Core pour dialoguer avec ma base SQL Server
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));


//ici j'ajoute mon service métier LikeService

builder.Services.AddScoped<LikeService>();


//  Ici je configure l’authentification JWT (token sécurisé pour mes utilisateurs)
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //  Vérifications de validité du token JWT
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,              // je vérifie l’émetteur
            ValidateAudience = true,            // je vérifie l’audience
            ValidateLifetime = true,            // je vérifie que le token n’est pas expiré
            ValidateIssuerSigningKey = true,    // je vérifie la signature
            ValidIssuer = jwt["Issuer"],        // valeur définie dans appsettings.json
            ValidAudience = jwt["Audience"],    // valeur définie dans appsettings.json
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)) // clé secrète
        };
    });

var app = builder.Build();

//  En mode développement, j’active Swagger pour tester facilement mes routes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); //  je force les appels en HTTPS pour sécuriser les échanges

app.UseCors("frontend"); //  j’applique ma règle CORS définie plus haut

app.UseAuthentication(); //  je vérifie les tokens JWT ( doit être AVANT UseAuthorization)
app.UseAuthorization();  //  je vérifie les droits d’accès des utilisateurs

//  Je mappe mes contrôleurs pour qu’ils répondent aux routes API
app.MapControllers();

//  Je lance l’application
app.Run();
