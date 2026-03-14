using UnityEngine;

[CreateAssetMenu(fileName = "ActorSO", menuName = "Dialogue/NPC")]
public class ActorSO : ScriptableObject
{
    /// <summary>
    /// Character portrait emotions.
    /// </summary>
    public enum Emotion
    {
        Idle,
        Idle_Talk,
        Sad,
        Sad_Talk,
        Quizzical,
        Quizzical_Talk
    }

    public string m_ActorName;
    public Sprite m_Portrait;
}
