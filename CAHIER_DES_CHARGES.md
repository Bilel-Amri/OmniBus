# 📋 CAHIER DES CHARGES
## Plateforme de Réservation et de Suivi de Bus - OmniBus

---

## 1. PRÉSENTATION DU PROJET

### 1.1 Contexte
OmniBus est une plateforme numérique complète dédiée à la gestion et à la réservation de services de transport par bus en Tunisie. Le système vise à moderniser le transport public tunisien en offrant une solution intégrée couvrant la réservation de billets, le suivi en temps réel des bus, et la gestion administrative de la flotte.

### 1.2 Objectifs
- **Digitaliser** le processus de réservation de billets de bus
- **Offrir** un suivi en temps réel de la position des bus
- **Faciliter** la gestion administrative de la flotte et des itinéraires
- **Améliorer** l'expérience utilisateur des passagers tunisiens
- **Intégrer** une assistance client par intelligence artificielle (IA)

### 1.3 Périmètre Géographique
- **Zone de couverture** : Ensemble du territoire tunisien
- **Types de lignes** : 
  - Lignes urbaines (City)
  - Lignes interurbaines (Intercity)
  - Lignes express
  - Navettes (Shuttle)

---

## 2. ANALYSE DES BESOINS

### 2.1 Acteurs du Système

| Acteur | Description | Rôle |
|--------|-------------|------|
| **Passager** | Utilisateur final qui réserve et voyage | Client principal |
| **Conducteur** | Chauffeur de bus | Opérateur terrain |
| **Administrateur** | Gestionnaire du système | Supervision et gestion |
| **Système IA** | Assistant virtuel | Support client automatisé |

### 2.2 Besoins Fonctionnels

#### 2.2.1 Module Passager

| Réf. | Fonctionnalité | Priorité | Description |
|------|---------------|----------|-------------|
| P-01 | Inscription/Connexion | Haute | Création de compte avec email et mot de passe sécurisé |
| P-02 | Recherche d'itinéraires | Haute | Recherche par origine, destination et date |
| P-03 | Sélection de siège | Haute | Interface graphique de sélection de siège avec disponibilité temps réel |
| P-04 | Réservation de billet | Haute | Processus de réservation avec verrouillage temporaire du siège |
| P-05 | Paiement en ligne | Haute | Intégration passerelle de paiement (TND) |
| P-06 | Historique des voyages | Moyenne | Liste des réservations passées et à venir |
| P-07 | Suivi en temps réel | Haute | Affichage GPS du bus sur carte |
| P-08 | QR Code billet | Haute | Génération de QR code pour validation |
| P-09 | Annulation de billet | Moyenne | Possibilité d'annuler avec conditions |
| P-10 | Notifications | Moyenne | Alertes retards, rappels de voyage |

#### 2.2.2 Module Conducteur

| Réf. | Fonctionnalité | Priorité | Description |
|------|---------------|----------|-------------|
| C-01 | Consultation trajets | Haute | Voir les trajets assignés du jour |
| C-02 | Mise à jour position | Haute | Envoi automatique/manuel de la position GPS |
| C-03 | Démarrage trajet | Haute | Signaler le départ du bus |
| C-04 | Fin de trajet | Haute | Signaler l'arrivée à destination |
| C-05 | Validation passager | Moyenne | Scanner QR code pour confirmer l'embarquement |
| C-06 | Signalement retard | Moyenne | Notifier les retards avec raison |

#### 2.2.3 Module Administrateur

| Réf. | Fonctionnalité | Priorité | Description |
|------|---------------|----------|-------------|
| A-01 | Gestion des bus | Haute | CRUD (Créer, Lire, Modifier, Supprimer) des bus |
| A-02 | Gestion des itinéraires | Haute | CRUD des routes avec arrêts intermédiaires |
| A-03 | Gestion des horaires | Haute | Création et modification des plannings |
| A-04 | Gestion utilisateurs | Haute | Administration des comptes (passagers, conducteurs) |
| A-05 | Tableau de bord | Haute | Vue d'ensemble avec métriques clés |
| A-06 | Carte temps réel | Haute | Visualisation de tous les bus actifs |
| A-07 | Rapports statistiques | Moyenne | Revenus, taux d'occupation, performance |
| A-08 | Gestion des paiements | Moyenne | Suivi des transactions, remboursements |
| A-09 | Gestion des réclamations | Basse | Traitement des plaintes clients |

#### 2.2.4 Module IA (Assistant Virtuel)

| Réf. | Fonctionnalité | Priorité | Description |
|------|---------------|----------|-------------|
| IA-01 | Chatbot support | Moyenne | Réponses automatiques aux questions fréquentes |
| IA-02 | Recommandations | Basse | Suggestions de routes personnalisées |
| IA-03 | FAQ intelligente | Moyenne | Réponses contextuelles aux questions |
| IA-04 | Analyse comportement | Basse | Insights sur les habitudes de voyage |

---

## 3. SPÉCIFICATIONS TECHNIQUES

### 3.1 Architecture Logicielle

