using UnityEngine;

namespace Player.RhythmBonusPrefabs
{
    /// <summary>
    /// Quick debug helper: press a key to test
    /// </summary>
    public class RhythmJudgeTester : MonoBehaviour
    {
        [SerializeField] private RhythmBonusJudge judge;
        [SerializeField] private KeyCode testKey = KeyCode.Mouse0; // Key to sim "attack"
        
        void Update()
        {
            // When key is pressed, evaluate rhythm and print result
            if (Input.GetKeyDown(testKey))
            {
                var (tier, mult) = judge.EvaluateNow(); // Call the judge
                Debug.Log($"[Rhythm Test] {tier} hit, Multiplier = x{mult}");
            }
        }
    }
}