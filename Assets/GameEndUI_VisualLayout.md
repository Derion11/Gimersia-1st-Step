# Visual UI Layout Reference

## Game End UI Screen Layout

```
┌───────────────────────────────────────────────────────────────┐
│                                                                 │
│  EndGamePanel (Full Screen Semi-Transparent Black Overlay)    │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │                                                           │  │
│  │              VictoryBox (800 x 600 White Box)           │  │
│  │  ┌───────────────────────────────────────────────────┐  │  │
│  │  │                                                     │  │  │
│  │  │              ┌─────────────────────┐               │  │  │
│  │  │              │   PLAYER 1 WINS!    │  ← WinnerText │  │  │
│  │  │              │   (Gold, Size 72)   │               │  │  │
│  │  │              └─────────────────────┘               │  │  │
│  │  │                                                     │  │  │
│  │  │                    ┌─────────┐                     │  │  │
│  │  │                    │         │                     │  │  │
│  │  │                    │  🎮👤   │  ← WinnerIcon      │  │  │
│  │  │                    │ (200x   │    (Optional)       │  │  │
│  │  │                    │  200)   │                     │  │  │
│  │  │                    └─────────┘                     │  │  │
│  │  │                                                     │  │  │
│  │  │  ┌──────────────────────────────────────────────┐  │  │  │
│  │  │  │        ButtonContainer (700 x 120)          │  │  │  │
│  │  │  │                                              │  │  │  │
│  │  │  │   ┌─────────┐  ┌──────────┐  ┌─────────┐   │  │  │  │
│  │  │  │   │RESTART  │  │  MAIN    │  │  QUIT   │   │  │  │  │
│  │  │  │   │ (Green) │  │  MENU    │  │  (Red)  │   │  │  │  │
│  │  │  │   │ 200x80  │  │ (Blue)   │  │ 200x80  │   │  │  │  │
│  │  │  │   └─────────┘  │ 200x80   │  └─────────┘   │  │  │  │
│  │  │  │                └──────────┘                 │  │  │  │
│  │  │  └──────────────────────────────────────────────┘  │  │  │
│  │  │                                                     │  │  │
│  │  └───────────────────────────────────────────────────┘  │  │
│  │                                                           │  │
│  └─────────────────────────────────────────────────────────┘  │
│                                                                 │
└───────────────────────────────────────────────────────────────┘
```

## Hierarchy Tree Structure

```
🎨 GameCanvas (Canvas)
│   Component: Canvas
│   Component: Canvas Scaler
│   Component: Graphic Raycaster
│   Component: GameEndUI ← ATTACH SCRIPT HERE
│
└── 📦 EndGamePanel (Panel/Image)
    │   Color: Black (0, 0, 0, 200)
    │   Stretch: Full Screen
    │   DISABLED at start ✓
    │
    └── 📦 VictoryBox (Image)
        │   Size: 800 x 600
        │   Color: White or Light
        │   Position: Center
        │
        ├── 📝 WinnerText (Text/TextMeshPro)
        │   Text: "Player 1 Wins!"
        │   Size: 72
        │   Color: Gold
        │   Alignment: Center
        │   Position: Top area
        │
        ├── 🖼️ WinnerIcon (Image)
        │   Size: 200 x 200
        │   Preserve Aspect: Yes
        │   Position: Center
        │   (Optional - shows pawn sprite)
        │
        └── 📦 ButtonContainer (Panel/Empty)
            │   Size: 700 x 120
            │   Position: Bottom area
            │
            ├── 🔘 RestartButton (Button)
            │   │   Size: 200 x 80
            │   │   Color: Green
            │   │   Position: Left (-220, 0)
            │   │
            │   └── 📝 Text: "Restart"
            │
            ├── 🔘 MainMenuButton (Button)
            │   │   Size: 200 x 80
            │   │   Color: Blue
            │   │   Position: Center (0, 0)
            │   │
            │   └── 📝 Text: "Main Menu"
            │
            └── 🔘 QuitButton (Button)
                │   Size: 200 x 80
                │   Color: Red
                │   Position: Right (220, 0)
                │
                └── 📝 Text: "Quit"
```

## RectTransform Settings Quick Reference

### EndGamePanel
- Anchors: Min(0, 0), Max(1, 1) ← Stretch both
- Pivot: (0.5, 0.5)
- Position: (0, 0, 0)
- Left: 0, Right: 0, Top: 0, Bottom: 0

### VictoryBox
- Anchors: Min(0.5, 0.5), Max(0.5, 0.5) ← Center
- Pivot: (0.5, 0.5)
- Position: (0, 0, 0)
- Width: 800, Height: 600

### WinnerText
- Anchors: Min(0.5, 1), Max(0.5, 1) ← Top Center
- Pivot: (0.5, 0.5)
- Position: (0, -100, 0)
- Width: 700, Height: 150

