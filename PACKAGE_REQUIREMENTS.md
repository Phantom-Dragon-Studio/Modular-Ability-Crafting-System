# Package Requirements

The Modular Ability Crafting System (MACS) is built using **modern Unity systems** and does NOT depend on legacy systems.

## Required Unity Version
- **Unity 6.0+** (or Unity 2022.3 LTS+)

## Required Packages

### 1. Unity Input System
**Package:** `com.unity.inputsystem`
**Version:** 1.7.0 or higher
**Required:** Yes

The example player controller uses the new Input System instead of legacy Input.

**Installation:**
```
Window > Package Manager > Unity Registry > Input System > Install
```

Or add to `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.unity.inputsystem": "1.7.0"
  }
}
```

### 2. UI Toolkit
**Package:** Built-in to Unity 6
**Required:** Yes (for UI examples)

The example UI uses UI Toolkit (UXML/USS) instead of legacy UGUI.

**Note:** UI Toolkit is built into Unity 6 and Unity 2022.3 LTS+. No additional package needed.

---

## Optional Packages

### TextMeshPro
**Package:** `com.unity.textmeshpro`
**Required:** No (but recommended for better text rendering)

While the core system doesn't require TMP, you may want it for better text quality in your game UI.

---

## Package Structure

```
/Assets/Modular Ability Crafting System/
├── Scripts/                      # Core C# scripts (no dependencies)
├── Examples/                     # Example implementations
│   ├── SpellCastingInputActions.inputactions  # Input System asset
│   ├── ExampleSpellCasterPlayer.cs            # Uses Input System
│   └── UI/                       # UI Toolkit examples
│       ├── SpellHotbarUI.uxml    # UI layout
│       ├── SpellHotbarUI.uss     # UI styles
│       └── SpellHotbarUIController.cs # UI controller
```

---

## Migration from Legacy Systems

### If Using Legacy Input

The example scripts use the **new Input System**. If your project still uses legacy Input:

**Option 1: Switch to New Input System** (Recommended)
1. Install Input System package
2. Edit > Project Settings > Player > Active Input Handling > "Input System Package (New)"
3. Restart Unity
4. Use the provided `SpellCastingInputActions.inputactions`

**Option 2: Create Your Own Input Handler**
```csharp
// Your custom legacy input handler
public class LegacyInputSpellCaster : MonoBehaviour
{
    private ExampleSpellCasterPlayer player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            player.CastSpellFromSlot(0);
        // ... etc
    }
}
```

### If Using Legacy UI (UGUI)

The example UI uses **UI Toolkit**. If your project uses UGUI:

**Option 1: Use UI Toolkit** (Recommended)
- Modern, performant, data-driven
- Better separation of concerns
- Easier to maintain

**Option 2: Create UGUI Version**
```csharp
using UnityEngine.UI;

public class UGUISpellHotbar : MonoBehaviour
{
    public Image[] slotIcons;
    public Text[] cooldownTexts;
    public Slider manaBar;
    // ... implement with UGUI components
}
```

---

## Core System Dependencies

The **core spell crafting system** has ZERO external dependencies:
- ✅ No Input System dependency (only examples use it)
- ✅ No UI Toolkit dependency (only examples use it)
- ✅ No TextMeshPro dependency
- ✅ No third-party packages

**You can use MACS with:**
- New Input System OR Legacy Input OR Custom Input OR VR Input
- UI Toolkit OR UGUI OR Custom UI OR No UI
- Any Unity version 2022.3 LTS or higher

---

## Development Dependencies

If you're extending MACS or contributing:
- **Unity 6.0+** for latest features
- **Input System package** for testing examples
- **UI Toolkit** for testing UI examples

---

## Package Installation

### Method 1: Unity Package Manager (Git URL)
```
Window > Package Manager > + > Add package from git URL
https://github.com/Phantom-Dragon-Studio/Modular-Ability-Crafting-System.git
```

### Method 2: Manual Installation
1. Download/clone repository
2. Copy `Assets/Modular Ability Crafting System/` to your project
3. Install required packages (Input System)

### Method 3: Unity Asset Store
*Coming soon*

---

## Verify Installation

After installation, verify everything works:

1. **Check Input System:**
   - Window > Package Manager > Input System (should show version 1.7.0+)

2. **Test Example Scene:**
   - Open `Examples/Demo Scene` (when available)
   - Should compile without errors
   - Player input should work

3. **Create Test Spell:**
```csharp
var builder = new SpellBuilder();
var spell = builder
    .WithName("Test")
    .WithFocus(focus)
    .WithEssence(essence)
    .Build();
// Should build without errors
```

---

## Troubleshooting

### "Input System not found"
**Solution:** Install Input System package via Package Manager

### "PlayerInput component missing"
**Solution:** Add PlayerInput component and assign SpellCastingInputActions asset

### "UI Toolkit documents not rendering"
**Solution:** Ensure UIDocument component has the UXML asset assigned

### "Assembly reference errors"
**Solution:** Enable "Input System" in Player Settings

---

## Compatibility Matrix

| Unity Version | Input System | UI Toolkit | MACS Status |
|---------------|--------------|------------|-------------|
| 2022.3 LTS    | ✅ 1.7.0+    | ✅ Built-in | ✅ Supported |
| 2023.1+       | ✅ 1.7.0+    | ✅ Built-in | ✅ Supported |
| Unity 6       | ✅ 1.7.0+    | ✅ Built-in | ✅ Fully Supported |
| 2021.x        | ⚠️ 1.5.0+    | ⚠️ Limited  | ⚠️ May work (untested) |
| 2020.x        | ❌           | ❌          | ❌ Not Supported |

---

## Future Proofing

MACS is built to be **future-proof**:
- Uses modern Unity APIs
- No legacy system dependencies
- Modular architecture
- Easy to upgrade

As Unity evolves, MACS will continue to use the latest recommended systems.

---

## Questions?

See main README.md or Examples/EXAMPLES_README.md for usage guides.