```
┌─────────────────────────────────────────────────────────────────┐
│                      COUCHE PRÉSENTATION                        │
│  ┌──────────────────────┐  ┌──────────────────────────────────┐ │
│  │   Application Web    │  │       Application Mobile         │ │
│  │   (React + TypeScript)│  │        (Future)                 │ │
│  └──────────────────────┘  └──────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                        COUCHE API                               │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │            ASP.NET Core 8 Web API                        │   │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────────────┐ │   │
│  │  │ REST API    │ │ SignalR Hub │ │ Swagger/OpenAPI    │ │   │
│  │  │ Controllers │ │ (Temps réel)│ │ Documentation      │ │   │
│  │  └─────────────┘ └─────────────┘ └─────────────────────┘ │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    COUCHE APPLICATION                           │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │   Services Métier │ DTOs │ Interfaces │ Validation       │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                     COUCHE INFRASTRUCTURE                       │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  Entity Framework Core │ Repositories │ Services Externes │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                      COUCHE DOMAINE                             │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │        Entités │ Énumérations │ Règles Métier            │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                     BASE DE DONNÉES                             │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │                    PostgreSQL 14+                        │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 Stack Technologique

#### Backend
| Composant | Technologie | Version |
|-----------|-------------|---------|
| Framework | ASP.NET Core | 8.0 |
| ORM | Entity Framework Core | 8.0 |
| Base de données | PostgreSQL | 14+ |
| Authentification | JWT Bearer Token | - |
| Temps réel | SignalR | 8.0 |
| Documentation API | Swagger/OpenAPI | 3.0 |
| Hachage mot de passe | BCrypt | - |
| QR Code | QRCoder | - |

#### Frontend
| Composant | Technologie | Version |
|-----------|-------------|---------|
| Framework | React | 18.x |
| Langage | TypeScript | 5.x |
| UI Components | Material-UI (MUI) | 5.x |
| Gestion d'état | React Query + Context | - |
| Cartographie | Leaflet (OpenStreetMap) | - |
| Client temps réel | @microsoft/signalr | - |
| Bundler | Vite | 5.x |
| Requêtes HTTP | Axios | - |

#### Intelligence Artificielle
| Composant | Technologie | Description |
|-----------|-------------|-------------|
| Plateforme IA | Dify | Chatbot et workflows IA |
| Modèles LLM | GPT-4 / Claude / Llama | Au choix |

---

## 4. MODÈLE DE DONNÉES

### 4.1 Schéma Entité-Relation

```
┌──────────────────┐       ┌──────────────────┐       ┌──────────────────┐
│      USER        │       │      TICKET      │       │     SCHEDULE     │
├──────────────────┤       ├──────────────────┤       ├──────────────────┤
│ Id (PK)          │──┐    │ Id (PK)          │    ┌──│ Id (PK)          │
│ FirstName        │  │    │ UserId (FK)      │────┘  │ BusId (FK)       │──┐
│ LastName         │  │    │ ScheduleId (FK)  │───────│ RouteId (FK)     │──┼─┐
│ Email (unique)   │  │    │ SeatNumber       │       │ DriverId (FK)    │──┼─┼──┐
│ PasswordHash     │  └────│ Status           │       │ DepartureTime    │  │ │  │
│ PhoneNumber      │       │ Price            │       │ ArrivalTime      │  │ │  │
│ Role             │       │ BookingReference │       │ Status           │  │ │  │
│ EmailVerified    │       │ BookingDate      │       │ BasePrice        │  │ │  │
│ ProfilePictureUrl│       │ PassengerName    │       │ AvailableSeats   │  │ │  │
│ NationalId       │       │ PassengerPhone   │       │ CurrentLatitude  │  │ │  │
│ AssignedBusId    │       │ QrCode           │       │ CurrentLongitude │  │ │  │
│ CreatedAt        │       │ CancellationReason│      │ IsRecurring      │  │ │  │
│ UpdatedAt        │       │ CancelledAt      │       │ DelayReason      │  │ │  │
└──────────────────┘       │ PaymentId (FK)   │───┐   └──────────────────┘  │ │  │
                           │ CreatedAt        │   │                         │ │  │
                           │ UpdatedAt        │   │   ┌──────────────────┐  │ │  │
                           └──────────────────┘   │   │       BUS        │  │ │  │
                                                  │   ├──────────────────┤  │ │  │
┌──────────────────┐                              │   │ Id (PK)          │──┘ │  │
│     PAYMENT      │                              │   │ PlateNumber      │    │  │
├──────────────────┤                              │   │ RegistrationNumber│   │  │
│ Id (PK)          │──────────────────────────────┘   │ Brand            │    │  │
│ TicketId (FK)    │                                  │ Model            │    │  │
│ UserId (FK)      │                                  │ YearManufactured │    │  │
│ Amount           │                                  │ Capacity         │    │  │
│ Currency         │                                  │ AvailableSeats   │    │  │
│ Status           │                                  │ Type             │    │  │
│ PaymentMethod    │                                  │ Status           │    │  │
│ TransactionId    │                                  │ HasAirConditioning│   │  │
│ GatewayResponse  │                                  │ HasWifi          │    │  │
│ CreatedAt        │                                  │ SeatsPerRow      │    │  │
│ PaidAt           │                                  │ CurrentLatitude  │    │  │
│ RefundedAt       │                                  │ CurrentLongitude │    │  │
└──────────────────┘                                  │ CurrentDriverId  │    │  │
                                                      └──────────────────┘    │  │
