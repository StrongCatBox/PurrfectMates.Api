using Microsoft.EntityFrameworkCore;
using PurrfectMates.Models; 

namespace PurrfectMates.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
        public DbSet<Animal> Animaux => Set<Animal>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<NiveauActivite> NiveauxActivites => Set<NiveauActivite>();
        public DbSet<TailleAnimal> TaillesAnimaux => Set<TailleAnimal>();
        public DbSet<TypeAnimal> TypesAnimaux => Set<TypeAnimal>();
    }
}
