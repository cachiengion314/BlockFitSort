using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] Transform spawnedParent;
  [SerializeField] Transform tubePref;
  [SerializeField] Transform blockPref;
  [SerializeField] Transform cube2DPref;

  Transform SpawnTube(float3 localPos)
  {
    var obj = Instantiate(tubePref, spawnedParent);
    obj.localPosition = localPos;
    return obj;
  }

  Transform SpawnBlock(float3 localPos)
  {
    var obj = Instantiate(blockPref, spawnedParent);
    obj.localPosition = localPos;
    return obj;
  }

  Transform SpawnCube2D(float3 localPos)
  {
    var obj = Instantiate(cube2DPref, spawnedParent);
    obj.localPosition = localPos;
    return obj;
  }
}