┌──────────────────┐       ┌──────────────────┐                               │  │
│      ROUTE       │       │    ROUTE_STOP    │                               │  │
├──────────────────┤       ├──────────────────┤                               │  │
│ Id (PK)          │───────│ Id (PK)          │───────────────────────────────┘  │
│ Name             │       │ RouteId (FK)     │                                  │
│ Origin           │       │ Name             │                                  │
│ OriginCode       │       │ Latitude         │       ┌──────────────────┐       │
│ Destination      │       │ Longitude        │       │    SEAT_LOCK     │       │
│ DestinationCode  │       │ Order            │       ├──────────────────┤       │
│ DistanceKm       │       │ DistanceFromOrigin│      │ Id (PK)          │       │
│ EstimatedDuration│       │ StopDurationMinutes│     │ ScheduleId (FK)  │       │
│ Description      │       │ CreatedAt        │       │ UserId (FK)      │───────┘
│ IsActive         │       │ UpdatedAt        │       │ SeatNumber       │
│ RouteStopsJson   │       └──────────────────┘       │ LockedAt         │
│ CreatedAt        │                                  │ ExpiresAt        │
│ UpdatedAt        │                                  │ Status           │
└──────────────────┘                                  │ SessionId        │
                                                      └──────────────────┘
```

### 4.2 Dictionnaire de Données

#### Table USER (Utilisateurs)
| Champ | Type | Contraintes | Description |
|-------|------|-------------|-------------|
| Id | UUID | PK | Identifiant unique |
| FirstName | VARCHAR(100) | NOT NULL | Prénom |
| LastName | VARCHAR(100) | NOT NULL | Nom de famille |
| Email | VARCHAR(255) | UNIQUE, NOT NULL | Adresse email |
| PasswordHash | VARCHAR(255) | NOT NULL | Mot de passe haché (BCrypt) |
| PhoneNumber | VARCHAR(20) | NULLABLE | Numéro de téléphone |
| Role | ENUM | NOT NULL | Passenger(0), Driver(1), Admin(2) |
| EmailVerified | BOOLEAN | DEFAULT FALSE | Email vérifié |
| NationalId | VARCHAR(20) | NULLABLE | Numéro CIN |
| AssignedBusId | UUID | FK NULLABLE | Bus assigné (conducteur) |
| CreatedAt | TIMESTAMP | NOT NULL | Date de création |
| UpdatedAt | TIMESTAMP | NOT NULL | Date de modification |
| IsDeleted | BOOLEAN | DEFAULT FALSE | Suppression logique |

#### Table BUS (Autobus)
| Champ | Type | Contraintes | Description |
|-------|------|-------------|-------------|
| Id | UUID | PK | Identifiant unique |
| PlateNumber | VARCHAR(20) | UNIQUE, NOT NULL | Immatriculation |
| RegistrationNumber | VARCHAR(50) | NOT NULL | Numéro d'enregistrement |
| Brand | VARCHAR(50) | NULLABLE | Marque du bus |
| Model | VARCHAR(50) | NULLABLE | Modèle |
| YearManufactured | INT | NULLABLE | Année de fabrication |
| Capacity | INT | NOT NULL | Capacité totale |
| AvailableSeats | INT | NOT NULL | Places disponibles |
| Type | ENUM | NOT NULL | City(0), Intercity(1), Express(2), Shuttle(3) |
| Status | ENUM | NOT NULL | Active(0), Maintenance(1), OutOfService(2), Retired(3) |
| HasAirConditioning | BOOLEAN | DEFAULT FALSE | Climatisation |
| HasWifi | BOOLEAN | DEFAULT FALSE | WiFi disponible |
| SeatsPerRow | INT | DEFAULT 4 | Sièges par rangée |
| CurrentLatitude | DECIMAL(10,8) | NULLABLE | Position GPS latitude |
| CurrentLongitude | DECIMAL(11,8) | NULLABLE | Position GPS longitude |
| CurrentDriverId | UUID | FK NULLABLE | Conducteur actuel |

#### Table ROUTE (Itinéraires)
| Champ | Type | Contraintes | Description |
|-------|------|-------------|-------------|
| Id | UUID | PK | Identifiant unique |
| Name | VARCHAR(100) | NOT NULL | Nom de la ligne |
| Origin | VARCHAR(100) | NOT NULL | Ville de départ |
| OriginCode | VARCHAR(10) | NULLABLE | Code gare départ |
| Destination | VARCHAR(100) | NOT NULL | Ville d'arrivée |
| DestinationCode | VARCHAR(10) | NULLABLE | Code gare arrivée |
| DistanceKm | DECIMAL(8,2) | NOT NULL | Distance en km |
| EstimatedDurationMinutes | INT | NOT NULL | Durée estimée (minutes) |
| Description | TEXT | NULLABLE | Description |
| IsActive | BOOLEAN | DEFAULT TRUE | Route active |
| RouteStopsJson | TEXT | NULLABLE | Arrêts intermédiaires (JSON) |

#### Table SCHEDULE (Horaires)
| Champ | Type | Contraintes | Description |
|-------|------|-------------|-------------|
| Id | UUID | PK | Identifiant unique |
| BusId | UUID | FK NOT NULL | Bus assigné |
| RouteId | UUID | FK NOT NULL | Itinéraire |
| DriverId | UUID | FK NULLABLE | Conducteur |
| DepartureTime | TIMESTAMP | NOT NULL | Heure de départ prévue |
| ArrivalTime | TIMESTAMP | NOT NULL | Heure d'arrivée prévue |
| ActualDepartureTime | TIMESTAMP | NULLABLE | Heure de départ réelle |
| ActualArrivalTime | TIMESTAMP | NULLABLE | Heure d'arrivée réelle |
| Status | ENUM | NOT NULL | Scheduled(0), InProgress(1), Completed(2), Cancelled(3), Delayed(4) |
| BasePrice | DECIMAL(10,3) | NOT NULL | Prix de base (TND) |
| AvailableSeats | INT | NOT NULL | Places disponibles |
| CurrentLatitude | DECIMAL(10,8) | NULLABLE | Position actuelle latitude |
| CurrentLongitude | DECIMAL(11,8) | NULLABLE | Position actuelle longitude |
| IsRecurring | BOOLEAN | DEFAULT FALSE | Horaire récurrent |
| OperatingDaysJson | TEXT | NULLABLE | Jours d'opération (JSON) |
| DelayReason | VARCHAR(255) | NULLABLE | Raison du retard |

#### Table TICKET (Billets)
| Champ | Type | Contraintes | Description |
|-------|------|-------------|-------------|
| Id | UUID | PK | Identifiant unique |
| UserId | UUID | FK NOT NULL | Passager |
| ScheduleId | UUID | FK NOT NULL | Horaire réservé |
| SeatNumber | INT | NOT NULL | Numéro de siège |
| Status | ENUM | NOT NULL | Reserved(0), Booked(1), Completed(2), Cancelled(3), Expired(4) |
| Price | DECIMAL(10,3) | NOT NULL | Prix payé (TND) |
| BookingReference | VARCHAR(20) | UNIQUE, NOT NULL | Référence de réservation |
| BookingDate | TIMESTAMP | NOT NULL | Date de réservation |
| PassengerName | VARCHAR(100) | NULLABLE | Nom du passager |
| PassengerPhone | VARCHAR(20) | NULLABLE | Téléphone passager |
| QrCode | TEXT | NULLABLE | QR Code encodé (Base64) |
| CancellationReason | TEXT | NULLABLE | Raison d'annulation |
| CancelledAt | TIMESTAMP | NULLABLE | Date d'annulation |
| BoardedBy | UUID | FK NULLABLE | Validé par (conducteur) |
| BoardingTime | TIMESTAMP | NULLABLE | Heure d'embarquement |
| PaymentId | UUID | FK NULLABLE | Paiement associé |

#### Table PAYMENT (Paiements)
| Champ | Type | Contraintes | Description |
|-------|------|-------------|-------------|
| Id | UUID | PK | Identifiant unique |
| TicketId | UUID | FK NOT NULL | Billet associé |
| UserId | UUID | FK NOT NULL | Utilisateur |
| Amount | DECIMAL(10,3) | NOT NULL | Montant (TND) |
| Currency | VARCHAR(3) | DEFAULT 'TND' | Devise |
| Status | ENUM | NOT NULL | Pending(0), Completed(1), Failed(2), Refunded(3), Cancelled(4) |
| PaymentMethod | VARCHAR(50) | NULLABLE | Méthode (carte, mobile, etc.) |
| TransactionId | VARCHAR(100) | NULLABLE | ID transaction externe |
| GatewayResponse | TEXT | NULLABLE | Réponse passerelle (JSON) |
| PaidAt | TIMESTAMP | NULLABLE | Date de paiement |
| RefundedAt | TIMESTAMP | NULLABLE | Date de remboursement |

#### Table SEAT_LOCK (Verrouillage de sièges)
| Champ | Type | Contraintes | Description |
|-------|------|-------------|-------------|
| Id | UUID | PK | Identifiant unique |
| ScheduleId | UUID | FK NOT NULL | Horaire |
| UserId | UUID | FK NOT NULL | Utilisateur |
| SeatNumber | INT | NOT NULL | Numéro de siège |
| LockedAt | TIMESTAMP | NOT NULL | Date de verrouillage |
| ExpiresAt | TIMESTAMP | NOT NULL | Date d'expiration |
| Status | ENUM | NOT NULL | Available(0), Locked(1), Booked(2) |
| SessionId | VARCHAR(100) | NOT NULL | ID de session |

---

## 5. SPÉCIFICATIONS DES INTERFACES

### 5.1 Interface Web Passager

#### 5.1.1 Pages Principales

| Page | URL | Description |
|------|-----|-------------|
| Accueil | `/` | Page d'accueil avec recherche rapide |
| Connexion | `/login` | Formulaire de connexion |
| Inscription | `/register` | Formulaire d'inscription |
| Recherche | `/search` | Résultats de recherche d'itinéraires |
| Réservation | `/booking/:scheduleId` | Sélection siège et paiement |
| Mes Billets | `/my-tickets` | Liste des réservations |
| Tableau de bord | `/dashboard` | Vue d'ensemble utilisateur |
| Carte Live | `/map` | Suivi temps réel des bus |

#### 5.1.2 Maquettes Fonctionnelles

**Page d'Accueil**
```
┌────────────────────────────────────────────────────────────────┐
│  🚌 OmniBus                    [Connexion] [Inscription]       │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│           Réservez votre trajet en Tunisie                    │
│                                                                │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │  De: [____________]  À: [____________]  📅 [__/__/____] │  │
│  │                      [🔍 Rechercher]                     │  │
│  └─────────────────────────────────────────────────────────┘  │
│                                                                │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐           │
│  │ 🚌 66+     │  │ 🏙️ 24      │  │ ⭐ 4.8     │           │
│  │ Lignes    │  │ Villes     │  │ Note       │           │
│  └─────────────┘  └─────────────┘  └─────────────┘           │
│                                                                │
│  Lignes Populaires:                                           │
│  [Tunis → Sousse]  [Sfax → Gabès]  [Tunis → Bizerte]        │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

