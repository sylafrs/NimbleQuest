using UnityEngine;
using System.Collections.Generic;

public enum GIZMOSTYPE
{
    NONE,
    MOVE,
    RANGE
}

/**
  * @class Game
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Game : MonoBehaviour
{

    [System.Serializable]
    public class Settings
    {
        public float speed = 1;                     //< Vitesse de base des unités
        public float heroSpeed = 5;                 //< Vitesse d'un heros
        public float distanceUnits = 2;             //< Distance entre chaque unité
        public float rotationSpeed = 3;             //< Vitesse de la rotation (smooth)
        public float fellowSmoothSpeed = 1;         //< Vitesse d'un compagnion (smooth)
        public int maxMonsterGroups = 5;            //< Nombre maximum de groupe de monstre
        public float minSpawnTime = 10;             //< Temps minimum à attendre entre deux spawns
        public float checkSpawnTime = 0.1f;         //< Temps entre deux lancés de dé pour le spawn (0 = chaque frame)
        public int spawnMargin = 2;                 //< Marge au limites du sol, où les monstres ne spawneront pas
        public int maxMonsterGroupCapacity = 3;     //< Taille max. d'un groupe de monstres
        public int securityMargin = 1;              //< Marge dans laquelle les monstres n'iront pas    
        public int securityDistance = 3;            //< Distance que les monstres tâcherons de respecter
        public bool dontMoveHero = false;           //< Empêche le héros de bouger
        public float distanceRatio = 1;             //< Ratio pour la range
        public float minMonsterRotationTime = 0.2f; //< Temps minimal que doit attendre un monstre pour changer d'orientation (Prioritaire aux collisions)
        public float timeShowingLife = 5;           //< Temps durant lequel on montre la vie (opaque)
        public AnimationCurve spawnChancesOverTime; //< Courbe : Chances qu'un monstre spawn dans le temps, une fois le temps min. dépassé.
        public Hero[] heroesPrefabs;                //< Prefab des unités jouables
        public Monster[] monsterPrefabs;            //< Prefab des unités ennemies
        public GameObject lifeBarPrefab;

        public GIZMOSTYPE GizmosType;
    }

    public Settings setSettings;
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
    public static List<MonsterGroup> monsterGroups;

    private static Hero selectedLeader;

    public static void GetMinMaxLevel(out Vector3 min, out Vector3 max) {
        GameObject floor = GameObject.Find("Level/Floor") as GameObject;
        if (floor == null)
        {
            throw new UnityException("I need the Level/Floor object");
        }

        min = floor.renderer.bounds.min;
        max = floor.renderer.bounds.max;
    }

    public void OnDrawGizmos()
    {
        if (Application.loadedLevelName.Equals("scene"))
        {
            // Get the level dimensions
            Vector3 min, max;
            Game.GetMinMaxLevel(out min, out max);
            Rect zone = RectUtility.MinMaxRect(min, max);

            RectUtility.GizmosRect(zone);

            zone.x += Game.settings.securityMargin;
            zone.y += Game.settings.securityMargin;
            zone.width -= (Game.settings.securityMargin * 2);
            zone.height -= (Game.settings.securityMargin * 2);

            RectUtility.GizmosRect(zone);
        }
    }

    public static void LaunchGame(Hero selectedLeader)
    {
        Game.selectedLeader = selectedLeader;
        Application.LoadLevelAsync("scene");
    }

    public static void OnLeaderDeath()
    {
        OnGameOver();
    }

    public static void OnBossDeath(MonsterGroup e)
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
