using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PolyBridgeBeamNGExporter.DmMeshLib.PolyBridgeExtensions
{
    public static class PolyBridgeMeshExtensions
    {
        public static void GenerateCubeGeometryFromLine(this Mesh mesh, Scene scene, Vector3 lineStart, Vector3 lineEnd, float size = 1f, string materialName = "pb_wood")
        {
            if (lineStart.X != lineEnd.X)
                throw new ArgumentException("Vectors do not represent a 2D line!");

            //
            //float cubeLength = (lineEnd - lineStart).Length();
            Vector3 forwardDir = Vector3.Normalize(lineEnd - lineStart);
            Vector3 rightDir = Vector3.UnitX;
            Vector3 upDir = -Vector3.Cross(forwardDir, rightDir);

            Vector3 rightDirH = rightDir / 2f;
            Vector3 upDirH = upDir / 2f;

            //generate cube points
            var cubeS_BL = lineStart + (-rightDirH * size) + (-upDirH * size);
            var cubeS_BR = lineStart + (rightDirH * size) + (-upDirH * size);
            var cubeS_TL = lineStart + (-rightDirH * size) + (upDirH * size);
            var cubeS_TR = lineStart + (rightDirH * size) + (upDirH * size);

            var cubeE_BL = lineEnd + (-rightDirH * size) + (-upDirH * size);
            var cubeE_BR = lineEnd + (rightDirH * size) + (-upDirH * size);
            var cubeE_TL = lineEnd + (-rightDirH * size) + (upDirH * size);
            var cubeE_TR = lineEnd + (rightDirH * size) + (upDirH * size);

            int vertexBase = mesh.Vertices.Count;
            mesh.Vertices.Add(cubeS_BL);
            mesh.Vertices.Add(cubeS_BR);
            mesh.Vertices.Add(cubeS_TR);
            mesh.Vertices.Add(cubeS_TL);
            mesh.Vertices.Add(cubeE_BL);
            mesh.Vertices.Add(cubeE_BR);
            mesh.Vertices.Add(cubeE_TR);
            mesh.Vertices.Add(cubeE_TL);

            //find applicable submeshes
            Submesh submesh = mesh.GetOrCreateSubmesh(materialName, scene);

            //left
            submesh.AddLoop(vertexBase);
            submesh.AddLoop(vertexBase + 3);
            submesh.AddLoop(vertexBase + 7);
            submesh.AddLoop(vertexBase + 4);
            submesh.AddLoop(vertexBase);
            submesh.AddLoop(vertexBase + 7);

            //top
            submesh.AddLoop(vertexBase + 3);
            submesh.AddLoop(vertexBase + 2);
            submesh.AddLoop(vertexBase + 6);           
            submesh.AddLoop(vertexBase + 7);
            submesh.AddLoop(vertexBase + 3);
            submesh.AddLoop(vertexBase + 6);

            //bottom
            submesh.AddLoop(vertexBase + 0);
            submesh.AddLoop(vertexBase + 4);
            submesh.AddLoop(vertexBase + 5);
            submesh.AddLoop(vertexBase + 5);
            submesh.AddLoop(vertexBase + 1);
            submesh.AddLoop(vertexBase + 0);

            //right
            submesh.AddLoop(vertexBase + 1);
            submesh.AddLoop(vertexBase + 5);
            submesh.AddLoop(vertexBase + 6);
            submesh.AddLoop(vertexBase + 1);
            submesh.AddLoop(vertexBase + 6);
            submesh.AddLoop(vertexBase + 2);
        }

        public static void GenerateRoadGeometryFromLine(this Mesh mesh, Scene scene, Vector3 lineStart, Vector3 lineEnd, float width = 1f, float height = 1f, string materialName = "pb_road", string edgeMaterialName = "pb_roadedge")
        {
            if (lineStart.X != lineEnd.X)
                throw new ArgumentException("Vectors do not represent a 2D line!");

            //
            //float cubeLength = (lineEnd - lineStart).Length();
            Vector3 forwardDir = Vector3.Normalize(lineEnd - lineStart);
            Vector3 rightDir = Vector3.UnitX;
            Vector3 upDir = -Vector3.Cross(forwardDir, rightDir);

            Vector3 rightDirH = rightDir / 2f;
            Vector3 upDirH = upDir / 2f;

            //generate cube points
            var cubeS_BLL = lineStart + (-rightDirH * width) + (-upDirH * height);
            var cubeS_BRR = lineStart + (rightDirH * width) + (-upDirH * height);
            var cubeS_TLL = lineStart + (-rightDirH * width) + (upDirH * height);
            var cubeS_TRR = lineStart + (rightDirH * width) + (upDirH * height);
            var cubeS_BL = Vector3.Lerp(cubeS_BLL, cubeS_BRR, 0.1f);
            var cubeS_BR = Vector3.Lerp(cubeS_BRR, cubeS_BLL, 0.1f);
            var cubeS_TL = Vector3.Lerp(cubeS_TLL, cubeS_TRR, 0.1f);
            var cubeS_TR = Vector3.Lerp(cubeS_TRR, cubeS_TLL, 0.1f);

            var cubeE_BLL = lineEnd + (-rightDirH * width) + (-upDirH * height);
            var cubeE_BRR = lineEnd + (rightDirH * width) + (-upDirH * height);
            var cubeE_TLL = lineEnd + (-rightDirH * width) + (upDirH * height);
            var cubeE_TRR = lineEnd + (rightDirH * width) + (upDirH * height);
            var cubeE_BL = Vector3.Lerp(cubeE_BLL, cubeE_BRR, 0.1f);
            var cubeE_BR = Vector3.Lerp(cubeE_BRR, cubeE_BLL, 0.1f);
            var cubeE_TL = Vector3.Lerp(cubeE_TLL, cubeE_TRR, 0.1f);
            var cubeE_TR = Vector3.Lerp(cubeE_TRR, cubeE_TLL, 0.1f);

            int vertexBase = mesh.Vertices.Count;
            mesh.Vertices.Add(cubeS_BLL);
            mesh.Vertices.Add(cubeS_BL);
            mesh.Vertices.Add(cubeS_BR);
            mesh.Vertices.Add(cubeS_BRR);
            mesh.Vertices.Add(cubeS_TRR);
            mesh.Vertices.Add(cubeS_TR);
            mesh.Vertices.Add(cubeS_TL);
            mesh.Vertices.Add(cubeS_TLL);

            mesh.Vertices.Add(cubeE_BLL);
            mesh.Vertices.Add(cubeE_BL);
            mesh.Vertices.Add(cubeE_BR);
            mesh.Vertices.Add(cubeE_BRR);
            mesh.Vertices.Add(cubeE_TRR);
            mesh.Vertices.Add(cubeE_TR);
            mesh.Vertices.Add(cubeE_TL);
            mesh.Vertices.Add(cubeE_TLL);

            //find applicable submeshes
            Submesh edgeSubmesh = mesh.GetOrCreateSubmesh(edgeMaterialName, scene);
            Submesh roadSubmesh = mesh.GetOrCreateSubmesh(materialName, scene);

            //left
            edgeSubmesh.AddLoop(vertexBase);
            edgeSubmesh.AddLoop(vertexBase + 7);
            edgeSubmesh.AddLoop(vertexBase + 15);
            edgeSubmesh.AddLoop(vertexBase + 8);
            edgeSubmesh.AddLoop(vertexBase);
            edgeSubmesh.AddLoop(vertexBase + 15);

            //top 0
            edgeSubmesh.AddLoop(vertexBase + 7);
            edgeSubmesh.AddLoop(vertexBase + 6);
            edgeSubmesh.AddLoop(vertexBase + 14);
            edgeSubmesh.AddLoop(vertexBase + 15);
            edgeSubmesh.AddLoop(vertexBase + 7);
            edgeSubmesh.AddLoop(vertexBase + 14);

            //top 1
            roadSubmesh.AddLoop(vertexBase + 6);
            roadSubmesh.AddLoop(vertexBase + 5);
            roadSubmesh.AddLoop(vertexBase + 13);
            roadSubmesh.AddLoop(vertexBase + 6);
            roadSubmesh.AddLoop(vertexBase + 13);
            roadSubmesh.AddLoop(vertexBase + 14);

            //top 2
            edgeSubmesh.AddLoop(vertexBase + 5);
            edgeSubmesh.AddLoop(vertexBase + 4);
            edgeSubmesh.AddLoop(vertexBase + 12);
            edgeSubmesh.AddLoop(vertexBase + 13);
            edgeSubmesh.AddLoop(vertexBase + 5);
            edgeSubmesh.AddLoop(vertexBase + 12);

            //right
            edgeSubmesh.AddLoop(vertexBase + 3);
            edgeSubmesh.AddLoop(vertexBase + 11);
            edgeSubmesh.AddLoop(vertexBase + 12);
            edgeSubmesh.AddLoop(vertexBase + 3);
            edgeSubmesh.AddLoop(vertexBase + 12);
            edgeSubmesh.AddLoop(vertexBase + 4);

            //bottom 0
            edgeSubmesh.AddLoop(vertexBase + 0);
            edgeSubmesh.AddLoop(vertexBase + 8);
            edgeSubmesh.AddLoop(vertexBase + 9);
            edgeSubmesh.AddLoop(vertexBase + 9);
            edgeSubmesh.AddLoop(vertexBase + 1);
            edgeSubmesh.AddLoop(vertexBase + 0);

            //bottom 1
            roadSubmesh.AddLoop(vertexBase + 1);
            roadSubmesh.AddLoop(vertexBase + 9);
            roadSubmesh.AddLoop(vertexBase + 10);
            roadSubmesh.AddLoop(vertexBase + 10);
            roadSubmesh.AddLoop(vertexBase + 2);
            roadSubmesh.AddLoop(vertexBase + 1);

            //bottom 2
            edgeSubmesh.AddLoop(vertexBase + 2);
            edgeSubmesh.AddLoop(vertexBase + 10);
            edgeSubmesh.AddLoop(vertexBase + 11);
            edgeSubmesh.AddLoop(vertexBase + 11);
            edgeSubmesh.AddLoop(vertexBase + 3);
            edgeSubmesh.AddLoop(vertexBase + 2);
        }
    }
}
