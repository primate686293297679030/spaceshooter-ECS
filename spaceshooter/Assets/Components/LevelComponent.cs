using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct LevelComponent : IComponentData
{
    public int wave;
    public int enemies;
    public int enemiesToSpawn;
}
