he Rhythm System is one of the main gameplay pillars. It ensures combat feels musical, rewarding, and skill-based. Players attack on beat, chaining perfect hits into powerful finishers and cinematic Quick-Time Events (QTEs)

## Perfect Hit System

- Perfect Hit: Landing an attack exactly on the beat (confirmed by visual + audio feedback).  
- Combo Points: Each perfect hit = +1 combo point.  
- Reset Rule: Missing a perfect hit resets combo points to 0.  
- No Timer: Combo points do not decay over time — only reset on a miss.  

## Combo Progression → Special Finishers

At certain thresholds of perfect hits, the player unlocks Special Finishers:

- 3 Perfect Hits → Backfire  
	- Guitar spins vertically forward like a boomerang and ricochets back to the player.  
- 6 Perfect Hits → Sanguine Slash  
	- Wide, blood infused horizontal slash that cuts through enemies in front of the player.  
- 9 Perfect Hits → Bloodborne  
	- Player radiates a purple/red aura that grants a lifesteal buff for a short duration.  

Upgrade Rules:

- Each finisher overrides the previous one.  
- Example: At 3 hits, Backfire is ready. At 6, it upgrades to Sanguine Slash. At 9, it upgrades to Bloodborne.  
- Using a finisher resets combo points to 0.  

## Feedback System

Every perfect hit should give clear rhythm reinforcement:

- Visual: On-screen rings, flashes, or timing UI cues.  
- Audio: Guitar strum / percussive hit sound.  

## Quick-Time Events (QTEs)

- Trigger: Scripted moments during boss fights (slams, grapples, transformations).  
- Mechanics:  
	- A shrinking outer ring aligns with an inner circle.  
	- Player presses attack at the right time to “hit the beat.”  
- Results:  
	- Perfect: Event succeeds + grants 3 stacks Rhythm Bonus Multiplier.  
	- Good: Event succeeds, no bonus.  
	- Miss: Player punished (damage taken, loss of multiplier, or boss gains advantage).  

Integration with Rhythm System:

- A perfect QTE input counts as a “perfect hit” → progresses the combo chain.  

QTEs always complement the rhythm system rather than interrupt it.  

## Rhythm Bonus Multiplier

- Definition: A stackable buff gained from Perfect QTEs (and potentially other rhythm-based rewards).  
- Effect: Increases damage or ability effectiveness  
- Cap: TBD (recommend starting with x3).  
- Reset: Resets when combo points reset.  

## Edge Cases / Clarifications

- Multiple Enemies: Combo points can be built across multiple targets.  
- Finisher Choice: Player may trigger a finisher early (ex: at 3 hits) instead of holding for 6/9.  
- Interruption: Taking damage interrupts the timing window but only resets combo if the attack is missed.  
- UI Integration: The HUD should clearly show current combo count and which finisher is queued.

#  Rhythm System FAQ

### 1. What counts as a “perfect hit”?

- A perfect hit is when the player lands an attack inside the “perfect” timing window.  
- Confirmed by visual cue (timing ring/flash) + audio cue (strum sound).  
- Off-beat attacks still deal damage, but don’t add to combo points.  

### 2. Does missing a beat fully reset progress?

- Yes. If the player misses the rhythm window, combo points reset to 0.  
- Example: At 5 hits → miss → player goes back to 0.  

### 3. Can a player trigger a finisher early?

- Yes.  
- Example: At 3 hits, Backfire unlocks. Player can use it immediately, or keep chaining to reach Sanguine Slash at 6 or Bloodborne at 9.  
- Using any finisher resets combo points to 0.  

### 4. Do specials/finishers count toward the combo?

- No. Specials don’t add to combo points.  
- Yes. Specials still grant Rhythm Bonus Multiplier when used successfully.  

### 5. Do combos carry across multiple enemies?

- Yes.  
- Players can chain perfect hits across different enemies.  
- If one enemy dies mid-chain, the next perfect hit continues the rhythm.  

### 6. How long does Bloodborne (lifesteal aura) last?

- Currently TBD (tunable). Recommend starting with 5–7 seconds.  
- Exact duration and lifesteal % should be data-driven so designers can balance it.  

### 7. What happens during QTEs?

- Boss triggers a Quick Time Event
-  at scripted moments.  
- Player presses input when outer ring aligns with inner circle.  
- Perfect: +3 Rhythm Multiplier stacks + counts as perfect hit.  
- Good: Event succeeds, no bonus.  
- Miss: Player punished (damage, knockback, or boss advantage).  

### 8. Does the Rhythm Multiplier stack forever?

- No. There will be a cap (TBD, suggest x3).  
- Resets when combo points reset.  

### 9. What if the player takes damage mid-combo?

- Taking damage interrupts input window but does not automatically reset the combo.  
- Only missing a beat (late/early input) resets combo points.  

### 10. Is there a time limit between hits?

- No.  
- Combos only reset when a perfect beat is missed — not from waiting.  
- This keeps the system fair and focused on timing, not rushing.  

### 11. How will players know what finisher is queued?

- The UI will display combo count + the currently available finisher.  
- Example: “3 Combo → Backfire Ready” or icon representing the finisher.