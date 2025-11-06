# Spell Examples

This document provides examples of how to create various spell combinations using the Modular Ability Crafting System (MACS).

## Basic Spells

### Fireball
**Description:** A basic fire projectile
**Components:**
- **Focus:** Actus
- **Essence:** Igni (Fire)
- **Runes:** Move

**Code:**
```csharp
SpellTemplate fireball = new SpellBuilder()
    .WithName("Fireball")
    .WithFocus(actusFocus)
    .WithEssence(igniEssence)
    .AddRune(moveRune)
    .Build();
```

### Ice Bolt
**Description:** Freezing projectile
**Components:**
- **Focus:** Actus
- **Essence:** Aqua (Ice)
- **Runes:** Move, Freeze Mastery

---

## Intermediate Spells

### Homing Fireball
**Description:** Fire projectile that seeks enemies
**Components:**
- **Focus:** Actus
- **Essence:** Igni
- **Runes:** Move, Homing

**Code:**
```csharp
SpellTemplate homingFireball = new SpellBuilder()
    .WithName("Homing Fireball")
    .WithFocus(actusFocus)
    .WithEssence(igniEssence)
    .AddRune(moveRune)
    .AddRune(homingRune)
    .Build();
```

### Triple Lightning Bolt
**Description:** Three electric projectiles in a spread pattern
**Components:**
- **Focus:** Actus
- **Essence:** Aura (Electric)
- **Runes:** Move, Duplicate (3 projectiles, arc spread)

**Code:**
```csharp
SpellTemplate tripleLightning = new SpellBuilder()
    .WithName("Triple Lightning")
    .WithFocus(actusFocus)
    .WithEssence(auraEssence)
    .AddRune(moveRune)
    .AddRune(duplicateRune) // Configure for 3 projectiles
    .Build();
```

### Bouncing Rock
**Description:** Earth projectile that bounces off surfaces
**Components:**
- **Focus:** Actus
- **Essence:** Gaea (Earth)
- **Runes:** Move, Bounce (3 bounces)

---

## Advanced Spells

### Meteor Shower
**Description:** Fireballs rain from the sky with explosions
**Components:**
- **Focus:** Actus
- **Essence:** Igni
- **Runes:** Rain, Duplicate, Explosion, Size

**Code:**
```csharp
SpellTemplate meteorShower = new SpellBuilder()
    .WithName("Meteor Shower")
    .WithFocus(actusFocus)
    .WithEssence(igniEssence)
    .AddRune(rainRune)
    .AddRune(duplicateRune) // 5+ projectiles
    .AddRune(explosionRune)
    .AddRune(sizeRune)
    .Build();
```

### Chain Lightning
**Description:** Lightning that jumps between multiple enemies
**Components:**
- **Focus:** Actus
- **Essence:** Aura
- **Runes:** Move, Homing, Chain, Shock Mastery

**Code:**
```csharp
SpellTemplate chainLightning = new SpellBuilder()
    .WithName("Chain Lightning")
    .WithFocus(actusFocus)
    .WithEssence(auraEssence)
    .AddRune(moveRune)
    .AddRune(homingRune)
    .AddRune(chainRune) // Chain to 3 targets
    .AddRune(shockMasteryRune)
    .Build();
```

### Overcharged Meteor
**Description:** Massive, powerful fire projectile with explosion
**Components:**
- **Focus:** Actus
- **Essence:** Igni
- **Runes:** Move, Size, Overcharge, Explosion, Burn Mastery

**Code:**
```csharp
SpellTemplate overchargedMeteor = new SpellBuilder()
    .WithName("Overcharged Meteor")
    .WithFocus(actusFocus)
    .WithEssence(igniEssence)
    .AddRune(moveRune)
    .AddRune(sizeRune)
    .AddRune(overchargeRune)
    .AddRune(explosionRune)
    .AddRune(burnMasteryRune)
    .Build();
```

---

## Utility Spells

### Ice Bridge
**Description:** Creates a platform of ice
**Components:**
- **Focus:** Creo
- **Essence:** Aqua
- **Runes:** (none needed for basic bridge)

### Fire Shield
**Description:** Orbiting fire orbs that protect the caster
**Components:**
- **Focus:** Ego
- **Essence:** Igni
- **Runes:** Orbit, Duplicate, Burn Mastery

**Code:**
```csharp
SpellTemplate fireShield = new SpellBuilder()
    .WithName("Fire Shield")
    .WithFocus(egoFocus)
    .WithEssence(igniEssence)
    .AddRune(orbitRune)
    .AddRune(duplicateRune) // 6 orbs
    .AddRune(burnMasteryRune)
    .Build();
```

### Dash
**Description:** Quick movement burst
**Components:**
- **Focus:** Ego
- **Essence:** Neutral
- **Runes:** Move

---

## Creative Combinations

### Spiral Freeze Barrage
**Description:** Spiraling ice projectiles that freeze targets
**Components:**
- **Focus:** Actus
- **Essence:** Aqua
- **Runes:** Move, Spiral, Duplicate, Freeze Mastery

