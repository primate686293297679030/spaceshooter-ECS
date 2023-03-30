using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;

[GenerateAuthoringComponent]
public struct ShootingComponent : IComponentData
{
    public Vector3 dir;
}
