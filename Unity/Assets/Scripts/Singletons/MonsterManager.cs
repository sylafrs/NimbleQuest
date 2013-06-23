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
    private List<MonsterGroup>  mobs;
    private Transform           mobsParent;
    
    public void Awake()
    {
        mobs = new List<MonsterGroup>();
        previousSpawnTime = Time.time - Game.settings.minSpawnTime;
        previousCheckTime = Time.time;

        GameObject mp = GameObject.Find("Units/Monsters");
        if(mp == null) {
            throw new UnityException("I need the 'Units/Monsters' gameobject");
        }

        mobsParent = mp.transform;
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

        if (mobs.Count < Game.settings.maxMonsterGroups)
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
            Monster boss = this.CreateMonster();
            boss.transform.position = pos;

            MonsterGroup group = new MonsterGroup(boss);
            mobs.Add(group);

            int maxMonsters = Mathf.Max(1, Game.settings.maxMonsterGroupCapacity);

            int nMonsters = Random.Range(0, maxMonsters);
            for (int i = 0; i < nMonsters; i++)
            {
                Monster monster = this.CreateMonster();
                group.AddFellow(monster);
            }

            Debug.Log("[MOB SPAWN]\nPosition : " + pos + "\nBoss : " + boss.name + "\nMonsters : " + group.fellows.Count);  
        }
        else
        {
            Debug.LogWarning("We can't spawn any monster (set Game > settings > Monster Prefabs) in " + Game.instance.name);
        }
    }

    public Monster CreateMonster()
    {
        Monster monsterPrefab = Game.settings.monsterPrefabs[
            Random.Range(0, Game.settings.monsterPrefabs.Length)
        ];

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
        this.mobs.Remove(e);
    }
}