**Page de Réservation**
```
┌────────────────────────────────────────────────────────────────┐
│  🚌 OmniBus                    👤 Bilel Amri  [Déconnexion]   │
├────────────────────────────────────────────────────────────────┤
│  Tunis → Sousse  |  Départ: 08:00  |  Arrivée: 10:30         │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  Sélectionnez votre siège:                                    │
│                                                                │
│         [CONDUCTEUR]                                          │
│    ┌───────────────────────────────┐                          │
│    │  [1]  [2]  │ 🚶 │  [3]  [4] │ Rangée 1                 │
│    │  [5]  [6]  │    │  [7]  [8] │ Rangée 2                 │
│    │  [9]  [10] │    │  [11] [12]│ Rangée 3                 │
│    │  [13] [14] │    │  [15] [16]│ Rangée 4                 │
│    │  [17] [18] │    │  [19] [20]│ Rangée 5                 │
│    └───────────────────────────────┘                          │
│                                                                │
│    ✅ Disponible  ❌ Réservé  🔵 Sélectionné                 │
│                                                                │
│  Siège sélectionné: 7                                         │
│  Prix: 15.500 TND                                             │
│                                                                │
│  [Continuer vers le paiement →]                               │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

### 5.2 Interface Conducteur

| Page | URL | Description |
|------|-----|-------------|
| Mes Trajets | `/driver` | Liste des trajets assignés |
| Trajet actif | `/driver/active` | Détails du trajet en cours |
| Validation | `/driver/validate` | Scanner QR code passager |

### 5.3 Interface Administrateur

| Page | URL | Description |
|------|-----|-------------|
| Dashboard | `/admin` | Tableau de bord administrateur |
| Gestion Bus | `/admin/buses` | CRUD des bus |
| Gestion Routes | `/admin/routes` | CRUD des itinéraires |
| Gestion Horaires | `/admin/schedules` | CRUD des plannings |
| Carte Live | `/admin/map` | Suivi de tous les bus |
| Utilisateurs | `/admin/users` | Gestion des utilisateurs |
| Rapports | `/admin/reports` | Statistiques et rapports |

---

## 6. SPÉCIFICATIONS API REST

### 6.1 Authentification

Toutes les requêtes API (sauf login/register) nécessitent un token JWT dans le header:
```
Authorization: Bearer <token>
```

### 6.2 Endpoints Principaux

#### 6.2.1 Authentification (`/api/auth`)

| Méthode | Endpoint | Description | Authentification |
|---------|----------|-------------|------------------|
| POST | `/register` | Inscription | Non |
| POST | `/login` | Connexion | Non |
| GET | `/me` | Profil utilisateur | Oui |
| PUT | `/profile` | Mise à jour profil | Oui |
| POST | `/change-password` | Changer mot de passe | Oui |
| POST | `/forgot-password` | Mot de passe oublié | Non |
| POST | `/reset-password` | Réinitialiser mot de passe | Non |

#### 6.2.2 Routes (`/api/routes`)

| Méthode | Endpoint | Description | Rôle |
|---------|----------|-------------|------|
| GET | `/` | Liste toutes les routes | Public |
| GET | `/{id}` | Détails d'une route | Public |
| GET | `/search?origin=X&dest=Y` | Recherche de routes | Public |
| GET | `/active` | Routes actives uniquement | Public |
| POST | `/` | Créer une route | Admin |
| PUT | `/{id}` | Modifier une route | Admin |
| DELETE | `/{id}` | Supprimer une route | Admin |

#### 6.2.3 Horaires (`/api/schedules`)

| Méthode | Endpoint | Description | Rôle |
|---------|----------|-------------|------|
| GET | `/` | Liste tous les horaires | Public |
| GET | `/{id}` | Détails d'un horaire | Public |
| GET | `/search` | Recherche d'horaires | Public |
| GET | `/{id}/seats` | Disponibilité des sièges | Public |
| GET | `/active` | Horaires actifs avec position | Public |
| POST | `/` | Créer un horaire | Admin |
| PUT | `/{id}` | Modifier un horaire | Admin |
| PUT | `/{id}/location` | Mettre à jour position | Conducteur |
| DELETE | `/{id}` | Supprimer un horaire | Admin |

#### 6.2.4 Billets (`/api/tickets`)

| Méthode | Endpoint | Description | Rôle |
|---------|----------|-------------|------|
| GET | `/my-tickets` | Mes billets | Passager |
| GET | `/upcoming` | Billets à venir | Passager |
| GET | `/history` | Historique | Passager |
| POST | `/lock-seat` | Verrouiller un siège | Passager |
| POST | `/unlock-seat` | Déverrouiller un siège | Passager |
| POST | `/book` | Réserver un billet | Passager |
| POST | `/{id}/cancel` | Annuler un billet | Passager |
| GET | `/stats` | Statistiques billets | Admin |
| GET | `/validate/{qrCode}` | Valider un QR code | Conducteur |

#### 6.2.5 Conducteur (`/api/driver`)

| Méthode | Endpoint | Description | Rôle |
|---------|----------|-------------|------|
| GET | `/trips` | Trajets assignés | Conducteur |
| GET | `/today` | Trajets du jour | Conducteur |
| PUT | `/location` | Mise à jour position GPS | Conducteur |
| POST | `/confirm-boarding` | Confirmer embarquement | Conducteur |
| PUT | `/schedule/{id}/start` | Démarrer trajet | Conducteur |
| PUT | `/schedule/{id}/complete` | Terminer trajet | Conducteur |
| PUT | `/schedule/{id}/delay` | Signaler retard | Conducteur |

#### 6.2.6 Paiements (`/api/payments`)

| Méthode | Endpoint | Description | Rôle |
|---------|----------|-------------|------|
| POST | `/process` | Traiter un paiement | Passager |
| GET | `/my-payments` | Mes paiements | Passager |
| GET | `/{id}` | Détails d'un paiement | Passager/Admin |
| GET | `/stats` | Statistiques paiements | Admin |
| POST | `/{id}/refund` | Rembourser | Admin |

#### 6.2.7 Assistant IA (`/api/aiassistant`)

| Méthode | Endpoint | Description | Rôle |
|---------|----------|-------------|------|
| POST | `/chat` | Envoyer message au chatbot | Passager |
| POST | `/recommendations` | Obtenir recommandations | Passager |
| GET | `/faq/{question}` | Obtenir réponse FAQ | Public |
| GET | `/insights` | Analyses de voyage | Passager |

### 6.3 Format des Réponses

**Succès (200/201)**
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation successful"
}
```

