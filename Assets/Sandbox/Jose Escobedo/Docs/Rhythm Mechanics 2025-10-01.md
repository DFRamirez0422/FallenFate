# Core Combat & Rhythm Mechanics

Owners: Jose Escobedo (Combat Programmer – Rhythm) + Erik Aguiar (Combat Programmer – Attacks)

## Tasks:

### Beat-Based Attack Bonus System

- [ ] Implement combo bonus counters at 3/6/9 hits
- [ ] Counter increments only on perfect (on-beat) hits
- [ ] Counter resets on misses
- [ ] Integrate parry window detection with rhythm timing

### Blocking System

- [ ] No Block Attempt: Player takes 1 full heart of damage
- [ ] Off-Rhythm Block: Player takes ½ heart of damage
- [ ] On-Rhythm Block (Parry):
    - Player takes no damage
    - Enemy is briefly stunnedPlayer is granted a free hit (Riposte)
    - Note: Riposte is just a normal attack, but flagged as a free hit awarded from a successful parry

### Perry/Perfect Block

Press a button and you get I-frames and don't take damage for a few seconds. It's a prototype so it doesn't need to look pretty.
You normally take damage but if you perry, you don't take damage. If you hit on beat, no damage. If you hit off beat, chip damage.