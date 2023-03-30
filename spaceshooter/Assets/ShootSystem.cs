using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public partial class ShootSystem : SystemBase
{
    Entity spawnedEntity;
    public List<Entity> EntitiesList=new List<Entity>();
    private float spawnTimer;
    ProjectilePrefab projectilePrefab;
    bool SpawnProjectile;
    EntityManager em;

    protected override void OnCreate()
    {
        Enabled = false;
    }
    protected override void OnStartRunning()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        projectilePrefab = GetSingleton<ProjectilePrefab>();
    }
    protected override void OnUpdate()
    {
        SpawnProjectile = EntityManager.World.GetExistingSystem<InputSystem>().shooting;
        if (SpawnProjectile)
        {

            spawnTimer -= Time.DeltaTime;
            
            if (spawnTimer <= 0f)
            {
              float3 playerTranslation=  EntityManager.GetComponentData<Translation>(World.GetExistingSystem<GameHandler>().playerEntity).Value;
               

                spawnTimer = .2f;
                spawnedEntity = EntityManager.Instantiate(projectilePrefab.prefabEntity);
                EntityManager.AddComponentData(spawnedEntity, new ProjectileComponent()
                {
                    position = playerTranslation,
                    velocity = EntityManager.World.GetExistingSystem<InputSystem>().projectileDir,
                    entity = spawnedEntity
                });
                EntityManager.AddComponentData(spawnedEntity, new Translation(){});
                EntitiesList.Add(spawnedEntity);

            }
        }
    
        float dT = Time.DeltaTime;
        Entities.WithStructuralChanges().ForEach((Entity e,ref ProjectileComponent projectile,ref Translation translation) =>
        { 
            projectile.position += Vector3.Normalize( projectile.velocity )* dT*6;
            translation.Value = projectile.position;
            em.SetComponentData(e, projectile);
            projectile.timeSinceCreated += dT;
            if(projectile.timeSinceCreated>=10)
            {
                em.DestroyEntity(e);
            }
        }).Run(); 
        if (HasComponent<isDeadTag>(World.GetExistingSystem<GameHandler>().playerEntity)){Enabled = false;}
    }
}