**Erreur (4xx/5xx)**
```json
{
  "success": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Description de l'erreur"
  }
}
```

---

## 7. COMMUNICATION TEMPS RÉEL (SignalR)

### 7.1 Hub de Suivi (`/hubs/tracking`)

#### Méthodes Client → Serveur
| Méthode | Paramètres | Description |
|---------|------------|-------------|
| `JoinRoute` | routeId | S'abonner aux mises à jour d'une route |
| `LeaveRoute` | routeId | Se désabonner d'une route |
| `JoinSchedule` | scheduleId | S'abonner aux mises à jour d'un horaire |
| `LeaveSchedule` | scheduleId | Se désabonner d'un horaire |
| `JoinAdmin` | - | S'abonner à toutes les mises à jour (admin) |

#### Événements Serveur → Client
| Événement | Données | Description |
|-----------|---------|-------------|
| `BusLocationUpdated` | {scheduleId, latitude, longitude, timestamp} | Position du bus mise à jour |
| `ScheduleStatusChanged` | {scheduleId, status, reason} | Statut du trajet modifié |
| `DelayNotification` | {scheduleId, delayMinutes, reason} | Notification de retard |

### 7.2 Hub de Réservation (`/hubs/booking`)

#### Méthodes Client → Serveur
| Méthode | Paramètres | Description |
|---------|------------|-------------|
| `JoinSchedule` | scheduleId | S'abonner aux mises à jour de sièges |
| `LeaveSchedule` | scheduleId | Se désabonner |

