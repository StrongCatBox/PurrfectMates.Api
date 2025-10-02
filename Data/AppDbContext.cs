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
        public DbSet<Temperament> Temperaments => Set<Temperament>();
        public DbSet<TypeLogement> TypesLogements => Set<TypeLogement>();
        public DbSet<TemperamentParAnimal> TemperamentsParAnimaux => Set<TemperamentParAnimal>();
        public DbSet<LogementParAnimal> LogementsParAnimaux => Set<LogementParAnimal>();
        public DbSet<Photo> Photos => Set<Photo>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  Configuration TemperamentParAnimal 

            // Définir la clé primaire composite (idAnimal + idTemperament)
            modelBuilder.Entity<TemperamentParAnimal>()
                .HasKey(temperamentParAnimal => new
                {
                    temperamentParAnimal.IdAnimal,
                    temperamentParAnimal.IdTemperament
                });

            // Relation: TemperamentParAnimal -> Animal
            modelBuilder.Entity<TemperamentParAnimal>()
                .HasOne(temperamentParAnimal => temperamentParAnimal.Animal)
                .WithMany(animal => animal.TemperamentsParAnimaux)
                .HasForeignKey(temperamentParAnimal => temperamentParAnimal.IdAnimal);

            // Relation: TemperamentParAnimal -> Temperament
            modelBuilder.Entity<TemperamentParAnimal>()
                .HasOne(temperamentParAnimal => temperamentParAnimal.Temperament)
                .WithMany()
                .HasForeignKey(temperamentParAnimal => temperamentParAnimal.IdTemperament);

            //  Configuration LogementParAnimal 

            // Définir la clé primaire composite (idAnimal + idTypeLogement)
            modelBuilder.Entity<LogementParAnimal>()
                .HasKey(logementParAnimal => new
                {
                    logementParAnimal.IdAnimal,
                    logementParAnimal.IdTypeLogement
                });

            // Relation: LogementParAnimal -> Animal
            modelBuilder.Entity<LogementParAnimal>()
                .HasOne(logementParAnimal => logementParAnimal.Animal)
                .WithMany(animal => animal.LogementsParAnimaux)
                .HasForeignKey(logementParAnimal => logementParAnimal.IdAnimal);

            // Relation: LogementParAnimal -> TypeLogement
            modelBuilder.Entity<LogementParAnimal>()
                .HasOne(logementParAnimal => logementParAnimal.TypeLogement)
                .WithMany()
                .HasForeignKey(logementParAnimal => logementParAnimal.IdTypeLogement);

            //  Configuration Match 

            // Définir la clé primaire composite (UtilisateurId + AnimalId)
            modelBuilder.Entity<Match>()
                .HasKey(match => new
                {
                    match.UtilisateurId,
                    match.AnimalId
                });

            // Relation: Match -> Utilisateur
            modelBuilder.Entity<Match>()
                .HasOne(match => match.Utilisateur)
                .WithMany(utilisateur => utilisateur.Matches)
                .HasForeignKey(match => match.UtilisateurId);

            // Relation: Match -> Animal
            modelBuilder.Entity<Match>()
                .HasOne(match => match.Animal)
                .WithMany(animal => animal.Matches)
                .HasForeignKey(match => match.AnimalId);

            //Configuration Photo

            modelBuilder.Entity<Photo>()
                .HasOne(photo => photo.Animal)
                .WithMany(animal => animal.Photos)
                .HasForeignKey(photo => photo.IdAnimal);
        }


    }
}
