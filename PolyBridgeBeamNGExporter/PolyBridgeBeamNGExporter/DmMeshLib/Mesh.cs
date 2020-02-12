using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PolyBridgeBeamNGExporter.DmMeshLib
{
    public class Submesh
    {
        public int MaterialIndex;
        public List<FaceLoop> FaceLoops = new List<FaceLoop>();


        public void AddLoop(int vertexIndex)
        {
            FaceLoops.Add(new FaceLoop() { VertexIndex = vertexIndex });
        }

        public void AddLoop(int vertexIndex, int normalsIndex)
        {
            FaceLoops.Add(new FaceLoop() { VertexIndex = vertexIndex, NormalIndex = normalsIndex });
        }

        public void AddLoop(int vertexIndex, int normalsIndex, int uvIndex)
        {
            var loop = new FaceLoop() { VertexIndex = vertexIndex, NormalIndex = normalsIndex };
            loop.UvIndices.Add(uvIndex);
            FaceLoops.Add(loop);
        }
    }

    public class FaceLoop
    {
        public int VertexIndex = -1;
        public int NormalIndex = -1;
        public List<int> UvIndices = new List<int>();

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + VertexIndex.GetHashCode();
                hash = hash * 31 + NormalIndex.GetHashCode();
                foreach(var uvIndex in UvIndices)
                    hash = hash * 31 + uvIndex.GetHashCode();
                return hash;
            }
        }
        public override bool Equals(object obj)
        {
            if(obj is FaceLoop other)
            {
                return other.VertexIndex == this.VertexIndex && other.NormalIndex == this.NormalIndex && other.UvIndices.SequenceEqual(UvIndices);
            }
            return false;
        }
    }

    public class Mesh
    {
        public string Name = "default";
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<List<Vector2>> UVChannels = new List<List<Vector2>>();
        public List<Submesh> Submeshes = new List<Submesh>();
        public Vector3 Position = Vector3.Zero;

        public void CalculateNormals()
        {
            Normals.Clear();
            for (int i = 0; i < Vertices.Count; i++)
                Normals.Add(Vector3.Zero);

            foreach (var submesh in Submeshes)
            {
                var loops = submesh.FaceLoops;
                for (int i = 0; i < loops.Count; i += 3)
                {
                    FaceLoop loop0 = loops[i];
                    FaceLoop loop1 = loops[i+1];
                    FaceLoop loop2 = loops[i+2];
                    
                    //normal calculation
                    Vector3 p1 = Vertices[loop0.VertexIndex];
                    Vector3 p2 = Vertices[loop1.VertexIndex];
                    Vector3 p3 = Vertices[loop2.VertexIndex];

                    Vector3 U = (p2 - p1);
                    Vector3 V = (p3 - p1);

                    Vector3 normal = Vector3.Zero;
                    normal.X = (U.Y * V.Z) - (U.Z * V.Y);
                    normal.Y = (U.Z * V.X) - (U.X * V.Z);
                    normal.Z = (U.X * V.Y) - (U.Y * V.X);
                    normal = Vector3.Normalize(normal);

                    Normals[loop0.VertexIndex] += normal;
                    Normals[loop1.VertexIndex] += normal;
                    Normals[loop2.VertexIndex] += normal;

                    //set loop normal
                    loop0.NormalIndex = loop0.VertexIndex;
                    loop1.NormalIndex = loop1.VertexIndex;
                    loop2.NormalIndex = loop2.VertexIndex;
                }
            }

            for (int i = 0; i < Normals.Count; i++)
                Normals[i] = Vector3.Normalize(Normals[i]);

        }
        public Submesh GetOrCreateSubmesh(string materialName, Scene scene)
        {
            int mtlIndex = scene.Materials.FindIndex(x => x.Name == materialName);
            if (mtlIndex < 0)
                throw new Exception($"Can't find material {materialName}");
            return GetOrCreateSubmesh(mtlIndex);
        }

        public Submesh GetOrCreateSubmesh(int materialIndex)
        {
            Submesh submesh = GetSubmesh(materialIndex);
            if (submesh != null)
                return submesh;
            Submeshes.Add(new Submesh() { MaterialIndex = materialIndex });
            return Submeshes[Submeshes.Count - 1];
        }

        public Submesh GetSubmesh(int materialIndex)
        {
            return Submeshes.FirstOrDefault(x => x.MaterialIndex == materialIndex);
        }

        public Submesh GetSubmesh(string materialName, Scene scene)
        {
            int mtlIndex = scene.Materials.FindIndex(x => x.Name == materialName);
            if (mtlIndex < 0)
                return null;
            return GetSubmesh(mtlIndex);
        }
    }
}
