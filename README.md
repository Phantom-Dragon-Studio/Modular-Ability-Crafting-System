# Modular Ability Crafting System (MACS)

A comprehensive, modular spell crafting system for Unity 6+ inspired by Mages of Mystralia. This package allows players to create custom spells by combining Runes, Focuses, and Essences in endless combinations.

## 🎯 Overview

The Modular Ability Crafting System (MACS) enables dynamic spell creation through a composition-based architecture. Players (or designers) combine:

- **Focuses** - Determine HOW spells manifest (projectile, instant, create, self-buff, etc.)
- **Essences** - Determine elemental properties (fire, ice, arcane, etc.)
- **Runes** - Modify spell behavior (movement, multiplication, augmentation, triggers)

## 🏗️ Architecture

### Core Concepts

#### 1. **Focus System**
Defines the base delivery method of spells:
- **Immedi** - Instant/melee effects (slashes, explosions)
- **Actus** - Projectiles with conditional duration (orbs, missiles)
- **Creo** - Persistent creations (walls, platforms, bridges)
- **Ego** - Self-targeting (shields, buffs, dashes)
- **Area** - Area of effect spells
- **Beam** - Continuous beam effects
- **Trap** - Delayed trigger spells
- **Totem** - Spawned autonomous entities

#### 2. **Essence System**
Defines elemental properties and damage types. Includes 16 essence types:

**Core Elements:**
- Igni (Fire), Aqua (Ice), Aura (Electric), Gaea (Earth)

**Extended Elements:**
- Ventus (Wind), Lux (Light), Umbra (Shadow), Arcanum (Arcane)
- Chaos (Void), Crystal (Gem), Vitae (Blood), Gravitas (Gravity)
- Tempus (Time), Toxin (Poison), Sonus (Sound), Neutral (Pure)

#### 3. **Rune Categories**

| Category | Purpose | Examples |
|----------|---------|----------|
| **Movement** | Control spell trajectory | Move, Homing, Bounce |
| **Multiplication** | Create multiple instances | Duplicate, Split |
| **Augmentation** | Modify spell stats | Size, Overcharge, Time |
| **Impact** | Add explosive effects | Detonate, Explosion |
| **Trigger** | Define activation conditions | OnImpact, OnProximity |

## 🚀 Quick Start

### 1. Setup

Add to your scene:
1. **SpellExecutor** - Handles spell casting
2. **Spellbook** (optional) - Manages spell collection

### 2. Creating Your First Spell

```csharp
using ModularAbilityCraftingSystem.Abilities;

// Build spell
SpellBuilder builder = new SpellBuilder();
SpellTemplate fireball = builder
    .WithName("Fireball")
    .WithFocus(actusFocus)
    .WithEssence(igniEssence)
    .AddRune(moveRune)
    .Build();
```

### 3. Casting a Spell

```csharp
executor.CastSpell(mySpell, transform.position, transform.forward, transform);
```

## 📦 Package Structure

```
Assets/Modular Ability Crafting System/
├── Scripts/
│   ├── Abilities/         # Core spell system
│   ├── Runes/            # Rune implementations
│   ├── Sockets/          # Hotbar & progression
│   └── Runtime Components/  # MonoBehaviour components
```

## 🎨 Creating Custom Runes

Extend the system with your own runes:

```csharp
[CreateAssetMenu(menuName = "MACS/Runes/My Custom Rune")]
public class MyRune : BaseBehaviour
{
    public override void ApplyToSpell(SpellContext context)
    {
        // Modify spell properties
    }

    public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
    {
        // Add runtime components
    }
}
```

## 🎮 Integration

Implement `ISpellCaster` and `ICastable` interfaces for seamless integration with:
- Hotbars
- VR gesture systems
- AI spell casting
- Custom UI

## 🌟 Features

- ✅ Composition-based architecture (SOLID principles)
- ✅ 16 essence types (expandable)
- ✅ 8 focus types (expandable)
- ✅ Comprehensive rune system
- ✅ Runtime spell crafting
- ✅ Validation & balancing rules
- ✅ Spellbook management
- ✅ Talent tree / progression system
- ✅ Object pooling for performance
- ✅ Spell chaining/nesting support
- ✅ Fully documented API

## 📝 License

Phantom Dragon Studio - Modular Ability Crafting System

## 🎓 Credits

Inspired by **Mages of Mystralia** spell crafting system.

---

**Version:** 1.0.0
**Unity Version:** Unity 6+
**Dependencies:** None
