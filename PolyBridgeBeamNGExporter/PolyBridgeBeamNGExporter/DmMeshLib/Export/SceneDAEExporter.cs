using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PolyBridgeBeamNGExporter.DmMeshLib.Export
{
    public class SceneDAEExporter
    {
        //publics
        public Vector3 Scale = Vector3.One;
        public string SourcePath = string.Empty;

        //privates
        private Scene scene;
        private bool useAbsolutePaths = false;

        //mess of DAE related functions :)
        private static string Vec2ListToFloatList(List<Vector2> list)
        {
            StringBuilder floatList = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];
                floatList.Append($"{value.X} ");
                floatList.Append($"{value.Y}");
                if (i != list.Count - 1)
                    floatList.Append(" ");
            }
            return floatList.ToString();
        }

        private static string Vec3ListToFloatList(List<Vector3> list)
        {
            StringBuilder floatList = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];
                floatList.Append($"{value.X} ");
                floatList.Append($"{value.Y} ");
                floatList.Append($"{value.Z}");
                if (i != list.Count - 1)
                    floatList.Append(" ");
            }
            return floatList.ToString();
        }

        private static void AddDAESource(string id, List<Vector2> data, StringBuilder writer, string accessor0 = "X", string accessor1 = "Y")
        {
            writer.AppendLine($"\t\t\t\t<source id=\"{id}\">");
            writer.AppendLine($"\t\t\t\t\t<float_array id=\"{id}-array\" count=\"{data.Count * 2}\">{Vec2ListToFloatList(data)}</float_array>");
            writer.AppendLine($"\t\t\t\t\t<technique_common>");
            writer.AppendLine($"\t\t\t\t\t\t<accessor source=\"#{id}-array\" count=\"{data.Count}\" stride=\"2\">");
            writer.AppendLine($"\t\t\t\t\t\t\t<param name=\"{accessor0}\" type=\"float\"/>");
            writer.AppendLine($"\t\t\t\t\t\t\t<param name=\"{accessor1}\" type=\"float\"/>");
            writer.AppendLine($"\t\t\t\t\t\t</accessor>");
            writer.AppendLine($"\t\t\t\t\t</technique_common>");
            writer.AppendLine($"\t\t\t\t</source>");
        }

        private static void AddDAESource(string id, List<Vector3> data, StringBuilder writer, string accessor0 = "X", string accessor1 = "Y", string accessor2 = "Z")
        {
            writer.AppendLine($"\t\t\t\t<source id=\"{id}\">");
            writer.AppendLine($"\t\t\t\t\t<float_array id=\"{id}-array\" count=\"{data.Count * 3}\">{Vec3ListToFloatList(data)}</float_array>");
            writer.AppendLine($"\t\t\t\t\t<technique_common>");
            writer.AppendLine($"\t\t\t\t\t\t<accessor source=\"#{id}-array\" count=\"{data.Count}\" stride=\"3\">");
            writer.AppendLine($"\t\t\t\t\t\t\t<param name=\"{accessor0}\" type=\"float\"/>");
            writer.AppendLine($"\t\t\t\t\t\t\t<param name=\"{accessor1}\" type=\"float\"/>");
            writer.AppendLine($"\t\t\t\t\t\t\t<param name=\"{accessor2}\" type=\"float\"/>");
            writer.AppendLine($"\t\t\t\t\t\t</accessor>");
            writer.AppendLine($"\t\t\t\t\t</technique_common>");
            writer.AppendLine($"\t\t\t\t</source>");
        }

        private static string CreateTriangleArray(List<FaceLoop> loops)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < loops.Count / 3; i++)
            {
                void appendIndices(FaceLoop loop)
                {
                    builder.Append($"{loop.VertexIndex}");
                    if (loop.NormalIndex >= 0)
                        builder.Append($" {loop.NormalIndex}");
                    for(int uvIndex = 0; uvIndex < loop.UvIndices.Count; i++)
                    {
                        builder.Append($" {loop.UvIndices[uvIndex]}");
                    }
                }

                appendIndices(loops[i*3]);
                builder.Append(" ");

                appendIndices(loops[(i*3)+1]);
                builder.Append(" ");

                appendIndices(loops[(i*3)+2]);
                if (i < (loops.Count / 3) - 1)
                    builder.Append(" ");
            }
            return builder.ToString();
        }

        private static string CleanNameOrId(string nameOrId)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(nameOrId.Replace(" ", "_").Replace("\t", "_"), "_");
        }

        private void WriteMaterialPrameter(string sid, StringBuilder writer, string texture, Vector4 color)
        {
            writer.AppendLine($"\t\t\t\t\t\t<{sid}>");
            if (!string.IsNullOrEmpty(texture))
            {
                writer.AppendLine($"\t\t\t\t\t\t\t<texture texture=\"{CleanNameOrId(texture)}-sampler\" texcoord=\"UVMap\"/>");
            }
            else
            {
                writer.AppendLine($"\t\t\t\t\t\t\t<color sid=\"{sid}\">{color.X} {color.Y} {color.Z} {color.W}</color>");
            }
            writer.AppendLine($"\t\t\t\t\t\t</{sid}>");
        }

        private void WriteTextureSurfaceData(StringBuilder writer, string textureName)
        {
            writer.AppendLine($"\t\t\t\t<newparam sid=\"{CleanNameOrId(textureName)}-surface\">");
            writer.AppendLine($"\t\t\t\t\t<surface type=\"2D\">");
            writer.AppendLine($"\t\t\t\t\t\t<init_from>{CleanNameOrId(textureName)}</init_from>");
            writer.AppendLine($"\t\t\t\t\t</surface>");
            writer.AppendLine($"\t\t\t\t</newparam>");
            writer.AppendLine($"\t\t\t\t<newparam sid=\"{CleanNameOrId(textureName)}-sampler\">");
            writer.AppendLine($"\t\t\t\t\t<sampler2D>");
            writer.AppendLine($"\t\t\t\t\t\t<source>{CleanNameOrId(textureName)}-surface</source>");
            writer.AppendLine($"\t\t\t\t\t</sampler2D>");
            writer.AppendLine($"\t\t\t\t</newparam>");
        }

        private List<string> GatherAllTextures()
        {
            List<string> textureList = new List<string>();
            foreach (var material in scene.Materials)
                textureList.AddRange(material.GetTextures());
            return textureList;
        }

        public string ExportToString()
        {
            var writer = new StringBuilder();
            writer.AppendLine(Properties.Resources.ColladaHeader);

            //write materials
            Console.WriteLine("Writing library_effects...");
            writer.AppendLine("\t<library_effects>");
            foreach (var material in scene.Materials)
            {
                writer.AppendLine($"\t\t<effect id=\"{CleanNameOrId(material.Name)}-effect\">");
                writer.AppendLine($"\t\t\t<profile_COMMON>");

                if (!string.IsNullOrEmpty(material.DiffuseTexture))
                    WriteTextureSurfaceData(writer, material.DiffuseTexture);
                if (!string.IsNullOrEmpty(material.ReflectionTexture))
                    WriteTextureSurfaceData(writer, material.ReflectionTexture);
                if (!string.IsNullOrEmpty(material.EmissiveTexture))
                    WriteTextureSurfaceData(writer, material.EmissiveTexture);

                writer.AppendLine($"\t\t\t\t<technique sid=\"common\">");
                writer.AppendLine($"\t\t\t\t\t<lambert>");

                WriteMaterialPrameter("diffuse", writer, material.DiffuseTexture, material.DiffuseColor);
                WriteMaterialPrameter("reflective", writer, material.ReflectionTexture, default(Vector4));
                WriteMaterialPrameter("emission", writer, material.EmissiveTexture, material.EmissiveColor);


                writer.AppendLine($"\t\t\t\t\t\t<index_of_refraction>");
                writer.AppendLine($"\t\t\t\t\t\t\t<float sid=\"ior\">1.45</float>");
                writer.AppendLine($"\t\t\t\t\t\t</index_of_refraction>");
                writer.AppendLine($"\t\t\t\t\t</lambert>");
                writer.AppendLine($"\t\t\t\t</technique>");
                writer.AppendLine($"\t\t\t</profile_COMMON>");
                writer.AppendLine($"\t\t</effect>");
            }
            writer.AppendLine("\t</library_effects>");

            Console.WriteLine("Writing library_materials...");
            writer.AppendLine("\t<library_materials>");
            foreach (var material in scene.Materials)
            {
                writer.AppendLine($"\t\t<material id=\"{CleanNameOrId(material.Name)}-material\" name=\"{material.Name}\">");
                writer.AppendLine($"\t\t\t<instance_effect url=\"#{CleanNameOrId(material.Name)}-effect\"/>");
                writer.AppendLine($"\t\t</material>");
            }
            writer.AppendLine("\t</library_materials>");

            //write textures
            var allTextures = GatherAllTextures();
            if (allTextures.Count > 0)
            {
                Console.WriteLine("Writing library_images...");
                writer.AppendLine("\t<library_images>");
                foreach (var texture in allTextures)
                {
                    string textureId = CleanNameOrId(texture);
                    writer.AppendLine($"\t\t<image id=\"{textureId}\" name=\"{textureId}\">");
                    if (useAbsolutePaths)
                    {
                        string fullTexturePath = System.IO.Path.Combine(SourcePath, texture);
                        string colladaAbsolutePath = "file:///" + fullTexturePath.Replace(System.IO.Path.DirectorySeparatorChar, '/').Replace(" ", "%20");
                        writer.AppendLine($"\t\t\t<init_from>{colladaAbsolutePath}</init_from>");
                    }
                    else
                    {
                        writer.AppendLine($"\t\t\t<init_from>{texture}</init_from>");
                    }
                    writer.AppendLine($"\t\t</image>");
                }
                writer.AppendLine("\t</library_images>");
            }

            //write geometry
            Console.WriteLine("Processing meshes...");
            writer.AppendLine("\t<library_geometries>");
            for (int mc = 0; mc < scene.Meshes.Count; mc++)
            {
                Console.Write($"Mesh {mc}... ");
                var mesh = scene.Meshes[mc];
                string meshId = CleanNameOrId(mesh.Name);

                writer.AppendLine($"\t\t<geometry id=\"{meshId}-mesh\" name=\"{mesh.Name}\">");
                writer.AppendLine($"\t\t\t<mesh>");

                Console.Write("sources ");
                AddDAESource($"{meshId}-mesh-positions", mesh.Vertices, writer);
                if (mesh.Normals.Count > 0)
                    AddDAESource($"{meshId}-mesh-normals", mesh.Normals, writer);

                for(int i=0; i < mesh.UVChannels.Count; i++)
                {
                    AddDAESource($"{meshId}-mesh-map-{i}", mesh.UVChannels[i], writer, "S", "T");
                }

                writer.AppendLine($"\t\t\t\t<vertices id=\"{meshId}-mesh-vertices\">");
                writer.AppendLine($"\t\t\t\t\t<input semantic=\"POSITION\" source=\"#{meshId}-mesh-positions\"/>");
                writer.AppendLine($"\t\t\t\t</vertices>");

                Console.Write("indices ");
                foreach (var submesh in mesh.Submeshes)
                {
                    int mtlIndex = submesh.MaterialIndex;
                    string mtlName = (mtlIndex >= 0 && mtlIndex < scene.Materials.Count) ? scene.Materials[mtlIndex].Name : null;

                    var indices = submesh.FaceLoops;
                    int count = indices.Count / 3;

                    if (mtlName != null)
                        writer.AppendLine($"\t\t\t\t<triangles material=\"{CleanNameOrId(mtlName)}-material\" count=\"{count}\">");
                    else
                        writer.AppendLine($"\t\t\t\t<triangles count=\"{count}\">");

                    int offset = 0;
                    writer.AppendLine($"\t\t\t\t\t<input semantic=\"VERTEX\" source=\"#{meshId}-mesh-vertices\" offset=\"{offset++}\"/>");

                    if (mesh.Normals.Count > 0)
                        writer.AppendLine($"\t\t\t\t\t<input semantic=\"NORMAL\" source=\"#{meshId}-mesh-normals\" offset=\"{offset++}\"/>");
                    for(int i=0; i < mesh.UVChannels.Count; i++)
                    {
                        writer.AppendLine($"\t\t\t\t\t<input semantic=\"TEXCOORD\" source=\"#{meshId}-mesh-map-{i}\" offset=\"{offset++}\" set=\"0\"/>");
                    }

                    writer.AppendLine($"\t\t\t\t\t<p>{CreateTriangleArray(indices)}</p>");
                    writer.AppendLine($"\t\t\t\t</triangles>");
                }

                writer.AppendLine($"\t\t\t</mesh>");
                writer.AppendLine($"\t\t</geometry>");
                Console.WriteLine();
            }
            writer.AppendLine("\t</library_geometries>");

            Console.WriteLine("Writing scene...");
            writer.AppendLine("\t<library_visual_scenes>");
            writer.AppendLine("\t\t<visual_scene id=\"Scene\" name=\"Scene\">");
            for (int mc = 0; mc < scene.Meshes.Count; mc++)
            {
                var mesh = scene.Meshes[mc];
                string objectName = mesh.Name;

                writer.AppendLine($"\t\t\t<node id=\"{objectName}\" name=\"{objectName}\" type=\"NODE\">");
                writer.AppendLine($"\t\t\t\t<matrix sid=\"transform\">{Scale.X} 0 0 {mesh.Position.X * Scale.X} 0 {Scale.Y} 0 {mesh.Position.Y * Scale.Y} 0 0 {Scale.Z} {mesh.Position.Z * Scale.Z} 0 0 0 1</matrix>");
                writer.AppendLine($"\t\t\t\t<instance_geometry url=\"#{CleanNameOrId(objectName)}-mesh\" name=\"{objectName}\">");
                writer.AppendLine("\t\t\t\t\t<bind_material>");
                writer.AppendLine("\t\t\t\t\t\t<technique_common>");

                foreach (var submesh in mesh.Submeshes)
                {
                    int mtlIndex = submesh.MaterialIndex;
                    string mtlName = (mtlIndex >= 0 && mtlIndex < scene.Materials.Count) ? scene.Materials[mtlIndex].Name : null;

                    if (mtlName != null)
                    {
                        writer.AppendLine($"\t\t\t\t\t\t\t<instance_material symbol=\"{CleanNameOrId(mtlName)}-material\" target=\"#{CleanNameOrId(mtlName)}-material\">");
                        for(int i=0; i < mesh.UVChannels.Count; i++)
                        {
                            writer.AppendLine($"\t\t\t\t\t\t\t\t<bind_vertex_input semantic=\"UVMap{i+1}\" input_semantic=\"TEXCOORD\" input_set=\"{i}\"/>");
                        }
                        writer.AppendLine($"\t\t\t\t\t\t\t</instance_material>");
                    }
                }

                writer.AppendLine("\t\t\t\t\t\t</technique_common>");
                writer.AppendLine("\t\t\t\t\t</bind_material>");
                writer.AppendLine("\t\t\t\t</instance_geometry>");
                writer.AppendLine("\t\t\t</node>");
            }
            writer.AppendLine("\t\t</visual_scene>");
            writer.AppendLine("\t</library_visual_scenes>");

            writer.AppendLine("\t<scene>");
            writer.AppendLine("\t\t<instance_visual_scene url=\"#Scene\"/>");
            writer.AppendLine("\t</scene>");

            writer.AppendLine("</COLLADA>");

            //and finally
            return writer.ToString();
        }

        public void ExportToFile(string fileName)
        {
            File.WriteAllText(fileName, ExportToString());
        }

        public SceneDAEExporter(Scene scene)
        {
            this.scene = scene;
        }
    }
}
