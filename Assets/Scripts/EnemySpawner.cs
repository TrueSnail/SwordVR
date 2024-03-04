using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    private class EnemyList
    {
        public List<GameObject> Enemies;
    }
    public GameObject Player;
    public GameObject Camera;
    public float SpawnDistance;
    public float SpawnDelay;
    [SerializeField]
    List<EnemyList> Waves;

    private int CurrentWaveIndex = -1;
    private List<GameObject> SpawnedEnemies = new List<GameObject>();

    void Start()
    {
        Invoke(nameof(SpawnNextWave), SpawnDelay);
    }

    private void SpawnNextWave()
    {
        if (CurrentWaveIndex < Waves.Count - 1) CurrentWaveIndex++;
        Vector3 SpawnDirection = new Vector3(Camera.transform.forward.x, 0, Camera.transform.forward.z).normalized;
        Vector3 SpawnPosition = Player.transform.position + (SpawnDirection * SpawnDistance);

        SpawnedEnemies = new List<GameObject>();
        foreach (GameObject enemy in Waves[CurrentWaveIndex].Enemies) SpawnedEnemies.Add(Instantiate(enemy, SpawnPosition, Quaternion.LookRotation(Camera.transform.position - SpawnPosition)));
    }

    private void FixedUpdate()
    {
        if (SpawnedEnemies.Count > 0)
        {
            foreach (GameObject enemy in SpawnedEnemies) if (enemy.gameObject != null) return;
            SpawnedEnemies = new List<GameObject>();
            Invoke(nameof(SpawnNextWave), SpawnDelay);
        }
    }
}
