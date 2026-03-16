using System;
using System.Collections.Generic;
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

    [Serializable]
    public struct EmotionPortrait
    {
        public Emotion emotion;
        public Sprite portrait;
    }

    public string m_ActorName;
    public Sprite m_DefaultPortrait;
    public EmotionPortrait[] m_EmotionPortraits;
}
