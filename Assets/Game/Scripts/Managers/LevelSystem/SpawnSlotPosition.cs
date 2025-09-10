using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem
{
    public GridWorld topGridWord;
    public GridWorld bottomGridWord;
    public GridWorld slotGridWord;
    int maxLengthGrid = 5;

    public NativeList<float3> GetPositionsTube(int amountSlot)
    {
        var positions = new NativeList<float3>(amountSlot, Allocator.Temp);
        var scale = new int2(2, 4);
        var topGridSize = int2.zero;
        var bottomGridSize = int2.zero;

        if (amountSlot <= maxLengthGrid)
            topGridSize = new int2(amountSlot, 1);
        else
        {
            topGridSize = new int2(5, 1);
            bottomGridSize = new int2(amountSlot - maxLengthGrid, 1);
        }

        topGridWord.GridSize = topGridSize;
        topGridWord.GridScale = scale;
        topGridWord.InitConvertedComponents();

        for (int i = 0; i < topGridSize.x * topGridSize.y; ++i)
        {
            var pos = topGridWord.ConvertIndexToWorldPos(i);
            positions.Add(pos);
        }

        bottomGridWord.GridSize = bottomGridSize;
        bottomGridWord.GridScale = scale;
        bottomGridWord.InitConvertedComponents();

        for (int i = 0; i < bottomGridSize.x * bottomGridSize.y; ++i)
        {
            var pos = bottomGridWord.ConvertIndexToWorldPos(i);
            positions.Add(pos);
        }

        return positions;
    }

    public NativeArray<float3> GetPositionsBlock(int amountSlot, float3 tubePosition)
    {
        var positions = new NativeArray<float3>(amountSlot, Allocator.Temp);
        var scale = new int2(1, 1);
        var gridSize = new int2(1, amountSlot);

        slotGridWord.GridSize = gridSize;
        slotGridWord.GridScale = scale;
        slotGridWord.transform.position = tubePosition;
        slotGridWord.InitConvertedComponents();

        for (int i = 0; i < amountSlot; ++i)
        {
            var pos = slotGridWord.ConvertIndexToWorldPos(i);
            positions[i] = pos;
        }
        return positions;
    }

    NativeList<TubeData> FindNeighberAt(TubeData tubeData)
    {
        var tubeNeighbers = new NativeList<TubeData>(2, Allocator.Temp);
        var gridWord = topGridWord;
        var index = tubeData.Index;
        if (tubeData.Index >= maxLengthGrid)
        {
            gridWord = bottomGridWord;
            index -= maxLengthGrid;
        }
        var neighbers = gridWord.FindNeighberAt(index);
        for (int i = 0; i < neighbers.Length; i++)
        {
            var neighber = neighbers[i];
            if (gridWord.IsGridPosOutsideAt(neighber)) continue;
            var neighberIdx = gridWord.ConvertGridPosToIndex(neighber);
            if (tubeData.Index >= maxLengthGrid)
                neighberIdx += maxLengthGrid;
            tubeNeighbers.Add(tubeDatas[neighberIdx]);
        }
        return tubeNeighbers;
    }
}