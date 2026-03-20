using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveData
{
    public string sceneName;

    public float[] playerPosition = new float[3];
    public int playerHealth;

    public List<string> activeEnemies = new List<string>();
    public List<string> collectedItems = new List<string>();

    public int questState;
}


