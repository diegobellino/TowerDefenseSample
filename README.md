# TowerDefenseSample

This is a personal project made with the goal of creating a system that would potentially support the creation of a Tower Defense type of game.
As such, the focus is not on creating a small sample game that could support a level or two, but a set of tools and systems that are scalable and robust enough to support a professional game.

## Project Overview

### Project structure
Project structure is meant to be as scalable as possible, scripts are subdivided in folders by responsibility. Each of them includes their Assembly Definition asset. 
As a general rule, MVC pattern is followed throughout the code. Gameplay data (towers, levels, enemies...) is stored in Scriptable Objects inside the Linked Resources folder.

### Level Editor Generator
Includes a custom tool for quickly generating and iterating through already made levels. Accessible through "Tower Defense -> Level Editor" menu.
Upon opening the window, the user can select whether to create a new level or load a previous one. Both options lead to the main Level Editor window, which presents options to edit all the values of the level, and updates them in real time on the level editing scene (which auto opens) so the user can see the changes. 
Level editing includes a "Test" button that starts a game automatically on the edited level (bypasses state management system using editor preferences)

<img width="1254" alt="image" src="https://user-images.githubusercontent.com/104199158/203123198-735e92ec-5d03-4984-bffc-5d32360db4a2.png">

### Performance
A generic pool system is included (anything that implements the IPoolable interface can be pooled and retrieved). A SmartUpdate system is included, which allows the developer to distribute the update load of any script on demand in order to reduce CPU load (4 update modes: always, even frames, odd frames, time scheduled)

### Pluggable AI system
Uses the Smart Update system to update to execute exactly 10 times per second (which can be easily edited). The system exploits Scriptable Objects in order to create a way for designers to easily modify behaviours, by constructing AI States with actions and transitions directly in the inspector window.

<img width="413" alt="image" src="https://user-images.githubusercontent.com/104199158/203125401-910899e6-d279-4cca-9ece-69fc243e9813.png">

### State Management
GameStateController is the main script in the State Management system. It's a singleton that creates all other states, is in charge of propagating game actions to active states, and changes between active states.
StateManagers are regular C# classes that bridge between 3D and UI controllers of every state.

### Input
Input is handled using Unity's Input Manager package.
