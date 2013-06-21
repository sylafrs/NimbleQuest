using UnityEngine;
using System.Collections.Generic;

/**
  * @class MonsterGroup
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class MonsterGroup : Group {
    public MonsterGroup(Enemy leader) : base(leader) { }
}
