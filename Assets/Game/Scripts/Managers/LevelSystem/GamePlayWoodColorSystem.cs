using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem
{
    void OnTouchTube(int indexTube)
    {
        if (indexTube == -1) return;
        if (AvailableTube.Index == indexTube) return;
        var tubeData = tubeDatas[indexTube];
        if (AvailableBlocks.Length == 0)
        {
            FindFirstBlockOfTheSameColor(tubeData);
            return;
        }

        if (tubeData.MaxBlock == tubeData.Blocks.Length)
        {
            FindFirstBlockOfTheSameColor(tubeData);
            return;
        }

        if (tubeData.Blocks.Length > 0)
        {
            var firstColorValue = tubeData.Blocks[^1].ColorValue;
            var availableColorValue = AvailableBlocks[0].ColorValue;
            if (firstColorValue != availableColorValue)
            {
                FindFirstBlockOfTheSameColor(tubeData);
                return;
            }
        }

        for (int i = 0; i < AvailableBlocks.Length; i++)
        {
            if (tubeData.Blocks.Length == tubeData.MaxBlock) break;
            var block = AvailableBlocks[i];
            AvailableTube.Blocks.RemoveAt(block.Index);

            var index = tubeData.Blocks.Length;
            block.Index = index;
            block.Position = tubeData.Positions[index];
            tubeData.Blocks.Add(block);
            AvailableBlocks[i] = block;
        }
        tubeDatas[AvailableTube.Index] = AvailableTube;
        tubeDatas[tubeData.Index] = tubeData;

        for (int i = 0; i < AvailableBlocks.Length; i++)
        {
            var block = AvailableBlocks[i];
            var blockInstance = blockInstances[block.IndexRef];
            blockInstance.position = block.Position;
        }

        AvailableBlocks.Clear();
        AvailableTube.Index = -1;
    }

    void FindFirstBlockOfTheSameColor(TubeData tubeData)
    {
        AvailableTube = tubeData;
        AvailableBlocks.Clear();
        var blockDatas = tubeData.Blocks;
        if (blockDatas.Length == 0) return;
        var firstColorValue = blockDatas[^1].ColorValue;
        for (int i = blockDatas.Length - 1; i >= 0; i--)
        {
            var block = blockDatas[i];
            if (firstColorValue.Equals(block.ColorValue))
                AvailableBlocks.Add(block);
        }
    }
}