# Script Attachment Guide (Updated)

## 22 scripts, organized by where they attach

---

## Fix first

Your error path showed `Assets\Scripts\Scripts\...` — you have a nested Scripts
folder. Move the subfolders (Audio, Core, Interactables, etc.) directly into
`Assets\Scripts\` so the path is `Assets\Scripts\Puzzles\TumblerLock.cs`.

---

## Before anything else

**Tag your player:** Select the player capsule inside `NestedParent_Unpack` →
Inspector → Tag dropdown → select **Player**.

**Create 5 InventoryItem assets:** Project window → right-click →
Create → Escape Game → Inventory Item. One for each:

| Asset Name   | itemID       | isKeyItem | isConsumable |
|--------------|--------------|-----------|--------------|
| OfficeKey    | office_key   | true      | true         |
| LibraryKey   | library_key  | true      | true         |
| ParlorKey    | parlor_key   | true      | true         |
| Screwdriver  | screwdriver  | false     | true         |
| (spare)      | (your call)  | false     | false        |

Give each one an icon sprite.

---

## 1. GameManager (exists at scene root — empty transform)

Add component: **GameManager.cs**

| Inspector Field    | What to drag in                              |
|--------------------|----------------------------------------------|
| Front Door         | The FrontDoor prefab instance in the scene    |
| Lobby Clock        | The clock object inside the props prefab      |
| Cutscene Manager   | The CutsceneManager object (create in step 3) |

That's it. The `InteractionSystem` field from the old version is gone —
GameManager now auto-finds it on your main camera at runtime.

---

## 2. SoundManager (exists at scene root — empty transform)

Add component: **SoundManager.cs**

**Create 4 child GameObjects** under SoundManager. On each one, click
Add Component → Audio → Audio Source:

| Child Name     | AudioSource: Loop | AudioSource: Play On Awake |
|----------------|-------------------|----------------------------|
| MusicSource    | ON                | OFF                        |
| AmbientSource  | ON                | OFF                        |
| SFXSource      | OFF               | OFF                        |
| UISource       | OFF               | OFF                        |

Then on the SoundManager component in the Inspector:
- Drag each child's AudioSource into: Music Source, Ambient Source, SFX Source, UI Source
- Drag your music AudioClips into: Exploration Music, Urgency Music, Win Music, Lose Music
- Drag ambient AudioClips into: Amb Lobby, Amb Office, Amb Library, Amb Parlor
- In the **SFX Clips** array, click + to add entries. Each entry has a Name
  (string) and a Clip (AudioClip). Names must match exactly:
  `pickup`, `door_unlock`, `door_open`, `puzzle_solve`, `puzzle_fail`,
  `drawer_open`, `locked_rattle`, `ui_click`, `inventory_open`,
  `key_insert`, `clock_tick`, `clock_chime`, `trap_slam`, `tumbler_click`,
  `tumbler_solved`, `lever_pull`, `morse_beep`, `keypad_beep`, `keypad_deny`,
  `box_open`, `screwdriver_use`

---

## 3. CutsceneManager (CREATE NEW at scene root)

Create an empty GameObject named "CutsceneManager". Add component: **CutsceneManager.cs**

| Inspector Field    | What to drag in                                  |
|--------------------|--------------------------------------------------|
| Cutscene Camera    | CutsceneCamera (create below, disabled)          |
| Player Camera      | The camera inside NestedParent_Unpack             |
| Directional Light  | The Directional Light in the scene                |
| Player Light       | PlayerLight (child of player capsule)             |
| Door Objects       | The FrontDoor prefab + any room doors to slam     |
| Fade Overlay       | FadeOverlay CanvasGroup (create below)            |
| Win Camera Path    | Empty transforms for win dolly waypoints          |
| Lose Camera Path   | Empty transforms for lose dolly waypoints         |

**Create CutsceneCamera:** Right-click CutsceneManager → 3D Object → Camera.
Name it "CutsceneCamera". **Uncheck the checkbox** at the top of Inspector to
disable it. It only turns on during cutscenes.

**Create dolly waypoints:** Right-click CutsceneManager → Create Empty. Name them
WinPath_1, WinPath_2, WinPath_3 (position where the camera should travel during
the win cutscene). Do the same for LosePath_1, LosePath_2, LosePath_3. Drag the
win ones into Win Camera Path array, lose ones into Lose Camera Path array.

**Create FadeOverlay:** Under `UI-Canvas`, right-click → UI → Image. Name it
"FadeOverlay". Set anchors to stretch-stretch (fills screen). Set color to
black (0,0,0,1). Then Add Component → **Canvas Group**. Set Alpha to 0.
Uncheck Raycast Target. Drag this object into CutsceneManager's Fade Overlay slot.

---

## 4. UI-Canvas (existing)

Add component: **UIManager.cs** to the UI-Canvas root

| Inspector Field | What to drag in                                     |
|-----------------|-----------------------------------------------------|
| Pause Menu      | A Panel you create with "Paused" text, starts disabled |
| Win Screen      | A Panel with "You Escaped!" text, starts disabled    |
| Lose Screen     | A Panel with "Trapped..." text, starts disabled      |

Create these 3 panels as children of UI-Canvas. Disable each one (uncheck
the checkbox). Drag them into the matching slots.

---

## 5. InventoryBackground (existing, under UI-Canvas → Grid)

Add component: **InventoryManager.cs**

| Inspector Field  | What to drag in                              |
|------------------|----------------------------------------------|
| Slots            | Drag all 6 Slot Image components in order    |
| Empty Slot Sprite| A default gray sprite for empty slots        |
| Inventory Panel  | The InventoryBackground object itself        |

---

## 6. Cursor (existing, under UI-Canvas)

Add component: **CursorManager.cs**

| Inspector Field | What to drag in         |
|-----------------|-------------------------|
| Point Cursor    | Your pointer sprite     |
| Grab Cursor     | Your grab/interact sprite |

---

## 7. DoorwayTriggers (existing — 3 children)

On each trigger child, add component: **RoomDetector.cs**

| Object              | Room Type field |
|---------------------|-----------------|
| OfficeDoorTrigger   | Office          |
| LibraryDoorTrigger  | Library         |
| ParlorDoorTrigger   | Parlor          |

---

## 8. NestedParent_Unpack (open the prefab)

Find the **Camera** child inside the prefab.
Add component: **InteractionSystem.cs**

| Inspector Field    | Set to                     |
|--------------------|----------------------------|
| Interact Range     | 3                          |
| Interactable Layer | Everything                 |
| Interact Key       | E                          |

---

## 9. FrontDoor (open the prefab)

### FrontDoor root

Add component: **FrontDoorController.cs**

| Inspector Field | What to drag in                               |
|-----------------|-----------------------------------------------|
| Door            | The single door transform (the part that swings) |
| Padlocks        | Drag all 3 padlock children into the array    |
| Open Angle      | 90 (adjust sign if door swings wrong way)     |
| Open Speed      | 2                                             |

### Each padlock child (×3)

Add component: **PadlockInteractable.cs** to each padlock mesh

| Inspector Field   | Padlock 1      | Padlock 2      | Padlock 3      |
|-------------------|----------------|----------------|----------------|
| Required Key ID   | office_key     | library_key    | parlor_key     |
| Padlock Name      | Office Padlock | Library Padlock| Parlor Padlock |
| Door Controller   | Drag the FrontDoor root (the one with FrontDoorController) |

**Each padlock also needs a collider** so the player's raycast can hit it:
Select the padlock mesh → Add Component → Physics → Box Collider.
The green wireframe box should roughly wrap the padlock shape. Adjust
Size and Center if needed.

---

## 10. OfficeKey (existing, scene root)

Add component: **PickupItem.cs**

| Inspector Field | What to drag in              |
|-----------------|------------------------------|
| Item            | The OfficeKey ScriptableObject asset |
| Animate Idle    | true (optional)              |

Add a collider: Select OfficeKey → Add Component → Physics → Box Collider.

### Office puzzle — TEAMMATE RESPONSIBILITY
Your teammate writes the puzzle script for this room.

---

## 11. Library (open the prefab)

### LibraryKey (child inside Library)

Add component: **PickupItem.cs**

| Inspector Field | What to drag in                |
|-----------------|--------------------------------|
| Item            | The LibraryKey ScriptableObject asset |

Add a collider if missing.

### Library puzzle + Pages — TEAMMATE RESPONSIBILITY
Your teammate writes the puzzle script. Pages are part of their puzzle.

---

## 12. Parlor (open the prefab)

### ParlorKey (child inside Parlor)

Add component: **PickupItem.cs**

| Inspector Field | What to drag in              |
|-----------------|------------------------------|
| Item            | The ParlorKey ScriptableObject asset |

**Start this object DISABLED** (uncheck checkbox). ParlorPuzzleBox will
enable it when the top lid opens.

### ParlorPuzzleBox root

Add component: **ParlorPuzzleBox.cs**

| Inspector Field      | What to drag in                                |
|----------------------|------------------------------------------------|
| Tumbler Lock         | The TumblerLock component (on the tumbler side, see below) |
| Keypad Input         | The KeypadInput component (on the keypad side, see below) |
| Tumbler Light        | The indicator light Renderer on the front face |
| Keypad Light         | The other indicator light Renderer on the front face |
| Light Red Mat        | A red material (see "Materials needed" below)  |
| Light Green Mat      | A green material (see "Materials needed" below) |
| Front Door           | The small front door transform                 |
| Screwdriver Object   | The screwdriver mesh (starts DISABLED)         |
| Screwdriver Pickup   | The PickupItem on the screwdriver              |
| Screwdriver Item ID  | screwdriver                                    |
| Top Lid              | The top lid transform                          |
| Top Interact Zone    | A BoxCollider on the top surface of the box    |
| Parlor Key Object    | The ParlorKey mesh (starts DISABLED)           |

---

### Tumbler side of the box

**On the parent object that holds all 4 tumblers + lever + light,**
add component: **TumblerLock.cs**

| Inspector Field     | What to set                                    |
|---------------------|------------------------------------------------|
| Tumblers array      | Set Size to 4. For each element:               |
|                     | — Transform: drag the hexagonal tumbler mesh   |
|                     | — Correct Face Index: 0, 1, 2, 2 (for H,E,L,P)|
|                     | — Face Letters: "HARMTW", "OESKBN", "DPLFGU", "CIPJVX" |
| Lever               | The lever transform                            |
| Morse Light         | The Point Light or Spot Light above the tumblers |
| Morse Light Renderer| The light bulb mesh Renderer (for material swap) |
| Light On Mat        | A bright emissive material (see below)         |
| Light Off Mat       | A dark/off material (see below)                |

#### How to add colliders to each tumbler

Select a tumbler mesh in the Hierarchy. In the Inspector:
1. Click **Add Component**
2. Type "Box Collider" and select it
3. A green wireframe box appears around the mesh
4. If it doesn't fit well, adjust **Center** and **Size** in the BoxCollider
   component until the green box wraps the tumbler snugly
5. Repeat for all 4 tumblers AND the lever

#### On each tumbler mesh (×4)

Each tumbler is its own file now. Select the tumbler mesh, then:
Add Component → search for **TumblerInteractable** → add it.

| Inspector Field | What to set                     |
|-----------------|---------------------------------|
| Parent Lock     | Drag the object that has TumblerLock.cs on it |
| Tumbler Index   | 0, 1, 2, or 3 (matching the array order in TumblerLock) |

#### On the lever mesh

Select the lever, then: Add Component → search for **LeverInteractable** → add it.

| Inspector Field | What to set                     |
|-----------------|---------------------------------|
| Parent Lock     | Drag the object that has TumblerLock.cs on it |

Add a Box Collider to the lever too (same steps as tumblers above).

---

### Keypad side of the box

**On the parent object that holds all 9 buttons + reset button,**
add component: **KeypadInput.cs**

| Inspector Field  | What to set                                    |
|------------------|------------------------------------------------|
| Number Buttons   | Set Size to 9. For each element:               |
|                  | — Collider: the BoxCollider on that button block |
|                  | — Mesh Transform: the button block transform   |
|                  | — Digit: 1 through 9                          |
| Reset Button     | Same fields for the reset button block         |
| Correct Code     | Array of digits, e.g., 3, 1, 4, 2             |
| Press Depth      | 0.05 (how far buttons push in)                 |

#### On each button block (×9 number buttons + 1 reset button)

Each button is its own file now. Select a button block, then:
Add Component → search for **KeypadButtonInteractable** → add it.

| Inspector Field | What to set                      |
|-----------------|----------------------------------|
| Parent Keypad   | Drag the object that has KeypadInput.cs on it |

**Each button also needs a Box Collider.** Select the button block →
Add Component → Box Collider. Adjust Size/Center so the green wireframe
fits the button. Repeat for all 9 number buttons + the reset button.

---

### Screwdriver (child inside ParlorPuzzleBox, behind front door)

Add component: **PickupItem.cs**

| Inspector Field | What to drag in                 |
|-----------------|---------------------------------|
| Item            | The Screwdriver ScriptableObject asset |

**Start this object DISABLED.** ParlorPuzzleBox enables it when the front door opens.

---

## 13. props prefab (for the Clock)

Open the `props` prefab. Find the clock on the lobby wall.

Add component: **ClockController.cs** to the clock object

| Inspector Field     | What to set                                 |
|---------------------|---------------------------------------------|
| Minute Hand         | Drag the minute hand child transform        |
| Hour Hand           | Drag the hour hand child transform          |
| Game Duration       | 600 (seconds = 10 minutes)                 |
| Warning Threshold   | 120 (seconds = 2 minutes remaining)        |

---

## Materials needed

You need to create a few simple materials for the puzzle lights:

### For the 2 indicator lights on the ParlorPuzzleBox front face

1. **Right-click** in your Materials folder → Create → Material
2. Name it **LightRed**
3. Set Base Color to red
4. If using URP: check **Emission**, set Emission Color to bright red
5. Repeat for **LightGreen** with green color and green emission

### For the Morse code light above the tumblers

1. Create material **MorseLightOn**
   - Set Base Color to bright yellow/white
   - Enable Emission, set to bright yellow
2. Create material **MorseLightOff**
   - Set Base Color to dark gray
   - Emission OFF (or very dim)

These are just standard materials — the scripts swap them at runtime to
make lights appear to turn on and off.

---

## Full checklist

```
Scene Root:
  ├─ GameManager              → GameManager.cs
  ├─ SoundManager             → SoundManager.cs + 4 AudioSource children
  ├─ CutsceneManager (new)    → CutsceneManager.cs
  │  └─ CutsceneCamera (new)  → Camera component, DISABLED
  ├─ OfficeKey                → PickupItem.cs + BoxCollider
  └─ FadeOverlay (under UI-Canvas) → Image + CanvasGroup

