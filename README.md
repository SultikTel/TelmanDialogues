# Telman Dialogues System

Node-based dialogue editor for Unity using GraphView + ScriptableObjects.
<img width="1026" height="412" alt="image" src="https://github.com/user-attachments/assets/2aa0e47e-c732-4dce-b051-902a4220d5b9" />


## Features

- Visual dialogue graph editor
- Branching conversations (choices)
- Typewriter text effect
- Dialogue events system
- Runtime compiled dialogue data
- ScriptableObject-based storage

---

## How it works

1. Create Dialogue System asset  
2. Open editor (OpenEditWindow)  
3. Build nodes and connect them  
4. Edit dialogue lines  
5. Press Save  
6. Use in game via controller  
<img width="450" height="865" alt="image" src="https://github.com/user-attachments/assets/5d45bcb0-0356-4944-b6c8-7044b13059a5" />


## Run dialogue
Just create a DialoguesSystem asset and open its editor window — it will generate a visual dialogue tree where you can freely build and connect nodes. The system is fully driven by the serialized DialoguesSystem reference ([SerializeField] private DialoguesSystem _dialoguesSystem), which acts as the core container for all dialogue data. All dialogues, connections, and node information are compiled into DialoguesPure, a runtime-ready list exposed as public List<DialoguesBlock> DialoguesPure, which stores the complete dialogue structure and branching logic. To use it in-game, simply call DialoguesController.Instance.Play("StartPoint"); — this will start dialogue playback from the specified node name, using the precompiled graph data.
