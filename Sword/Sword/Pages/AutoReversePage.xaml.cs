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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sword
{




    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AutoReversePage : Page
    {
        public bool IsLoading = false;
        public AutoReversePage()
        {
            InitializeComponent();

            IsLoading = true;

            GlobalConfig.ReadConfig();

            ComboBox_GameName.Items.Clear();
            ComboBox_GameName.Items.Add(AutoReverseGameName.GI);
            ComboBox_GameName.Items.Add(AutoReverseGameName.HI3);
            ComboBox_GameName.Items.Add(AutoReverseGameName.HSR);
            ComboBox_GameName.Items.Add(AutoReverseGameName.ZZZ);
            ComboBox_GameName.Items.Add(AutoReverseGameName.WWMI);
            ComboBox_GameName.Items.Add(AutoReverseGameName.IdentityV);
            ComboBox_GameName.Items.Add(AutoReverseGameName.IdentityV2);
            ComboBox_GameName.SelectedItem = GlobalConfig.AutoReverseGameName;

            DecideGUIVisibilityByGame();

            ComboBox_WWMIReverseStyle.Items.Clear();
            ComboBox_WWMIReverseStyle.Items.Add(WWMIReverseStyle.WWMI);
            ComboBox_WWMIReverseStyle.Items.Add(WWMIReverseStyle.SSMT);
            ComboBox_WWMIReverseStyle.SelectedItem = GlobalConfig.WWMIReverseStyle;

            IsLoading = false;
        }


        private void DecideGUIVisibilityByGame()
        {
            //WWMI only show one reverse button
            //Only WWMI show WWMIReverse style combobox
            if (GlobalConfig.AutoReverseGameName == AutoReverseGameName.WWMI)
            {
                SettingsCard_WWMIReverseStyle.Visibility = Visibility.Visible;

                Button_ReverseSingleIni.Visibility = Visibility.Visible;
                Button_ReverseBufferBasedToggleIni.Visibility = Visibility.Collapsed;
                Button_ReverseDrawIndexedBasedToggleIni.Visibility = Visibility.Collapsed;
            }
            else
            {
                SettingsCard_WWMIReverseStyle.Visibility = Visibility.Collapsed;

                Button_ReverseSingleIni.Visibility = Visibility.Visible;
                Button_ReverseBufferBasedToggleIni.Visibility = Visibility.Visible;
                Button_ReverseDrawIndexedBasedToggleIni.Visibility = Visibility.Visible;
            }
        }

        private void ComboBox_GameName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoading)
            {
                return;
            }

            GlobalConfig.AutoReverseGameName = ComboBox_GameName.SelectedItem.ToString();
            GlobalConfig.SaveConfig();

            DecideGUIVisibilityByGame();

        }

        private void ComboBox_WWMIReverseStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoading)
            {
                return;
            }

            GlobalConfig.WWMIReverseStyle = ComboBox_WWMIReverseStyle.SelectedItem.ToString();
            GlobalConfig.SaveConfig();
        }


        public static async Task<string> RunReverseIniCommand(string commandStr, string GameName)
        {


            //选择Mod的ini文件
            string ReverseIniFilePath = await SSMTCommandHelper.ChooseFileAndGetPath(".ini");
            if (ReverseIniFilePath == null || ReverseIniFilePath == "")
            {
                return "";
            }

            JObject runInputJson = new JObject();
            if (File.Exists(PathManager.Path_RunInputJson))
            {
                string json = File.ReadAllText(PathManager.Path_RunInputJson); // 读取文件内容
                runInputJson = JObject.Parse(json);
            }
            runInputJson["GameName"] = GameName;
            runInputJson["WWMIReverseStyle"] = GlobalConfig.WWMIReverseStyle;
            runInputJson["ReverseFilePath"] = ReverseIniFilePath;
            File.WriteAllText(PathManager.Path_RunInputJson, runInputJson.ToString());
            LOG.Info("RunReverseCommand::Start");
            bool RunResult = SSMTCommandHelper.RunPluginExeCommand(commandStr, "3Dmigoto-Sword-Lv5.exe", true, true);
            LOG.Info("RunReverseCommand::End");
            LOG.Info(RunResult.ToString());

            if (RunResult)
            {
                return ReverseIniFilePath;
            }
            else
            {
                return "";
            }

        }

        /// <summary>
        /// 
        /// 逆向结果文件夹写到ReverseResult.json
        /// 这样在我们的Blender面板里就能一键导入了
        /// 
        /// </summary>
        /// <param name="ModReverseFolderPath"></param>
        private void SaveReverseOutputFolderPathToConfig(string ModReverseFolderPath)
        {
            GlobalConfig.ReverseOutputFolder = ModReverseFolderPath;
            GlobalConfig.SaveConfig();
        }


        private async void Menu_Reverse_SingleIni_Click(object sender, RoutedEventArgs e)
        {
            LOG.Initialize(PathManager.Path_LogsFolder);
            try
            {
                if (!File.Exists(PathManager.Path_SwordLv5Exe))
                {
                    _ = SSMTMessageHelper.Show("您当前Plugins目录下的3Dmigoto-Sword-Lv5.exe不存在，请联系NicoMico获取后安装到Plugins目录下来使用。");
                    return;
                }
                LOG.Info("RunReverseIniCommand::Start");
                string CurrentSelectedGame = ComboBox_GameName.SelectedItem.ToString();
                string ModIniFilePath = await RunReverseIniCommand("ReverseSingleLv5", CurrentSelectedGame);
                LOG.Info(ModIniFilePath);
                LOG.Info("RunReverseIniCommand::End");

                if (ModIniFilePath == "")
                {
                    return;
                }

                if (File.Exists(ModIniFilePath))
                {
                    string ModFolderPath = Path.GetDirectoryName(ModIniFilePath);

                    //转换贴图
                    if (GlobalConfig.ConvertOriginalTextures == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, GlobalConfig.TextureConversionFormat);
                    }
                    string ModFolderName = Path.GetFileName(ModFolderPath);
                    string ModFolderParentPath = Path.GetDirectoryName(ModFolderPath);
                    string ModReverseFolderPath = ModFolderParentPath + "\\" + ModFolderName + "-SingleModIniReverse\\";

                    string iniFileName = Path.GetFileNameWithoutExtension(ModIniFilePath);
                    if (CurrentSelectedGame == AutoReverseGameName.WWMI)
                    {
                        ModReverseFolderPath = Path.Combine(ModFolderParentPath, ModFolderName + "-" + iniFileName + "-SingleModIniReverse\\");
                    }

                    if (GlobalConfig.ConvertTexturesToOutputFolder == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, GlobalConfig.TextureConversionFormat, ModReverseFolderPath);
                    }


                    SaveReverseOutputFolderPathToConfig(ModReverseFolderPath);

                    if (GlobalConfig.PostReverseAction == 0)
                    {
                        SSMTCommandHelper.ShellOpenFolder(ModReverseFolderPath);
                    }
                    else
                    {
                        _ = SSMTMessageHelper.Show("逆向成功!", "Reverse Success!");
                    }
                }
                else
                {
                    _ = SSMTCommandHelper.ShellOpenFile(PathManager.Path_LatestDBMTLogFile);
                }
            }
            catch (Exception ex)
            {

                _ = SSMTMessageHelper.Show("Error: " + ex.ToString());
            }


        }

        private async void Menu_Reverse_ToggleIni_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(PathManager.Path_SwordLv5Exe))
                {
                    _ = SSMTMessageHelper.Show("您当前Plugins目录下的3Dmigoto-Sword-Lv5.exe不存在，请检查是否被杀软错误删除或关闭杀软后重新完整下载本软件使用。");
                    return;
                }

                string ModIniFilePath = await RunReverseIniCommand("ReverseMergedLv5", ComboBox_GameName.SelectedItem.ToString());
                if (ModIniFilePath == "")
                {
                    return;
                }
                if (File.Exists(ModIniFilePath))
                {
                    string ModFolderPath = Path.GetDirectoryName(ModIniFilePath);

                    //转换贴图
                    if (GlobalConfig.ConvertOriginalTextures == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, GlobalConfig.TextureConversionFormat);
                    }

                    string ModFolderName = Path.GetFileName(ModFolderPath);
                    string ModFolderParentPath = Path.GetDirectoryName(ModFolderPath);
                    string ModReverseFolderPath = ModFolderParentPath + "\\" + ModFolderName + "-BufferBasedToggleModIniReverse\\";
                    if (GlobalConfig.ConvertTexturesToOutputFolder == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, GlobalConfig.TextureConversionFormat, ModReverseFolderPath);
                    }

                    SaveReverseOutputFolderPathToConfig(ModReverseFolderPath);


                    if (GlobalConfig.PostReverseAction == 0)
                    {
                        SSMTCommandHelper.ShellOpenFolder(ModReverseFolderPath);
                    }
                    else
                    {
                        _ = SSMTMessageHelper.Show("逆向成功!", "Reverse Success!");
                    }
                }
                else
                {
                    _ = SSMTCommandHelper.ShellOpenFile(PathManager.Path_LatestDBMTLogFile);
                }
            }
            catch (Exception ex)
            {

                _ = SSMTMessageHelper.Show("Error: " + ex.ToString());
            }



        }

        private async void Menu_Reverse_DrawIndexedIni_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(PathManager.Path_SwordLv5Exe))
                {
                    _ = SSMTMessageHelper.Show("您当前Plugins目录下的3Dmigoto-Sword-Lv5.exe不存在，请检查是否被杀软错误删除或关闭杀软后重新完整下载本软件使用。");
                    return;
                }

                string ModIniFilePath = await RunReverseIniCommand("ReverseOutfitCompilerLv4", ComboBox_GameName.SelectedItem.ToString());
                if (ModIniFilePath == "")
                {
                    return;
                }

                if (File.Exists(ModIniFilePath))
                {
                    string ModFolderPath = Path.GetDirectoryName(ModIniFilePath);

                    //转换贴图
                    if (GlobalConfig.ConvertOriginalTextures)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, GlobalConfig.TextureConversionFormat);
                    }

                    string ModFolderName = Path.GetFileName(ModFolderPath);
                    string ModFolderParentPath = Path.GetDirectoryName(ModFolderPath);
                    string ModReverseFolderPath = ModFolderParentPath + "\\" + ModFolderName + "-DrawIndexedBasedToggleModIniReverse\\";

                    if (GlobalConfig.ConvertTexturesToOutputFolder)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, GlobalConfig.TextureConversionFormat, ModReverseFolderPath);
                    }

                    SaveReverseOutputFolderPathToConfig(ModReverseFolderPath);

                    if (GlobalConfig.PostReverseAction == 0)
                    {
                        SSMTCommandHelper.ShellOpenFolder(ModReverseFolderPath);
                    }
                    else
                    {
                        _ = SSMTMessageHelper.Show("逆向成功!", "Reverse Success!");
                    }
                }
                else
                {
                    _ = SSMTCommandHelper.ShellOpenFile(PathManager.Path_LatestDBMTLogFile);
                }
            }
            catch (Exception ex)
            {

                _ = SSMTMessageHelper.Show("Error: " + ex.ToString());
            }


        }


    }
}
