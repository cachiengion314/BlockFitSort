using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using System.Collections;
using DG.Tweening;

public partial class LevelSystem : MonoBehaviour
{
  public static LevelSystem Instance { get; private set; }
  [SerializeField] CinemachineCamera cinemachineCamera;
  LevelInformation _levelInformation;
  [SerializeField][Range(1, 30)] int levelSelected = 1;
  public bool IsSelectedLevel;
  bool isLoadedLevel = false;

  IEnumerator Start()
  {
    if (Instance == null)
    {
      DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(1500, 100);
      Instance = this;
    }
    else Destroy(gameObject);

    if (IsSelectedLevel)
    {
      GameManager.Instance.CurrentLevelIndex = levelSelected - 1;
      LoadLevelFrom(levelSelected);
    }
    else LoadLevelFrom(GameManager.Instance.CurrentLevelIndex + 1);

    GameManager.Instance.SetGameState(GameState.Gameplay);
    SubscribeTouchEvent();
    yield return new WaitForSeconds(0.1f);
    SetupCurrentLevel(_levelInformation);
    isLoadedLevel = true;
  }

  void OnDestroy()
  {
    UnsubscribeTouchEvent();
    DisposeDataBuffers();
  }

  void Update()
  {
    if (!isLoadedLevel) return;
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;

  }

  void SetupCurrentLevel(LevelInformation levelInformation)
  {
    BakingGrids(levelInformation);
    InitEntitiesDataBuffers(levelInformation);
    SpawnAndBakingEntityDatas(levelInformation);
    SetSizeCamera();
  }

  void BakingGrids(LevelInformation levelInformation)
  {

  }

  void SpawnAndBakingEntityDatas(LevelInformation levelInformation)
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
        blockInstances[blockData.IndexRef] = blockInstance;
      }
    }
  }

  public void LoadLevelFrom(int level)
  {
    var _rawLevelInfo = Resources.Load<TextAsset>("Levels/" + KeyString.NAME_LEVEL_FILE + level).text;
    var levelInfo = JsonUtility.FromJson<LevelInformation>(_rawLevelInfo);

    if (levelInfo == null) { print("This level is not existed!"); return; }
    _levelInformation = levelInfo;
    print("Load level " + level + " successfully ");
  }

  void SetSizeCamera()
  {
    // var size = 27;
    // cinemachineCamera.Lens.OrthographicSize = size;
  }
}
