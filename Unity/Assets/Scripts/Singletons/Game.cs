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
    public enum State
    {
        NONE,
        PLAYING,
        PAUSED,
        GAMEOVER,
        EXITING
    }
    
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
        public float distanceRatio = 1;             //< Ratio pour la range
        public float minMonsterRotationTime = 0.2f; //< Temps minimal que doit attendre un monstre pour changer d'orientation (Prioritaire aux collisions)
        public float timeShowingLife = 5;           //< Temps durant lequel on montre la vie (opaque)
        public float freezeMult = 0.2f;
                
        [System.Obsolete]
        public float forwardFieldWidth = 1;         //< Marge d'erreur pour viser 'tout droit'    
       
        public float itemSize = 1;                  //< Taille d'un item (pour passer dessus. utilise distanceRatio)
        public float itemTime = 10;                 //< Durée d'apparition maximale d'un objet
        public float ChancesOverTenToGetItem = 5;   //< La chance d'avoir un item (/10)

        public bool dontMoveHero = false;           //< Empêche le héros de bouger
        public bool undeadMode = false;                //< Empêche de mourir.. ..si on a plus de vie

        public AnimationCurve spawnChancesOverTime; //< Courbe : Chances qu'un monstre spawn dans le temps, une fois le temps min. dépassé.
        public Hero[] heroesPrefabs;                //< Prefab des unités jouables
        public Monster[] monsterPrefabs;            //< Prefab des unités ennemies
        public GameObject lifeBarPrefab;            //< Prefab des barres de vie
        public Item[] itemsPrefab;                  //< Prefab des items
        public GameObject freezeFeedbackPrefab;     //< Prefab du gel

        public GIZMOSTYPE GizmosType;
    }

    // --------------------------------------  //

    public Settings setSettings;
    private MonsterManager monsters;
    private ItemManager items;

    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        settings = this.setSettings;
        started = true;
        instance = this;
        state = State.NONE;
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
        deathCounter = 0;
        CreateHeroicGroup();

        monsters = instance.gameObject.AddComponent<MonsterManager>();
        instance.gameObject.AddComponent<InputManager>();
        items = instance.gameObject.AddComponent<ItemManager>();

        Camera.main.GetComponent<FollowOffset>().target = heroicGroup.leader.transform;
        state = State.PLAYING;        
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

        heroicGroup = new HeroicGroup(leader.GetComponent<Hero>());
    }

    private void OnGUI()
    {
        if (state == State.PLAYING)
        {
            GUILayout.Label("Death counter : " + deathCounter + " / 50");
        }
        if (state == State.PAUSED)
        {
            if (GUILayout.Button("Resume"))
            {
                OnResume();
            }

            //if (GUILayout.Button("Restart"))
            //{
            //    OnRestart();
            //}

            if (Camera.main.audio.mute)
            {
                if (GUILayout.Button("Play music"))
                {
                    Camera.main.audio.mute = false;
                }
            }
            else
            {
                if (GUILayout.Button("Mute music"))
                {
                    Camera.main.audio.mute = true;
                }
            }
            
            if (GUILayout.Button("Exit"))
            {
                OnExit();
            }
        }
        if (state == State.GAMEOVER)
        {
            GUILayout.Label("GameOver");
            if (GUILayout.Button("Exit"))
            {
                OnExit();
            }

            //if (GUILayout.Button("Retry"))
            //{
            //    OnRestart();
            //}
        }
    }

    // ---------------------------------------  //

    public static Game instance { get; private set; }

    public static bool started = false;
    public static Settings settings;
    public static HeroicGroup heroicGroup;
    public static List<MonsterGroup> monsterGroups;
    public static int deathCounter;

    public static State state { get; private set; }

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

    public static void OnRestart()
    {
        LaunchGame(selectedLeader);
    }

    public static void OnLeaderDeath()
    {
        OnGameOver();
    }

    public static void OnBossDeath(MonsterGroup e)
    {
        instance.monsters.OnBossDeath(e);
        instance.items.OnBossDeath(e);        
    }

    public static void OnPauseButton()
    {
        if (state == State.PAUSED)
            OnResume();
        else if (state == State.PLAYING)
            OnPause();
    }

    public static void OnPause()
    {
        state = State.PAUSED;
    }

    public static void OnResume()
    {
        state = State.PLAYING;
    }

    public static void OnGameOver()
    {
        state = State.GAMEOVER;
    }

    public static void OnExit() {
        if (state != State.EXITING)
        {
            state = State.EXITING;
            GameObject.Destroy(instance.gameObject);
            Application.LoadLevel(0);
        }
    }

    public static void OnDirectionKey(Orientation o)
    {
        heroicGroup.NewOrientation(o);
    }

    public static void OnUnitHitsWall(Unit u)
    {
        if (u is Hero && u.IsLeader)
            u.ReceiveAttack(AttackType.DAMAGES, 10000);
        else
            Debug.Log(u.name + " hits a wall");
    }

    public static void OnMonsterKilled(Monster m)
    {
        deathCounter++;
    }
}
