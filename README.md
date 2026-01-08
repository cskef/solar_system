# 🌍 Solar System AR – Projet de Réalité Augmentée

[![Unity](https://img.shields.io/badge/Unity-2022.3+-blue)](https://unity.com/) [![License](https://img.shields.io/badge/License-Educational-green)](LICENSE) [![AR](https://img.shields.io/badge/AR-Vuforia-orange)](https://developer.vuforia.com/)

> **Application mobile immersive** pour explorer le système solaire en réalité augmentée. Découvrez les planètes, apprenez de manière interactive et testez vos connaissances !

---

## 📋 Table des matières

- [📌 Présentation du projet](#-présentation-du-projet)
- [🎯 Objectifs pédagogiques](#-objectifs-pédagogiques)
- [🧠 Fonctionnalités principales](#-fonctionnalités-principales)
- [🛠️ Technologies utilisées](#️-technologies-utilisées)
- [🧱 Architecture du projet](#-architecture-du-projet)
- [▶️ Utilisation de l’application](#️-utilisation-de-lapplication)
- [📱 Plateforme cible](#-plateforme-cible)
- [👥 Projet académique](#-projet-académique)
- [📌 Auteur(s)](#-auteurs)
- [📄 Licence](#-licence)

---

## 📌 Présentation du projet

**Solar System AR** est une application mobile en **réalité augmentée** développée avec **Unity**.
Elle permet de découvrir le système solaire de manière interactive : l’utilisateur peut visualiser les planètes en 3D, consulter des informations pédagogiques, écouter des explications audio et tester ses connaissances à travers un quiz.

Le projet a été conçu dans un cadre **académique**, avec un objectif à la fois **pédagogique** et **technologique**.

### 🎥 Démo rapide
*(Ajoutez ici une capture d'écran ou un GIF de l'app en action)*

---

## 🎯 Objectifs pédagogiques

* 🌟 Comprendre l’organisation du système solaire
* 🔄 Visualiser les trajectoires (orbites) des planètes autour du Soleil
* 📊 Associer des informations scientifiques à chaque planète
* 🧠 Tester les connaissances de l’utilisateur via un quiz interactif

---

## 🧠 Fonctionnalités principales

### 🧭 Menu principal

* **🚀 Explorer** : accéder à la scène de réalité augmentée
* **❓ Quiz** : lancer directement le quiz
* **❌ Quitter** l’application

---

### 🌌 Mode Explorer (Réalité augmentée)

* Affichage du système solaire en AR
* Placement du système solaire par **tap sur l’écran** 🖱️
* Planètes en rotation autour du Soleil 🌞
* Orbites visibles (activables / désactivables) 🔄
* Interaction par clic/touch sur une planète :

  * Affichage d’informations pédagogiques 📖
  * Lecture d’un audio explicatif 🔊
* Mise en pause automatique de la révolution lors de l’affichage des informations ⏸️

---

### 🪐 Informations par planète

Pour chaque planète :

| Aspect | Détail |
|--------|--------|
| **Nom** | Ex: Terre 🌍 |
| **Description** | Pédagogique et engageante |
| **Diamètre** | En km |
| **Distance au Soleil** | En millions de km |
| **Temps de révolution** | En jours terrestres |
| **Temps de rotation** | En heures |
| **Audio** | Explicatif 🎵 |

---

### ❓ Quiz

* Quiz accessible depuis le menu
* Questions à choix multiples
* Score final affiché 🏆
* Bouton **🔄 Rejouer** visible uniquement à la fin
* Bouton **🚪 Fermer** pour revenir au menu

---

## 🛠️ Technologies utilisées

* **🎮 Unity** : Moteur de jeu principal
* **💻 C#** : Langage de programmation
* **🔍 Réalité Augmentée (AR)** : Via Vuforia
* **📝 TextMeshPro** : Rendu de texte avancé
* **🔊 AudioSource** : Gestion audio immersive
* **📚 Git & GitHub** : Gestion de version collaborative

---

## 🧱 Architecture du projet

### 📂 Scripts principaux

| Script | Fonction |
|--------|----------|
| `OrbitAround` | Gestion de la révolution des planètes |
| `OrbitRingDrawer` | Affichage des orbites |
| `TapToPlaceSolarSystem` | Placement du système solaire par tap |
| `ClickRaycaster` | Détection des clics/touches sur les planètes |
| `PlanetInfo` | Données pédagogiques + audio |
| `UIManager` | Gestion de l’interface d’information |
| `QuizManager` | Gestion du quiz |
| `AudioManager` | Gestion des sons |
| `OrbitToggle` | Activation/désactivation des orbites |
| `LaunchState` | Gestion du mode Explorer / Quiz |

---

## ▶️ Utilisation de l’application

1. **📱 Lancer l’application**
2. **🎯 Choisir Explorer ou Quiz**
3. **🌌 En mode Explorer :**
   - Taper sur l’écran pour placer le système solaire
   - Toucher une planète pour afficher ses informations
4. **❓ En mode Quiz :**
   - Répondre aux questions
   - Consulter le score final
   - Rejouer ou revenir au menu

---

## 📱 Plateforme cible

* **🤖 Android (mobile)**
* **📐 Orientation : Paysage**

---

## 👥 Projet académique

Ce projet a été réalisé dans un cadre pédagogique afin de mettre en pratique :

* la programmation orientée objet en C#
* la gestion d’un projet Unity
* la réalité augmentée
* l’UX mobile
* le travail collaboratif avec GitHub

---

## 📌 Auteur(s)

* **👤 KENNE KEYANYEM Frank**
* **👤 MEZAGO Wilfried Aymar**
* **👤 TAMBA MBE Yohan**
* **🎓 Art & Intelligence Artificielle / ENSPY**
* **📅 2025/2026**

---

## 📄 Licence

Projet à usage **éducatif**. 📚

---

*Pour des questions ou contributions, ouvrez une issue sur GitHub !* 🚀