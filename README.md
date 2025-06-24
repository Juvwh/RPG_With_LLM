# 📘 Master's Thesis – LLMs for Role-Playing Games

This repository contains all the materials related to the master's thesis titled **"Large Language Models as RPG Game Masters"**. The goal of this README is to provide an overview of the repository structure and explain the role of each folder and the general structure of the code.

You can play to the game here : https://rpgwithllm.z28.web.core.windows.net/

## 🗂 Repository Structure
For simplicity we note only the relevant folder and file, if it is not present it means that we did not touch the content of those file.
```bash
.
├── Assets/
│   ├── Animations/          # All animations (eg: dice)
│   ├── Dice/                # graphical material for the dice
│   ├── Prefabs/             # Prefabs object that are instantiable in the scene
│   ├── Scene/               # List of different scene
│   ├── Scripts/ 
│   │   ├── Agent/           # scripts that handle request to model API (text)
│   │   ├── BlackForest/     # scripts that handle request to model API (image)
│   │   ├── CharacterMenu/   # scripts responsible for the management of characters
│   │   ├── Combat/          # scripts that manage the combat
│   │   ├── Debug/           # useful scripts for ease of testing
│   │   ├── Dice/            # scripts that manage the use of Dice
│   │   ├── ElevenLabs/      # scripts that handle request to model API (voice)
│   │   ├── Generation/      # scripts that manage the generation of the story and the input check
│   │   ├── Groups/          # scripts that manage the group composition
│   │   ├── Intrigue/        # scripts that manage the progression of the intrigue (current room, memory, ...)
│   │   ├── Inventaire/      # scripts related to the inventory and it's management
│   │   ├── MainMenu/        # script for the main menu 
│   │   ├── Map/             # scripts for the generation of the map, the visualization and the management
│   │   ├── Mistral/         # scripts that handle request to mistral API (text)
│   │   ├── PlayerDatas/     # script that manage the data of the playing heroes
│   │   └── UI/              # scripts that have impact on the UI
│   ├── StreamingAssets/     # data that are exported with the game
│   ├── Textures/            # texture used
│   ├── Tiles/               # texture used specific to the map
│   ├── UI/                  # texture used specific to the UI
│   └── _Sprites/            # Sprite used
│
├── .gitignore               # list of file to ignore when merging/pushing
└── README.md                # This file
```
## 📄 Folder Descriptions

### `Scripts/Agent/`
Contains the basic logic for sending HTTP request to the `groq` and `OpenAI` API with different jobs depending on the task.
Please note that this script contains all the API keys (which we have obviously removed), so you will need to create one on Groq or Mistral (it's free).

### `Scripts/BlackForest/`
Contains the basic logic for sending HTTP request to the `Blackforest` model API via `HuggingFace` to generate Image.

### `Scripts/CharacterMenu/`
Contains all the logic for the creation of a character, the menu of the creation and the management of the selection of heroes at the begining. Contains also the `CharacterManager.cs` which maintains information of the selected characters for the current run of the game.

### `Scripts/Combat/`
Contains all the logic for the start and the flow of the combat in `CombatManager.cs`.

### `Scripts/ElevenLabs/`
Contains the basic logic for sending HTTP request to the `ElevenLabs` API for a text voice over (currently not used).

### `Scripts/Generation/`
Contains the logic for generating the story in exploration mode and validating the player input in `Free.cs`, the management of what happen when a chest is open is also present in `Chest.cs` and `Generate_Next.cs` manage the change of room.

### `Scripts/Groups/`
Contains the logic for separating/merging and managing the group in `GroupManager.cs` using the different class inheriting from `Group.cs`

### `Scripts/Intrigue/`
Manage the progress of the game in `HistoryManager.cs`. Typically this script is call at the begining to generate the starting plot and between each player action to change the game mode (exploration ⇄ combat). `MemoryManager.cs` contains all the string data. `NextManager.cs` is currently depreciated since we now only use the Free and combat mode.

### `Scripts/Inventaire/`
Contains all the logic from the creation to the attribution of the loot to the inventory in `UI_Inventaire_Buttons.cs`, and in `UI_Inventaire_Loot.cs` manage the visualization on thu UI.

### `Scripts/MainMenu/`
Define the behaviour of the different option in the main menu

### `Scripts/Map/`
In `Coord/` contains the data structure that correspond to a tile of the map. In `~DungeonGenerator.cs` scripts contains the basic logic for the generation of the map layout that are done in `ProceduralALgo.cs`. `TilemapVisualizer.cs` and `WallGenerator.cs` respectively display the ground and wall tile. Finnaly `MapManager.cs` manage the logic of the map such as the creation based on the model ouput, the spawn of event, ...

### `Scripts/Mistral/`
Contains the basic logic for sending HTTP request to the `Mistral` API with different agentID depending on the task.

### `Scripts/PlayerDatas/`
Contains the data structure that correspond to the player used for the current run of the game

### `Scripts/UI/`
Contains all the scripts related to the management of UI elements.

## 🧪 Notes

- There is a known issue that sometimes occurs when separating and then merging the group, leading to the disappearance of the console.
