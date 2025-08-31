#  PurrfectMates.Api

PurrfectMates.Api est l’API en C# (.NET) du projet **PurrfectMates**, une application visant à faciliter la mise en relation entre adoptants et propriétaires d’animaux.  
Elle fournit les endpoints nécessaires à la gestion des utilisateurs, des animaux et du matchmaking.

---

##  Fonctionnalités principales
- Gestion des **utilisateurs**
- Gestion des **animaux à l’adoption**
- Système de **matchmaking** (adoptant ↔ propriétaire)
- Connexion à une base de données **SQL Server**

---

##  Technologies utilisées
- **C# / .NET**
- **ASP.NET Core Web API**
- **SQL Server**
- **Entity Framework Core**
- **Architecture en couches** (Controller, Métier, Repository)

---

##  Installation et exécution

1. **Cloner le dépôt :**
   ```bash
   git clone https://github.com/TON-USER/PurrfectMates.Api.git
   cd PurrfectMates.Api


## Configurer la base de données :

```

Modifier la chaîne de connexion dans appsettings.json :

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLSERVER;Database=PurrfectMatesDb;User Id=SA;Password=VotreMotDePasse;TrustServerCertificate=True;"
}

```
