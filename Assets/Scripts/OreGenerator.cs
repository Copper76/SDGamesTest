using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreGenerator : MonoBehaviour
{
    [Header("Spawn Parameters")]
    [SerializeField] private float interval;
    private float nextSpawn;
    [SerializeField] private float goldChance;
    [SerializeField] private Transform collectibleContainer;

    [Header("SpawnTerrain")]
    [SerializeField] private GameObject terrain;

    [Header("Spawn Prefabs")]
    [SerializeField] private GameObject copperPrefab;
    [SerializeField] private GameObject goldPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            float randValue = Random.value;

            Vector3 terrainPosition = terrain.transform.position;

            Mesh terrainMesh = terrain.GetComponent<MeshFilter>().mesh;
            Vector3 terrainSize = terrainMesh.bounds.size;

            float minX = terrainPosition.x - terrainSize.x * terrain.transform.localScale.x / 2;
            float maxX = terrainPosition.x + terrainSize.x * terrain.transform.localScale.x / 2;
            float minZ = terrainPosition.z - terrainSize.z * terrain.transform.localScale.z / 2;
            float maxZ = terrainPosition.z + terrainSize.z * terrain.transform.localScale.z / 2;

            Vector3 spawnPos = new Vector3(Random.Range(minX, maxX), terrainPosition.y + 1.0f, Random.Range(minZ, maxZ));

            while (Physics.CheckBox(spawnPos, goldPrefab.transform.localScale * 0.5f))
            {
                spawnPos.x = Random.Range(minX, maxX);
                spawnPos.z = Random.Range(minZ, maxZ);
            }

            GameObject newSpawn;
            if (randValue < goldChance)
            {
                newSpawn = Instantiate(goldPrefab, spawnPos, Quaternion.identity, collectibleContainer);
            }
            else
            {
                newSpawn = Instantiate(copperPrefab, spawnPos, Quaternion.identity, collectibleContainer);
            }
            nextSpawn = Time.time + interval * (0.9f + randValue * 0.1f);
        }
    }
}
