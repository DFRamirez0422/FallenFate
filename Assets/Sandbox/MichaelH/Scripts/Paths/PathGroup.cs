using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Defines a group of paths that will fade in sequence. Each path in the group should have an ObjectFader component.
/// </summary>
[System.Serializable]
public class PathGroup
{
    [Header("Paths In This Set")]
    [Tooltip("Ordered list of path tiles that will fade in sequence.")]
    public List<GameObject> Paths;
}