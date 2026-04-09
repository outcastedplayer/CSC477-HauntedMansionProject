public enum GameState { Exploring, SolvingPuzzle, AtFrontDoor, Escaped, Trapped }
public enum PlayerState { Idle, Walking, Interacting, InUI, Frozen }
public enum PuzzleState { Inactive, Active, InProgress, Solved, Failed }
public enum UIState { HUD, InventoryOpen, PuzzleUI, CluePanel, PauseMenu, WinScreen, LoseScreen }
public enum RoomType { Lobby, Office, Library, Parlor }
public enum DoorState { FullyLocked, PartiallyLocked, Unlocked, Opening, Open }
public enum ClockState { Running, Warning, Expired, Stopped }
public enum CutsceneState { Idle, Playing, Finished }
public enum InventoryState { Hidden, Visible, ItemSelected, UsingItem }
public enum InteractionState { Ready, Hovering, Interacting, Disabled }
