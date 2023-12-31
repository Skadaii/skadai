# Skadaï: *Decision-making and movements in AI* - **Unity/C#**
### ISART DIGITAL GP3, School Project: *Benjamin MARIN, Noé MOUCHEL, Rémi GINER*  

![CircleFormation](Annexes/TitleImage.png)

<!-- ABOUT THE PROJECT -->
# About The Project 
[![Unity](https://img.shields.io/badge/dynamic/yaml?url=https://raw.githubusercontent.com/skadaii/skadai/master/ProjectSettings/ProjectVersion.txt&query=m_EditorVersion&logo=unity&label=Unity&color=grey)](https://unity.com)
[![Gitlab](https://img.shields.io/badge/GitLab-292961?&logo=gitlab)](https://gitlab.com)

The goal of the project is to make an NPC AI that moves as a squad and can interact with the player, in a real-time action game.

# Table of contents
1. [Features & usage](#features--usage)
    - [Features](#features)
    - [Controls](#controls)
3. [How to launch](#how-to-launch)
5. [Details](#details)
    - [Utility System](#utility-system)
    - [Squad System](#squad-system)
    - [Gameplay Integration](#gameplay-integration)
5. [Reference](#references)
6. [Versionning](#versionning)
6. [Autors](#authors)


# Features & usage

## Features
- Smart AIs
- Utility system
- Ally and enemy squad system
- Character classes (Healer, Support...)

## Controls
The controls are made for keyboard only:
- WASD - Move
- Mouse movements - Aim and move squad
- Left click - Shoot
- Right click - Call for support fire

# How to launch
Launch the exe directly from the archive.

# Details

## Utility system
To integrate artificial intelligence in the project, we have to make a decisional model which is the utility system.

### **Action Set**
The utility system is composed of a list of actions stored in a scriptable object.
<div style="text-align:center">

![ActionSet](Annexes/ActionSet.gif)

*Creation of an action list in the utility ai*
</div>

An action is composed of methods and a consideration.

### **Consideration**
Taking an action into account will be evaluated and will give a value between 0 and 1, this value is evaluated from an input and a curve.
The input is a method that will return a value, for example, the distance of the player from an enemy.
And we will plot a curve against this input that will determine the utility (between 0 and 1) that will be returned by the consideration evaluation.

<div style="text-align:center">

![Consideration](Annexes/Consideration.png)

*Exemple of consideration*
</div>

The utility system will evaluate all the considerations of the actions in order to derive the best action to perform. 

### **Methods**
When an action is executed it will invoke all the methods of the action.
The listed components are all the components attached to the Game Object where the utility system is and the listed methods are all the public methods of the component.
<div style="text-align:center">

![Methods](Annexes/Methods.gif)

*Creation of a method in an action*
</div>

To facilitate the integration of the actions in the utility system we decided to make a custom inspector, for example thanks to this we can list the methods available in the components.

## Squad system

### **Unit system**

To make a Squad system compatible with different types of controllers ( direct player inputs and NavMesh calculations), we had to create a leader-squad system. When the leader decides to make a move, the rest of the squad must follow this order. The units will not try to reachg the leader directly, but the position where the leader wants to be. This allows us to easily do a patrol system and a unit tracking.

To make the leader send its orders each time it wants to move, we used inheritance and UnityEvents. The leader's orders are linked to its movements updates.

```cs
leader.GetComponent<Movement>().OnMoveChange.AddListener(UpdatePositions);
```

<div style="text-align:center">

![UnitLeader](Annexes/UnitLeader.gif)
*Movements of a Player directed squad and an AI directed squad*
</div>

To create an AI team, we had to create a virtual leader (with no collision nor rendering) who cannot die.

<div style="text-align:center">

![VirtualLeader](Annexes/VirtualLeader.png)

*Prefab of a Virtual Leader*
</div>

### **Formation system**
To make the units follow the leader's order, the leader must tell the squad (which is a separate GameObject) where it wants to go. Before the squad tells its units where to go, it calculates the correct position that follows its formation rule (which is a manually configured ScriptableObject) for each unit.

<div style="text-align:center">

![CircleFormation](Annexes/CircleFormation.gif)
*Modular circular formation of a squad*
</div>

### **Squad controller**
To achieve a proper Squad system, we had to create an entity that represents a Squad to manage its own units. The Squad Controller instantiates its units according to its initial formation and number of units. Then, it will be the bridge between the leader and the rest of the squad, through which the leader's orders will pass. 

<div style="text-align:center">

![SquadController](Annexes/SquadController.gif)
*Enemy patrol squad instantiation*
</div>

## Gameplay integration
We used the utility system to make specific AI action as healing, coverig or even shooting. We just had to make a consideration function then method(s) function(s),
here an example of the healer utility system.
 

<div style="text-align:center">

![HealerUtilitySystem](Annexes/HealerutilitySystem.png)  

*Healer utility system*
</div>

The healer has two actions, shoot at target and heal, shoot at target consideration has lower maximum value, the healing consideration can return a higher value so **the healer will in priority heal hurted teammates instead of figthing enemies**.

<div style="text-align:center">

![HealerUtilitySystem](Annexes/HealerTargetConsideration.png)  

*Healer target consideration*
</div>

<div style="text-align:center">

![HealerUtilitySystem](Annexes/HealerHealConsideration.png)  

*Healer healing consideration*
</div>

Our system is not limited to only help the player, AI can help each others, a healer can heal anyone but himself, and supports can cover everyone but not supports. The player stays in priority when the unit choose who to help. The targeting choice is managed in the file *[Assets/Scripts/Units/UnitSquad.cs](./Assets/Scripts/Units/UnitSquad.cs#L80)*.  


<br>
<div style="text-align:center">

![HealerUtilitySystem](Annexes/HealerHealing.gif)  
*Healer healing - [Assets/Scripts/Units/HealerUnit.cs](./Assets/Scripts/Units/HealerUnit.cs)*
</div>

<br><br>


In the *support* action, for support units, we use a double method, the shootAtTarget and the cover methods are called so support can cover and shoot at the same time.

<div style="text-align:center">

![HealerUtilitySystem](Annexes/SupportCoverAction.png)  
*Support action*
</div>

<div style="text-align:center">

![HealerUtilitySystem](Annexes/SupportCovering.gif)  
*Support covering - [Assets/Scripts/Units/SupportUnit.cs](./Assets/Scripts/Units/SupportUnit.cs)*
</div>

<br>

You can also find in *[Assets/Scripts/Agents/AIAgent.cs](./Assets/Scripts/Agents/AIAgent.cs#L80)*, the consideration and method for the shooting behaviour of the units and turrets.

<br>


## References:
- https://docs.unity3d.com/Manual/nav-BuildingNavMesh.html


## Versionning
Git Lab was used for the versioning during of the development of this project.

# Authors
* **Noé MOUCHEL** [![Github](https://img.shields.io/badge/NoeMouchel-grey?logo=github)](https://github.com/NoeMouchel) [![itch.io](https://img.shields.io/badge/noe--mouchel-%23FF0B34?logo=itch.io&logoColor=white)](https://noe-mouchel.itch.io)
* **Benjamin MARIN** [![Github](https://img.shields.io/badge/Bijima-grey?logo=github)](https://github.com/Bijima) [![itch.io](https://img.shields.io/badge/bijima-%23FF0B34?logo=itch.io&logoColor=white)](https://bijima.itch.io)
* **Rémi GINER** [![Github](https://img.shields.io/badge/remisansfamine-grey?logo=github)](https://github.com/remisansfamine) [![itch.io](https://img.shields.io/badge/remisansfamine-%23FF0B34?logo=itch.io&logoColor=white)](https://remisansfamine.itch.io)