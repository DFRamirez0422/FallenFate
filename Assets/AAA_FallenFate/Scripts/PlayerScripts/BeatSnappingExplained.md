# Beat Snapping - Preventing Death Spirals

## The Problem You Identified 🎯

**Question:** "Once I miss and recovery is slowed, can I still attack on beat or will it always be off beat?"

This is a **critical design question** that affects whether the rhythm system feels fair or frustrating.

## The Scenarios

### Scenario 1: Without Beat Snapping (Current Default)

```
Beat:     |----1----|----2----|----3----|----4----|----5----|
Attack 1:      ⚔️ (MISS)
Recovery:      [=========450ms=========]
Attack 2:                           ⚔️ (Player presses here)
Result:                            MISS (still off-beat!)
Recovery:                           [=========450ms=========]
Attack 3:                                              ⚔️
Result:                                               MISS again!

Problem: DEATH SPIRAL 💀
- First miss → Slow recovery → Pushed off beat → Miss again → Forever off-beat
```

### Scenario 2: With Beat Snapping (New Option)

```
Beat:     |----1----|----2----|----3----|----4----|----5----|
Attack 1:      ⚔️ (MISS)
Recovery:      [=========450ms=========]
Input:                         🎮 (Player presses here)
System:                        "Wait for beat 4..."
Attack 2:                                    ⚔️ (Auto-executes on beat 4)
Result:                                     PERFECT! ✨
Recovery:                                    [====200ms====]
Attack 3:                                              ⚔️
Result:                                               PERFECT! ✨

Solution: RECOVERY MECHANISM ✅
- First miss → Slow recovery → Input buffered → Auto-snaps to next beat → Back on rhythm!
```

## Comparison Table

| Feature | Standard Combat | Rhythm (No Snap) | Rhythm (With Snap) |
|---------|----------------|------------------|-------------------|
| **Miss Recovery** | 200ms | 300ms | 300ms |
| **Next Attack Timing** | Immediate | Off-beat likely | Waits for beat |
| **Skill Floor** | Low | Medium | Low |
| **Skill Ceiling** | Low | High | High |
| **Forgiveness** | N/A | Low | High |
| **Death Spiral Risk** | None | **HIGH** ⚠️ | **LOW** ✅ |

## Which Script to Use?

### Use `PlayerCombatRhythm.cs` (No Snapping) If:
✅ You want **pure skill-based** combat  
✅ Players should be punished for bad timing  
✅ You want a **high skill ceiling**  
✅ Your game is aimed at **rhythm game veterans**

**Feel:** Unforgiving but rewarding when mastered

### Use `PlayerCombatRhythmSnap.cs` (With Snapping) If:
✅ You want **accessible** rhythm combat  
✅ Players should have a **recovery path** from mistakes  
✅ You want rhythm to feel **natural, not punishing**  
✅ Your game is for **general audiences**

**Feel:** Forgiving but still rewards good timing

## Beat Snapping Settings

### Option 1: Soft Snapping (Recommended)
```csharp
enableBeatSnapping = true;
maxSnapDelay = 0.3f;        // Wait up to 300ms for next beat
alwaysSnapToBeats = false;  // Only snap buffered inputs
```

**What it does:**
- Normal attacks execute immediately (can be on/off beat)
- Buffered attacks wait for next beat window
- Prevents death spirals without forcing rhythm

### Option 2: Hard Snapping (Musical)
```csharp
enableBeatSnapping = true;
maxSnapDelay = 0.5f;        // Wait longer for beats
alwaysSnapToBeats = true;   // Force ALL attacks to snap
```

**What it does:**
- ALL attacks wait for beat windows
- Guaranteed on-beat attacks
- Feels more like a rhythm game
- Slight input delay

### Option 3: No Snapping (Pure Skill)
```csharp
enableBeatSnapping = false;
```

**What it does:**
- Attacks execute exactly when pressed
- No automatic correction
- Pure player skill determines timing
- Risk of death spirals

## Visual Examples

### Death Spiral (No Snapping)
```
Time:     0ms        500ms       1000ms      1500ms
Beat:     |----1----|----2----|----3----|----4----|
Input:    ⚔️         (recovery)            ⚔️
Timing:   MISS 450ms                       MISS 450ms
Next:     (recovery)                       (recovery)            ⚔️
Result:   MISS                             MISS                  MISS

Player: "Why is every attack off-beat?! I hate this!" 😡
```

### Recovery Path (With Snapping)
```
Time:     0ms        500ms       1000ms      1500ms
Beat:     |----1----|----2----|----3----|----4----|
Input:    ⚔️         (recovery) 🎮
Timing:   MISS 450ms            [waiting...]  ⚔️
Next:                                        PERFECT 200ms
Result:   MISS                               PERFECT             ⚔️

Player: "I got back on beat! This feels good!" 😊
```

## Implementation Examples

### Example 1: Forgiving Rhythm Game
```csharp
// In PlayerCombatRhythmSnap:
enableBeatSnapping = true;
maxSnapDelay = 0.4f;
alwaysSnapToBeats = false;

perfectRecoveryMult = 0.6f;  // Moderate reward
missRecoveryMult = 1.4f;     // Gentle punishment

Result: Accessible, fun, no death spirals
```

### Example 2: Hardcore Rhythm Combat
```csharp
// In PlayerCombatRhythm (no snapping):
perfectRecoveryMult = 0.3f;  // Huge reward
missRecoveryMult = 2.5f;     // Severe punishment

Result: High skill ceiling, mastery required
Warning: Death spirals possible!
```

### Example 3: Musical Fighter (Forced Beat)
```csharp
// In PlayerCombatRhythmSnap:
enableBeatSnapping = true;
maxSnapDelay = 0.6f;
alwaysSnapToBeats = true;    // Force rhythm!

perfectRecoveryMult = 0.5f;
missRecoveryMult = 1.5f;

Result: Every attack is on-beat, very musical feel
```

## Player Psychology

### Without Snapping (High Risk):
**First Miss:** "Oops, mistimed that"  
**Second Miss:** "Wait, why am I still off?"  
**Third Miss:** "This system is broken!" ❌

**Only works if players understand they need to:**
1. Recognize they're in a slow recovery
2. Manually wait for the next beat
3. Re-sync themselves

**Skill Required:** HIGH

### With Snapping (Forgiving):
**First Miss:** "Oops, mistimed that"  
**System Helps:** *Waits for next beat*  
**Back on Track:** "Oh, I'm back in rhythm!" ✅

**What players learn:**
1. Missing feels bad (slow recovery)
2. System helps them recover
3. Staying on-beat feels better

**Skill Required:** MEDIUM

## Recommendation

For your game, I recommend **starting with `PlayerCombatRhythmSnap`** because:

1. **No Death Spirals** - Players can always recover
2. **Natural Learning** - System teaches rhythm without forcing it
3. **Skill Progression** - Still rewards perfect timing
4. **Player Retention** - Less frustrating for newcomers
5. **Tunable** - Can disable snapping later if too easy

You can always switch to the non-snapping version if you find players are mastering it too easily!

## Quick Setup

### Replace Current Script
```
Remove: PlayerCombatRhythm
Add: PlayerCombatRhythmSnap
```

### Configure Beat Snapping
```
✓ Enable Beat Snapping: true
  Max Snap Delay: 0.3
  Always Snap To Beats: false  (for soft assistance)
```

### Test Both Modes
1. Play with snapping ON → see if it feels right
2. Toggle snapping OFF → see if it's too hard
3. Adjust `maxSnapDelay` to tune forgiveness

The sweet spot is usually **0.2-0.4 seconds** of snap delay!
