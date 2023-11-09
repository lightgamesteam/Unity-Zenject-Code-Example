using UnityEngine;
using XDPaint.Tools.Raycast;

namespace XDPaint.Tools
{
    public static class ExtendedMethods
    {
        public static Vector2 Clamp(this Vector2 val, Vector2 from, Vector2 to)
        {
            if (val.x < from.x)
            {
                val.x = from.x;
            }

            if (val.y < from.y)
            {
                val.y = from.y;
            }

            if (val.x > to.x)
            {
                val.x = to.x;
            }

            if (val.y > to.y)
            {
                val.y = to.y;
            }
            return val;
        }
        
        public static bool IsNaNOrInfinity(this float val)
        {
            return float.IsInfinity(val) || float.IsNaN(val);
        }
        
        public static Triangle[] GetTrianglesData(this Mesh mesh, bool fillNeighbors = true)
        {
            var indices = mesh.triangles;
            if (indices.Length == 0)
            {
                Debug.LogError("Mesh doesn't have indices!");
                return new Triangle[0];
            }
            if (mesh.uv.Length == 0)
            {
                Debug.LogError("Mesh doesn't have UV!");
                return new Triangle[0];
            }

            var indexesCount = indices.Length;
            var triangles = new Triangle[indexesCount / 3];
            for (int i = 0; i < indexesCount; i += 3)
            {
                var index = i / 3;
                var index0 = indices[i + 0];
                var index1 = indices[i + 1];
                var index2 = indices[i + 2];
                
                triangles[index] = new Triangle((ushort)index, (ushort)index0, (ushort)index1, (ushort)index2);
            }

            if (fillNeighbors)
            {
                var positions = mesh.vertices;
                foreach (var triangle in triangles)
                {
                    var index0 = triangle.I0;
                    var index1 = triangle.I1;
                    var index2 = triangle.I2;

                    var position0 = positions[triangle.I0];
                    var position1 = positions[triangle.I1];
                    var position2 = positions[triangle.I2];

                    foreach (var triangleFind in triangles)
                    {
                        var indexFind0 = triangleFind.I0;
                        var indexFind1 = triangleFind.I1;
                        var indexFind2 = triangleFind.I2;

                        var positionFind0 = positions[triangleFind.I0];
                        var positionFind1 = positions[triangleFind.I1];
                        var positionFind2 = positions[triangleFind.I2];

                        if (triangleFind.Id != triangle.Id)
                        {
                            if (index0 == indexFind0 || index0 == indexFind1 || index0 == indexFind2 ||
                                index1 == indexFind0 || index1 == indexFind1 || index1 == indexFind2 ||
                                index2 == indexFind0 || index2 == indexFind1 || index2 == indexFind2 ||
                                position0 == positionFind0 || position0 == positionFind1 || position0 == positionFind2 ||
                                position1 == positionFind0 || position1 == positionFind1 || position1 == positionFind2 ||
                                position2 == positionFind0 || position2 == positionFind1 || position2 == positionFind2)
                            {
                                if (!triangle.N.Contains(triangleFind.Id))
                                {
                                    triangle.N.Add(triangleFind.Id);
                                }
                            }
                        }
                    }
                }
            }
            return triangles;
        }
    }
}