using System.Collections.Generic;
using UnityEngine;
using Bizier;
using Bizier.Structures;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {
    [SerializeField] private PathCreator PathCreator;
    [SerializeField] private MeshFilter meshFilter;

    [SerializeField] [Range(0, 1)] private float t;
    [SerializeField] private float angleBetween;
    [SerializeField] private float drawGap;
    [SerializeField] private int persistanceCount;
    [SerializeField] private float size;

    private void FixedUpdate() {
        var data = PathCreator.GetBzizierPointsData(
            t, drawGap, persistanceCount, PathCreator.IsClosed);
        meshFilter.mesh = Generate(data);
    }

    private Mesh Generate(List<CurvePointData> pathData) {
        var res = new Mesh();
        var points = new List<Vector3>();
        var tris = new List<int>();
        var uv = new List<Vector2>();
        var angleRad = Mathf.Deg2Rad * angleBetween;
        var cos = Mathf.Cos(angleRad);
        var sin = Mathf.Sin(angleRad);
        var fraction = 1f / (pathData.Count - 1);

        for (int i = 0; i < pathData.Count; i++) {
            var data = pathData[i];
            var up = data.Normal;
            var right = data.Right;
            var pp = data.Position;
            var p1 = pp + (up * sin + right * cos).normalized * size;
            var p2 = pp;
            var p3 = pp + (up * sin + -right * cos).normalized * size;
            points.AddTriplet(p1, p2, p3);
            var uvx = fraction * i;
            uv.AddTriplet(new Vector2(uvx, 1), new Vector2(uvx, 0.5f), new Vector2(uvx, 0));
        }

        for (int i = 0; i < pathData.Count - 1; i++) {
            var p1 = i * 3 + 0;
            var p2 = i * 3 + 1;
            var p3 = i * 3 + 2;
            var n1 = (i + 1) * 3 + 0;
            var n2 = (i + 1) * 3 + 1;
            var n3 = (i + 1) * 3 + 2;

            tris.AddTriplet(p1, n1, p2);
            tris.AddTriplet(p2, n1, n2);
            tris.AddTriplet(p2, n2, n3);
            tris.AddTriplet(p2, n3, p3);
        }

        res.vertices = points.ToArray();
        res.triangles = tris.ToArray();
        res.uv = uv.ToArray();

        return res;
    }
}

public static class ListExtensions {
    public static void AddTriplet<T>(this List<T> list, T p1, T p2, T p3) {
        list.Add(p1);
        list.Add(p2);
        list.Add(p3);
    }
}

