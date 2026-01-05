# Système Solaire

## Description

Ce projet Unity est une simulation interactive du système solaire. Il permet aux utilisateurs d'explorer les planètes, d'afficher des informations détaillées sur chacune d'elles, de participer à un quiz éducatif et d'expérimenter la réalité augmentée (AR) pour visualiser le système solaire dans le monde réel.

Le projet utilise le pipeline de rendu universel (URP) de Unity et intègre Vuforia pour les fonctionnalités AR.

## Fonctionnalités principales

- **Exploration des planètes** : Cliquez sur les planètes pour afficher des informations telles que le diamètre, la distance au Soleil, les périodes de révolution et de rotation.
- **Quiz éducatif** : Testez vos connaissances sur le système solaire avec un quiz intégré.
- **Réalité augmentée** : Utilisez votre appareil mobile pour placer le système solaire dans votre environnement réel.
- **Interface utilisateur** : Navigation intuitive avec menus et boutons pour basculer entre les modes.
- **Audio** : Effets sonores pour améliorer l'expérience utilisateur.
- **Animations** : Les planètes tournent sur elles-mêmes et orbitent autour du Soleil.

## Installation et exécution

### Prérequis

- Unity 2022.3 ou version supérieure (recommandé pour la compatibilité avec URP 17.0.4).
- Un appareil compatible AR pour les fonctionnalités AR (iOS ou Android avec support Vuforia).

### Étapes d'installation

1. Clonez ou téléchargez ce dépôt.
2. Ouvrez Unity Hub et ajoutez le projet en sélectionnant le dossier racine `Solar_system`.
3. Ouvrez le projet dans Unity.
4. Assurez-vous que les packages sont installés (ils devraient l'être automatiquement via le `manifest.json`).
5. Ouvrez la scène `MenuScene` depuis `Assets/Scenes/`.
6. Appuyez sur Play pour lancer l'application.

### Construction pour mobile

- Pour Android : Allez dans File > Build Settings, sélectionnez Android, configurez les paramètres et construisez.
- Pour iOS : Sélectionnez iOS dans Build Settings et construisez (nécessite un Mac avec Xcode).

## Structure du projet

- **Assets/** : Contient tous les assets du projet.
  - **Editor/** : Scripts et outils d'édition.
  - **Materials/** : Matériaux pour les objets 3D.
  - **Prefabs/** : Préfabriqués des planètes (Soleil, Mercure, Vénus, Terre, Mars, Jupiter, Saturne, Uranus, Neptune) et éléments du système solaire.
  - **Resources/** : Ressources chargées dynamiquement.
  - **Scenes/** : Scènes Unity (MenuScene, SampleScene).
  - **Scripts/** : Code source C#.
    - **AR/** : Scripts pour la réalité augmentée (rotation des planètes, orbites, informations des planètes).
    - **data/** : Données pour le quiz.
    - **Managers/** : Gestionnaires principaux (lancement, menu, quiz, son, etc.).
    - **UI/** : Gestion de l'interface utilisateur.
  - **Settings/** : Paramètres du projet.
  - **Sounds/** : Fichiers audio.
  - **TextMesh Pro/** : Assets pour le texte.
  - **TutorialInfo/** : Informations tutoriel.
  - **UIBacground/** : Arrière-plans UI.
- **Packages/** : Dépendances Unity (manifest.json, packages-lock.json).
- **ProjectSettings/** : Paramètres du projet Unity.

## Technologies utilisées

- **Unity** : Moteur de jeu principal.
- **Universal Render Pipeline (URP)** : Pipeline de rendu pour des graphismes optimisés.
- **Vuforia** : SDK pour la réalité augmentée.
- **Input System** : Gestion des entrées (souris, tactile).
- **TextMesh Pro** : Rendu de texte avancé.

## Comment contribuer

1. Forkez le projet.
2. Créez une branche pour votre fonctionnalité (`git checkout -b feature/nouvelle-fonction`).
3. Commitez vos changements (`git commit -am 'Ajout de nouvelle fonctionnalité'`).
4. Poussez vers la branche (`git push origin feature/nouvelle-fonction`).
5. Ouvrez une Pull Request.

## Licence

Ce projet est sous licence MIT. Voir le fichier LICENSE pour plus de détails.

## Crédits

- Développé avec Unity.
- Icônes et assets fournis par Unity et Vuforia.
- Inspiré par des projets éducatifs sur le système solaire.

## Support

Pour des questions ou des problèmes, ouvrez une issue sur GitHub ou contactez-nous.