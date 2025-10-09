using UnityEngine;

public class RT_BossAI : MonoBehaviour
{
    //When the player enters the boss arena close the door behind them and play a cutscene
    //When the cutscene ends start phase 1

    // Phase 1 Logic
    // Play the boss song
    // Start attack pattern 1

    //Attack pattern 1 logic (4 attacks)

    // Add boss invunerability
    // Do FlySummonAttack
    // Play Idle animation
    // Do MachineGunRiff (This should line up to the song)
    // Play Idle animation
    // Do LongRangeAttacks (1 out of the 3 potential attacks)
    // Play Idle animation
    // MachineGunRiff (This should line up to the song)
    // Play Idle animation

    // After attack pattern 1 remove invunerability and start using CloseRangeAttacks (1 out of 3 potential attacks)
    // If boss health reaches 0 trigger a QTE event
    // 4 QTE's happen and the player needs to hit them to go into phase 2
    // if they fail to hit the QTE's reset the players position to the entrance, reset boss health, and start phase 1
    // if they succeed the QTE's reset the players position to the entrance and reset boss health. Start phase 2

    // Start Phase 2

    // Phase 2 logic
    // Play the boss song
    // Start attack pattern 2

    // Attack pattern 2 logic (4 attacks)

    // Add boss invunerability
    // Do BeetleSummonAttack
    // Play Idle animation
    // Do MachineGunRiff (This should line up to the song)
    // Play Idle animation
    // Do LongRangeAttacks (1 out of the 3 potential attacks)
    // Play Idle animation
    // MachineGunRiff (This should line up to the song)
    // Play Idle animation

    // After attack pattern 2 remove invunerability and start using CloseRangeAttacks (1 out of 3 potential attacks)
    // If boss health reaches 0 trigger a QTE event
    // 7 QTE's happen and the player needs to hit them to go into phase 3
    // if they fail to hit the QTE's reset the players position to the entrance, reset boss health, and start phase 2
    // if they succeed to hit the QTE's reset the players position to the entrance and set boss health to 1 (1 shottable)

    // Start phase 3

    // Phase 3 logic

    // Play boss song
    // Do Guitar Solo Lane Attack
    // If you hit the boss 1 time you can trigger the final QTE's which is 11 in total
    // if they fail to hit the QTE's reset the players position to the entrance, reset boss health to 1 and restart phase 3
    // if they succeed to hit the QTE's play the boss's death cutscene


}
