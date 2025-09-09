using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem
{
    public GridWorld gridWord;
    public Transform topPositionTube;
    public Transform bottomPositionTube;

    public NativeList<float3> GetPositionsTube(int amountSlot)
    {
        var positions = new NativeList<float3>(amountSlot, Allocator.Temp);
        var scale = new int2(2, 4);
        var topGridSize = int2.zero;
        var bottomGridSize = int2.zero;

        if (amountSlot <= 5)
            topGridSize = new int2(amountSlot, 1);
        else
        {
            topGridSize = new int2(5, 1);
            bottomGridSize = new int2(amountSlot - 5, 1);
        }

        gridWord.GridSize = topGridSize;
        gridWord.GridScale = scale;
        gridWord.transform.position = topPositionTube.position;
        gridWord.InitConvertedComponents();

        for (int i = 0; i < topGridSize.x * topGridSize.y; ++i)
        {
            var pos = gridWord.ConvertIndexToWorldPos(i);
            positions.Add(pos);
        }

        gridWord.GridSize = bottomGridSize;
        gridWord.GridScale = scale;
        gridWord.transform.position = bottomPositionTube.position;
        gridWord.InitConvertedComponents();

        for (int i = 0; i < bottomGridSize.x * bottomGridSize.y; ++i)
        {
            var pos = gridWord.ConvertIndexToWorldPos(i);
            positions.Add(pos);
        }

        return positions;
    }

    public NativeArray<float3> GetPositionsBlock(int amountSlot, float3 tubePosition)
    {
        var positions = new NativeArray<float3>(amountSlot, Allocator.Temp);
        var scale = new int2(1, 1);
        var gridSize = new int2(1, amountSlot);

        gridWord.GridSize = gridSize;
        gridWord.GridScale = scale;
        gridWord.transform.position = tubePosition;
        gridWord.InitConvertedComponents();

        for (int i = 0; i < amountSlot; ++i)
        {
            var pos = gridWord.ConvertIndexToWorldPos(i);
            positions[i] = pos;
        }
        return positions;
    }
}