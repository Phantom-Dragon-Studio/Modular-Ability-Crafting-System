# MACS Examples

This folder contains example scripts demonstrating how to use the Modular Ability Crafting System in your game.

## Example Scripts

### ExampleSpellCasterPlayer.cs
**Purpose:** Player controller with spell casting capabilities

**Features:**
- Implements `ISpellCaster` interface
- Mana management (current, max, regeneration)
- Hotbar spell slots (6 slots by default)
- Input handling (number keys 1-6, mouse buttons)
- Level progression
- Integration with Spellbook

**How to Use:**
1. Attach to your player GameObject
2. Add a `Spellbook` component
3. Reference or let it auto-find `SpellExecutor` in scene
4. Assign spells to slots via code or inspector
5. Use number keys or mouse buttons to cast

**Example Setup:**
```csharp
ExampleSpellCasterPlayer player = GetComponent<ExampleSpellCasterPlayer>();
player.AssignSpellToSlot(fireballSpell, 0); // Slot 1
player.AssignSpellToSlot(iceBoltSpell, 1);  // Slot 2
```

---

### ExampleDamageableTarget.cs
**Purpose:** Target that can receive damage and status effects

**Features:**
- Implements `IDamageable` interface
- Health system (current, max)
- Visual damage feedback (material flash)
- Death handling (VFX, sound, destruction)
- Status effect integration via `StatusEffectManager`
- Debug damage numbers

**How to Use:**
1. Attach to enemies, destructible objects, etc.
2. Add `StatusEffectManager` component (automatic)
3. Configure health, materials, VFX
4. Works automatically with all spell damage

---

## Quick Setup Guide

### Minimal Scene Setup

1. **Create SpellExecutor:**
   - Create empty GameObject named "SpellExecutor"
   - Add `SpellExecutor` component
   - Configure default prefabs (optional)

2. **Setup Player:**
   - Add `ExampleSpellCasterPlayer` to player GameObject
   - Add `Spellbook` component
   - Reference `SpellExecutor`
   - Configure mana (100 current, 100 max, 5 regen/sec)

3. **Create Target:**
   - Add `ExampleDamageableTarget` to enemy GameObject
   - Configure health (100 default)
   - Add collider and rigidbody

4. **Create Spells:**
   - Right-click → Create → MACS → Spell Template
   - Configure Focus, Essence, Runes
   - Add to player's Spellbook

5. **Assign to Slots:**
   ```csharp
   Spellbook book = player.GetComponent<Spellbook>();
   book.AddSpell(fireballSpell);
   book.ActivateSpell(fireballSpell);
   player.AssignSpellToSlot(fireballSpell, 0);
   ```

6. **Play and Test:**
   - Press `1` to cast slot 1
   - Press `2` to cast slot 2
   - Click to cast from mouse buttons

---

## Integration Examples

### Custom Player Controller

```csharp
using ModularAbilityCraftingSystem.Abilities.Interfaces;

public class MyPlayerController : MonoBehaviour, ISpellCaster
{
    private ExampleSpellCasterPlayer spellCaster;

    void Start()
    {
        spellCaster = GetComponent<ExampleSpellCasterPlayer>();
    }

    // Your custom casting logic
    public void CastSpellAtTarget(int slotIndex, Transform target)
    {
        spellCaster.CastSpellFromSlot(slotIndex);
    }

    // Implement ISpellCaster methods...
}
```

### UI Integration

```csharp
using UnityEngine.UI;

public class SpellHotbarUI : MonoBehaviour
{
    public ExampleSpellCasterPlayer player;
    public Image[] slotIcons;
    public Text[] cooldownTexts;

    void Update()
    {
        UpdateHotbar();
    }

    void UpdateHotbar()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            var socket = player.GetSocket(i);
            if (socket != null && !socket.IsEmpty)
            {
                slotIcons[i].sprite = socket.AssignedSpell.SpellIcon;

                var castable = socket.GetCastableSpell();
                if (castable.IsOnCooldown())
                {
                    cooldownTexts[i].text = $"{castable.GetRemainingCooldown():F1}s";
                }
            }
        }
    }
}
```

### AI Enemy Casting

```csharp
public class AISpellCaster : MonoBehaviour, ISpellCaster
{
    public SpellTemplate[] aiSpells;
    public SpellExecutor executor;

    void AttackPlayer()
    {
        SpellTemplate spell = ChooseSpell();
        Vector3 direction = (player.position - transform.position).normalized;
        executor.CastSpell(spell, transform.position, direction, transform);
    }

    SpellTemplate ChooseSpell()
    {
        // Your AI logic here
        return aiSpells[Random.Range(0, aiSpells.Length)];
    }

    // Implement ISpellCaster methods...
}
```

---

## Tips

1. **Mana Management:**
   - Balance mana costs with regeneration rate
   - Consider mana potions or mana cost reductions
   - Higher level spells should cost more

2. **Spell Slots:**
   - 6 slots is a good default
   - Can expand based on player progression
   - Group similar spells (damage, utility, defense)

3. **Input Handling:**
   - Number keys for quick access
   - Mouse buttons for primary/secondary spells
   - Consider controller support for console games

4. **Performance:**
   - `SpellExecutor` handles object pooling automatically
   - Limit simultaneous active spells
   - Use LOD for spell VFX

5. **Balance:**
   - Test spell combinations for exploits
   - Watch for infinite mana loops
   - Ensure high-cost spells feel powerful

---

## Common Issues

### Q: Spells don't cast
**A:** Check:
- Is SpellExecutor in scene?
- Does spell have Focus and Essence?
- Does player have enough mana?
- Is spell on cooldown?

### Q: Targets don't take damage
**A:** Check:
- Does target have `IDamageable` component?
- Does target have collider?
- Is spell reaching target?
- Check layer collision matrix

### Q: Status effects don't apply
**A:** Check:
- Does target have `StatusEffectManager`?
- Is mastery rune configured with status effect definition?
- Check apply chance (might be RNG)

---

For more examples, see SPELL_EXAMPLES.md in the root directory.
