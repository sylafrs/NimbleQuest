using UnityEngine;
using System.Collections.Generic;

/**
  * @class Item
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public abstract class Item : MonoBehaviour {

    Rect itemZone;
    float remainingTime;

    void Start()
    {
        Vector3 pos = this.transform.position;
        float size = Game.settings.itemSize * Game.settings.distanceRatio / 2.0f;
        itemZone = RectUtility.FromCenter(pos, size);

        remainingTime = Game.settings.itemTime;
    }

    void Update()
    {
        Vector3 leaderPos = Game.heroicGroup.leader.transform.position;  
        if (RectUtility.ContainsXZ(itemZone, leaderPos))
        {
            Debug.Log("[ITEM PICK]\nName : " + this.name);
            this.DoFunction();
            this.Disappear();
        }
        
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            this.Disappear();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (itemZone != null)
            RectUtility.GizmosRect(itemZone);
    }

    protected abstract void DoFunction();

    protected virtual void Disappear()
    {
        Debug.Log("[ITEM DISAPPEAR]\nName : " + this.name);
        GameObject.Destroy(this.gameObject);
    }
}
