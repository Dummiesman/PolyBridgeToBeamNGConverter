using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyBridgeBeamNGExporter
{
    class DocumentsHelper
    {
        public static string DocumentsFolder => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string BeamNGFolder => Path.Combine(DocumentsFolder, "BeamNG.drive");
        public static string ModsFolder => Path.Combine(BeamNGFolder, "mods");
        public static string VehiclesFolder => Path.Combine(BeamNGFolder, "vehicles");

        public static string GetUnpackedModPath(string name)
        {
            return Path.Combine(VehiclesFolder, name);
        }

        public static string GetPackedModPath(string name)
        {
            return Path.Combine(ModsFolder, $"{name}.zip");
        }

        public static bool PackedModExists(string name)
        {
            return File.Exists(Path.Combine(ModsFolder, $"{name}.zip"));
        }

        public static bool UnpackedModExists(string name)
        {
            return Directory.Exists(Path.Combine(VehiclesFolder, name));
        }

        public static void EnsureBeamNGFoldersExist()
        {
            if (!Directory.Exists(BeamNGFolder))
                Directory.CreateDirectory(BeamNGFolder);
            if (!Directory.Exists(ModsFolder))
                Directory.CreateDirectory(ModsFolder);
            if (!Directory.Exists(VehiclesFolder))
                Directory.CreateDirectory(VehiclesFolder);
        }
    }
}
