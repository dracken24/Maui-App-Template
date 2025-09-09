# Template MAUI - Application Bureau Mobile

Ce template MAUI fournit une application mobile complète avec un design cellulaire moderne, similaire au projet WPF-Template mais adapté aux appareils mobiles.

## 🚀 Fonctionnalités

### ✅ Implémentées
- **Authentification complète** : Connexion et inscription avec validation
- **Interface cellulaire moderne** : Design responsive avec cartes arrondies et ombres
- **Navigation par onglets** : Interface intuitive adaptée aux mobiles
- **Base de données SQLite** : Stockage local avec Entity Framework Core
- **Gestion des rendez-vous** : Calendrier interactif avec liste des événements
- **Pages complètes** :
  - Page de connexion
  - Page d'inscription
  - Page d'accueil avec statistiques
  - Calendrier interactif
  - Gestion des fonctionnalités
  - Paramètres utilisateur
  - Page à propos

### 🔧 Architecture
- **Modèles** : `User`, `RendezVous` avec relations
- **Services** : `AuthService`, `DatabaseService`
- **Base de données** : SQLite avec Entity Framework Core
- **Navigation** : Shell avec TabBar pour interface mobile
- **Design** : Interface cellulaire avec couleurs cohérentes

## 📱 Design Cellulaire

L'application utilise un design cellulaire moderne avec :
- **Cartes arrondies** (Frames) avec ombres
- **Couleurs cohérentes** : #007ACC, #17A2B8, #28A745, etc.
- **Navigation par onglets** en bas d'écran
- **Interface responsive** adaptée aux écrans mobiles
- **Icônes emoji** pour une interface conviviale

## 🛠️ Installation et Utilisation

### Prérequis
- .NET 9.0
- Visual Studio 2022 ou VS Code
- Workload MAUI installé

### Compilation
```bash
dotnet build
```

### Exécution
```bash
# Windows
dotnet run --framework net9.0-windows10.0.19041.0

# Android
dotnet run --framework net9.0-android

# iOS
dotnet run --framework net9.0-ios
```

## 📋 Utilisation

1. **Première utilisation** : L'application démarre sur la page de connexion
2. **Créer un compte** : Utilisez la page d'inscription
3. **Se connecter** : Utilisez vos identifiants
4. **Navigation** : Utilisez les onglets en bas pour naviguer
5. **Gestion** : Créez et gérez vos rendez-vous via le calendrier

## 🎨 Personnalisation

### Couleurs
Les couleurs principales sont définies dans les pages XAML :
- **Primaire** : #007ACC (bleu)
- **Secondaire** : #17A2B8 (cyan)
- **Succès** : #28A745 (vert)
- **Danger** : #DC3545 (rouge)

### Ajout de fonctionnalités
- **Nouvelles pages** : Ajoutez dans le dossier `Pages/`
- **Nouveaux services** : Ajoutez dans le dossier `Services/`
- **Nouveaux modèles** : Ajoutez dans le dossier `Models/`

## 🔮 Fonctionnalités à Implémenter

- Création/édition de rendez-vous
- Gestion avancée des utilisateurs
- Notifications push
- Synchronisation cloud
- Mode sombre
- Export/import de données

## 📁 Structure du Projet

```
MauiTemplate/
├── Models/              # Modèles de données
├── Data/               # Contexte de base de données
├── Services/           # Services métier
├── Pages/              # Pages de l'application
├── Resources/          # Ressources (images, polices)
└── Platforms/          # Code spécifique aux plateformes
```

## 🐛 Résolution de Problèmes

### Problèmes de Compilation
- Vérifiez que le workload MAUI est installé : `dotnet workload list`
- Nettoyez le projet : `dotnet clean`
- Recompilez : `dotnet build`

### Problèmes d'Exécution
- Vérifiez les logs dans la console de débogage
- Assurez-vous que la base de données est initialisée
- Vérifiez les permissions sur les plateformes mobiles

## 📄 Licence

Ce template est fourni à des fins éducatives et de développement.

---

**Template MAUI Cellulaire** - Version 1.0.0
Développé avec .NET MAUI et Entity Framework Core
