using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using SSMT;
using SSMT_Core;
using Sword.Configs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sword
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProtectPage : Page
    {
        // 定义一个可观察的字符串集合
        public ObservableCollection<string> MyStringList { get; set; }


        public ProtectPage()
        {
            InitializeComponent();

            ProtectConfig.ReadConfig();
            ReadConfig();

        }



        private void Menu_OpenACLFolder_Click(object sender, RoutedEventArgs e)
        {
            string ACLFolderPath = TextBox_ACLFolderPath.Text;
            if (Directory.Exists(ACLFolderPath))
            {
                SSMTCommandHelper.ShellOpenFolder(ACLFolderPath);
            }
            else
            {
                _ = SSMTMessageHelper.Show("无法找到当前填写的ACL文件夹的路径", "Can't find current ACL folder path.");
            }
        }

        private void Menu_OpenTargetFolder_Click(object sender, RoutedEventArgs e)
        {
            string TargetModFolderPath = TextBox_TargetFolderPath.Text;
            if (Directory.Exists(TargetModFolderPath))
            {
                SSMTCommandHelper.ShellOpenFolder(TargetModFolderPath);
            }
            else
            {
                _ = SSMTMessageHelper.Show("无法找到当前填写的目标Mod文件夹的路径", "Can't find current target Mod folder path.");
            }
        }

        private async void Button_SelectACLFolderPath_Click(object sender, RoutedEventArgs e)
        {
            string ACLFolderPath = await SSMTCommandHelper.ChooseFolderAndGetPath();

            if (ACLFolderPath != "")
            {
                TextBox_ACLFolderPath.Text = ACLFolderPath;
                FlushKeyList();

                SaveConfig();
            }

        }

        private async void Button_SelectTargetFolderPath_Click(object sender, RoutedEventArgs e)
        {
            string TargetModFolderPath = await SSMTCommandHelper.ChooseFolderAndGetPath();

            if (TargetModFolderPath != "")
            {
                TextBox_TargetFolderPath.Text = TargetModFolderPath;

                SaveConfig();
            }
        }

        public void FlushKeyList()
        {
            MyListBox.Items.Clear();
            //遍历ACL 文件夹下面的ini文件

            string ACLFolderPath = TextBox_ACLFolderPath.Text; // 替换为指定目录的路径

            if (!Directory.Exists(ACLFolderPath))
            {
                return;
            }

            // 获取目录中以".key"结尾的所有文件
            string[] KeyFiles = Directory.GetFiles(ACLFolderPath, "*.key");

            // 遍历文件并进行处理
            foreach (string KeyFilePath in KeyFiles)
            {
                string KeyFileName = Path.GetFileName(KeyFilePath);
                MyListBox.Items.Add(KeyFileName);
            }
        }

        private void Button_FlushKeyList_Click(object sender, RoutedEventArgs e)
        {
            FlushKeyList();
            _ = SSMTMessageHelper.Show("刷新完成", "Flush Success");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // 执行你想要在这个页面被关闭或导航离开时运行的代码
            SaveConfig();
            // 如果需要，可以调用基类的 OnNavigatedFrom 方法
            base.OnNavigatedFrom(e);
        }

        public void ReadConfig()
        {
            TextBox_ACLFolderPath.Text = ProtectConfig.DBMT_Protect_ACLFolderPath;
            TextBox_TargetFolderPath.Text = ProtectConfig.DBMT_Protect_TargetModPath;
            FlushKeyList();
        }

        public void SaveConfig()
        {
            ProtectConfig.DBMT_Protect_ACLFolderPath = TextBox_ACLFolderPath.Text;
            ProtectConfig.DBMT_Protect_TargetModPath = TextBox_TargetFolderPath.Text;
            ProtectConfig.SaveConfig();
        }

        private void Button_AddProtect_Click(object sender, RoutedEventArgs e)
        {
            // 批量锁机器码： 递归遍历目录下所有的.ini文件加密
            // 先统计所有文件，ini文件改为.resS后缀，然后创建一个新的目录去生成
            // 这个过程在C++里执行就好了，这里只负责调用

            SaveConfig();

            string TargetModFolderPath = TextBox_TargetFolderPath.Text;
            if (!Directory.Exists(TargetModFolderPath))
            {
                _ = SSMTMessageHelper.Show("要锁机器码的目标Mod文件夹不存在: " + TargetModFolderPath, "Target Mod folder path doesn't exists: " + TargetModFolderPath);
                return;
            }



            //复制整个Mod文件夹到新的文件夹路径

            string TargetModFolderName = Path.GetFileName(TargetModFolderPath);
            string TargetModFolderParentFolderPath = Path.GetDirectoryName(TargetModFolderPath);
            //_ = MessageHelper.Show(TargetModFolderName);

            string NewTargetModFolderName = TargetModFolderName + "_Release";
            string NewTargetModFolderPath = Path.Combine(TargetModFolderParentFolderPath, NewTargetModFolderName + "\\");
            //_ = MessageHelper.Show(NewTargetModFolderPath);

            if (!Directory.Exists(NewTargetModFolderPath))
            {
                Directory.CreateDirectory(NewTargetModFolderPath);
            }

            //Mod文件复制到新目录后再加密，这样就不用每次都复制一次了。
            DBMTFileUtils.CopyDirectory(TargetModFolderPath, NewTargetModFolderPath, true);


            //把当前的ACL文件以及加密的目标文件夹目录存放到对应配置文件中。
            string ACLSettingJsonPath = ProtectConfig.Path_ACLSettingJson;

            JObject jobj = DBMTJsonUtils.CreateJObject();
            jobj["targetACLFile"] = NewTargetModFolderPath;

            // 获取目录中以".key"结尾的所有文件
            string ACLFolderPath = TextBox_ACLFolderPath.Text;
            if (!Directory.Exists(ACLFolderPath))
            {
                return;
            }
            string[] KeyFiles = Directory.GetFiles(ACLFolderPath, "*.key");
            jobj["AccessControlList"] = new JArray(KeyFiles);

            DBMTJsonUtils.SaveJObjectToFile(jobj, ACLSettingJsonPath);

            bool result = SSMTCommandHelper.RunPluginExeCommand("ACPTPRO_Batch_V4", "DBMT-Protect.exe");


            SSMTCommandHelper.ShellOpenFolder(NewTargetModFolderPath);

            //_ = MessageHelper.Show("批量锁机器码ACPT_PRO_V4算法执行成功! ");
        }

        private void Button_GenerateKeyFile_Click(object sender, RoutedEventArgs e)
        {
            JObject jsonObject = DBMTJsonUtils.CreateJObject();
            jsonObject["UserName"] = TextBox_UserName.Text;
            jsonObject["DeviceID"] = TextBox_UUID.Text;
            DBMTJsonUtils.SaveJObjectToFile(jsonObject, ProtectConfig.Path_DeviceKeySetting);
            bool result = SSMTCommandHelper.RunPluginExeCommand("generateKeyByDeviceID", "DBMT-Protect.exe");

            if (result)
            {
                SSMTCommandHelper.ShellOpenFolder(ProtectConfig.Path_GeneratedAESKeyFolder);
            }
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // ✅ 每次进入页面都会执行，适合刷新 UI
            // 因为开启了缓存模式之后，是无法刷新页面语言的，只能在这里执行来刷新
            TranslatePage();
        }

        public void TranslatePage()
        {
            if (GlobalConfig.Chinese)
            {
                Menu_ModEncryption.Title = "Mod加密";
                Menu_ObfuscateAndEncryptBufferAndIni.Text = "一键混淆名称并加密Buffer和ini文件";
                Menu_EncryptBufferAndIni.Text = "一键加密Buffer和ini文件";

                Menu_Obfuscate.Text = "一键混淆Mod中的资源名称(Play版)";
                Menu_EncryptBuffer.Text = "一键加密Buffer文件";
                Menu_EncryptIni.Text = "一键加密ini文件";
            }
            else
            {
                Menu_ModEncryption.Title = "Mod Encryption";
                Menu_ObfuscateAndEncryptBufferAndIni.Text = "Obfuscate And Encrypt Mod's Buffer And Ini File";
                Menu_EncryptBufferAndIni.Text = "Encrypt Mod's Buffer And Ini File";

                Menu_Obfuscate.Text = "Obfuscate Resource Name In Mod's .ini File(Used Only In Play Version d3d11.dll)";
                Menu_EncryptBuffer.Text = "Encrypt Mod's Buffer File";
                Menu_EncryptIni.Text = "Encrypt Mod's .ini File";
            }
        }

        public async void Encryption_EncryptAll(object sender, RoutedEventArgs e)
        {
            //混淆并返回新的ini文件的路径
            string NewModInIPath = await Obfuscate_ModFileName("Play");
            if (NewModInIPath == "")
            {
                return;
            }
            //调用加密Buffer并加密ini文件
            DBMT_Encryption_RunCommand("encrypt_buffer_ini_v5", NewModInIPath);
        }


        public static async Task<string> Obfuscate_ModFileName(string obfusVersion = "Dev")
        {
            FileOpenPicker picker = SSMTCommandHelper.Get_FileOpenPicker(".ini");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                string readIniPath = file.Path;


                if (string.IsNullOrEmpty(readIniPath))
                {
                    _ = SSMTMessageHelper.Show("Please select a correct ini file.");
                    return "";
                }
                string readDirectoryPath = Path.GetDirectoryName(readIniPath);

                //Read every line and obfuscate every Resource section.
                //need a dict to store the old filename and the new filename.
                string[] readIniLines = File.ReadAllLines(readIniPath);
                List<string> newIniLines = new List<string>();
                Dictionary<string, string> fileNameUuidDict = new Dictionary<string, string>();
                foreach (string iniLine in readIniLines)
                {
                    string lowerIniLine = iniLine.ToLower();
                    if (lowerIniLine.StartsWith("filename")
                        || (lowerIniLine.Contains("\\") && lowerIniLine.Contains("=") && lowerIniLine.Contains("."))
                        )
                    {
                        int firstEqualSignIndex = iniLine.IndexOf("=");
                        string valSection = iniLine.Substring(firstEqualSignIndex);
                        string resourceFileName = valSection.Substring(1).Trim();
                        //generate a uuid to replace this filename
                        string randomUUID = Guid.NewGuid().ToString();

                        //因为不能有重复键
                        if (!fileNameUuidDict.ContainsKey(resourceFileName))
                        {
                            fileNameUuidDict.Add(resourceFileName, randomUUID);
                        }
                        else
                        {
                            randomUUID = fileNameUuidDict[resourceFileName];
                        }

                        string newIniLine = "";
                        if (resourceFileName.EndsWith(".dds"))
                        {
                            if (obfusVersion == "Dev")
                            {
                                newIniLine = iniLine.Replace(resourceFileName, randomUUID + ".dds");
                            }
                            else
                            {
                                newIniLine = iniLine.Replace(resourceFileName, randomUUID + ".bundle");
                            }
                        }
                        else if (resourceFileName.EndsWith(".png"))
                        {
                            newIniLine = iniLine.Replace(resourceFileName, randomUUID + ".png");
                        }
                        else
                        {
                            newIniLine = iniLine.Replace(resourceFileName, randomUUID + ".assets");
                        }
                        newIniLines.Add(newIniLine);
                    }
                    else
                    {
                        newIniLines.Add(iniLine);

                    }
                }


                string parentDirectory = Directory.GetParent(readDirectoryPath).FullName;
                string ModFolderName = Path.GetFileName(readDirectoryPath);

                string newOutputDirectory = parentDirectory + "\\" + ModFolderName + "-Release\\";

                Directory.CreateDirectory(newOutputDirectory);

                //Create a new ini file.
                string newIniFilePath = newOutputDirectory + Guid.NewGuid().ToString() + ".ini";
                File.WriteAllLines(newIniFilePath, newIniLines);

                foreach (KeyValuePair<string, string> pair in fileNameUuidDict)
                {
                    string key = pair.Key;
                    string value = pair.Value;

                    string oldResourceFilePath = readDirectoryPath + "\\" + key;


                    string newResourceFilePath = "";
                    if (key.EndsWith(".dds"))
                    {
                        if (obfusVersion == "Dev")
                        {
                            newResourceFilePath = newOutputDirectory + value + ".dds";
                        }
                        else
                        {
                            newResourceFilePath = newOutputDirectory + value + ".bundle";
                        }
                    }
                    else if (key.EndsWith(".png"))
                    {
                        newResourceFilePath = newOutputDirectory + value + ".png";
                    }
                    else
                    {
                        newResourceFilePath = newOutputDirectory + value + ".assets";
                    }

                    if (File.Exists(oldResourceFilePath))
                    {
                        File.Copy(oldResourceFilePath, newResourceFilePath, true);
                    }

                }

                await SSMTMessageHelper.Show("混淆成功", "Obfuscated success.");

                return newIniFilePath;

            }


            return "";
        }



        public static bool DBMT_Encryption_RunCommand(string CommandString, string IniPath)
        {

            JObject jsonObject = new JObject();
            jsonObject["EncryptFilePath"] = IniPath;

            string ArmorSettingJsonPath = Path.Combine(ProtectConfig.Path_ConfigsFolder, "ArmorSetting.json");

            File.WriteAllText(ArmorSettingJsonPath, jsonObject.ToString());

            SSMTCommandHelper.RunPluginExeCommand(CommandString, "DBMT-Encryptor.exe");
            return true;
        }




        public async void Encryption_EncryptBufferAndIni(object sender, RoutedEventArgs e)
        {
            string ini_file_path = await SSMTCommandHelper.ChooseFileAndGetPath(".ini");
            if (ini_file_path != "")
            {
                DBMT_Encryption_RunCommand("encrypt_buffer_ini_v5", ini_file_path);
            }
        }

        public async void Encryption_ObfuscatePlay(object sender, RoutedEventArgs e)
        {
            await Obfuscate_ModFileName("Play");
        }

        public async void Encryption_EncryptBuffer(object sender, RoutedEventArgs e)
        {
            string EncryptionCommand = "encrypt_buffer_acptpro_V4";

            string selected_folder_path = await SSMTCommandHelper.ChooseFolderAndGetPath();
            Debug.WriteLine("加密文件夹路径:" + selected_folder_path);

            if (selected_folder_path != "")
            {

                //判断目标路径下是否有ini文件
                // 使用Directory.GetFiles方法，并指定搜索模式为*.ini
                string[] iniFiles = Directory.GetFiles(selected_folder_path, "*.ini");
                if (iniFiles.Length == 0)
                {
                    await SSMTMessageHelper.Show("目标路径中无法找到mod的ini文件", "Target Path Can't find ini file.");
                    return;
                }


                JObject jsonObject = DBMTJsonUtils.CreateJObject();
                jsonObject["targetACLFile"] = selected_folder_path;
                DBMTJsonUtils.SaveJObjectToFile(jsonObject, ProtectConfig.Path_ACLSettingJson);

                SSMTCommandHelper.RunPluginExeCommand(EncryptionCommand, "DBMT-Encryptor.exe");

                _ = SSMTMessageHelper.Show("Buffer文件加密成功", "Buffer Files Encrypt Success.");
            }


        }

        public async void Encryption_EncryptIni(object sender, RoutedEventArgs e)
        {
            string ini_file_path = await SSMTCommandHelper.ChooseFileAndGetPath(".ini");
            if (ini_file_path != "")
            {
                DBMT_Encryption_RunCommand("encrypt_ini_acptpro_V5", ini_file_path);
            }
        }




    }
}
