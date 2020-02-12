using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PolyBridgeBeamNGExporter.DmMeshLib
{
    public class Material
    {
        public string Name = "default";
        public Vector4 DiffuseColor = Vector4.One; //TODO: Proper color class
        public Vector4 EmissiveColor = Vector4.Zero;

        public string DiffuseTexture = null;
        public string ReflectionTexture = null;
        public string EmissiveTexture = null;

        public IEnumerable<string> GetTextures()
        {
            if (!string.IsNullOrEmpty(DiffuseTexture))
                yield return DiffuseTexture;
            if (!string.IsNullOrEmpty(ReflectionTexture))
                yield return ReflectionTexture;
            if (!string.IsNullOrEmpty(EmissiveTexture))
                yield return EmissiveTexture;
        }
    }
}
