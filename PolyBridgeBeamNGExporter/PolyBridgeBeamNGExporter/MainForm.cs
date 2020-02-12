using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolyBridgeBeamNGExporter.DmMeshLib;
using PolyBridgeBeamNGExporter.DmMeshLib.Export;
using PolyBridgeBeamNGExporter.DmMeshLib.PolyBridgeExtensions;
using SharpCompress.Compressors.Deflate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolyBridgeBeamNGExporter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //helpers
        private string GenerateInfoFile()
        {
            var writer = new StringBuilder();
            writer.AppendLine("{");
            writer.AppendLine($"\t\"Name\":\"{bridgeNameTextbox.Text}\",");
            writer.AppendLine($"\t\"Author\":\"Poly Bridge Player\",");
            writer.AppendLine($"\t\"Type\":\"Prop\"");
            writer.AppendLine("}");
            return writer.ToString();
        }
            
        private string GetModName()
        {
            string bridgeName = string.IsNullOrEmpty(bridgeNameTextbox.Text) ? "PolyBridgeBridge" : bridgeNameTextbox.Text;
            return string.Join("_", bridgeName.Split(Path.GetInvalidFileNameChars())).Replace(' ', '_');
        }

        private bool IsZlib(byte[] file)
        {
            if (file == null || file.Length < 2)
                return false;

            bool firstByteMatches = file[0] == 0x78;
            bool secondByteMatches = (file[1] == 0x01 || file[1] == 0x5E || file[1] == 0x9C || file[1] == 0xDA);
            return firstByteMatches && secondByteMatches;
        }

        public static bool TryGetFromBase64String(string input, out byte[] output)
        {
            output = null;
            try
            {
                output = Convert.FromBase64String(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private string DecompressByteArray(byte[] array)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (ZlibStream sr = new ZlibStream(new MemoryStream(array), SharpCompress.Compressors.CompressionMode.Decompress))
                {
                    sr.CopyTo(output);
                }

                string str = Encoding.UTF8.GetString(output.GetBuffer(), 0, (int)output.Length);
                return str;
            }
        }

        private string DecompressBase64String(string base64)
        {
            byte[] file = Convert.FromBase64String(base64);
            return DecompressByteArray(file);
        }

        private void SelectNewFile(string file)
        {
            //how?
            if (!File.Exists(file))
                return;

            //setup things
            bridgePathTextBox.Text = file;
            convertButton.Enabled = true;

            GetSaveMetadata(out string description);
            bridgeNameTextbox.Text = string.IsNullOrEmpty(description) ? "My Poly Bridge Bridge" : description;
        }

        private void GetSaveMetadata(out string description)
        {
            byte[] fileBytes = File.ReadAllBytes(bridgePathTextBox.Text);
            string fileText = System.Text.Encoding.UTF8.GetString(fileBytes);
            bool isPblFile = fileText.Contains("\"description\"");

            if (isPblFile)
            {
                var pblJson = JObject.Parse(fileText);
                description = pblJson.Value<string>("description") ?? string.Empty;
            }
            else
            {
                //try extracting from save
                if (TryGetFromBase64String(fileText, out byte[] compressedSave) && IsZlib(compressedSave))
                {
                    string saveData = DecompressByteArray(compressedSave);
                    var json = JsonConvert.DeserializeObject<PolyBridgeSave>(saveData);
                    if(string.IsNullOrEmpty(json.DisplayName) || string.IsNullOrWhiteSpace(json.DisplayName))
                    {
                        description = string.Empty;
                    }
                    else
                    {
                        description = json.DisplayName;
                    }
                }
                else
                {
                    description = string.Empty;
                }
            }

            //use folder name as a fallback
            if(string.IsNullOrEmpty(description) || string.IsNullOrWhiteSpace(description))
            {
                var fileInfo = new FileInfo(bridgePathTextBox.Text);
                description = fileInfo.Directory.Name;
            }
        }

        private string GetSaveJsonFromPath()
        {
            byte[] fileBytes = File.ReadAllBytes(bridgePathTextBox.Text);
            string fileText = System.Text.Encoding.UTF8.GetString(fileBytes);

            bool isSaveFile = TryGetFromBase64String(fileText, out byte[] saveFileBytes) && IsZlib(saveFileBytes);

            bool isDecompressedData = fileText.Contains("\"DisplayName\"");
            bool isPblFile = fileText.Contains("\"description\"");

            if (isSaveFile)
            {
                string json = DecompressByteArray(saveFileBytes);
                return json;
            }
            if (isDecompressedData)
            {
                return fileText;
            }
            else if (isPblFile)
            {
                //extract from PBL file
                var pblJson = JObject.Parse(fileText);
                var saveFileArray = pblJson.Value<JArray>("saveFiles");

                //unsupported
                if (saveFileArray.Count != 1)
                    return null;

                var saveFileBase64 = saveFileArray[0].Value<string>().Replace("\\", string.Empty);
                string json = DecompressBase64String(saveFileBase64);
                return json;
            }
            else
            {
                //Can't determine file type
                return null;
            }
        }

        private PolyBridgeSave GetSaveFromPath()
        {
            var json = GetSaveJsonFromPath();
            if(json != null)
                return JsonConvert.DeserializeObject<PolyBridgeSave>(json);
            return null;
        }

        //events
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            //place them in their appdata folder if it exists in locallow
            string pbSavesPath = Path.Combine(WinAPI.GetLocallowPath(), "Dry Cactus", "Poly Bridge");
            if (Directory.Exists(pbSavesPath))
                browseDialog.InitialDirectory = pbSavesPath;

            //open dialog
            browseDialog.ShowDialog();
        }

        private void DoExport()
        {
            var bridgeData = GetSaveFromPath();
            if (bridgeData == null)
            {
                MessageBox.Show("Could not load bridge file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //create converter
            var currentTime = DateTime.UtcNow;
            string flexbodyName = $"pbexport_{currentTime.ToBinary()}";
            var converter = new BridgeConverter(bridgeData, flexbodyName, bridgeNameTextbox.Text);

            //parse bridge width
            float.TryParse(bridgeWidthTextbox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out converter.BridgeWidth);


            //save
            DocumentsHelper.EnsureBeamNGFoldersExist();
            string modName = GetModName();
            SceneDAEExporter meshExporter = new SceneDAEExporter(converter.CreateModel());

            if (packedRadioButton.Checked)
            {
                string modPath = DocumentsHelper.GetPackedModPath(modName);

                //kill if already exists
                if (DocumentsHelper.PackedModExists(modName))
                {
                    if (MessageBox.Show($"A mod with the name {modName} already exists. Do you want to replace it?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        File.Delete(modPath);
                    }
                    else
                    {
                        return;
                    }
                }

                //create zip file
                var zf = new Ionic.Zip.ZipFile();

                //add preview
                using (var ms = new MemoryStream())
                {
                    Bitmap imageIn = PolyBridgeBeamNGExporter.Properties.Resources.pb_thumbnail;
                    imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    zf.AddEntry($"\\vehicles\\{modName}\\default.png", ms.ToArray());
                }

                //add other files
                zf.AddEntry($"\\vehicles\\{modName}\\polyBridgeExport.jbeam", converter.CreateJbeam());
                zf.AddEntry($"\\vehicles\\{modName}\\main.materials.json", Properties.Resources.MaterialsFile);
                zf.AddEntry($"\\vehicles\\{modName}\\model.dae", meshExporter.ExportToString());
                zf.AddEntry($"\\vehicles\\{modName}\\info.json", GenerateInfoFile());

                //save
                zf.Save(modPath);
                zf.Dispose();
            }
            else
            {
                string modPath = DocumentsHelper.GetUnpackedModPath(modName);

                //kill if already exists
                if (DocumentsHelper.UnpackedModExists(modName))
                {
                    if (MessageBox.Show($"A mod with the name {modName} already exists. Do you want to replace it?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Directory.Delete(modPath, true);
                    }
                    else
                    {
                        return;
                    }
                }

                //make a brand new folder
                Directory.CreateDirectory(modPath);

                //write files
                File.WriteAllText(Path.Combine(modPath, "info.json"), GenerateInfoFile());
                File.WriteAllText(Path.Combine(modPath, "polyBridgeExport.jbeam"), converter.CreateJbeam());
                File.WriteAllText(Path.Combine(modPath, "main.materials.json"), PolyBridgeBeamNGExporter.Properties.Resources.MaterialsFile);
                meshExporter.ExportToFile(Path.Combine(modPath, "model.dae"));
                PolyBridgeBeamNGExporter.Properties.Resources.pb_thumbnail.Save(Path.Combine(modPath, "default.png"), System.Drawing.Imaging.ImageFormat.Png);
            }

            MessageBox.Show("Export Complete!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            try
            {
                DoExport();
            }
            catch(System.IO.IOException ioex)
            {
                MessageBox.Show($"Conversion failed: {ioex.Message}\n\nMake sure the disk isn't full and the mod isn't currently loaded in BeamNG.drive.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Conversion failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            SelectNewFile(browseDialog.FileName);
        }

        private void ConvertButton_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 'd')
            {
                exportSaveButton.Visible = true;
            }
        }

        /// <summary>
        /// Debug function for dumping the raw json
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportSaveButton_Click(object sender, EventArgs e)
        {
            var save = GetSaveJsonFromPath();
            if(save != null)
            {
                File.WriteAllText("save.txt", save);
                System.Media.SystemSounds.Beep.Play();
            }
        }
    }
}
