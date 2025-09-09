using System;
using Unity.Mathematics;
using UnityEngine;

namespace HoangNam
{
  public enum ColorIndex
  {
    None,
    White,
    Blue,
    Red,
    Green,
    Yellow,
    Cyan,
    Azure,
    Beige,
    Chocolate,
    Coral
  }

  public class Utility
  {
    public static string GetCurrentDate()
    {
      DateTime dateTime = DateTime.Now.Date;
      return dateTime.ToString("dd/MM/yyyy");
    }

    public static string FormatDateFrom(float seconds)
    {
      TimeSpan time = TimeSpan.FromSeconds(seconds);
      string str = time.ToString(@"mm\:ss");
      return str;
    }

    public static int GetEpochTime()
    {
      TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
      int secondsSinceEpoch = (int)t.TotalSeconds;
      return secondsSinceEpoch;
    }

    public static float3 Lerp(float3 start, float3 end, float t)
    {
      return (1 - t) * start + t * end;
    }

    public static void InterpolateMoveUpdate(
      in float3 currentPos,
      in float3 startPos,
      in float3 targetPos,
      in float speed,
      out float _t,
      out float3 nextPos
    )
    {
      var distanceFromStart = math.length(currentPos - startPos);
      var totalDistance = ((Vector3)targetPos - (Vector3)startPos).magnitude;
      var t = distanceFromStart / totalDistance + speed * 1 / totalDistance * Time.deltaTime;
      _t = math.min(t, 1);
      nextPos = Lerp(startPos, targetPos, _t);
    }

    /// <summary>
    /// EDIT: That's in radians, btw. If you need degrees, multiply by Mathf.Deg2Rad or Mathf.Rad2Deg depending on direction.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public static Vector2 Rotate(Vector2 v, float delta)
    {
      return new Vector2(
        v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
        v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
      );
    }

    public static float4 RandomColor(ref Unity.Mathematics.Random random)
    {
      var hue = (random.NextFloat() + 0.618034005f) % 1;
      return (Vector4)Color.HSVToRGB(hue, 1.0f, 1.0f);
    }

    public static float2 GetRandomDir()
    {
      float randomX = UnityEngine.Random.Range(-1f, 1f);
      float randomY = UnityEngine.Random.Range(-1f, 1f);

      Vector2 randomDirection = new Vector2(randomX, randomY);
      randomDirection.Normalize();
      return randomDirection;
    }

    /// <summary>
    /// BurstCompile not capable
    /// </summary>
    /// <param name="o"></param>
    public static void Print(in object o)
    {
      Debug.Log(o);
    }

    public static Color GetColorFrom(in ColorIndex colorIndex = 0)
    {
      var color = Color.white;
      if (colorIndex == ColorIndex.Blue)
        color = Color.blue;
      else if (colorIndex == ColorIndex.Red)
        color = Color.red;
      else if (colorIndex == ColorIndex.Green)
        color = Color.green;
      else if (colorIndex == ColorIndex.Yellow)
        color = Color.yellow;
      else if (colorIndex == ColorIndex.Cyan)
        color = Color.cyan;
      else if (colorIndex == ColorIndex.Azure)
        color = Color.azure;
      else if (colorIndex == ColorIndex.Beige)
        color = Color.beige;
      else if (colorIndex == ColorIndex.Chocolate)
        color = Color.chocolate;
      else if (colorIndex == ColorIndex.Coral)
        color = Color.coral;
      return color;
    }

    /// <summary>
    /// BurstCompile capable\
    /// 0: White
    /// 1: Blue
    /// 2: Red
    /// 3: Green
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public static void DrawLine(in float3 start, in float3 end, in ColorIndex colorIndex = 0)
    {
      var color = GetColorFrom(colorIndex);

      Debug.DrawLine(start, end, color);
    }

    /// <summary>
    /// BurstCompile capable
    /// 0: White
    /// 1: Blue
    /// 2: Red
    /// 3: Green
    /// </summary>
    /// <param name="start"></param>
    /// <param name="dir"></param>
    public static void DrawRay(in float3 start, in float3 dir, in ColorIndex colorIndex = 0)
    {
      var color = GetColorFrom(colorIndex);

      Debug.DrawRay(start, dir, color);
    }

    public static float3x3 GetMatrixWith(float3 degAround)
    {
      var degAroundX = degAround.x;
      var degAroundY = degAround.y;
      var degAroundZ = degAround.z;

      var ɸx = degAroundX * math.PI / 180;
      var c1x = new float3(1, 0, 0);
      var c2x = new float3(0, math.cos(ɸx), math.sin(ɸx));
      var c3x = new float3(0, -math.sin(ɸx), math.cos(ɸx));
      var Mx = new float3x3(c1x, c2x, c3x);

      var ɸy = degAroundY * math.PI / 180;
      var c1y = new float3(math.cos(ɸy), 0, math.sin(ɸy));
      var c2y = new float3(0, 1, 0);
      var c3y = new float3(-math.sin(ɸy), 0, math.cos(ɸy));
      var My = new float3x3(c1y, c2y, c3y);

      var ɸz = degAroundZ * math.PI / 180;
      var c1z = new float3(math.cos(ɸz), math.sin(ɸz), 0);
      var c2z = new float3(-math.sin(ɸz), math.cos(ɸz), 0);
      var c3z = new float3(0, 0, 1);
      var Mz = new float3x3(c1z, c2z, c3z);

      var MxMy = math.mul(Mx, My);
      var MxMyMz = math.mul(MxMy, Mz);
      return MxMyMz;
    }

    /// <summary>
    /// BurstCompile capable
    /// </summary>
    public static void DrawQuad(in float3 worldPos, in float2 size, in float3 degAround, in ColorIndex colorIndex = 0)
    {
      var color = GetColorFrom(colorIndex);

      var x = size.x / 2;
      var y = size.y / 2;

      var rotatedMatrix = GetMatrixWith(degAround);

      var offset = worldPos;
      var pos1 = offset + math.mul(new float3(-x, -y, 0), rotatedMatrix);
      var pos2 = offset + math.mul(new float3(-x, y, 0), rotatedMatrix);
      var pos3 = offset + math.mul(new float3(x, y, 0), rotatedMatrix);
      var pos4 = offset + math.mul(new float3(x, -y, 0), rotatedMatrix);

      Debug.DrawLine(pos1, pos2, color);
      Debug.DrawLine(pos2, pos3, color);
      Debug.DrawLine(pos3, pos4, color);
      Debug.DrawLine(pos4, pos1, color);
    }

    static public void DrawString(string text, Vector3 worldPos, Color? colour = null)
    {
#if UNITY_EDITOR
      UnityEditor.Handles.BeginGUI();

      var restoreColor = GUI.color;

      if (colour.HasValue) GUI.color = colour.Value;
      var view = UnityEditor.SceneView.currentDrawingSceneView;
      Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

      if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
      {
        GUI.color = restoreColor;
        UnityEditor.Handles.EndGUI();
        return;
      }

      Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
      GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height - 35, size.x, size.y), text);
      GUI.color = restoreColor;
      UnityEditor.Handles.EndGUI();
#endif
    }
  }
}