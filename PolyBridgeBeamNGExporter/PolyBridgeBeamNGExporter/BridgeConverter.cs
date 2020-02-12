using PolyBridgeBeamNGExporter.DmMeshLib;
using PolyBridgeBeamNGExporter.DmMeshLib.PolyBridgeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PolyBridgeBeamNGExporter
{
    public class BridgeConverter
    {
        //publics
        public float BridgeWidth = 5f;

        //privates
        private PolyBridgeSave save;
        private string flexbodyName;
        private string bridgeName;

        public string CreateJbeam()
        {
            //tweakables
            float beamSpring = 2001000f;
            float bridgeWidthHalf = BridgeWidth / 2f;

            //create our builders
            var jbeamBuilder = new StringBuilder();
            var nodeBuilder = new StringBuilder();
            var beamBuilder = new StringBuilder();
            var quadsBuilder = new StringBuilder();

            jbeamBuilder.AppendLine("{");
            jbeamBuilder.AppendLine("\"bridge\": {");
            jbeamBuilder.AppendLine("\"information\": {");
            jbeamBuilder.AppendLine("\"authors\": \"Poly Bridge Player\",");
            jbeamBuilder.AppendLine($"\"name\": \"{bridgeName}\",");
            jbeamBuilder.AppendLine("\"value\": \"100\",");
            jbeamBuilder.AppendLine("}");
            jbeamBuilder.AppendLine("\"slotType\" : \"main\",");
            jbeamBuilder.AppendLine("\"flexbodies\":[");
            jbeamBuilder.AppendLine("[\"mesh\", \"[group]:\", \"nonFlexMaterials\"],");
            jbeamBuilder.AppendLine("{\"rot\":{\"x\":0, \"y\":0, \"z\":180}, \"pos\":{\"x\":0, \"y\":0, \"z\":0}},");
            jbeamBuilder.AppendLine($"[\"{flexbodyName}\", [\"pb_bridgemodel\"]],");
            jbeamBuilder.AppendLine("],");

            nodeBuilder.AppendLine("\"nodes\": [");
            nodeBuilder.AppendLine("[\"id\", \"posX\", \"posY\", \"posZ\"],");
            nodeBuilder.AppendLine("{\"selfCollision\":false}");
            nodeBuilder.AppendLine("{\"collision\":false}");
            nodeBuilder.AppendLine("{\"nodeWeight\":10.0}");

            beamBuilder.AppendLine("\"beams\": [");
            beamBuilder.AppendLine("[\"id1:\", \"id2:\"],");
            beamBuilder.AppendLine($"{{\"beamSpring\":{beamSpring},\"beamDamp\":300}},");
            beamBuilder.AppendLine("{\"beamDeform\":100000,\"beamStrength\":50000},");

            quadsBuilder.AppendLine("\"quads\":[");
            quadsBuilder.AppendLine("[\"id1:\", \"id2:\", \"id3:\", \"id4:\"],");
            quadsBuilder.AppendLine("{\"groundModel\":\"asphalt\"},");
            quadsBuilder.AppendLine("{\"dragCoef\":100},");
            quadsBuilder.AppendLine("{\"liftCoef\":100},");

            //add ref nodes
            jbeamBuilder.AppendLine("\"refNodes\":[");
            jbeamBuilder.AppendLine("[\"ref:\", \"back:\", \"left:\", \"up:\"]");
            jbeamBuilder.AppendLine("[\"refN\", \"refY\", \"refX\", \"refZ\"],");
            jbeamBuilder.AppendLine("],");

            nodeBuilder.AppendLine("{\"fixed\":true}");
            nodeBuilder.AppendLine("[\"refN\", 0.0, 0.0, 0.0],");
            nodeBuilder.AppendLine("[\"refX\", 1.0, 0.0, 0.0],");
            nodeBuilder.AppendLine("[\"refY\", 0.0, 1.0, 0.0],");
            nodeBuilder.AppendLine("[\"refZ\", 0.0, 0.0, 1.0],");
            nodeBuilder.AppendLine("{\"fixed\":false}");
            nodeBuilder.AppendLine("{\"collision\":true}");
            nodeBuilder.AppendLine("{\"group\":\"pb_bridgemodel\"}");

            //process bridge data, nodes first
            foreach (var obj in save.Objects)
            {
                if (obj.anchorAID != 0 || obj.anchorBID != 0) //not a source node!
                    continue;

                float nxPosLeft = bridgeWidthHalf;
                float nxPosRight = -bridgeWidthHalf;
                float nyPos = obj.x;
                float nzPos = obj.y;

                string extraData = "{}";
                if (obj.isKinematic)
                {
                    extraData = "{\"fixed\":true}";
                }

                int id = obj.id;
                if (obj.SplitForDrawBridge)
                {
                    //create split nodes
                    for (int i = 0; i <= 1; i++)
                    {
                        float yPosOffset = (i == 0) ? -0.005f : 0.005f; //BeamNG breaks flexbodies with touching nodes, this is a workaround
                        nodeBuilder.AppendLine($"[\"{id}l_s{i}\", {nxPosLeft}, {nyPos + yPosOffset}, {nzPos}, {extraData}],");
                        nodeBuilder.AppendLine($"[\"{id}r_s{i}\", {nxPosRight}, {nyPos + yPosOffset}, {nzPos}, {extraData}],");
                    }
                }
                else
                {
                    nodeBuilder.AppendLine($"[\"{id}l\", {nxPosLeft}, {nyPos}, {nzPos}, {extraData}],");
                    nodeBuilder.AppendLine($"[\"{id}r\", {nxPosRight}, {nyPos}, {nzPos}, {extraData}],");
                }
            }

            //then beams
            HashSet<int> ltrBeamMap = new HashSet<int>();
            HashSet<long> crossBeamMap = new HashSet<long>();
            float lastBeamStrength = -1f;
            foreach (var obj in save.Objects)
            {
                int anchorAID = obj.anchorAID;
                int anchorBID = obj.anchorBID;
                if (anchorAID == 0 || anchorBID == 0) //this is node data
                    continue;
                if (obj.type == ObjectType.Piston) //this is a hydro
                    continue;

                long anchorHash = (long)obj.anchorAID << 32 | (long)(uint)obj.anchorBID;
                bool isCableType = (obj.type == ObjectType.Rope || obj.type == ObjectType.CableStrong);
                int id = obj.id;

                //split stuff
                string beamSuffixA = string.Empty;
                string beamSuffixB = string.Empty;
                if (obj.splitAID >= 0)
                    beamSuffixA = $"_s{obj.splitAID}";
                if (obj.splitBID >= 0)
                    beamSuffixB = $"_s{obj.splitBID}";

                //append beam info
                //beamBuilder.AppendLine($"{{\"breakGroup\":\"bm{id}\"}}");

                float beamStrength = obj.BeamStrength;
                if (beamStrength != lastBeamStrength)
                {
                    lastBeamStrength = beamStrength;
                    beamBuilder.AppendLine($"{{\"beamStrength\":{beamStrength}}}");
                }

                //2d plane
                beamBuilder.AppendLine($"[\"{anchorAID}l{beamSuffixA}\", \"{anchorBID}l{beamSuffixB}\"],");
                beamBuilder.AppendLine($"[\"{anchorAID}r{beamSuffixA}\", \"{anchorBID}r{beamSuffixB}\"],");

                //left to right
                if (!isCableType)
                {
                    string disableMeshBreakingTrue = "{\"disableMeshBreaking\":true}";
                    //string disableMeshBreakingFalse = "{\"disableMeshBreaking\":false},";
                    if (ltrBeamMap.Add(anchorAID))
                    {
                        beamBuilder.AppendLine($"[\"{anchorAID}l{beamSuffixA}\", \"{anchorAID}r{beamSuffixA}\", {disableMeshBreakingTrue}],");
                    }
                    if (ltrBeamMap.Add(anchorBID))
                    {
                        beamBuilder.AppendLine($"[\"{anchorBID}l{beamSuffixB}\", \"{anchorBID}r{beamSuffixB}\", {disableMeshBreakingTrue}],");
                    }
                    if (crossBeamMap.Add(anchorHash))
                    {
                        beamBuilder.AppendLine($"[\"{anchorBID}l{beamSuffixB}\", \"{anchorAID}r{beamSuffixA}\", {disableMeshBreakingTrue}],");
                        beamBuilder.AppendLine($"[\"{anchorAID}l{beamSuffixA}\", \"{anchorBID}r{beamSuffixB}\", {disableMeshBreakingTrue}],");
                    }
                }

                //add quad if road
                if (obj.type == ObjectType.Road)
                {
                    quadsBuilder.AppendLine($"[\"{anchorAID}r{beamSuffixA}\", \"{anchorAID}l{beamSuffixA}\", \"{anchorBID}l{beamSuffixB}\", \"{anchorBID}r{beamSuffixB}\"]");
                }
                beamBuilder.AppendLine($"{{\"breakGroup\":\"\"}}");
            }

            //finish off
            nodeBuilder.AppendLine("{\"group\":\"\"},"); //finalize node section
            nodeBuilder.AppendLine("],"); //close node section
            jbeamBuilder.AppendLine(nodeBuilder.ToString()); //add to jbeam
            beamBuilder.AppendLine("],"); //close beams section
            jbeamBuilder.AppendLine(beamBuilder.ToString()); //add to jbeam
            quadsBuilder.AppendLine("],"); //close quads section
            jbeamBuilder.AppendLine(quadsBuilder.ToString()); //add to jbeam
            jbeamBuilder.AppendLine("}"); //close main section
            jbeamBuilder.AppendLine("}"); //close json

            //and return
            return jbeamBuilder.ToString();
        }

        public Scene CreateModel()
        {
            float bridgeWidthHalf = BridgeWidth / 2f;

            var scene = new Scene();
            var bridgeMesh = new Mesh() { Name = flexbodyName };
            scene.Meshes.Add(bridgeMesh);

            scene.Materials.Add(new Material() { Name = "pb_wood", DiffuseColor = Vector4.One });
            scene.Materials.Add(new Material() { Name = "pb_cable", DiffuseColor = Vector4.One });
            scene.Materials.Add(new Material() { Name = "pb_rope", DiffuseColor = Vector4.One });
            scene.Materials.Add(new Material() { Name = "pb_hydro", DiffuseColor = Vector4.One });
            scene.Materials.Add(new Material() { Name = "pb_steel", DiffuseColor = Vector4.One });
            scene.Materials.Add(new Material() { Name = "pb_road", DiffuseColor = Vector4.One });
            scene.Materials.Add(new Material() { Name = "pb_roadedge", DiffuseColor = Vector4.One });

            foreach (var obj in save.Objects)
            {
                int anchorAID = obj.anchorAID;
                int anchorBID = obj.anchorBID;
                if (anchorAID == 0 || anchorBID == 0) //this is node data
                    continue;

                var anchorAObject = save.Objects.FirstOrDefault(x => x.id == anchorAID);
                var anchorBObject = save.Objects.FirstOrDefault(x => x.id == anchorBID);

                if (obj.type != ObjectType.Road)
                {
                    string materialName = "pb_wood";
                    float thickness = 0.100f;
                    if (obj.type == ObjectType.CableStrong)
                        materialName = "pb_cable";
                    else if (obj.type == ObjectType.Rope)
                        materialName = "pb_rope";
                    else if (obj.type == ObjectType.Piston)
                        materialName = "pb_hydro";
                    else if (obj.type == ObjectType.SegmentStrong)
                    {
                        materialName = "pb_steel";
                        thickness = 0.125f;
                    }

                    bridgeMesh.GenerateCubeGeometryFromLine(scene, new Vector3(bridgeWidthHalf, anchorAObject.y, -anchorAObject.x),
                                                                   new Vector3(bridgeWidthHalf, anchorBObject.y, -anchorBObject.x), thickness, materialName);
                    bridgeMesh.GenerateCubeGeometryFromLine(scene, new Vector3(-bridgeWidthHalf, anchorAObject.y, -anchorAObject.x),
                                                                   new Vector3(-bridgeWidthHalf, anchorBObject.y, -anchorBObject.x), thickness, materialName);
                    
                }
                else
                {
                    bridgeMesh.GenerateRoadGeometryFromLine(scene, new Vector3(0f, anchorAObject.y, -anchorAObject.x),
                                                                   new Vector3(0f, anchorBObject.y, -anchorBObject.x), BridgeWidth - 0.125f, 0.125f);
                }
            }

            bridgeMesh.CalculateNormals();
            return scene;
        }

        public BridgeConverter(PolyBridgeSave save, string flexbodyName, string bridgeName)
        {
            this.save = save;
            this.flexbodyName = flexbodyName;
            this.bridgeName = bridgeName;
        }
    }
}
