using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathGroup
{
    [Header("Paths In This Set")]
    [Tooltip("Ordered list of path tiles that will fade in sequence.")]
    public List<GameObject> Paths;
}