using System.Collections.Generic;
using DG.Tweening;
using Firebase.Analytics;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem
{
  void VisualzeMoveBlocks(TubeData tubeData)
  {
    var maxY = math.max(tubeData.TubePosition.y, AvailableTube.TubePosition.y);
    var targetPos1 = AvailableTube.TubePosition;
    targetPos1.y = maxY + 2.5f;
    var targetPos2 = tubeData.TubePosition;
    targetPos2.y = maxY + 2.5f;

    Sequence seq = DOTween.Sequence();
    var duration = 0.3f;
    for (var i = 0; i < AvailableBlocks.Length; i++)
    {
      var block = AvailableBlocks[i];
      var blockInstance = blockInstances[block.IndexRef];
      Vector3[] path;
      if (block.IndexTube == tubeData.Index)
      {
        path = new Vector3[4];
        path[0] = blockInstance.position;
        path[1] = targetPos1;
        path[2] = targetPos2;
        path[3] = block.Position;
      }
      else
      {
        path = new Vector3[2];
        path[0] = blockInstance.position;
        path[1] = block.Position;
      }
      Tween tween = blockInstance.DOPath(path, duration);
      seq.Join(tween);
    }
  }

  void VisualzeTrigerBlock()
  {
    
  }
}