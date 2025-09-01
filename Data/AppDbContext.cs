using Microsoft.EntityFrameworkCore;
using PurrfectMates.Models;


namespace PurrfectMates.Api.Data
{

    // Ici, j’hérite de IdentityDbContext<ApplicationUser> ça ajoute automatiquement
    public class AppDbContext : DbContext
    {

        // Constructeur  il reçoit les options (connexion SQL, provider utilisé)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Chaque DbSet représente une table dans ma base SQL Server.

        public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
        public DbSet<Animal> Animaux => Set<Animal>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<NiveauActivite> NiveauxActivites => Set<NiveauActivite>();
        public DbSet<TailleAnimal> TaillesAnimaux => Set<TailleAnimal>();
        public DbSet<TypeAnimal> TypesAnimaux => Set<TypeAnimal>();

       
    }
}
