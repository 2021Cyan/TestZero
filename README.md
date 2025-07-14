<h1 align="center">
 <img src="./DevJournal/SteamPage/library_header.jpg" alt="Test Zero logo">
    <br />
    Test Zero
    <br />
    2D Unity Game Development Journal
</h1>

#### The journal will explore the path and hardships we experienced throughout the development of a 2D shooter with Unity. We hope it will help whoever is interested in game development, especially with Unity, to learn from our experiences.

# Table of Contents

1. [Basics in Unity](https://github.com/2021Cyan/INTD450#Basics-in-Unity)
    - [GameObjects and Components]()
    - [Camera & UI]()
    - [Input Handling]()
    - [Optimization]()
    - [Debugging]()
    - [Version Control]()
    - [Build]()
2. [Key points in Test Zero](https://github.com/2021Cyan/INTD450#Key-points-in-Test-Zero)
    - [2D Light]()
    - [Shader]()
    - [FMOD (Audio)]()
    - [Procedural Content Generation (PCG)]()
    - [Rigging (Animation)]()
    - [?]()

# Basics in Unity

[Back to the Top](#table-of-contents)

If you have little expereince in programing languages, you would have heard of Object oriented programming (OOP). However, in many game engines, including Unity, understanding Entity-Component-System (ECS) would be helpful. According to Wikipedia, ["An ECS comprises entities composed from components of data, with systems which operate on the components."](https://en.wikipedia.org/wiki/Entity_component_system#:~:text=Entity%E2%80%93component%E2%80%93system%20ECS,which%20operate%20on%20the%20components).

Entities are the objects in your game, such as a player or an enemy. Components are the data and functionality that tell the entity how to behave, such as its position, health, or what it can do. Systems are the logic that processes the components of entities. In other words, systems are to control the behaviors of entities based on their components.

## GameObjects and Components

In Unity, GameObjects are the entities. Interesting thing is that GameObjects can work as folders. You can organize your GameObjects in a hierarchy, where a GameObject can have child GameObjects.

<div style="display: flex; align-items: flex-start;">
    <!-- Left section - Image -->
    <div style="flex: 1; padding-right: 20px;">
        <img src="./DevJournal/Basic/GameObject&Component/Scene.png" width="100%">
    </div>
    <div style="flex: 1;">
        <h3>Object Hierarchy</h3>
        <p>This is the Unity Scene view showing the GameObject hierarchy. Notice how objects are organized in parent-child relationships, allowing for grouped transformations and better scene organization.</p>
    </div>
</div>

Each GameObject can contain multiple components that define its behavior and appearance. In this scene, each wall and ceiling object contains a BoxCollider2D component, which allows them to interact with other objects in the game world. These BoxColliders are represented by the green outlines visible in the scene view. 

While these objects also have SpriteRenderer components (which would normally make them visible), they aren't visually apparent in the scene because this level uses a tile-based approach for visuals rather than individual sprites for background and each collision object.

<div style="display: flex; align-items: flex-start;">
    <!-- Left section - Image -->
    <div style="flex: 1; padding-right: 20px;">
        <img src="./DevJournal/Basic/GameObject&Component/Comp.png" width="100%">
    </div>
    <div style="flex: 1;">
        <h3>Scene View</h3>
        <p>This is the Unity Scene view.</p>
        <p>Components can include scripts, physics properties, renderers, and more.</p>
    </div>
</div>



## Camera & UI

The Camera is the viewpoint of the game. It determines what is visible on the screen. UI (User Interface) is the visual elements that allow players to interact with the game, such as menus, buttons, and HUD (Heads-Up Display).

## Input Handling

Old Input System VS New Input System

Check usr input through Update() or through Events.

## Optimization

Reduce the use of Update(). Use events or coroutines where possible to reduce unnecessary processing.

Events...

Coroutines...

## Debugging

## Version Control

## Build

# Key points in Test Zero

[Back to the Top](#table-of-contents)

## 2D Light

## Shader

## FMOD (Audio)

## Procedural Content Generation (PCG)

## Rigging (Animation)

## ?

[Back to the Top](#table-of-contents)