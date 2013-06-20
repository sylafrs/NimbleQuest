using UnityEngine;
using System.Collections.Generic;

/**
  * @class Game
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Game : MonoBehaviour {

    static HeroicGroup hg;

    public void Start()
    {
        OnGameStart();
    }

    public void Update()
    {
        hg.UpdateFellowPositions();
    }

    // ---------------------------------------  //

    public static void OnGameStart()
    {
        Hero leader = GameObject.Find("Warrior").GetComponent<Hero>();
        Hero a = GameObject.Find("Archer").GetComponent<Hero>();
        Hero b = GameObject.Find("Knight").GetComponent<Hero>();
        Hero c = GameObject.Find("Mage").GetComponent<Hero>();

        hg = new HeroicGroup(leader);
        hg  .AddFellow(a)
            .AddFellow(b)
            .AddFellow(c);
    }

    public static void OnLeaderDeath()
    {
        OnGameOver();
    }
    
    public static void OnGameOver()
    {

    }
}
