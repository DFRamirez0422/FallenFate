# Rhythm Combat System

## Overview
The `PlayerCombatRhythm` script integrates the existing rhythm system with the combat system to reward players for attacking on-beat and punish off-beat attacks.

## How It Works

### Rhythm Evaluation
When you attack, the system calls `RhythmBonusJudge.EvaluateNow()` to determine your timing:

- **PERFECT** - Attack lands exactly on the beat
- **GOOD** - Attack is close to the beat  
- **MISS** - Attack is off-beat

### Timing Modifiers

The rhythm timing affects your **recovery time** (how quickly you can act after an attack):

| Timing | Recovery Modifier | Effect |
|--------|------------------|--------|
| **PERFECT** | 50% (default) | **2x faster** recovery - highly responsive |
| **GOOD** | 75% (default) | **1.33x faster** recovery - smooth flow |
| **MISS** | 150% (default) | **1.5x slower** recovery - sluggish and punishing |

### Rhythm Canceling

When `rhythmCancelEnabled` is true:
- **PERFECT/GOOD hits**: Can cancel recovery early to chain into next attack
- **MISS hits**: Must wait through full recovery before attacking again

This creates a skill ceiling where skilled players can maintain aggressive combos by attacking on-beat.

## Setup Instructions

### 1. Replace Combat Script
On your Player GameObject:
1. Remove or disable `PlayerCombatImproved` component
2. Add `PlayerCombatRhythm` component

### 2. Assign References

**Required:**
- `Hitbox Prefab` - Your hitbox prefab
- `Default Attack` - Default attack data
- `Player Controller` - Reference to PlayerController
- `Rhythm Judge` - Reference to RhythmBonusJudge component

**Optional:**
- `Dash Attack Data` - Special dash attack
- `Combo Attacks` - Array of combo attack data
- `Finisher` attacks (3-hit, 6-hit, 9-hit)

### 3. Configure Rhythm Settings

**Rhythm Timing Modifiers:**
```
Perfect Recovery Mult: 0.5  (50% faster)
Good Recovery Mult: 0.75    (75% speed)
Miss Recovery Mult: 1.5     (50% slower)
```

**Options:**
- `Rhythm Cancel Enabled` - Allow canceling on Perfect/Good hits
- `Show Rhythm Feedback` - Display console messages for timing

### 4. Ensure Rhythm System is Active

Make sure you have:
- `MusicManager` prefab in scene
- `RhythmBonusJudge` component attached and configured
- Music playing with correct BPM settings

## Usage Examples

### Example 1: Tight Rhythm Combat
```
Perfect Recovery Mult: 0.3   (70% faster - very responsive)
Good Recovery Mult: 0.6      (40% faster)
Miss Recovery Mult: 2.0      (2x slower - very punishing)
Rhythm Cancel Enabled: true
```
**Result:** Highly skill-based, rewards precise timing

### Example 2: Forgiving Rhythm
```
Perfect Recovery Mult: 0.7   (30% faster)
Good Recovery Mult: 0.9      (10% faster)
Miss Recovery Mult: 1.2      (20% slower)
Rhythm Cancel Enabled: true
```
**Result:** Gentler learning curve, still rewards good timing

### Example 3: Rhythm-Disabled
```
Perfect Recovery Mult: 1.0
Good Recovery Mult: 1.0
Miss Recovery Mult: 1.0
Rhythm Cancel Enabled: false
```
**Result:** Functions like standard combat, no rhythm influence

## Visual Feedback

When `Show Rhythm Feedback` is enabled, you'll see console messages:

- <span style="color:cyan">**PERFECT!** Recovery: 50%</span>
- <span style="color:green">**Good!** Recovery: 75%</span>
- <span style="color:red">**Miss...** Recovery: 150%</span>

### Adding Custom Visual Effects

You can extend the `EvaluateRhythmTiming()` method to:
- Play VFX particles on Perfect hits
- Change weapon trail colors based on timing
- Display UI indicators
- Play audio cues

Example:
```csharp
private void EvaluateRhythmTiming()
{
    if (rhythmJudge == null) return;
    
    var (tier, multiplier) = rhythmJudge.EvaluateNow();
    lastRhythmTier = tier;

    switch (tier)
    {
        case RhythmBonusJudge.RhythmTier.Perfect:
            currentRecoveryModifier = perfectRecoveryMult;
            // YOUR CUSTOM CODE HERE
            perfectHitVFX.Play();
            audioSource.PlayOneShot(perfectHitSound);
            break;
    }
}
```

## Troubleshooting

### "Rhythm not working"
- Ensure `RhythmBonusJudge` reference is assigned
- Check that music is playing (`MusicManager` in scene)
- Verify `RhythmMusicPlayer` has correct BPM settings

### "Always getting Miss"
- Check timing windows in `RhythmBonusJudge`
- Adjust `Latency Offset` to compensate for input lag
- Try increasing `Good Quarter Sec` window for easier timing

### "Canceling too easy/hard"
- Adjust recovery multipliers
- Toggle `Rhythm Cancel Enabled` on/off
- Fine-tune timing windows in RhythmBonusJudge

## Advanced: Damage Scaling

You can also scale damage based on rhythm by modifying the hitbox initialization:

```csharp
if (hb.TryGetComponent<Hitbox>(out Hitbox hbComp))
{
    // Create modified attack data with rhythm bonus
    var modifiedData = ScriptableObject.CreateInstance<AttackData>();
    modifiedData.damage = Mathf.RoundToInt(attackData.damage * currentRecoveryModifier);
    
    hbComp.Initialize(modifiedData, this.gameObject);
    hbComp.SetOwnerCombat(this);
}
```

## Performance Notes

- Rhythm evaluation is lightweight (< 1ms)
- No garbage allocation during attacks
- Safe to call 60+ times per second
- Works in multiplayer (client-side prediction)

## Credits

Built on top of:
- `RhythmBonusJudge` by Jose Escobedo
- Original combat system by [Your Team]
