# Console Feedback Guide - PlayerCombatRhythmSnap

## 🎮 Now you'll ALWAYS see feedback when you press attack!

### **Every Input State Explained:**

---

## ✅ **Attack Executed (Rhythm Evaluation)**

### **PERFECT!** 
```
<cyan>PERFECT! Recovery: 50%</cyan>
```
✨ You attacked exactly on-beat!  
→ 50% faster recovery (configurable)  
→ Can cancel into next attack

### **Good!**
```
<green>Good! Recovery: 75%</green>
```
👍 You attacked close to beat!  
→ 25% faster recovery (configurable)  
→ Can cancel into next attack

### **Miss...**
```
<red>Miss... Recovery: 150%</red>
```
💢 You attacked off-beat  
→ 50% slower recovery (configurable)  
→ Cannot cancel, must wait

---

## 🚨 **Spam Detection**

### **⚠️ SPAM! Snapping OFF**
```
<red>⚠️ SPAM! Snapping OFF</red>
```
You're button mashing!  
→ Beat snapping disabled  
→ Attacks will be off-beat  
→ Will get Miss timing

**Fix:** Slow down your inputs!

---

## 📥 **Input States (Why attack didn't execute)**

### **→ Input Buffered**
```
<cyan>→ Input Buffered</cyan>
```
You pressed during an attack  
→ Input saved for after current attack finishes  
→ Will execute with beat snapping (if not spamming)

### **→ Recovery Cancel**
```
<yellow>→ Recovery Cancel</yellow>
```
You pressed during recovery and had Good/Perfect timing  
→ Canceling recovery early  
→ Next attack starting now

### **→ Busy (already attacking)**
```
<gray>→ Busy (already attacking)</gray>
```
You pressed during an attack but buffer is full  
→ Input ignored  
→ Can only buffer 1 input at a time

### **→ Cooldown (0.15s)**
```
<orange>→ Cooldown (0.15s)</orange>
```
Attack cooldown still active  
→ Shows remaining time  
→ Wait for it to finish

---

## 🎵 **Beat Snapping (when waiting)**

### **Waiting for next beat...**
```
<yellow>Waiting for next beat...</yellow>
```
Snapping enabled, looking for beat window  
→ Will execute on next Perfect/Good timing  
→ Max wait time: 0.3 seconds (configurable)

### **Snapped to Perfect!**
```
<cyan>Snapped to Perfect!</cyan>
```
Found perfect beat window!  
→ Attack executing now on-beat  
→ You'll get Perfect timing

### **Snapped to Good!**
```
<cyan>Snapped to Good!</cyan>
```
Found good beat window!  
→ Attack executing now near-beat  
→ You'll get Good timing

### **Snap timeout - attacking anyway**
```
<orange>Snap timeout - attacking anyway</orange>
```
Waited 0.3s but no beat found  
→ Executing anyway  
→ Will likely be Miss timing

---

## 📊 **Typical Console Output Examples**

### **Example 1: Perfect Rhythm Flow**
```
PERFECT! Recovery: 50%
PERFECT! Recovery: 50%
Good! Recovery: 75%
PERFECT! Recovery: 50%
```
**What happened:** Skilled player attacking on-beat consistently

---

### **Example 2: Spam Punishment**
```
PERFECT! Recovery: 50%
⚠️ SPAM! Snapping OFF
Miss... Recovery: 150%
⚠️ SPAM! Snapping OFF
Miss... Recovery: 150%
```
**What happened:** Started well, then mashed buttons

---

### **Example 3: Input Buffering**
```
PERFECT! Recovery: 50%
→ Input Buffered
Snapped to Good!
Good! Recovery: 75%
```
**What happened:** Pressed during attack → buffered → snapped to beat → executed

---

### **Example 4: Spam Recovery**
```
⚠️ SPAM! Snapping OFF
Miss... Recovery: 150%
⚠️ SPAM! Snapping OFF
Miss... Recovery: 150%
[player slows down]
PERFECT! Recovery: 50%
```
**What happened:** Player learned to stop spamming

---

### **Example 5: Cooldown Spam**
```
PERFECT! Recovery: 50%
→ Cooldown (0.25s)
→ Cooldown (0.18s)
→ Cooldown (0.12s)
PERFECT! Recovery: 50%
```
**What happened:** Player mashing during cooldown, then attacks when ready

---

## 🔧 **Enabling/Disabling Feedback**

### **In Inspector:**
```
Show Rhythm Feedback: ✓ (enabled)
```

### **In Code:**
```csharp
showRhythmFeedback = true;  // Show all messages
showRhythmFeedback = false; // Silent mode
```

---

## 🎯 **What Each Message Teaches You**

| Message | Teaches You |
|---------|-------------|
| **PERFECT/Good/Miss** | Your timing accuracy |
| **SPAM** | You're mashing, slow down |
| **Input Buffered** | Your input was saved |
| **Recovery Cancel** | You can chain attacks |
| **Busy** | Already in an attack |
| **Cooldown (X.XXs)** | Wait this long |
| **Waiting for beat** | Snapping is helping you |
| **Snapped to X** | System corrected your timing |

---

## 💡 **Pro Tip: Learn the Rhythm**

Watch the pattern:
```
Good → Good → Good → PERFECT!
```
You're getting closer to the beat!

```
PERFECT → PERFECT → PERFECT → PERFECT
```
You've mastered the rhythm! 🎵⚔️

```
SPAM → Miss → Miss → SPAM → Miss
```
Stop mashing! Learn the timing!

---

Now you have **complete visibility** into what the combat system is doing! 🎮