#### Événements Serveur → Client
| Événement | Données | Description |
|-----------|---------|-------------|
| `SeatLocked` | {scheduleId, seatNumber, userId} | Siège verrouillé |
| `SeatUnlocked` | {scheduleId, seatNumber} | Siège déverrouillé |
| `SeatBooked` | {scheduleId, seatNumber} | Siège réservé définitivement |

---

## 8. SÉCURITÉ

### 8.1 Authentification et Autorisation

| Aspect | Implémentation |
|--------|----------------|
| Authentification | JWT Bearer Tokens |
| Durée de validité | 7 jours |
| Algorithme de signature | HS256 |
| Hachage mot de passe | BCrypt (work factor 12) |
| Rôles | Passenger, Driver, Admin |
| Gestion des sessions | Stateless (JWT) |

### 8.2 Protection des Données

| Mesure | Description |
|--------|-------------|
| HTTPS | Toutes les communications chiffrées |
| CORS | Origines autorisées configurées |
| Validation des entrées | Validation côté serveur de toutes les données |
| Injection SQL | Prévention via Entity Framework (paramétrage) |
| XSS | Échappement automatique des sorties |
| CSRF | Token CSRF pour les formulaires (si applicable) |

### 8.3 Conformité RGPD

| Exigence | Implémentation |
|----------|----------------|
| Consentement | Case à cocher lors de l'inscription |
| Droit d'accès | Endpoint `/api/auth/my-data` |
| Droit à l'oubli | Endpoint `/api/auth/delete-account` |
| Portabilité | Export des données en JSON |
| Suppression logique | Champ `IsDeleted` sur toutes les entités |

