# Uplift

Uplift est un __gestionnaire de paquets__ pour le moteur de jeu [Unity](https://unity3d.com/), développé en __C#__, et propose un standard pour la distribution d'assets Unity3d, et facilite la gestion et l'utilisation d'assets externes au sein de projets Unity.

## Que permet-il de faire?

 Uplift a pour design principal de rendre l'utilisation de packages Unity facile et intuitive. Il permet d'utiliser aussi bien des fichiers .unitypackage téléchargés depuis l'AssetStore que des packages présents sur Github ou sur votre système. Uplift introduit les concepts de gestionnaire de paquets, définition de paquets et dépendences transitives entre autre à l'univers de Unity.

A l'heure actuelle, il est capable d'__installer__, de __mettre à jour__ et de __retirer__ des packages comme bon vous semble.

## Comment fonctionne-t-il?

Le fonctionnement de Uplift est relativement simple: il se base sur quelques fichiers qui définissent quels packages vous souhaitez utiliser.

Le fichier le plus important quand vous êtes un utilisateur de packages est le fichier __Upfile.xml__, situé à la racine de votre projet. A l'intérieur, vous pouvez spécifier chacun des packages que vous souhaitez utiliser ainsi que la source depuis laquelle vous voulez les obtenir, et vous pouvez ensuite utiliser la catégorie `Uplift` de l'éditeur Unity pour installer, mettre à jour ou encore retirer les packages spécifiés.

Les autres fichiers essentiels au bon fonctionnement de Uplift sont les fichiers __Upset.xml__ présents dans chacun des packages utilisés par Unity. Ces ichiers décrivent le contenu du package: son nom, sa version et le type de fichiers qu'il contient. Ce fichier ne vous concerne que peu si vous vous contentez d'utiliser des packages, mais c'est un rouage essentiel de Uplift.

## Comment l'utiliser?

Que vous souhaitiez créer des packages ou en utiliser, la première chose à faire et d'ajouter Uplift au projet Unity avec lequel vous voulez travailler. Au choix, vous pouvez télécharger le fichier .unitypackage présent à la racine de ce répertoire, ou bien _vous le procurer directement depuis l'[AssetStore](TODO/publish/to/the/assetstore) de Unity_.

### Je suis un utilisateur de package

La seule chose que vous allez avoir à faire est de spécifier un fichier __Upfile.xml__ à la racine de votre projet. Vous pouvez utiliser le menu `Upflit > Generate Upfile` pour générer un fichier type.

__NOTE__: Vous pouvez ajouter ou retirer des informations du Upfile à n'importe quel moment, assurez vous néanmoins d'aller dans `Uplift > Refresh Upfile` si vous le faites afin de prendre en compte vos modifications!

Une fois le fichier créé, il va vous falloir le remplir. La première étape va être d'indiquer depuis quelle(s) source(s) les packages seront obtenus, dans la section `Repositories` du fichier Upfile. Vous pouvez y indiquer une liste de répertoires depuis lesquels vous obtiendrez vos packages.

__Exemple:__ si vous voulez obtenir des paquets depuis un dossier spécifique de votre machine, vous pouvez ajouter la ligne suivante au Repositories:
```xml
<FileRepository Path="Chemin/Vers/Mon/Dossier" />
```
En remplaçant la valeur de l'attribut `Path` par le chemin voulu.

Après avoir renseigné les sources, vous pourrez éventuellement compléter la partie `Configuration` du fichier. Vous pouvez y spécifier l'endroit où vous souhaitez que Uplift installe les packages obtenus depuis les `Repertories`.

__Exemple:__ si vous voulez dépaquetter les Exemples des packages que vous utilisez dans un dossier 'Exemples', plutôt que dans les Assets de Unity, vous pouvez indiquer:
```xml
<ExamplesPath Location="Exemples" />
```

Vous pouvez ensuite ajoutez tous les paquets dont votre projet dépend à la section `Dependencies` du Upfile.

__Exemple:__ si vous voulez spécifier que votre projet dépend de la version `1.8` d'un package nommé `foo`, vous pouvez ajouter la ligne suivante au Dependencies:
```xml
<Package Name="foo" Version="1.8" />
```

Et voilà! Vous pouvez maintenant utiliser le menu `Uplift > Install Dependencies`, et Uplift va alors parcourir les packages de `Dependencies`, allez les chercher dans les répertoires de `Repositories` et les installer de manière conforme à votre `Configuration`.

### Je suis un créateur de package

Si vous avez l'habitude de créer des packages pour Unity, créer des packages pour Uplift sera tout aussi facile. Travaillez comme vous en avez l'habitude mais juste avant de finaliser votre package, vous devrez simplement créer un fichier __Upset.xml__ qui contiendra les informations relatives à votre package.

Le minimum d'information dont Uplift a besoin est:

* Le nom du package: sans surprise, c'est le nom de votre package.
```xml
<PackageName>nom_de_mon_package</PackageName>
```
* La version du package: la version actuelle de votre package.
```xml
<PackageVersion>1.2.3</PackageVersion>
```
* La licence du package*: La licence sous laquelle votre package est distribué.
```xml
<PackageLicense>MIT License</PackageLicense>
```

Vous pouvez rajouter des informations supplémentaires dans le Upset:
* Des critères sur Unity: si vous ne supportez pas toutes les versions de Unity, vous pouvez le préciser ici afin que votre package ne soit pas utilisé sur des versions de Unity qui ne répondent pas à vos critères.

__Exemple:__ J'ai créé un packqge pour les versions de Unity 5 et postérieures et je ne prévoie pas de supporter les versions antérieures. Je peux alors rajouter:
```xml
<UnityVersion>
  <MinVersion>5.0.0</MinVersion>
</UnityVersion>
```

* Configuration du package: de manière similaire au fonctionnement de la configuratio du Upfile, vous pouvez quel type de fichier votre package contient afin que l'utilisateur puisse les charger là où il le souhaite.

__Exemple:__ J'ai mis toute la documentation de mon package dans un dossier intitulé 'Documentation'. Je peux alors rajouter cette ligne à la section `Configuration`:
```xml
<Spec Type="Docs" Path="Documentation" />
```

\* __Pourquoi une licence est-elle nécessaire?__ Parce qu'elle est essentielle à votre package. Nombre de projets Unity vont avoir une utilisation commerciale et s'ils utilisent votre package, ils vont utiliser du matériel que vous avez créé; l'utilisateur de votre package doit s'assurer qu'il peut utiliser votre package dans un cadre légal. [Davantage sur la question](https://unity3d.com/legal/as_terms).

## Feuille de route
### Ce qui doit être réalisé

* Support de Upset pour les fichiers .unitypackage; à l'heure actuelle le nom et la version du package sont déduits du nom du fichier, mais ce comportement doit être modifié.

* Davantage de types de répertoire: nous ne supportons pour le moment que des répertoire de fichiers (ie présents sur le système), et nous devrions pouvoir supporter d'autres sources telles que (non limitées à): Git, Dropbox ou Google Drive.

* Résolution de dépendences transitives: si un package A dépend d'un package B, nous devons nous assurer que le package B est installé avant d'installer le package A, avec correspondance des versions.

* Meilleure implémentation de l'UI

### Ce qui est prévu

* Exporteur de package/éditeur de Upset: afin de faciliter la création de packages pour Uplift, nous prévoyons de créer un exporteur de package qui serait essentiellement une surcouche à l'exporteur de package Unity, avec la création et l'intégration automatique de fichiers Upset lors de la création du package.

* Support pour l'initialisation de packages à leur installation: lors de l'installation d'un package, il est courant d'avoir à le configurer/l'initialiser à la main et il serait intéressant de pouvoir automatiser l'initialisation des packages à l'aide de quelques lignes XML dans la création de dépendances du Upfile.