UI-Canvas:
  ├─ (root)                   → UIManager.cs
  ├─ InventoryBackground      → InventoryManager.cs
  ├─ Cursor                   → CursorManager.cs
  ├─ PauseMenu (new, disabled)
  ├─ WinScreen (new, disabled)
  └─ LoseScreen (new, disabled)

DoorwayTriggers:
  ├─ OfficeDoorTrigger        → RoomDetector.cs (Office)
  ├─ LibraryDoorTrigger       → RoomDetector.cs (Library)
  └─ ParlorDoorTrigger        → RoomDetector.cs (Parlor)

NestedParent_Unpack (open prefab):
  └─ Camera child             → InteractionSystem.cs

FrontDoor (open prefab):
  ├─ (root)                   → FrontDoorController.cs
  ├─ Padlock 1                → PadlockInteractable.cs + BoxCollider
  ├─ Padlock 2                → PadlockInteractable.cs + BoxCollider
  └─ Padlock 3                → PadlockInteractable.cs + BoxCollider

Office:
  └─ Puzzle                   → TEAMMATE scripts (extend PuzzleBase)

Library (open prefab):
  ├─ LibraryKey               → PickupItem.cs
  └─ Puzzle + Pages           → TEAMMATE scripts (extend PuzzleBase)

Parlor (open prefab):
  ├─ ParlorKey (DISABLED)     → PickupItem.cs
  ├─ ParlorPuzzleBox          → ParlorPuzzleBox.cs
  │  ├─ Tumbler side parent   → TumblerLock.cs
  │  │  ├─ Tumbler 0          → TumblerInteractable.cs + BoxCollider
  │  │  ├─ Tumbler 1          → TumblerInteractable.cs + BoxCollider
  │  │  ├─ Tumbler 2          → TumblerInteractable.cs + BoxCollider
  │  │  ├─ Tumbler 3          → TumblerInteractable.cs + BoxCollider
  │  │  └─ Lever              → LeverInteractable.cs + BoxCollider
  │  ├─ Keypad side parent    → KeypadInput.cs
  │  │  ├─ Button 1           → KeypadButtonInteractable.cs + BoxCollider
  │  │  ├─ Button 2           → KeypadButtonInteractable.cs + BoxCollider
  │  │  ├─ ...                → (same for buttons 3-9)
  │  │  └─ Reset button       → KeypadButtonInteractable.cs + BoxCollider
  │  └─ Screwdriver (DISABLED)→ PickupItem.cs
  └─ Box top                  → BoxCollider (ParlorPuzzleBox handles interaction)

props (open prefab):
  └─ Clock                    → ClockController.cs
```

---

## Notes for teammates — Office & Library puzzles

See the bottom of the previous version of this guide for full PuzzleBase API
reference, or read PuzzleBase.cs directly. Key points:

- Extend `PuzzleBase`
- Override `OnPuzzleActivated()`, `CheckSolution()`, `OnPuzzleCompleted()`
- Call `CompletePuzzle()` when solved (auto-gives key if `giveKeyDirectly = true`)
- Call `GameManager.Instance?.SetState(GameState.SolvingPuzzle)` to lock player
- Call `GameManager.Instance?.ReturnToExploring()` to unlock player
- Implement `IInteractable` on anything the player clicks
- Every interactable object needs a Collider
- Use `SoundManager.Instance?.PlaySFX("puzzle_solve")` for sounds
- Look at `ParlorPuzzleBox.cs` as the reference implementation
