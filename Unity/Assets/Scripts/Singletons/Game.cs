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
        public  float   speed               = 1;    //< Vitesse de base des unités
        public  float   heroSpeed           = 5;    //< Vitesse d'un heros
        public  float   distanceUnits       = 2;    //< Distance entre chaque unité
        public  float   rotationSpeed       = 3;    //< Vitesse de la rotation (smooth)
        public  float   fellowSmoothSpeed   = 1;    //< Vitesse d'un compagnion (smooth)
        public  int     maxMonsterGroups    = 5;    //< Nombre maximum de groupe de monstre
        public  float   minSpawnTime        = 10;   //< Temps minimum à attendre entre deux spawns
        public AnimationCurve spawnChancesOverTime;

        public  Hero[]  heroesPrefabs;              //< Prefab des unités jouables
        public  Enemy[] monsterPrefabs;             //< Prefab des unités ennemies
    }

    public Settings setSettings;    
    private Level level;
    private MonsterManager monsters;

    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        settings = this.setSettings;
        started = true;
        instance = this;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 2)
        {
            OnMainSceneLoaded();
        }
    }

    private void OnMainSceneLoaded()
    {
        CreateHeroicGroup();

        monsters = instance.gameObject.AddComponent<MonsterManager>();
        instance.gameObject.AddComponent<InputManager>();

        Camera.main.GetComponent<FollowOffset>().target = hg.leader.transform;
    }

    private void CreateHeroicGroup()
    {
        GameObject leader = GameObject.Instantiate(selectedLeader.gameObject) as GameObject;
        if (leader == null)
            throw new UnityException("Can't instantiate " + selectedLeader.name);

        GameObject units = GameObject.Find("Units");
        if (units == null)
            throw new System.MissingMemberException("Missing 'Units' gameobject");

        Transform heroes = units.transform.FindChild("Heroes");
        if (heroes == null)
            throw new System.MissingMemberException("Missing 'Units/Heroes' gameobject");

        leader.transform.parent = heroes;
        leader.name = selectedLeader.name;

        hg = new HeroicGroup(leader.GetComponent<Hero>());
    }

    // ---------------------------------------  //

    public static Game instance { get; private set; }

    public static bool started = false;
    public static Settings settings;
    public static HeroicGroup hg;

    private static Hero selectedLeader;
    
    public static void LaunchGame(Hero selectedLeader)
    {
        Game.selectedLeader = selectedLeader;
        Application.LoadLevelAsync("scene");        
    }
    
    public static void OnLeaderDeath()
    {
        OnGameOver();
    }

    public static void OnBossDeath(Enemy e)
    {
        instance.monsters.OnBossDeath(e);
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
