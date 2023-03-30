using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using System.Collections.Generic;

public partial class EntitySpawnerSystem : SystemBase {

    public List<Entity> EnemyList;
    private Entity gameHandlerEntity;
    public LevelComponent lvl;
    public int score;

    private Random random;
    float pauseTimer;
    public bool condition=false;
    public bool isPaused=true;
    private float spawnTimer = 0;
    private float3[] spawnPoints = new float3[12]
    {
        math.float3(10,0,0),
        math.float3(-10,0,0),
        math.float3(0,7,0),
        math.float3(0,-7,0),
        math.float3(10,10,0),
        math.float3(-10,-10,0),
        math.float3(7,7,0),
        math.float3(-7,-7,0),
        math.float3(-8,2,0),
        math.float3(-8,-2,0),
        math.float3(10,2,0),
        math.float3(10,-2,0),

    };
    protected override void OnCreate(){Enabled = false;}
    protected override void OnStartRunning()
    {
        score = 0;
        MenuComponent.Instance.UpdateScore(score);
        isPaused = false;
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        gameHandlerEntity = em.World.GetExistingSystem<GameHandler>().GameHandlerEntity;
        random = new Random(56);
        EnemyList = new List<Entity>();
        lvl = GetComponent<LevelComponent>(gameHandlerEntity);
    } 
    protected override void OnUpdate()
    {
        if(isPaused==false)
        {
            spawnTimer -= Time.DeltaTime;
            pauseTimer -= Time.DeltaTime;
        //Loops through list and destroys enemies that has collided with projectiles if they have the isDeadTag
        for (int i= EnemyList.Count - 1; i>=0; i--)
        {
            if (HasComponent<isDeadTag>(EnemyList[i]))
            {
                EntityManager.DestroyEntity(EnemyList[i]);
                lvl.enemies--;
                    score++;
                    MenuComponent.Instance.UpdateScore(score);

            }
        }
        //between waves
        if (condition)
        {
            if (pauseTimer <= 0){condition = false;}
        }
        //Spawn enemies
        else if (spawnTimer<=0&&lvl.enemiesToSpawn!=0) 
        {
            spawnTimer = 1f;
            // Spawn!
            EnemyPrefabComponent prefabEntityComponent = GetSingleton<EnemyPrefabComponent>();
            Entity spawnedEntity = EntityManager.Instantiate(prefabEntityComponent.prefabEntity);
            EnemyList.Add(spawnedEntity);
            EntityManager.SetComponentData(spawnedEntity,
                new Translation { Value =spawnPoints[random.NextInt(0,11)]
             }
            );
            EntityManager.AddComponentData(spawnedEntity, new EnemyComponent());
            lvl.enemiesToSpawn--;
            lvl.enemies++;
        }
        //initialize new wave attributes
        if(lvl.enemies==0&&lvl.enemiesToSpawn<=0 )
        {
            condition = true;
            pauseTimer = 5;
            MenuComponent.Instance.displayNextWave = true;
            lvl.wave++;
            lvl.enemiesToSpawn = lvl.wave * 10;
        }
        if (HasComponent<isDeadTag>(World.GetExistingSystem<GameHandler>().playerEntity)){Enabled = false;}
    }
    }

}
