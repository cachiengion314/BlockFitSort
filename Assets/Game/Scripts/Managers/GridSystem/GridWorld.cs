using HoangNam;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class GridWorld : MonoBehaviour
{
  [Header("Setting")]
  [SerializeField] bool shouldDrawGrid;
  [Range(0, 10)]
  [SerializeField] int colorIndex;
  float3x3 _originalMatrix;
  public float3x3 OriginalMatrix { get { return _originalMatrix; } }
  float3x3 _rotatedMatrix;
  public float3x3 RotatedMatrix { get { return _rotatedMatrix; } }
  public float3 DegAround;
  public int2 GridSize;
  public float2 GridScale;
  public float2 Offset { get; private set; }

  private void Update()
  {
#if UNITY_EDITOR
    DrawGrid();
#endif
  }

  public static float3x3 GetMatrixWith(float3 degAround)
  {
    var degAroundX = degAround.x;
    var degAroundY = degAround.y;
    var degAroundZ = degAround.z;

    var phiX = degAroundX * math.PI / 180;
    var c1x = new float3(1, 0, 0);
    var c2x = new float3(0, math.cos(phiX), math.sin(phiX));
    var c3x = new float3(0, -math.sin(phiX), math.cos(phiX));
    var Mx = new float3x3(c1x, c2x, c3x);

    var phiY = degAroundY * math.PI / 180;
    var c1y = new float3(math.cos(phiY), 0, math.sin(phiY));
    var c2y = new float3(0, 1, 0);
    var c3y = new float3(-math.sin(phiY), 0, math.cos(phiY));
    var My = new float3x3(c1y, c2y, c3y);

    var phiZ = degAroundZ * math.PI / 180;
    var c1z = new float3(math.cos(phiZ), math.sin(phiZ), 0);
    var c2z = new float3(-math.sin(phiZ), math.cos(phiZ), 0);
    var c3z = new float3(0, 0, 1);
    var Mz = new float3x3(c1z, c2z, c3z);

    var MxMy = math.mul(Mx, My);
    var MxMyMz = math.mul(MxMy, Mz);
    return MxMyMz;
  }

  public void InitConvertedComponents()
  {
    _originalMatrix = GetMatrixWith(0);
    _rotatedMatrix = GetMatrixWith(DegAround);
    Offset = new Vector2((GridSize.x - 1) / 2f, (GridSize.y - 1) / 2f);
  }

  public int ConvertGridPosToIndex(in int2 gridPos)
  {
    var index = GridSize.y * gridPos.x + gridPos.y;
    return index;
  }

  static public int ConvertGridPosToIndex(
    in int2 gridPos,
    in int2 GridSize
  )
  {
    var index = GridSize.y * gridPos.x + gridPos.y;
    return index;
  }

  public int2 ConvertIndexToGridPos(in int index)
  {
    int x = (int)(uint)math.floor(index / GridSize.y);
    int y = index - (x * GridSize.y);
    return new int2(x, y);
  }

  static public int2 ConvertIndexToGridPos(
    in int index,
    in int2 GridSize
  )
  {
    int x = (int)(uint)math.floor(index / GridSize.y);
    int y = index - (x * GridSize.y);
    return new int2(x, y);
  }

  public float3 ConvertGridPosToWorldPos(in int2 gridPos)
  {
    var A2 = gridPos;
    var O2 = Offset;
    var O2A2 = A2 - O2;
    var A1 = new float2(O2A2.x * GridScale.x, O2A2.y * GridScale.y);
    var worldPos = new float3(A1.x, A1.y, 0);
    return ConvertToRotated(worldPos);
  }

  static public float3 ConvertGridPosToWorldPos(
    in int2 gridPos,
    float2 GridScale,
    float2 Offset,
    float3 coordPosition,
    float3x3 _rotatedMatrix
  )
  {
    var A2 = gridPos;
    var O2 = Offset;
    var O2A2 = A2 - O2;
    var A1 = new float2(O2A2.x * GridScale.x, O2A2.y * GridScale.y);
    var worldPos = new float3(A1.x, A1.y, 0);
    return ConvertToRotated(worldPos, _rotatedMatrix, coordPosition);
  }

  public int2 ConvertWorldPosToGridPos(in float3 worldPos)
  {
    var O1A1 = ConvertToNonRotated(worldPos);
    var O2A2 = new float2(O1A1.x * 1 / GridScale.x, O1A1.y * 1 / GridScale.y);
    var A2 = Offset + new float2(O2A2.x, O2A2.y);
    int xRound = (int)math.round(A2.x);
    int yRound = (int)math.round(A2.y);
    var gridPos = new int2(xRound, yRound);
    return gridPos;
  }

  static public int2 ConvertWorldPosToGridPos(
    in float3 worldPos,
    in float2 GridScale,
    in float2 Offset,
    in float3 coordPosition,
    in float3x3 _rotatedMatrix
  )
  {
    var O1A1 = ConvertToNonRotated(worldPos, coordPosition, _rotatedMatrix);
    var O2A2 = new float2(O1A1.x * 1 / GridScale.x, O1A1.y * 1 / GridScale.y);
    var A2 = Offset + new float2(O2A2.x, O2A2.y);
    int xRound = (int)math.round(A2.x);
    int yRound = (int)math.round(A2.y);
    var gridPos = new int2(xRound, yRound);
    return gridPos;
  }

  public int ConvertWorldPosToIndex(in float3 worldPos)
  {
    var grid = ConvertWorldPosToGridPos(worldPos);
    var index = ConvertGridPosToIndex(grid);
    return index;
  }

  static public int ConvertWorldPosToIndex(
    in float3 worldPos,
    in int2 GridSize,
    in float2 GridScale,
    in float2 Offset,
    in float3 coordPosition,
    in float3x3 _rotatedMatrix
  )
  {
    var grid = ConvertWorldPosToGridPos(worldPos, GridScale, Offset, coordPosition, _rotatedMatrix);
    var index = ConvertGridPosToIndex(grid, GridSize);
    return index;
  }

  public float3 ConvertIndexToWorldPos(in int index)
  {
    var grid = ConvertIndexToGridPos(index);
    var worldPos = ConvertGridPosToWorldPos(grid);
    return worldPos;
  }

  static public float3 ConvertIndexToWorldPos(
    in int index,
    in int2 GridSize,
    in float2 GridScale,
    in float2 Offset,
    in float3 coordPosition,
    in float3x3 _rotatedMatrix
  )
  {
    var grid = ConvertIndexToGridPos(index, GridSize);
    var worldPos = ConvertGridPosToWorldPos(
      grid, GridScale, Offset, coordPosition, _rotatedMatrix
    );
    return worldPos;
  }

  public bool IsPosOutsideAt(float3 worldPos)
  {
    int2 gridPos = ConvertWorldPosToGridPos(worldPos);
    return IsGridPosOutsideAt(gridPos);
  }

  static public bool IsPosOutsideAt(
    in float3 worldPos,
    in int2 GridSize,
    in float2 GridScale,
    in float2 Offset,
    in float3 coordPosition,
    in float3x3 _rotatedMatrix
  )
  {
    int2 gridPos = ConvertWorldPosToGridPos(worldPos, GridScale, Offset, coordPosition, _rotatedMatrix);
    return IsGridPosOutsideAt(gridPos, GridSize);
  }

  public bool IsGridPosOutsideAt(int2 gridPos)
  {
    if (gridPos.x > GridSize.x - 1 || gridPos.x < 0) return true;
    if (gridPos.y > GridSize.y - 1 || gridPos.y < 0) return true;
    return false;
  }

  static public bool IsGridPosOutsideAt(int2 gridPos, int2 GridSize)
  {
    if (gridPos.x > GridSize.x - 1 || gridPos.x < 0) return true;
    if (gridPos.y > GridSize.y - 1 || gridPos.y < 0) return true;
    return false;
  }

  public float3 ConvertToRotated(in float3 worldPos)
  {
    return math.mul(worldPos, _rotatedMatrix) + (float3)transform.position;
  }

  static public float3 ConvertToRotated(
    in float3 worldPos,
    in float3x3 _rotatedMatrix,
    in float3 coordPosition
  )
  {
    return math.mul(worldPos, _rotatedMatrix) + coordPosition;
  }

  /// <summary>
  /// return a world position without any rotating processes.
  /// </summary>
  /// <returns></returns>
  public float3 ConvertToNonRotated(in float3 worldPos)
  {
    return math.mul(worldPos - (float3)transform.position, math.inverse(_rotatedMatrix));
  }

  static public float3 ConvertToNonRotated(
    in float3 worldPos,
    float3 coordPosition,
    float3x3 _rotatedMatrix
  )
  {
    return math.mul(worldPos - coordPosition, math.inverse(_rotatedMatrix));
  }

  /// <summary>
  /// Only for debugging
  /// </summary>
  void DrawGrid()
  {
    if (!shouldDrawGrid) return;
    for (int x = 0; x < GridSize.x; ++x)
    {
      for (int y = 0; y < GridSize.y; ++y)
      {
        var worldPos = ConvertGridPosToWorldPos(new int2(x, y));
        HoangNam.Utility.DrawQuad(worldPos, GridScale, DegAround, (ColorIndex)colorIndex);
      }
    }
  }

  /// <summary>
  /// Only for debugging
  /// </summary>
  void OnDrawGizmosSelected()
  {
    var color = HoangNam.Utility.GetColorFrom((ColorIndex)colorIndex);
    Gizmos.color = color;

    var centerPos = transform.position;
    var rotatedMatrix = GetMatrixWith(DegAround);
    var x = GridSize.x * GridScale.x;
    var y = GridSize.y * GridScale.y;
    var leftDir = new Vector3(-x / 2f, 0, 0);
    var bottomDir = new Vector3(0, -y / 2f, 0);
    var rightDir = new Vector3(x / 2f, 0, 0);
    var topDir = new Vector3(0, y / 2f, 0);
    Vector3 bottomLeftDir = math.mul(bottomDir + leftDir, rotatedMatrix);
    Vector3 bottomRightDir = math.mul(bottomDir + rightDir, rotatedMatrix);
    Vector3 topLeftDir = math.mul(topDir + leftDir, rotatedMatrix);
    Vector3 topRightDir = math.mul(topDir + rightDir, rotatedMatrix);

    var bottomLeftPos = centerPos + bottomLeftDir;
    var bottomRightPos = centerPos + bottomRightDir;
    var topLeftPos = centerPos + topLeftDir;
    var topRightPos = centerPos + topRightDir;

    var points = new Vector3[8]
    {
      bottomLeftPos,
      bottomRightPos,
      bottomRightPos,
      topRightPos,
      topRightPos,
      topLeftPos,
      topLeftPos,
      bottomLeftPos
    };

    Gizmos.DrawLineList(points);
  }

  public NativeArray<int2> FindNeighberAt(int index)
  {
    using var directions = new NativeArray<int2>(
      new int2[4] { new(0, 1), new(1, 0), new(0, -1), new(-1, 0) }, Allocator.Temp);
    var neighbers = new NativeArray<int2>(directions.Length, Allocator.Temp);

    var gridPos = ConvertIndexToGridPos(index);
    for (int i = 0; i < directions.Length; i++)
    {
      var direction = directions[i];
      var neighber = gridPos + direction;
      neighbers[i] = neighber;
    }
    return neighbers;
  }
}