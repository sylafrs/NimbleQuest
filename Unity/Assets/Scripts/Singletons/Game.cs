using UnityEngine;
using System.Collections.Generic;

/**
  * @class Game
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Game : MonoBehaviour {

    [System.Serializable]
    public class Settings
    {
        public float speed = 1;
        public float heroSpeed = 5;
        public float distanceUnits = 2;
        public float rotationSpeed = 3;
        public float fellowSmoothSpeed = 1;
    }

    public Settings setSettings;

    public void Awake()
    {
        settings = this.setSettings;
    }

    public void Start()
    {
        Hero leader = GameObject.Find("Warrior").GetComponent<Hero>();
        Hero a = GameObject.Find("Archer").GetComponent<Hero>();
        Hero b = GameObject.Find("Knight").GetComponent<Hero>();
        Hero c = GameObject.Find("Mage").GetComponent<Hero>();

        hg = new HeroicGroup(leader);
        hg  .AddFellow(a)
            .AddFellow(b)
            .AddFellow(c);

        this.gameObject.AddComponent<InputManager>();
    }

    // ---------------------------------------  //

    public static Settings settings;
    public static HeroicGroup hg;

    public static void OnLeaderDeath()
    {
        OnGameOver();
    }
    
    public static void OnGameOver()
    {

    }

    public static void OnDirectionKey(Orientation o)
    {
        hg.NewOrientation(o);
    }

    public static void OnUnitHitsWall(Unit u)
    {
        //this.transform.localPosition = Vector3.zero;
    }
}
