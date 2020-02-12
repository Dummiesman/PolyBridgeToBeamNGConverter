using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PolyBridgeBeamNGExporter
{
    class Mesh3D
    {
        public string Name;
        private List<int> triangleIndexList = new List<int>();
        private List<Vector3> vertexList = new List<Vector3>();
        private List<Vector3> normalList = new List<Vector3>();
        private List<string> materialNameList = new List<string>();

        public void CalculateNormals()
        {
            normalList.Clear();
            for (int i = 0; i < vertexList.Count; i++)
                normalList.Add(Vector3.Zero);

            for (int i = 0; i < triangleIndexList.Count; i += 3)
            {
                int i0 = triangleIndexList[i];
                int i1 = triangleIndexList[i + 1];
                int i2 = triangleIndexList[i + 2];
                Vector3 p1 = vertexList[i0];
                Vector3 p2 = vertexList[i1];
                Vector3 p3 = vertexList[i2];

                Vector3 U = (p2 - p1);
                Vector3 V = (p3 - p1);

                Vector3 normal = Vector3.Zero;
                normal.X = (U.Y * V.Z) - (U.Z * V.Y);
                normal.Y = (U.Z * V.X) - (U.X * V.Z);
                normal.Z = (U.X * V.Y) - (U.Y * V.X);
                normal = Vector3.Normalize(normal);

                normalList[i0] += normal;
                normalList[i1] += normal;
                normalList[i2] += normal;
            }

            
            for (int i = 0; i < normalList.Count; i++)
                normalList[i] = Vector3.Normalize(normalList[i]);
        }
    }
}
