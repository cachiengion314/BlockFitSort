using System.Collections.Generic;
using Lean.Touch;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [Header("Touch Control System")]
  bool _isUserScreenTouching;
  public bool IsUserScreenTouching { get { return _isUserScreenTouching; } }
  readonly RaycastHit[] results = new RaycastHit[10];

  void SubscribeTouchEvent()
  {
    LeanTouch.OnFingerDown += OnFingerDown;
    LeanTouch.OnGesture += OnGesture;
    LeanTouch.OnFingerInactive += OnFingerInactive;
  }

  void UnsubscribeTouchEvent()
  {
    LeanTouch.OnFingerDown -= OnFingerDown;
    LeanTouch.OnGesture -= OnGesture;
    LeanTouch.OnFingerInactive -= OnFingerInactive;
  }

  private void OnFingerDown(LeanFinger finger)
  {
    _isUserScreenTouching = true;

    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    var userTouchScreenPosition = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);
    Collider2D[] colliders = Physics2D.OverlapPointAll(
      new float2(userTouchScreenPosition.x, userTouchScreenPosition.y)
    );
    OnTouchTube(FindTubeIndex(colliders));
  }

  void OnGesture(List<LeanFinger> list)
  {
    _isUserScreenTouching = true;
  }

  private void OnFingerInactive(LeanFinger finger)
  {
    _isUserScreenTouching = false;
  }

  public Collider FindObjIn<T>(RaycastHit[] cols, int hitCount)
  {
    for (int i = 0; i < hitCount; ++i)
    {
      if (cols[i].collider == null) continue;
      if (cols[i].collider.TryGetComponent<T>(out var comp))
      {
        return cols[i].collider;
      }
    }
    return default;
  }

  public int FindTubeIndex(Collider2D[] cols)
  {
    for (int i = 0; i < cols.Length; ++i)
    {
      if (cols[i] == null) continue;
      for (int j = 0; j < tubeInstances.Count; j++)
      {
        if (cols[i].transform == tubeInstances[j])
          return j;
      }
    }
    return -1;
  }
}