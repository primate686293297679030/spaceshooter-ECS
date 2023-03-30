using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;

public partial class EnemySystem : SystemBase
{
    private Entity playerEntity;
    protected override void OnCreate()
    {
        Enabled = false;
    }
    protected override void OnStartRunning()
    {
       playerEntity = World.GetExistingSystem<GameHandler>().playerEntity;
    }
   
    protected override void OnUpdate()
    {
        float3 dest = EntityManager.GetComponentData<Translation>(playerEntity).Value;
        float dT = Time.DeltaTime;
        // Moves Enemies Towards Player
        Entities.ForEach((Entity e, ref Translation translation,ref EnemyComponent enemyComponent) =>
        {
           float3 origin = translation.Value;
           translation.Value +=math.normalize(dest- origin) * dT;

        }).ScheduleParallel();

        if (HasComponent<isDeadTag>(World.GetExistingSystem<GameHandler>().playerEntity))
        {
            Enabled = false;
        }
    }
}
