using UnityEngine;
using System.Collections.Generic;

/**
  * @class MonsterManager
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class MonsterManager : MonoBehaviour {

    private float               previousSpawnTime;
    private List<MonsterGroup>  mobs;
    
    public void Awake()
    {
        mobs = new List<MonsterGroup>();
        previousSpawnTime = Time.time;
    }

    public void Update()
    {
        this.UpdateSpawn();
    }

    private void UpdateSpawn() 
    {
        float currentTime = Time.time;
        float elapsedTime = currentTime - previousSpawnTime;

        if (mobs.Count < Game.settings.maxMonsterGroups)
        {
            if (elapsedTime >= Game.settings.minSpawnTime)
            {
                float t = elapsedTime - Game.settings.minSpawnTime;
                float v = Mathf.Clamp01(Game.settings.spawnChancesOverTime.Evaluate(t));

                if (v != 0)
                {
                    float c = Random.Range(0f, 1f);
                    if (c <= v)
                    {
                        previousSpawnTime = currentTime;
                        SpawnMonsterGroup();
                    }
                }
            }
        }
    }

    public void SpawnMonsterGroup()
    {
        //Vector3 pos;
    }
    
    public void OnBossDeath(Enemy e)
    {

    }
}