---

## 9. PERFORMANCE ET SCALABILITÉ

### 9.1 Objectifs de Performance

| Métrique | Objectif |
|----------|----------|
| Temps de réponse API | < 200ms (95ème percentile) |
| Temps de chargement page | < 3 secondes |
| Uptime | 99.5% |
| Connexions SignalR simultanées | 10,000+ |
| Transactions par seconde | 100+ |

### 9.2 Stratégies d'Optimisation

| Technique | Description |
|-----------|-------------|
| Mise en cache | Redis pour les données fréquemment accédées |
| Pagination | Toutes les listes paginées (défaut: 20 éléments) |
| Compression | Gzip pour les réponses API |
| Lazy loading | Chargement différé des entités liées |
| Index BDD | Index sur les colonnes fréquemment recherchées |
| Connection pooling | Pool de connexions PostgreSQL |

---

## 10. TESTS ET QUALITÉ

### 10.1 Stratégie de Tests

| Type de Test | Couverture Cible | Outils |
|--------------|------------------|--------|
| Tests unitaires | 80%+ | xUnit, Moq |
| Tests d'intégration | 60%+ | TestContainers |
| Tests E2E | Scénarios critiques | Playwright |
| Tests de charge | Endpoints critiques | k6, Artillery |

### 10.2 Scénarios de Test Critiques

1. **Inscription et connexion utilisateur**
2. **Recherche et sélection d'itinéraire**
3. **Verrouillage et réservation de siège**
4. **Processus de paiement complet**
5. **Annulation de billet et remboursement**
6. **Mise à jour position GPS (conducteur)**
7. **Réception mises à jour temps réel (SignalR)**
8. **Accès concurrent au même siège**

---

## 11. DÉPLOIEMENT

### 11.1 Environnements

| Environnement | Usage | URL |
|---------------|-------|-----|
| Développement | Tests développeurs | localhost |
| Staging | Tests QA | staging.omnibus.tn |
| Production | Utilisateurs finaux | www.omnibus.tn |

### 11.2 Infrastructure Recommandée

```
                    ┌─────────────────┐
                    │   Load Balancer │
                    │    (Nginx)      │
                    └────────┬────────┘
                             │
            ┌────────────────┼────────────────┐
            ▼                ▼                ▼
    ┌───────────────┐ ┌───────────────┐ ┌───────────────┐
    │   API Server  │ │   API Server  │ │   API Server  │
    │   (ASP.NET)   │ │   (ASP.NET)   │ │   (ASP.NET)   │
    └───────┬───────┘ └───────┬───────┘ └───────┬───────┘
            │                 │                 │
            └────────────────┬┴─────────────────┘
                             │
            ┌────────────────┼────────────────┐
            ▼                ▼                ▼
    ┌───────────────┐ ┌───────────────┐ ┌───────────────┐
    │  PostgreSQL   │ │     Redis     │ │  Dify AI      │
    │  (Primary)    │ │   (Cache)     │ │  (Chatbot)    │
    └───────────────┘ └───────────────┘ └───────────────┘
            │
            ▼
    ┌───────────────┐
    │  PostgreSQL   │
    │  (Replica)    │
    └───────────────┘
```

### 11.3 CI/CD Pipeline

```
┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
│   Push   │───▶│  Build   │───▶│  Test    │───▶│  Docker  │───▶│  Deploy  │
│  Git     │    │  .NET    │    │  xUnit   │    │  Build   │    │  K8s     │
└──────────┘    └──────────┘    └──────────┘    └──────────┘    └──────────┘
```

---

## 12. PLANNING PRÉVISIONNEL

### 12.1 Phases du Projet

| Phase | Durée | Livrables |
|-------|-------|-----------|
| **Phase 1: Fondations** | 4 semaines | Architecture, BDD, Auth, CRUD de base |
| **Phase 2: Fonctionnalités Core** | 6 semaines | Recherche, Réservation, Paiement |
| **Phase 3: Temps Réel** | 3 semaines | SignalR, Suivi GPS, Notifications |
| **Phase 4: Interface Utilisateur** | 4 semaines | Frontend React complet |
| **Phase 5: Module Conducteur** | 2 semaines | Interface et fonctionnalités conducteur |
| **Phase 6: Administration** | 3 semaines | Dashboard admin, Rapports |
| **Phase 7: IA & Optimisations** | 2 semaines | Chatbot Dify, Performance |
| **Phase 8: Tests & Déploiement** | 2 semaines | Tests complets, Mise en production |

