using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PickUpObjects))] // Ensure that the EnemySpawner component is attached to the same GameObject
public class Room1_EnemySpawner : MonoBehaviour
{
    [Header("Enemines")]
    [SerializeField] List<Transform> _spawnPoints = new List<Transform>(6); // List of spawn points for the enemies
    [SerializeField] List<GameObject> _AnmountOfEnemiesinGame; // List of the number of enemies to spawn for each type
    [SerializeField] private int _numberOfEnemiesToSpawn; // Number of enemies to spawn
    [SerializeField] private GameObject _EchoPrefab; // Reference to the enemy prefab
    [SerializeField] private GameObject _AngryEchoPrefab; // Reference to the enemy prefab

    [Header("NPC")]
    [SerializeField] private GameObject NPCPrefab_Despawn; // Reference to the NPC prefab
    private PickUpObjects pickUpObjects; // Reference to the PickUpObjects component

    void Start()
    {
        pickUpObjects = GetComponent<PickUpObjects>();
    }

    void FixedUpdate()
    {
        if (pickUpObjects.isPickedUp)
        {
            SpawnEnemies();
        }
    }


    public void SpawnEnemies()
    {
        // Loop through the number of enemies to spawn
        for (int i = 0; i < _numberOfEnemiesToSpawn; i++)
        {
            // Randomly select a spawn point from the list
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];

            // Randomly select an enemy type to spawn based on the amount of enemies in the game
            int enemyTypeIndex = Random.Range(0, 2);

            if (enemyTypeIndex == 0)
            {
                _AnmountOfEnemiesinGame.Add(Instantiate(_EchoPrefab, spawnPoint.position, Quaternion.identity)); // Spawn the Echo enemy at the selected spawn point
            }
            else if (enemyTypeIndex == 1)
            {
              _AnmountOfEnemiesinGame.Add(Instantiate(_AngryEchoPrefab, spawnPoint.position, Quaternion.identity));
            }
        }
         Destroy(NPCPrefab_Despawn); // Destroy the NPC prefab when the item is picked up
         Destroy(this.gameObject); // Destroy the EnemySpawner GameObject after spawning enemies and despawning NPC
    }
}
