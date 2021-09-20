using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> SpawnableMonsters = new List<GameObject>();
    public List<GameObject> MonsterSpawnLocations = new List<GameObject>();
    public List<WellHeart> WellHearts;
    public List<GameObject> VillagerSpawnLocations = new List<GameObject>();
    public GameObject VillagerPrefab;
    public int monsterNightTimeSpawnRate = 3;
    public int villagerSpawnRate = 4;
    public static SpawnManager instance;

    private List<GameObject> m_villagers = new List<GameObject>();
    private List<GameObject> m_monsters = new List<GameObject>();

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
        Sun sun = GameObject.Find("Sun").GetComponent<Sun>();
        sun.SunSet += OnSunSet;
        sun.SunRise += OnSunRise;
        foreach (WellHeart wellHeart in WellHearts)
        {
            wellHeart.WellHeartDeath += WellHeart_WellHeartDeath;
        }
    }

    private void WellHeart_WellHeartDeath(object sender, System.EventArgs e)
    {
        WellHeart wellHeart = (WellHeart)sender;
        if (wellHeart)
        {
            WellHearts.Remove(wellHeart);
        }
    }

    private void OnSunSet(object sender, System.EventArgs e)
    {
        SpawnMonsters();
        DespawnVillagers();
    }

    private void OnSunRise(object sender, System.EventArgs e)
    {
        SpawnVillagers();
    }

    private void OnMonsterDied(object sender, System.EventArgs e)
    {
        Monster monster = (Monster)sender;
        m_monsters.Remove(monster.gameObject);
    }

    private void SpawnVillagers()
    {
        foreach (GameObject spawnLoc in VillagerSpawnLocations)
        {
            for (int i = 0; i < villagerSpawnRate; i++)
            {
                GameObject villager = Instantiate(VillagerPrefab, spawnLoc.transform.position, Quaternion.identity);
                m_villagers.Add(villager);
            }
        }
    }

    private void SpawnMonsters()
    {
        for (int i = 0; i < MonsterSpawnLocations.Count; i++)
        {
            for (int k = 0; k < monsterNightTimeSpawnRate; k++)
            {
                var randomIndex = Random.Range(0, SpawnableMonsters.Count - 1);
                GameObject monster = Instantiate(SpawnableMonsters[randomIndex], MonsterSpawnLocations[i].transform.position, Quaternion.identity);
                m_monsters.Add(monster);
                monster.GetComponent<Monster>().Died += OnMonsterDied;
            }
        }
    }

    private void DespawnVillagers()
    {
        foreach (var item in m_villagers)
        {
            if(item)
                Destroy(item);
        }
        m_villagers.Clear();
    }

    private void DespawnMonsters()
    {
        foreach (var item in m_monsters)
        {
            if (item)
                Destroy(item);
        }
        m_monsters.Clear();
    }

    public void DestroyAll()
    {
        DespawnVillagers();
        DespawnMonsters();
    }
}
