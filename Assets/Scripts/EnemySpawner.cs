using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;

public class EnemySpawner : MonoBehaviourPun
{
    public string enemyPrefab;
    public float maxEnemies;
    public float spawnRadius;
    public float spawnCheckTime;

    private float lastSpawnCheckTime;
    private List<GameObject> curEnemies = new List<GameObject>();

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (Time.time - lastSpawnCheckTime > spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        // remove any dead enemies
        for(int x = 0; x < curEnemies.Count; ++x)
        {
            if (!curEnemies[x])
                curEnemies.RemoveAt(x);
        }

        // Maxed out?
        if (curEnemies.Count >= maxEnemies)
            return;

        // spawn!
        Vector3 randomInCircle = Random.insideUnitCircle * spawnRadius;
        GameObject enemy = PhotonNetwork.Instantiate(enemyPrefab, transform.position + randomInCircle, Quaternion.identity);

        curEnemies.Add(enemy);
    }
}
