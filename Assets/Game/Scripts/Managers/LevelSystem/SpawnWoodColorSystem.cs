using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem
{
    List<Transform> tubeInstances;
    Transform[] blockInstances;
    int totalBlocks = 0;
    NativeList<TubeData> tubeDatas;

    void InitEntitiesDataBuffersWoodColor(LevelInformation levelInformation)
    {
        var tubeDatas = levelInformation.TubeDatas;
        InitDataTubes(tubeDatas);
        InitDataBlocks(tubeDatas);
    }

    void InitDataTubes(TubeDataEditor[] tubeDatas)
    {
        this.tubeDatas = new NativeList<TubeData>(10, Allocator.Persistent);
        using var tubePositions = GetPositionsTube(tubeDatas.Length);
        for (int i = 0; i < tubeDatas.Length; i++)
        {
            var _tubeData = tubeDatas[i];
            using var blockSlots = GetPositionsBlock(_tubeData.MaxBlock, tubePositions[i]);
            var tubeData = new TubeData
            {
                Index = i,
                TubePosition = tubePositions[i],
                Positions = new NativeArray<float3>(blockSlots, Allocator.Persistent),
                Blocks = new NativeList<BlockData>(_tubeData.MaxBlock, Allocator.Persistent)
            };
            this.tubeDatas.Add(tubeData);
        }
    }

    void InitDataBlocks(TubeDataEditor[] tubeDatas)
    {
        totalBlocks = 0;
        for (int i = 0; i < tubeDatas.Length; i++)
        {
            var _tubeData = tubeDatas[i];
            var tubeData = this.tubeDatas[i];
            for (int j = 0; j < _tubeData.Blocks.Length; j++)
            {
                var _blockData = _tubeData.Blocks[j];
                var blockData = new BlockData
                {
                    Index = totalBlocks,
                    Position = tubeData.Positions[j],
                    ColorValue = _blockData.ColorValue
                };
                tubeData.Blocks.Add(blockData);
                totalBlocks++;
            }
            this.tubeDatas[i] = tubeData;
        }
    }

    void SpawnAndBakingEntityDatasWoodColor(LevelInformation levelInformation)
    {
        tubeInstances = new List<Transform>(10);
        blockInstances = new Transform[totalBlocks];
        for (int i = 0; i < tubeDatas.Length; i++)
        {
            var tubeData = tubeDatas[i];
            var tubeInstance = SpawnTube(tubeData.TubePosition);
            tubeInstances.Add(tubeInstance);
            for (int j = 0; j < tubeData.Blocks.Length; j++)
            {
                var blockData = tubeData.Blocks[j];
                var blockInstance = SpawnBlock(blockData.Position);
                if (blockInstance.TryGetComponent(out ISpriteRenderer spriteRdrComp))
                    spriteRdrComp.SetColor(blockData.ColorValue);
                blockInstances[blockData.Index] = blockInstance;
            }
        }
    }
}

public struct BlockData
{
    public int Index;
    public float3 Position;
    public int ColorValue;
}

public struct TubeData
{
    public int Index;
    public float3 TubePosition;
    public NativeArray<float3> Positions;
    public NativeList<BlockData> Blocks;
}