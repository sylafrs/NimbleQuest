using UnityEngine;
using System.Collections.Generic;

/**
  * @class MonsterManager
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class MonsterManager : MonoBehaviour {
    
    private float               previousCheckTime;
    private float               previousSpawnTime;
    private Transform           mobsParent;

    private Monster[] fellowsPrefab;

    public void Awake()
    {
        Game.monsterGroups = new List<MonsterGroup>();
        previousSpawnTime = Time.time - Game.settings.minSpawnTime;
        previousCheckTime = Time.time;

        GameObject mp = GameObject.Find("Units/Monsters");
        if(mp == null) {
            throw new UnityException("I need the 'Units/Monsters' gameobject");
        }

        mobsParent = mp.transform;

        List<Monster> fellow = new List<Monster>();
        foreach (Monster m in Game.settings.monsterPrefabs)
        {
            if (!m.BossOnly)
                fellow.Add(m);
        }

        this.fellowsPrefab = fellow.ToArray();
    }

    public void Update()
    {
        this.UpdateSpawn();
    }

    private void UpdateSpawn() 
    {
        float currentTime = Time.time;
        float elapsedTime = currentTime - previousSpawnTime;
        float checkElapsedTime = currentTime - previousCheckTime;

        if (Game.monsterGroups.Count < Game.settings.maxMonsterGroups)
        {
            if (elapsedTime >= Game.settings.minSpawnTime)
            {
                if (checkElapsedTime >= Game.settings.checkSpawnTime)
                {
                    previousCheckTime = currentTime;

                    float t = elapsedTime - Game.settings.minSpawnTime;
                    float v = Mathf.Clamp01(Game.settings.spawnChancesOverTime.Evaluate(t));

                    if (v != 0)
                    {
                        float c = Random.Range(0f, 1f);
                        //Debug.Log(elapsedTime + " " + v + " " + c);

                        if (c <= v)
                        {
                            //Debug.Log(elapsedTime.ToString("F2") + " s");
                            previousSpawnTime = currentTime;
                            SpawnMonsterGroup();
                        }
                    }
                }
            }
        }
    }

    public void SpawnMonsterGroup()
    {
        Vector3 min, max;
        Game.GetMinMaxLevel(out min, out max);
        
        int margin = Game.settings.spawnMargin;

        // Gets a random position within the level area
        Vector3 pos = new Vector3(
            Random.Range(min.x + margin, max.x - margin),
            0,
            Random.Range(min.z + margin, max.z - margin)
        );

        if (Game.settings.monsterPrefabs.Length > 0)
        {
            Monster boss = this.CreateMonster(null);
            boss.transform.position = pos;

            MonsterGroup group = new MonsterGroup(boss);
            Game.monsterGroups.Add(group);

            bool isWithClonesIfRacist = (Random.Range(0, 5) == 0);           

            int maxMonsters = Mathf.Max(1, Game.settings.maxMonsterGroupCapacity);

            if (!boss.Racist || isWithClonesIfRacist)
            {
                int minMonsters = 0;
                if (boss.Weak)
                {
                    minMonsters = 1;
                }

                int nMonsters = Random.Range(minMonsters, maxMonsters);
                for (int i = 0; i < nMonsters; i++)
                {
                    Monster monster = this.CreateMonster(boss);
                    group.AddFellow(monster);
                }
            }

            Debug.Log("[MOB SPAWN]\nPosition : " + pos + "\nBoss : " + boss.name + "\nMonsters : " + group.fellows.Count);  
        }
        else
        {
            Debug.LogWarning("We can't spawn any monster (set Game > settings > Monster Prefabs) in " + Game.instance.name);
        }
    }

    public Monster CreateMonster(Monster boss)
    {
        Monster monsterPrefab = null;
        List<Monster> prefabs = null;
        if (boss)
        {
            if (boss.Racist)
            {
                monsterPrefab = boss;
            }
            else
            {
                prefabs = new List<Monster>(this.fellowsPrefab);
                if (boss.Unique && !boss.BossOnly)
                {
                    prefabs.Remove(boss);
                }

                foreach (Monster m in fellowsPrefab)
                {
                    if (prefabs.Contains(m) && m.Unique && boss.group.Contains(m.name))
                    {
                        prefabs.Remove(m);
                    }
                }                
            }
        }
        else
        {
            prefabs = new List<Monster>(Game.settings.monsterPrefabs);
        }

        if (monsterPrefab == null && prefabs != null)
        {
            monsterPrefab = prefabs[
                Random.Range(0, prefabs.Count)
            ];
        }

        GameObject monster = GameObject.Instantiate(monsterPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        if (monster == null)
        {
            throw new UnityException(monsterPrefab.name + " can't be instantiated");
        }

        monster.name = monsterPrefab.name;
        monster.transform.parent = mobsParent;

        return monster.GetComponent<Monster>();            
    }
    
    public void OnBossDeath(MonsterGroup e)
    {
        previousSpawnTime = Time.time; // Don't spawn now.
        Game.monsterGroups.Remove(e);
    }
}