**Durée totale estimée: 26 semaines (6-7 mois)**

### 12.2 Jalons (Milestones)

| Jalon | Date Cible | Critères d'Acceptation |
|-------|------------|------------------------|
| M1 - MVP Backend | Semaine 10 | API fonctionnelle avec auth et CRUD |
| M2 - Réservation Complète | Semaine 14 | Flux réservation + paiement OK |
| M3 - Temps Réel | Semaine 17 | SignalR fonctionnel |
| M4 - Frontend Complet | Semaine 21 | Toutes les pages passager |
| M5 - Version Beta | Semaine 24 | Tests utilisateurs |
| M6 - Production | Semaine 26 | Déploiement final |

---

## 13. BUDGET ESTIMATIF

### 13.1 Ressources Humaines

| Rôle | Nombre | Durée | Coût Estimé |
|------|--------|-------|-------------|
| Chef de Projet | 1 | 6 mois | 18,000 TND |
| Développeur Backend Senior | 1 | 6 mois | 24,000 TND |
| Développeur Frontend | 1 | 5 mois | 15,000 TND |
| Développeur Full Stack | 1 | 6 mois | 21,000 TND |
| Designer UI/UX | 1 | 2 mois | 6,000 TND |
| Testeur QA | 1 | 3 mois | 7,500 TND |
| **Sous-total RH** | | | **91,500 TND** |

### 13.2 Infrastructure (Annuel)

| Service | Coût Mensuel | Coût Annuel |
|---------|--------------|-------------|
| Serveurs Cloud (3x) | 450 TND | 5,400 TND |
| Base de données PostgreSQL | 200 TND | 2,400 TND |
| Redis Cache | 100 TND | 1,200 TND |
| Dify AI (Cloud) | 150 TND | 1,800 TND |
| Nom de domaine + SSL | - | 200 TND |
| CDN & Stockage | 100 TND | 1,200 TND |
| **Sous-total Infra** | | **12,200 TND** |

### 13.3 Licences et Services

| Service | Coût |
|---------|------|
| Passerelle de paiement (setup) | 500 TND |
| SMS Gateway (10,000 SMS) | 300 TND |
| Cartographie (OpenStreetMap) | Gratuit |
| **Sous-total Licences** | **800 TND** |

### 13.4 Budget Total

| Catégorie | Montant |
|-----------|---------|
| Ressources Humaines | 91,500 TND |
| Infrastructure (1ère année) | 12,200 TND |
| Licences et Services | 800 TND |
| Contingence (10%) | 10,450 TND |
| **TOTAL** | **114,950 TND** |

---

## 14. RISQUES ET MITIGATIONS

| Risque | Probabilité | Impact | Mitigation |
|--------|-------------|--------|------------|
| Retard de développement | Moyenne | Élevé | Méthodologie Agile, sprints courts |
| Problèmes de performance | Moyenne | Élevé | Tests de charge réguliers |
| Indisponibilité du service | Faible | Critique | Redondance, monitoring 24/7 |
| Faille de sécurité | Faible | Critique | Audit sécurité, mises à jour régulières |
| Adoption utilisateur faible | Moyenne | Élevé | Marketing, UX intuitive |
| Problèmes d'intégration paiement | Moyenne | Élevé | Tests sandbox extensifs |

---

## 15. ANNEXES

### 15.1 Glossaire

| Terme | Définition |
|-------|------------|
| **JWT** | JSON Web Token - standard pour l'authentification |
| **SignalR** | Bibliothèque pour la communication temps réel |
| **CRUD** | Create, Read, Update, Delete - opérations de base |
| **ORM** | Object-Relational Mapping - Entity Framework |
| **API REST** | Interface de programmation RESTful |
| **SPA** | Single Page Application |
| **TND** | Dinar Tunisien |

### 15.2 Routes SNTRI Intégrées

Le système inclut **66+ routes** couvrant les principales villes tunisiennes:
- Tunis, Sousse, Sfax, Gabès, Bizerte, Nabeul
- Kairouan, Monastir, Mahdia, Médenine
- Gafsa, Tozeur, Nefta, Tataouine
- Kasserine, Le Kef, Tabarka, Jendouba
- Djerba, Zarzis, Ben Guerdane
- Et bien d'autres...

### 15.3 Contacts

| Rôle | Contact |
|------|---------|
| Product Owner | À définir |
| Tech Lead | À définir |
| Support Technique | support@omnibus.tn |

---

**Document rédigé le**: 3 Janvier 2026  
**Version**: 1.0  
**Statut**: Approuvé pour développement

---

*Ce cahier des charges est un document vivant qui sera mis à jour au fur et à mesure de l'évolution du projet.*
