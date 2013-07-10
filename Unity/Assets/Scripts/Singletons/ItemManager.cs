using UnityEngine;
using System.Collections.Generic;

/**
  * @class ItemManager
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class ItemManager : MonoBehaviour {
    public void OnBossDeath(MonsterGroup group)
    {
        // We have item prefabs
        if (Game.settings.itemsPrefab.Length > 0)
        {
            // Shall we drop ?
            bool randDrop = Random.Range(0, 10) < Game.settings.ChancesOverTenToGetItem;
            if (randDrop)
            {
                // Which item ?
                int randItem = Random.Range(0, Game.settings.itemsPrefab.Length);
                GameObject prefab = Game.settings.itemsPrefab[randItem].gameObject;

                // Let's drop the item !
                Debug.Log("[ITEM DROP]\nName : " + prefab.name);

                GameObject g = GameObject.Instantiate(prefab) as GameObject;
                g.transform.position = group.leader.transform.position;
                g.transform.parent = GameObject.Find("Units/Items").transform;
                g.transform.name = prefab.name;
            }
        }
    }
}