### Piercing Fire Lance
**Description:** Fire beam that pierces through multiple enemies
**Components:**
- **Focus:** Beam
- **Essence:** Igni
- **Runes:** Pierce, Burn Mastery

### Earthquake Meteor
**Description:** Falling rock with knockback shockwave
**Components:**
- **Focus:** Actus
- **Essence:** Gaea
- **Runes:** Rain, Size, Explosion, Knockback Mastery

### Lightning Storm
**Description:** Multiple homing lightning bolts with chain effects
**Components:**
- **Focus:** Actus
- **Essence:** Aura
- **Runes:** Move, Homing, Duplicate, Rain, Chain, Shock Mastery

**Code:**
```csharp
SpellTemplate lightningStorm = new SpellBuilder()
    .WithName("Lightning Storm")
    .WithFocus(actusFocus)
    .WithEssence(auraEssence)
    .AddRune(moveRune)
    .AddRune(homingRune)
    .AddRune(duplicateRune)  // 8 projectiles
    .AddRune(rainRune)        // From above
    .AddRune(chainRune)       // Chain to nearby targets
    .AddRune(shockMasteryRune) // Stun effect
    .Build();
```

---

## Combo Spells (Linked Spells)

### Mine Field
**Description:** Place fire mines that explode on proximity
**Components:**
- **Focus:** Trap
- **Essence:** Igni
- **Runes:** Duplicate, Explosion
- **Linked Spell:** Fireball (casts on trigger)

### Decoy Barrage
**Description:** Create a decoy that shoots fireballs
**Components:**
- **Focus:** Ego
- **Essence:** Neutral
- **Runes:** Duplicate, Periodic
- **Linked Spell:** Fireball

---

## Mana Cost Considerations

### Low Cost (10-30 mana)
- Basic Fireball
- Ice Bolt
- Simple Shield

### Medium Cost (30-60 mana)
- Homing Fireball
- Triple Lightning
- Bouncing Rock
- Fire Shield

### High Cost (60-100+ mana)
- Meteor Shower
- Chain Lightning
- Overcharged Meteor
- Lightning Storm

### Extreme Cost (100+ mana)
- Multi-spell combinations
- Overcharged + Multiple Runes
- Large AOE effects

---

## Tips for Spell Design

1. **Start Simple:** Begin with Focus + Essence + Move
2. **Add One Rune at a Time:** Test each addition
3. **Balance Cost vs Power:** More runes = more mana
4. **Consider Synergies:**
   - Fire + Explosion = Area damage
   - Ice + Slow/Freeze = Control
   - Electric + Chain = Multi-target
   - Earth + Knockback = CC
5. **Test Interaction:** Some runes work better together:
   - Homing + Bounce = Coverage
   - Rain + Duplicate = Saturation
   - Pierce + Chain = Maximum hits
6. **Watch Mana Costs:** Overcharge, Duplicate, and Explosion are expensive

---

## Progression Recommendations

### Early Game (Basic Runes)
- Move
- Size
- Duplicate (2-3 projectiles)

### Mid Game (Add Control)
- Homing
- Bounce
- Burn Mastery
- Freeze Mastery

### Late Game (Advanced Effects)
- Chain
- Explosion
- Pierce
- Overcharge
- Rain
- Orbit
- Spiral

### End Game (Master Combinations)
- Combine 5+ runes
- Use Shock Mastery with Chain
- Multi-spell linked combos
- Knockback + Explosion AOE

---

## Common Mistakes to Avoid

1. **Too Many Runes:** Makes spells too expensive to cast
2. **Conflicting Runes:** Check compatibility (e.g., Orbit + Move might conflict)
3. **No Damage Scaling:** Remember to add Mastery runes for damage
4. **Ignoring Mana:** Build can be unusable if too expensive
5. **Single Purpose:** Create variety in your spellbook

---

## Example Spellbook Loadout

### Balanced Mage
1. **Fireball** - Basic damage (Actus + Igni + Move)
2. **Ice Bolt** - Crowd control (Actus + Aqua + Move + Freeze)
3. **Chain Lightning** - Multi-target (Actus + Aura + Move + Chain)
4. **Meteor** - High damage (Actus + Igni + Move + Size + Explosion)
5. **Shield** - Defense (Ego + Neutral + Orbit)
6. **Dash** - Mobility (Ego + Neutral + Move)

### Crowd Control Specialist
1. **Freeze Ray** (Beam + Aqua + Freeze)
2. **Shock Nova** (Area + Aura + Shock)
3. **Knockback Blast** (Immedi + Gaea + Knockback + Explosion)
4. **Ice Wall** (Creo + Aqua)
5. **Root Trap** (Trap + Gaea)

### Artillery Mage
1. **Meteor Shower** (Actus + Igni + Rain + Duplicate + Explosion)
2. **Lightning Storm** (Actus + Aura + Rain + Duplicate + Chain)
3. **Rock Barrage** (Actus + Gaea + Move + Pierce + Duplicate)

---

For more information on creating custom runes and spells, see the main README.md.
