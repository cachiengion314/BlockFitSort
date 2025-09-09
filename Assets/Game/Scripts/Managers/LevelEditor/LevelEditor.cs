using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class LevelInformation
{
  [ViewOnly] public int Index;
  public TubeDataEditor[] TubeDatas;
}

[Serializable]
public struct TubeDataEditor
{
  public int MaxBlock;
  public BlockDataEditor[] Blocks;
}

[Serializable]
public struct BlockDataEditor
{
  public int ColorValue;
}

public class LevelEditor : MonoBehaviour
{
  [Header("Level Editor")]
  [SerializeField] GridEditorControl gridEditorPref;
  [SerializeField] GridEditorControl[] gridEditorInstance;
  [SerializeField] LevelInformation levelInformation;
  [SerializeField][Range(1, 30)] int levelSelected = 1;
  [SerializeField] GridWorld gridWord;

  [NaughtyAttributes.Button]
  void CreateGrid()
  {
    ClearGrid();
    gridWord.InitConvertedComponents();

    gridEditorInstance = new GridEditorControl[gridWord.GridSize.x * gridWord.GridSize.y];
    for (int i = 0; i < gridEditorInstance.Length; ++i)
    {
      var instance = Instantiate(gridEditorPref, gridWord.transform);
      var pos = gridWord.ConvertIndexToWorldPos(i);
      var scale = gridWord.GridScale * 0.9f;
      instance.transform.position = pos;
      instance.transform.localScale = new Vector3(scale.x, scale.y, 1);
      gridEditorInstance[i] = instance;
    }
  }

  [NaughtyAttributes.Button]
  void Clear()
  {
    levelInformation = new LevelInformation();
    ClearGrid();
  }

  void ClearGrid()
  {
    for (int i = gridWord.transform.childCount - 1; i >= 0; i--)
    {
      var child = gridWord.transform.GetChild(i);
      DestroyImmediate(child.gameObject);
    }
  }

  [NaughtyAttributes.Button]
  void LoadLevel()
  {
    LoadLevelFrom(levelSelected);
  }

  public void LoadLevelFrom(int level)
  {
    var _rawLevelInfo = Resources.Load<TextAsset>(
      "Levels/" + KeyString.NAME_LEVEL_FILE + level
    ).text;
    var levelInfo = JsonUtility.FromJson<LevelInformation>(_rawLevelInfo);

    if (levelInfo == null) { print("This level is not existed!"); return; }
    levelInformation = levelInfo;

    CreateGrid();
    print("Load level successfully");
  }

  [NaughtyAttributes.Button]
  void SaveLevel()
  {
    levelInformation.Index = levelSelected - 1;

    HoangNam.SaveSystem.Save(
      levelInformation,
      "Resources/Levels/" + KeyString.NAME_LEVEL_FILE + levelSelected
    );
    print("Save level successfully");
  }
}
