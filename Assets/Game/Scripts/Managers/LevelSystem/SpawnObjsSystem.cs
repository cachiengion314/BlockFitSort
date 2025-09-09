using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] Transform spawnedParent;
  [SerializeField] Transform tubePref;
  [SerializeField] Transform blockPref;

  Transform SpawnTube(float3 pos)
  {
    var localPos = spawnedParent.InverseTransformPoint(pos);
    var obj = Instantiate(tubePref, spawnedParent);
    obj.localPosition = localPos;
    return obj;
  }

  Transform SpawnBlock(float3 pos)
  {
    var localPos = spawnedParent.InverseTransformPoint(pos);
    var obj = Instantiate(blockPref, spawnedParent);
    obj.localPosition = localPos;
    return obj;
  }
}