### WinnerIcon
- Anchors: Min(0.5, 0.5), Max(0.5, 0.5) ← Center
- Pivot: (0.5, 0.5)
- Position: (0, 50, 0)
- Width: 200, Height: 200

### ButtonContainer
- Anchors: Min(0.5, 0), Max(0.5, 0) ← Bottom Center
- Pivot: (0.5, 0.5)
- Position: (0, 80, 0)
- Width: 700, Height: 120

### RestartButton
- Anchors: Min(0.5, 0.5), Max(0.5, 0.5) ← Center
- Pivot: (0.5, 0.5)
- Position: (-220, 0, 0)
- Width: 200, Height: 80

### MainMenuButton
- Anchors: Min(0.5, 0.5), Max(0.5, 0.5) ← Center
- Pivot: (0.5, 0.5)
- Position: (0, 0, 0)
- Width: 200, Height: 80

### QuitButton
- Anchors: Min(0.5, 0.5), Max(0.5, 0.5) ← Center
- Pivot: (0.5, 0.5)
- Position: (220, 0, 0)
- Width: 200, Height: 80

## Color Codes (RGB 0-255)

### Recommended Colors:
- **EndGamePanel Background**: (0, 0, 0, 200) - Semi-transparent black
- **VictoryBox**: (255, 250, 220, 255) - Light cream
- **WinnerText**: (255, 215, 0, 255) - Gold
- **RestartButton**: (100, 200, 100, 255) - Green
- **MainMenuButton**: (100, 150, 255, 255) - Blue
- **QuitButton**: (255, 100, 100, 255) - Red

## Font Settings

### WinnerText:
- **Font Size**: 72
- **Font Style**: Bold
- **Alignment**: Horizontal Center + Vertical Center
- **Best Fit**: Optional (enable for auto-resize)
- **Color**: Gold (255, 215, 0)

### Button Text (all buttons):
- **Font Size**: 36
- **Font Style**: Bold
- **Alignment**: Center
- **Color**: White (255, 255, 255)

## Animation Preview

### Panel Appearance (0.5 seconds):
```
Frame 0:    Scale = 0.0    (Invisible)
Frame 25%:  Scale = 0.5    (Growing)
Frame 50%:  Scale = 0.9    (Almost full)
Frame 75%:  Scale = 1.1    (Overshoot)
Frame 100%: Scale = 1.0    (Final position)
```

This creates a smooth "pop-in" effect with slight bounce.

## Inspector Connection Diagram

```
┌─────────────────────────────────────┐
│  gridBoard (Script Component)      │
│  ┌───────────────────────────────┐ │
│  │ Game End                      │ │
│  │  Game End UI: [GameCanvas]━━━━━━━━┐
│  └───────────────────────────────┘ │ │
└─────────────────────────────────────┘ │
                                        │
                                        ▼
┌─────────────────────────────────────────────────┐
│  GameCanvas (GameEndUI Script Component)       │
│  ┌──────────────────────────────────────────┐  │
│  │ UI References                            │  │
│  │  End Game Panel: [EndGamePanel]          │  │
│  │  Winner Text: [WinnerText]               │  │
│  │  Winner Icon: [WinnerIcon]               │  │
│  │ Optional Buttons                         │  │
│  │  Restart Button: [RestartButton]         │  │
│  │  Main Menu Button: [MainMenuButton]      │  │
│  │  Quit Button: [QuitButton]               │  │
│  │ Audio Settings                           │  │
│  │  Victory SFX: "victory"                  │  │
│  │ Animation Settings                       │  │
│  │  Animation Duration: 0.5                 │  │
│  └──────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

## Step-by-Step Creation Order

1. ✅ Create Canvas
2. ✅ Add EventSystem (auto-created)
3. ✅ Create EndGamePanel (child of Canvas)
4. ✅ Create VictoryBox (child of EndGamePanel)
5. ✅ Create WinnerText (child of VictoryBox)
6. ✅ Create WinnerIcon (child of VictoryBox)
7. ✅ Create ButtonContainer (child of VictoryBox)
8. ✅ Create RestartButton (child of ButtonContainer)
9. ✅ Create MainMenuButton (child of ButtonContainer)
10. ✅ Create QuitButton (child of ButtonContainer)
11. ✅ Add GameEndUI script to GameCanvas
12. ✅ Connect all references in Inspector
13. ✅ Disable EndGamePanel
14. ✅ Test in Play mode

## Preview of Final Result

When a player wins:
1. Screen darkens (EndGamePanel overlay)
2. White victory box pops up with bounce
3. "Player X Wins!" text displays in gold
4. Winner's pawn sprite shows (optional)
5. Three buttons appear below
6. Victory sound plays
7. Game is locked (no more moves)

**The UI provides clear visual feedback and options to continue!**
