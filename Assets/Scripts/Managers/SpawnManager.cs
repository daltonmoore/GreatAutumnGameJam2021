using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> Monsters = new List<GameObject>();
    public List<GameObject> MonsterSpawnLocations = new List<GameObject>();
    public List<WellHeart> WellHearts;
    public int spawnRate = 3;
    public static SpawnManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        GameObject.Find("Sun").GetComponent<Sun>().OnSunSet += SpawnManager_OnSunSet;
    }

    private void SpawnManager_OnSunSet(object sender, System.EventArgs e)
    {
        SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        for (int i = 0; i < MonsterSpawnLocations.Count; i++)
        {
            for (int k = 0; k < spawnRate; k++)
            {
                var randomIndex = Random.Range(0, Monsters.Count - 1);
                Instantiate(Monsters[randomIndex], MonsterSpawnLocations[i].transform.position, Quaternion.identity);
            }
        }
    }
}
