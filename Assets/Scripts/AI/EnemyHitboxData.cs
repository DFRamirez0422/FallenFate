using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public enum HitboxShape
{
    Box,
    Sphere,
    Cone
}

[System.Serializable]
public struct HitboxDefinition
{
    [FormerlySerializedAs("ID")]
    [Header("Identification")]
    [Tooltip("Unique name ID to reference this hitbox (e.g., 'MeleeSwing', 'RoarAOE').")]
    public string id;

    [Header("Shape Settings")]
    [Tooltip("Shape type of this hitbox: Box, Sphere, or Cone.")]
    public HitboxShape shape;

    [Tooltip("Local offset from the enemy’s origin or attack point transform.")]
    public Vector3 offset;

    [Tooltip("Optional local rotation for the hitbox, mainly used for cone directions.")]
    public Vector3 rotation;

    [Tooltip("Box half-extents for Box-shaped hitboxes.")]
    public Vector3 size;

    [Tooltip("Radius used for Sphere or Cone shapes.")]
    public float radius;

    [Tooltip("Cone angle in degrees. Only used when Shape is set to Cone.")]
    [Range(0f, 180f)]
    public float angle;

    [Header("Combat Settings")]
    [Tooltip("How long (in seconds) this hitbox remains active after being triggered.")]
    public float activeTime;

    [Tooltip("Amount of damage dealt to targets within this hitbox.")]
    public int damage;

    [Tooltip("Maximum allowed Z-axis difference between enemy and target to count as a hit (2.5D depth tolerance).")]
    public float depthTolerance;

    [Tooltip("Which layers this hitbox can damage (e.g., Player).")]
    public LayerMask targetMask;

    [Header("Debug")]
    [Tooltip("If true, draws debug gizmos for this hitbox while active.")]
    public bool bDebugDraw;

    [Tooltip("Color of the debug gizmo in the Scene view.")]
    public Color debugColor;
}

[CreateAssetMenu(fileName = "DA_Hitboxes_", menuName = "Combat/Enemy Hitbox Data")]
public class EnemyHitboxData : ScriptableObject
{
    [Tooltip("Editor-friendly name for clarity (e.g., 'Melee Grunt', 'Boss A').")]
    public string enemyName;

    [Tooltip("List of all hitboxes this enemy can use (e.g., LightPunch, HeavySlam, RoarAOE).")]
    public List<HitboxDefinition> hitboxes = new List<HitboxDefinition>();
}