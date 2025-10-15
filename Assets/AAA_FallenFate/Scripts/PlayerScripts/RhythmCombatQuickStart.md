# Rhythm Combat - Quick Start Guide

## 🎵 What It Does

Attacks on-beat = **FASTER** recovery (cancel into combos)  
Attacks off-beat = **SLOWER** recovery (sluggish, punishing)

## ⚡ Quick Setup (2 minutes)

### Step 1: Replace Component
On your **Player** GameObject:
1. Remove `PlayerCombatImproved`
2. Add `PlayerCombatRhythm`

### Step 2: Drag & Drop References
- **Rhythm Judge** ← Drag your `RhythmBonusJudge` component
- **Hitbox Prefab** ← Your hitbox
- **Player Controller** ← Your PlayerController
- **Combo Attacks** ← Your attack data assets

### Step 3: Adjust Feel (Optional)
```
Perfect Recovery Mult: 0.5  ← 50% faster (default)
Miss Recovery Mult: 1.5     ← 50% slower (default)
Rhythm Cancel Enabled: ✓    ← Allow early cancels
```

### Step 4: Test!
Press Play and attack:
- **On-beat** → Smooth, fast combos ✨
- **Off-beat** → Slow, clunky feel 🐌

## 🎮 How It Feels

### PERFECT Hit (on-beat)
```
Attack → [50ms startup] → Hit → [100ms recovery] → NEXT ATTACK!
         ⚡ Super responsive
```

### MISS Hit (off-beat)
```
Attack → [50ms startup] → Hit → [300ms recovery] → ...wait... → next attack
         😴 Sluggish and punishing
```

## 🔧 Tuning Guide

### Make it MORE skill-based:
```csharp
perfectRecoveryMult = 0.3f;  // Even faster on perfect
missRecoveryMult = 2.0f;     // More punishing on miss
```

### Make it MORE forgiving:
```csharp
perfectRecoveryMult = 0.7f;  // Less reward
missRecoveryMult = 1.2f;     // Less punishment
```

### Turn OFF rhythm (classic combat):
```csharp
perfectRecoveryMult = 1.0f;
goodRecoveryMult = 1.0f;
missRecoveryMult = 1.0f;
rhythmCancelEnabled = false;
```

## 💡 Pro Tips

1. **Visual Feedback**: Enable "Show Rhythm Feedback" to see timing in console
2. **Practice Mode**: Start with forgiving settings, tighten as you improve  
3. **BPM Matters**: Slower music (60-90 BPM) = easier to hit beats
4. **Latency**: Adjust "Latency Offset" in RhythmBonusJudge if timing feels off

## 🎯 Expected Player Experience

**Beginner:**
- Randomly hits some beats
- Notices attacks "feel better" sometimes
- Gradually learns the rhythm

**Intermediate:**
- Consciously times attacks to music
- Can chain 3-4 hit combos consistently
- Understands risk/reward of timing

**Advanced:**
- Perfect timing muscle memory
- Extended combos (8+ hits)
- Uses rhythm to control combat flow

## ⚠️ Troubleshooting

**"Not working"**
- Is music playing? Check MusicManager in scene
- Is RhythmJudge assigned? Check Inspector
- Console messages enabled? Check "Show Rhythm Feedback"

**"Too hard/easy"**
- Adjust multipliers in Inspector
- Check timing windows in RhythmBonusJudge component
- Try different BPM music

**"Feels delayed"**
- Adjust "Latency Offset" in RhythmBonusJudge
- Check your monitor's input lag
- Try predictive timing (attack slightly early)

## 📊 Comparison

| System | Perfect Hit | Miss Hit | Skill Ceiling |
|--------|------------|----------|---------------|
| **Standard Combat** | Same | Same | Low |
| **Rhythm Combat** | 2x faster | 1.5x slower | High |

The rhythm system adds **depth without complexity** - easy to understand, hard to master!
