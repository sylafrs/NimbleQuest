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

        public Hero[] herosPrefabs;
    }

    public Settings setSettings;
    public static Game instance { get; private set; }

    public void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        settings = this.setSettings;
        started = true;
        instance = this;
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level == 2)
        {
            OnMainSceneLoaded();
        }
    }

    // ---------------------------------------  //

    public static bool started = false;
    public static Settings settings;
    public static HeroicGroup hg;

    private static Hero selectedLeader;

    public static void LaunchGame(Hero selectedLeader)
    {
        Game.selectedLeader = selectedLeader;
        Application.LoadLevelAsync("scene");        
    }

    private static void OnMainSceneLoaded()
    {
        GameObject leader = GameObject.Instantiate(selectedLeader.gameObject) as GameObject;
        if (leader == null)
            throw new UnityException("Can't instantiate " + selectedLeader.name);

        //leader.transform.parent = GameObject.Find("Heros").transform;//GameObject.Find("Units").transform.FindChild("Heros");

        GameObject units = GameObject.Find("Units");
        if (units == null)
            throw new System.MissingMemberException("Missing 'Units' gameobject");

        Transform heroes = units.transform.FindChild("Heroes");
        if(heroes == null)
            throw new System.MissingMemberException("Missing 'Units/Heroes' gameobject");

        leader.transform.parent = heroes;
        leader.name = selectedLeader.name;
        hg = new HeroicGroup(leader.GetComponent<Hero>());
        instance.gameObject.AddComponent<InputManager>();

        Camera.main.GetComponent<FollowOffset>().target = leader.transform;
    }

    public static void OnLeaderDeath()
    {
        OnGameOver();
    }
    
    public static void OnGameOver()
    {
        GameObject.Destroy(instance.gameObject);
        Application.LoadLevel(0);
    }

    public static void OnDirectionKey(Orientation o)
    {
        hg.NewOrientation(o);
    }

    public static void OnUnitHitsWall(Unit u)
    {
        if (u is Hero && u.IsLeader)
            OnLeaderDeath();
        else
            Debug.Log(u.name + " hits a wall");
    }
}
