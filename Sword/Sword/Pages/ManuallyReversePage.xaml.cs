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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sword
{
    public class IBDrawIndexed
    {
        public string StartIndex { get; set; } = "";
        public string IndexCount { get; set; } = "";
    }

    public class IndexBufferItem
    {
        public string Format { get; set; } = "";
        public string IBFilePath { get; set; } = "";
    }

    public class CategoryBufferItem
    {
        public string BufFilePath { get; set; } = "";
        public string Category { get; set; } = "";

    }

    public class ShapeKeyPositionBufferItem
    {
        public string Category { get; set; } = "";

        public string BufFilePath { get; set; } = "";
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManuallyReversePage : Page
    {

        private ObservableCollection<IndexBufferItem> IndexBufferItemList = new ObservableCollection<IndexBufferItem>();
        private ObservableCollection<CategoryBufferItem> CategoryBufferItemList = new ObservableCollection<CategoryBufferItem>();
        private ObservableCollection<ShapeKeyPositionBufferItem> ShapeKeyPositionBufferItemList = new ObservableCollection<ShapeKeyPositionBufferItem>();
        private ObservableCollection<IBDrawIndexed> IBDrawIndexedList = new ObservableCollection<IBDrawIndexed>();


        public ManuallyReversePage()
        {
            InitializeComponent();

            ComboBox_GameName.Items.Add("GI");
            ComboBox_GameName.Items.Add("HI3");
            ComboBox_GameName.Items.Add("HSR");
            ComboBox_GameName.Items.Add("ZZZ");
            ComboBox_GameName.Items.Add("WWMI");

            ComboBox_GameName.Items.Add("IdentityV");
            ComboBox_GameName.Items.Add("IdentityV2");

            
            ComboBox_GameName.SelectedIndex = 0;

            ComboBox_IBFormat.SelectedIndex = 0;
            ComboBox_Category.SelectedIndex = 0;


            ComboBox_GameTypeName.Items.Clear();
            string[] GameTypeFolderPathList = Directory.GetDirectories(GlobalConfig.Path_GameTypeConfigsFolder);

            foreach (string GameTypeFolderPath in GameTypeFolderPathList)
            {
                string GameTypeFolderName = Path.GetFileName(GameTypeFolderPath);
                ComboBox_GameTypeName.Items.Add(GameTypeFolderName);
            }
            ComboBox_GameTypeName.SelectedIndex = 0;
        }




        private void ComboBox_GameName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string GameName = ComboBox_GameName.SelectedItem.ToString();

            if (GameName == "WWMI")
            {
                Menu_Reverse_DrawIndexedIni.Visibility = Visibility.Collapsed;
                Menu_Reverse_ToggleIni.Visibility = Visibility.Collapsed;
            }
            else
            {
                Menu_Reverse_DrawIndexedIni.Visibility = Visibility.Visible;
                Menu_Reverse_ToggleIni.Visibility = Visibility.Visible;
            }
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
            if (File.Exists(GlobalConfig.Path_RunInputJson))
            {
                string json = File.ReadAllText(GlobalConfig.Path_RunInputJson); // 读取文件内容
                runInputJson = JObject.Parse(json);
            }
            runInputJson["GameName"] = GameName;
            runInputJson["ReverseFilePath"] = ReverseIniFilePath;
            File.WriteAllText(GlobalConfig.Path_RunInputJson, runInputJson.ToString());
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

        private async void Menu_Reverse_SingleIni_Click(object sender, RoutedEventArgs e)
        {
            LOG.Initialize(GlobalConfig.Path_LogsFolder);
            try
            {
                if (!File.Exists(GlobalConfig.Path_SwordLv5Exe))
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
                    if (CheckBox_AutoConvertTexturesRecursivelyInOriginalModFolder.IsChecked == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, "jpg");
                    }
                    string ModFolderName = Path.GetFileName(ModFolderPath);
                    string ModFolderParentPath = Path.GetDirectoryName(ModFolderPath);
                    string ModReverseFolderPath = ModFolderParentPath + "\\" + ModFolderName + "-SingleModIniReverse\\";

                    string iniFileName = Path.GetFileNameWithoutExtension(ModIniFilePath);
                    if (CurrentSelectedGame == "WWMI")
                    {
                        ModReverseFolderPath = Path.Combine(ModFolderParentPath, ModFolderName + "-" + iniFileName + "-SingleModIniReverse\\");
                    }

                    if (CheckBox_AutoConvertTexturesRecursively.IsChecked == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, "jpg", ModReverseFolderPath);
                    }


                    SaveReverseOutputFolderPathToConfig(ModReverseFolderPath);

                    if (CheckBox_AutoOpenFolderAfterReverse.IsChecked == true)
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
                    _ = SSMTCommandHelper.ShellOpenFile(GlobalConfig.Path_LatestDBMTLogFile);
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
                if (!File.Exists(GlobalConfig.Path_SwordLv5Exe))
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
                    if (CheckBox_AutoConvertTexturesRecursivelyInOriginalModFolder.IsChecked == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, "jpg");
                    }

                    string ModFolderName = Path.GetFileName(ModFolderPath);
                    string ModFolderParentPath = Path.GetDirectoryName(ModFolderPath);
                    string ModReverseFolderPath = ModFolderParentPath + "\\" + ModFolderName + "-BufferBasedToggleModIniReverse\\";
                    if (CheckBox_AutoConvertTexturesRecursively.IsChecked == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, "jpg", ModReverseFolderPath);
                    }

                    SaveReverseOutputFolderPathToConfig(ModReverseFolderPath);


                    if (CheckBox_AutoOpenFolderAfterReverse.IsChecked == true)
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
                    _ = SSMTCommandHelper.ShellOpenFile(GlobalConfig.Path_LatestDBMTLogFile);
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
                if (!File.Exists(GlobalConfig.Path_SwordLv5Exe))
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
                    if (CheckBox_AutoConvertTexturesRecursivelyInOriginalModFolder.IsChecked == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, "jpg");
                    }

                    string ModFolderName = Path.GetFileName(ModFolderPath);
                    string ModFolderParentPath = Path.GetDirectoryName(ModFolderPath);
                    string ModReverseFolderPath = ModFolderParentPath + "\\" + ModFolderName + "-DrawIndexedBasedToggleModIniReverse\\";

                    if (CheckBox_AutoConvertTexturesRecursively.IsChecked == true)
                    {
                        SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(ModFolderPath, "jpg", ModReverseFolderPath);
                    }

                    SaveReverseOutputFolderPathToConfig(ModReverseFolderPath);

                    if (CheckBox_AutoOpenFolderAfterReverse.IsChecked == true)
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
                    _ = SSMTCommandHelper.ShellOpenFile(GlobalConfig.Path_LatestDBMTLogFile);
                }
            }
            catch (Exception ex)
            {

                _ = SSMTMessageHelper.Show("Error: " + ex.ToString());
            }


        }


        private void Menu_ReverseIniNew_Click(object sender, RoutedEventArgs e)
        {

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
                //上方菜单
                Menu_File.Title = "文件";
                Menu_OpenPluginsFolder.Text = "打开Assets文件夹";
                Menu_OpenLogsFolder.Text = "打开Logs文件夹";
                Menu_OpenLatestLogFile.Text = "打开最新的日志文件";
                Menu_OpenConfigsFolder.Text = "打开Configs文件夹";
                Menu_GameTypeFolder.Text = "打开数据类型文件夹";
                Menu_ReversedFolder.Text = "打开Reversed文件夹";

                Menu_Reverse.Title = "一键逆向";
                Menu_Reverse_SingleIni.Text = "一键逆向Mod的ini";
                Menu_Reverse_DrawIndexedIni.Text = "一键逆向基于DrawIndexed的分支Mod的ini(常用)";
                Menu_Reverse_ToggleIni.Text = "一键逆向基于Buffer的分支Mod的ini(很少用)";

                Menu_Textures.Title = "贴图";
                Menu_Textures_ConvertJpg.Text = "批量转换.dds格式贴图到.jpg格式";
                Menu_Textures_ConvertPng.Text = "批量转换.dds格式贴图到.png格式";
                Menu_Textures_ConvertTga.Text = "批量转换.dds格式贴图到.tga格式";

                Menu_ManuallyReverseList.Title = "列表配置";
                Menu_ManuallyReverseList_SaveCurrentList.Text = "保存当前所有列表内容";
                Menu_ManuallyReverseList_ReadListFromConfig.Text = "读取上次保存的列表内容";


                //右侧菜单
                ComboBox_GameName.Header = "游戏名称";
                ComboBox_GameTypeName.Header = "数据类型文件夹";
                Button_ClearAllList.Content = "清除当前列表";
                Button_ExecuteManuallyReverse.Content = "执行手动逆向";

                CheckBox_AutoConvertTexturesRecursively.Content = "转换贴图格式到逆向出的文件夹";
                ToolTipService.SetToolTip(CheckBox_AutoConvertTexturesRecursively, "逆向后自动递归转换Mod的贴图格式，开启后每次只要运行一键逆向功能，就会自动递归转换ini所在文件夹及其所有子文件夹下的dds格式贴图到设置的全局贴图转换格式");

                CheckBox_AutoOpenFolderAfterReverse.Content = "打开逆向好的文件夹";
                ToolTipService.SetToolTip(CheckBox_AutoOpenFolderAfterReverse, "开启后每次只要运行一键逆向功能，就会自动打开逆向好的目标文件夹，不开启则逆向完成后只弹出逆向成功提示");

                CheckBox_AutoConvertTexturesRecursivelyInOriginalModFolder.Content = "递归转换Mod原文件中的贴图格式";
                ToolTipService.SetToolTip(CheckBox_AutoConvertTexturesRecursivelyInOriginalModFolder, "勾选后，逆向完成后自动把Mod文件夹里的所有.dds贴图，原地转换到旁边的ConvertedTextures文件夹中，方便使用");

                //主要内容
                TextBlock_IndexBufferFileList.Text = "Index Buffer文件列表";
                ToolTipService.SetToolTip(TextBlock_IndexBufferFileList, "单个DrawIB对应Mod文件中的IndexBuffer文件，后缀名一般为.buf或.ib，格式一般为DXGI_FORMAT_R32_UINT或DXGI_FORMAT_R16_UINT");

                Menu_DeleteIBFilePath.Text = "删除选中项";
                DataGridTextColumn_IB_Format.Header = "格式";
                DataGridTextColumn_IB_FilePath.Header = "IB文件路径";
                TextBlock_IndexBufferFilePath.Text = "IndexBuffer文件路径:";
                Button_ChooseIBBufFile.Content = "选择Buffer文件";
                TextBlock_IndexBufferFormat.Text = "格式:";
                Button_AddToIndexBufferList.Content = "添加到列表";

                //Category Buffer List
                TextBlock_CategoryBufferFileList.Text = "CategoryBuffer文件列表";
                ToolTipService.SetToolTip(TextBlock_CategoryBufferFileList, "对于手动逆向来说一般的Mod会包含Position、Texcoord、Blend等分类文件，大部分未经混淆的CategoryBuffer文件都是.buf格式且可以根据文件名看出来所属分类");

                Menu_DeleteCategoryBufFilePath.Text = "删除选中项";
                DataGridTextColumn_CategoryBuffer_Category.Header = "类别";
                DataGridTextColumn_CategoryBuffer_FilePath.Header = "Buffer文件路径";

                TextBlock_CategoryBuffer_FilePath.Text = "分类Buffer文件路径:";
                Button_ChooseCategoryBufferFile.Content = "选择Buffer文件";

                TextBlock_CategoryBuffer_Category.Text = "类别:";
                Button_AddToCategoryBufferList.Content = "添加到列表";

                //ShapeKey Position Buffer List
                TextBlock_ShapeKeyPositionBufferFileList.Text = "ShapeKey Position Buffer文件列表";
                ToolTipService.SetToolTip(TextBlock_ShapeKeyPositionBufferFileList, "仅用于滑块面板Mod中，存在多个大小相同的Position类型buf文件的情况，此时可以将多余的Position类型buf文件变为形态键");

                Menu_DeleteShapeKeyPositionBufFilePath.Text = "删除选中项";
                DataGridTextColumn_ShapeKeyPositionBuffer_Category.Header = "形态键名称";
                DataGridTextColumn_ShapeKeyPositionBuffer_FilePath.Header = "ShapeKey Position Buffer文件路径";

                TextBlock_ShapeKeyPositionBuffer_FilePath.Text = "ShapeKey Position Buffer文件路径";
                Button_ChooseShapeKeyPositionBufferFile.Content = "选择作为ShapeKey的Position类型Buffer文件";

                TextBlock_ShapeKeyPositionBuffer_Category.Text = "形态键名称";
                Button_AddToShapeKeyPositionBufferList.Content = "添加到列表";

            }
            else
            {
                //上方菜单
                Menu_File.Title = "File";
                Menu_OpenPluginsFolder.Text = "Open Assets Folder";
                Menu_OpenLogsFolder.Text = "Open Logs Folder";
                Menu_OpenLatestLogFile.Text = "Open Latest Log File";
                Menu_OpenConfigsFolder.Text = "Open Configs Folder";
                Menu_GameTypeFolder.Text = "Open GameType Folder";
                Menu_ReversedFolder.Text = "Open Reversed Folder";

                Menu_Reverse.Title = "Reverse";
                Menu_Reverse_SingleIni.Text = "Reverse Single Mod's ini";
                Menu_Reverse_DrawIndexedIni.Text = "Reverse DrawIndexed Based Toggle Mod's ini";
                Menu_Reverse_ToggleIni.Text = "Reverse Buffer Based Toggle Mod's ini";

                Menu_Textures.Title = "Textures";
                Menu_Textures_ConvertJpg.Text = "Batch Convert .dds Format Texture To .jpg Format";
                Menu_Textures_ConvertPng.Text = "Batch Convert .dds Format Texture To .png Format";
                Menu_Textures_ConvertTga.Text = "Batch Convert .dds Format Texture To .tga Format";


                Menu_ManuallyReverseList.Title = "List Config";
                Menu_ManuallyReverseList_SaveCurrentList.Text = "Save List To Config";
                Menu_ManuallyReverseList_ReadListFromConfig.Text = "Read List From Config";


                //右侧菜单
                ComboBox_GameName.Header = "GameName";
                ComboBox_GameTypeName.Header = "GameType Folder";
                Button_ClearAllList.Content = "Clear All List";
                Button_ExecuteManuallyReverse.Content = "Execute Manually Reverse";

                CheckBox_AutoConvertTexturesRecursively.Content = "Convert Textures To Reversed Folder";
                CheckBox_AutoOpenFolderAfterReverse.Content = "Open Reversed Folder";
                CheckBox_AutoConvertTexturesRecursivelyInOriginalModFolder.Content = "Convert Textures In Mod Folder";


                //Main Content
                TextBlock_IndexBufferFileList.Text = "Index Buffer File List";

                Menu_DeleteIBFilePath.Text = "Delete Selected Item";
                DataGridTextColumn_IB_Format.Header = "Format";
                DataGridTextColumn_IB_FilePath.Header = "IB File Path";

                TextBlock_IndexBufferFilePath.Text = "IndexBuffer File Path:";
                Button_ChooseIBBufFile.Content = "Choose Buffer File";

                TextBlock_IndexBufferFormat.Text = "Format:";
                Button_AddToIndexBufferList.Content = "Add To List";

                TextBlock_CategoryBufferFileList.Text = "Category Buffer File List";
                Menu_DeleteCategoryBufFilePath.Text = "Delete Selected Item";
                DataGridTextColumn_CategoryBuffer_Category.Header = "Category";
                DataGridTextColumn_CategoryBuffer_FilePath.Header = "Buffer File Path";

                TextBlock_CategoryBuffer_FilePath.Text = "CategoryBuffer File Path:";
                Button_ChooseCategoryBufferFile.Content = "Choose Buffer File";

                TextBlock_CategoryBuffer_Category.Text = "Category:";
                Button_AddToCategoryBufferList.Content = "Add To List";

                //ShapeKey Position Buffer List
                TextBlock_ShapeKeyPositionBufferFileList.Text = "ShapeKey Position Buffer File List";

                Menu_DeleteShapeKeyPositionBufFilePath.Text = "Delete Selected Item";
                DataGridTextColumn_ShapeKeyPositionBuffer_Category.Header = "ShapeKey Name";
                DataGridTextColumn_ShapeKeyPositionBuffer_FilePath.Header = "ShapeKey Position Buffer File Path";

                TextBlock_ShapeKeyPositionBuffer_FilePath.Text = "ShapeKey Position Buffer File Path";
                Button_ChooseShapeKeyPositionBufferFile.Content = "Choose ShapeKey Position Buffer File";

                TextBlock_ShapeKeyPositionBuffer_Category.Text = "ShapeKey Name";
                Button_AddToShapeKeyPositionBufferList.Content = "Add To List";

            }
        }



        private void Button_ClearAllList_Click(object sender, RoutedEventArgs e)
        {
            IndexBufferItemList.Clear();
            CategoryBufferItemList.Clear();
            ShapeKeyPositionBufferItemList.Clear();
        }


        private void Menu_ReversedFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(GlobalConfig.Path_ReversedFolder))
            {
                SSMTCommandHelper.ShellOpenFolder(GlobalConfig.Path_ReversedFolder);
            }
            else
            {
                _ = SSMTMessageHelper.Show("当前Reversed文件夹不存在，请先进行手动逆向生成此文件夹再来打开此文件夹。");
            }
        }


        private async void Menu_Textures_ConvertJpg_Click(object sender, RoutedEventArgs e)
        {
            string selected_folder_path = await SSMTCommandHelper.ChooseFolderAndGetPath();
            if (selected_folder_path == "")
            {
                return;
            }

            try
            {
                SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(selected_folder_path, "jpg");

                SSMTCommandHelper.ShellOpenFolder(selected_folder_path + "\\ConvertedTextures\\");

            }
            catch (Exception ex)
            {
                _ = SSMTMessageHelper.Show(ex.ToString());
            }
        }

        private async void Menu_Textures_ConvertPng_Click(object sender, RoutedEventArgs e)
        {
            string selected_folder_path = await SSMTCommandHelper.ChooseFolderAndGetPath();
            if (selected_folder_path == "")
            {
                return;
            }

            try
            {
                SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(selected_folder_path, "png");

                SSMTCommandHelper.ShellOpenFolder(selected_folder_path + "\\ConvertedTextures\\");

            }
            catch (Exception ex)
            {
                _ = SSMTMessageHelper.Show(ex.ToString());
            }
        }

        private async void Menu_Textures_ConvertTga_Click(object sender, RoutedEventArgs e)
        {
            string selected_folder_path = await SSMTCommandHelper.ChooseFolderAndGetPath();
            if (selected_folder_path == "")
            {
                return;
            }

            try
            {
                SSMTTextureHelper.ConvertAllTexturesIntoConvertedTexturesReverse(selected_folder_path, "tga");

                SSMTCommandHelper.ShellOpenFolder(selected_folder_path + "\\ConvertedTextures\\");

            }
            catch (Exception ex)
            {
                await SSMTMessageHelper.Show(ex.ToString());
            }
        }

        private void Menu_OpenPluginsFolder_Click(object sender, RoutedEventArgs e)
        {
            SSMTCommandHelper.ShellOpenFolder(GlobalConfig.Path_AssetsFolder);
        }

        private void Menu_OpenLogsFolder_Click(object sender, RoutedEventArgs e)
        {
            SSMTCommandHelper.ShellOpenFolder(GlobalConfig.Path_LogsFolder);
        }

        private async void Menu_OpenLatestLogFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SSMTCommandHelper.ShellOpenFile(GlobalConfig.Path_LatestDBMTLogFile);
            }
            catch (Exception ex)
            {
                await SSMTMessageHelper.Show("Error: " + ex.ToString());
            }
        }

        private void Menu_OpenConfigsFolder_Click(object sender, RoutedEventArgs e)
        {
            SSMTCommandHelper.ShellOpenFolder(GlobalConfig.Path_ConfigsFolder);
        }

        private void Menu_GameTypeFolder_Click(object sender, RoutedEventArgs e)
        {
            SSMTCommandHelper.ShellOpenFolder(GlobalConfig.Path_GameTypeConfigsFolder);
        }


        private void Menu_ManuallyReverseList_SaveCurrentList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JObject ManuallyReversePageConfigJOBJ = new JObject();

                //IndexBufferItemList
                JArray IndexBufferItemListJarray = new JArray();
                foreach (IndexBufferItem indexBufferItem in IndexBufferItemList)
                {
                    JObject jobj = new JObject();
                    jobj["Format"] = indexBufferItem.Format;
                    jobj["IBFilePath"] = indexBufferItem.IBFilePath;
                    IndexBufferItemListJarray.Add(jobj);
                }
                ManuallyReversePageConfigJOBJ["IndexBufferItemList"] = IndexBufferItemListJarray;

                //CategoryBufferItemList
                JArray CategoryBufferItemListJarray = new JArray();
                foreach (CategoryBufferItem categoryBufferItem in CategoryBufferItemList)
                {
                    JObject jobj = new JObject();
                    jobj["Category"] = categoryBufferItem.Category;
                    jobj["BufFilePath"] = categoryBufferItem.BufFilePath;
                    CategoryBufferItemListJarray.Add(jobj);
                }
                ManuallyReversePageConfigJOBJ["CategoryBufferItemList"] = CategoryBufferItemListJarray;

                //ShapeKeyPositionBufferItemList
                JArray ShapeKeyPositionBufferItemListJarray = new JArray();
                foreach (ShapeKeyPositionBufferItem shapeKeyPositionBufferItem in ShapeKeyPositionBufferItemList)
                {
                    JObject jobj = new JObject();
                    jobj["Category"] = shapeKeyPositionBufferItem.Category;
                    jobj["BufFilePath"] = shapeKeyPositionBufferItem.BufFilePath;
                    ShapeKeyPositionBufferItemListJarray.Add(jobj);
                }
                ManuallyReversePageConfigJOBJ["ShapeKeyPositionBufferItemList"] = ShapeKeyPositionBufferItemListJarray;


                string ManuallyReversePageConfigFilePath = Path.Combine(GlobalConfig.Path_ConfigsFolder, "ManuallyReversePageConfig.json");
                DBMTJsonUtils.SaveJObjectToFile(ManuallyReversePageConfigJOBJ, ManuallyReversePageConfigFilePath);

                _ = SSMTMessageHelper.Show("保存成功", "Save Success");
            }
            catch (Exception ex)
            {
                _ = SSMTMessageHelper.Show(ex.ToString());
            }
        }

        private void Menu_ManuallyReverseList_ReadListFromConfig_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string ManuallyReversePageConfigFilePath = Path.Combine(GlobalConfig.Path_ConfigsFolder, "ManuallyReversePageConfig.json");

                if (!File.Exists(ManuallyReversePageConfigFilePath))
                {
                    _ = SSMTMessageHelper.Show("当前暂无任何保存过的配置文件");
                    return;
                }

                JObject ManuallyReversePageConfigJOBJ = DBMTJsonUtils.ReadJObjectFromFile(ManuallyReversePageConfigFilePath);

                JArray IndexBufferItemListJarray = (JArray)ManuallyReversePageConfigJOBJ["IndexBufferItemList"];
                JArray CategoryBufferItemListJarray = (JArray)ManuallyReversePageConfigJOBJ["CategoryBufferItemList"];
                JArray ShapeKeyPositionBufferItemListJarray = (JArray)ManuallyReversePageConfigJOBJ["ShapeKeyPositionBufferItemList"];

                IndexBufferItemList.Clear();
                foreach (JObject jobj in IndexBufferItemListJarray)
                {
                    IndexBufferItem indexBufferItem = new IndexBufferItem();
                    indexBufferItem.Format = (string)jobj["Format"];
                    indexBufferItem.IBFilePath = (string)jobj["IBFilePath"];
                    IndexBufferItemList.Add(indexBufferItem);
                }

                CategoryBufferItemList.Clear();
                foreach (JObject jobj in CategoryBufferItemListJarray)
                {
                    CategoryBufferItem categoryBufferItem = new CategoryBufferItem();
                    categoryBufferItem.Category = (string)jobj["Category"];
                    categoryBufferItem.BufFilePath = (string)jobj["BufFilePath"];
                    CategoryBufferItemList.Add(categoryBufferItem);
                }

                ShapeKeyPositionBufferItemList.Clear();
                foreach (JObject jobj in ShapeKeyPositionBufferItemListJarray)
                {
                    ShapeKeyPositionBufferItem shapeKeyPositionBufferItem = new ShapeKeyPositionBufferItem();
                    shapeKeyPositionBufferItem.Category = (string)jobj["Category"];
                    shapeKeyPositionBufferItem.BufFilePath = (string)jobj["BufFilePath"];
                    ShapeKeyPositionBufferItemList.Add(shapeKeyPositionBufferItem);
                }

                _ = SSMTMessageHelper.Show("读取配置完成", "Read Config Success");
            }
            catch (Exception ex)
            {
                _ = SSMTMessageHelper.Show(ex.ToString());
            }

        }


        private async void Button_ManuallyReverseModel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //先检测Reversed目录下
                if (Directory.Exists(GlobalConfig.Path_ReversedFolder))
                {


                    string[] ReversedFileList = Directory.GetFiles(GlobalConfig.Path_ReversedFolder);

                    if (ReversedFileList.Length != 0)
                    {
                        bool IfDeleteReversedFiles = await SSMTMessageHelper.ShowConfirm("检测到您当前Reversed目录下含有之前的逆向文件，是否清除？", "Your Reversed folder is not empty,do you want to delete all files in it?");
                        if (IfDeleteReversedFiles)
                        {
                            Directory.GetFiles(GlobalConfig.Path_ReversedFolder).ToList().ForEach(file =>
                            {
                                try
                                {
                                    File.Delete(file);
                                }
                                catch (Exception ex)
                                {
                                    LOG.Error("删除Reversed目录下的文件失败: " + file + " 错误信息: " + ex.Message);
                                }
                            });

                            _ = SSMTMessageHelper.Show("已清除Reversed目录下的所有文件。", "All files in your Reversed folder have been deleted.");
                        }
                    }
                }



                LOG.Initialize(GlobalConfig.Path_LogsFolder);

                //先搞一个Category，BufFilePath的字典，顺便过滤空行
                Dictionary<string, string> CategoryBufFilePathDict = new Dictionary<string, string>();

                LOG.Info("CategoryBufFilePathDict::");
                foreach (CategoryBufferItem categoryBufferItem in CategoryBufferItemList)
                {
                    if (categoryBufferItem.Category.Trim() == "" || categoryBufferItem.BufFilePath.Trim() == "")
                    {
                        continue;
                    }

                    CategoryBufFilePathDict[categoryBufferItem.Category] = categoryBufferItem.BufFilePath;
                    LOG.Info("Category: " + categoryBufferItem.Category + " BufFilePath: " + categoryBufferItem.BufFilePath);
                }
                LOG.NewLine();

                D3D11GameTypeLv2 d3D11GameTypeLv2 = new D3D11GameTypeLv2(ComboBox_GameTypeName.SelectedItem.ToString());

                List<D3D11GameType> MatchedGameTypeList = [];
                foreach (D3D11GameType d3D11GameType in d3D11GameTypeLv2.Ordered_GPU_CPU_D3D11GameTypeList)
                {
                    LOG.Info("尝试匹配数据类型: " + d3D11GameType.GameTypeName);
                    int CategoryCount = d3D11GameType.CategoryStrideDict.Count;
                    int BufferFileCount = CategoryBufFilePathDict.Count;
                    LOG.Info("CategoryCount:" + CategoryCount.ToString());
                    LOG.Info("BufferFileCount:" + BufferFileCount.ToString());

                    if (CategoryCount != BufferFileCount)
                    {
                        LOG.NewLine("当前数据类型分类数量和CategoryBuffer文件数量不匹配，跳过此数据类型匹配。");
                        continue;
                    }

                    //获取Position文件的大小，根据步长计算顶点数
                    if (!CategoryBufFilePathDict.ContainsKey("Position"))
                    {
                        _ = SSMTMessageHelper.Show("当前填写的CategoryBuffer文件列表中，未找到分类为Position的条目，请重新填写。");
                        return;
                    }

                    string PositionFilePath = CategoryBufFilePathDict["Position"];
                    int FileSize = (int)DBMTFileUtils.GetFileSize(PositionFilePath);

                    if (!d3D11GameType.CategoryStrideDict.ContainsKey("Position"))
                    {
                        LOG.NewLine("当前数据类型不含有Position分类，跳过此数据类型。");
                        continue;
                    }

                    int Stride = d3D11GameType.CategoryStrideDict["Position"];
                    int VertexCount = FileSize / Stride;

                    bool AllCategoryMatch = true;
                    foreach (var item in d3D11GameType.CategoryStrideDict)
                    {
                        string CategoryName = item.Key;
                        int CategoryStride = item.Value;

                        if (!CategoryBufFilePathDict.ContainsKey(CategoryName))
                        {
                            AllCategoryMatch = false;
                            break;
                        }

                        string CategoryBufFilePath = CategoryBufFilePathDict[CategoryName];
                        int CategoryBufFileSize = (int)DBMTFileUtils.GetFileSize(CategoryBufFilePath);
                        int CategoryBufVertexCount = CategoryBufFileSize / CategoryStride;

                        if (CategoryBufVertexCount != VertexCount)
                        {
                            AllCategoryMatch = false;
                            break;
                        }
                    }

                    if (AllCategoryMatch)
                    {
                        MatchedGameTypeList.Add(d3D11GameType);
                        LOG.Info("数据类型: " + d3D11GameType.GameTypeName + " 匹配成功。");
                    }

                    LOG.NewLine();
                }
                LOG.NewLine();

                string ModReverseOutputFolderPath = Path.Combine(GlobalConfig.Path_ReversedFolder);
                if (!Directory.Exists(ModReverseOutputFolderPath))
                {
                    Directory.CreateDirectory(ModReverseOutputFolderPath);
                }

                if (MatchedGameTypeList.Count == 0)
                {
                    _ = SSMTMessageHelper.Show("未检测到任何满足条件的数据类型。可能的原因有：\n1.尚未添加此数据类型，可以去数据类型管理页面添加\n 2.未正确填写Buffer文件，请详细检查并重试。");
                    return;
                }

                //对于每个匹配的数据类型，都输出ib vb fmt文件
                foreach (D3D11GameType d3D11GameType in MatchedGameTypeList)
                {

                    //拼接出vb0
                    List<Dictionary<int, byte[]>> BufDictList = [];
                    foreach (string CategoryName in d3D11GameType.OrderedCategoryNameList)
                    {
                        string BufFilePath = CategoryBufFilePathDict[CategoryName];
                        int CategoryStride = d3D11GameType.CategoryStrideDict[CategoryName];
                        Dictionary<int, byte[]> BufDict = DBMTBinaryUtils.ReadBinaryFileByStride(BufFilePath, CategoryStride);
                        BufDictList.Add(BufDict);
                    }

                    Dictionary<int, byte[]> MergedVB0Dict = DBMTBinaryUtils.MergeByteDicts(BufDictList);
                    byte[] FinalVB0 = DBMTBinaryUtils.MergeDictionaryValues(MergedVB0Dict);

                    //获取顶点数
                    int MeshVertexCount = MergedVB0Dict.Count;
                    LOG.Info("顶点数: " + MeshVertexCount.ToString());

                    int Count = 0;
                    foreach (IndexBufferItem indexBufferItem in IndexBufferItemList)
                    {
                        if (indexBufferItem.IBFilePath.Trim() == "")
                        {
                            continue;
                        }

                        string IBFileName = Path.GetFileNameWithoutExtension(indexBufferItem.IBFilePath);

                        string SingleGameTypeFolderPath = Path.Combine(ModReverseOutputFolderPath, "ManuallyReverse_" + d3D11GameType.GameTypeName + "\\");
                        if (!Directory.Exists(SingleGameTypeFolderPath))
                        {
                            Directory.CreateDirectory(SingleGameTypeFolderPath);
                        }

                        string NamePrefix = "Mesh-" + Count.ToString() + "-" + IBFileName;

                        string VBOutputPath = Path.Combine(SingleGameTypeFolderPath, NamePrefix + ".vb");
                        string IBOutputPath = Path.Combine(SingleGameTypeFolderPath, NamePrefix + ".ib");
                        string FmtOutputPath = Path.Combine(SingleGameTypeFolderPath, NamePrefix + ".fmt");

                        VertexBufferBufFile VBBufFile = new VertexBufferBufFile(FinalVB0);
                        VBBufFile.SaveToFile(VBOutputPath);

                        IndexBufferBufFile IBBufFile = new IndexBufferBufFile(indexBufferItem.IBFilePath, indexBufferItem.Format);
                        IBBufFile.SelfCleanExtraVertexIndex(MeshVertexCount);
                        IBBufFile.SaveToFile_UInt32(IBOutputPath, 0);

                        FmtFile fmtFile = new FmtFile(d3D11GameType);
                        fmtFile.OutputFmtFile(FmtOutputPath);

                        Count += 1;
                    }


                }
                LOG.SaveFile(GlobalConfig.Path_LogsFolder);

                SaveReverseOutputFolderPathToConfig(ModReverseOutputFolderPath);

                if (CheckBox_AutoOpenFolderAfterReverse.IsChecked == true)
                {
                    SSMTCommandHelper.ShellOpenFolder(ModReverseOutputFolderPath);
                }
                else
                {
                    _ = SSMTMessageHelper.Show("逆向成功!", "Reverse Success!");
                }
            }
            catch (Exception ex)
            {
                LOG.SaveFile(GlobalConfig.Path_LogsFolder);
                _ = SSMTMessageHelper.Show(ex.ToString());
            }

        }


        private void Button_AddToIndexBufferList_Click(object sender, RoutedEventArgs e)
        {

            string IBBufFilePath = TextBox_IBBufFilePath.Text;

            if (IBBufFilePath.Trim() == "")
            {
                _ = SSMTMessageHelper.Show("IB文件路径不能为空");
                return;
            }

            if (!File.Exists(IBBufFilePath))
            {
                _ = SSMTMessageHelper.Show("当前填写的IB文件不存在");
                return;
            }

            string Format = ComboBox_IBFormat.Text;

            IndexBufferItemList.Add(new IndexBufferItem { IBFilePath = IBBufFilePath, Format = Format });
        }

        private void TextBox_IBBufFilePath_DragOver(object sender, DragEventArgs e)
        {
            // 检查拖拽的数据是否包含文件路径
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        private async void TextBox_IBBufFilePath_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                // 获取拖拽的文件
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file)
                    {
                        // 获取文件路径
                        string filePath = file.Path;
                        // 将文件路径添加到TextBox中
                        TextBox_IBBufFilePath.Text = filePath;
                    }
                }
            }
        }

        private void Menu_DeleteIBFilePath_Click(object sender, RoutedEventArgs e)
        {
            int index = DataGrid_IBFile.SelectedIndex;
            IndexBufferItemList.RemoveAt(index);
        }

        private async void Button_ChooseIBBufFile_Click(object sender, RoutedEventArgs e)
        {
            List<string> SuffixList = [];
            SuffixList.Add(".ib");
            SuffixList.Add(".buf");
            SuffixList.Add("*");
            string IBFilePath = await SSMTCommandHelper.ChooseFileAndGetPath(SuffixList);
            if (IBFilePath != "")
            {
                TextBox_IBBufFilePath.Text = IBFilePath;
            }
        }

        private async void Button_ChooseCategoryBufferFile_Click(object sender, RoutedEventArgs e)
        {
            List<string> SuffixList = [];
            SuffixList.Add(".buf");
            SuffixList.Add("*");
            string IBFilePath = await SSMTCommandHelper.ChooseFileAndGetPath(SuffixList);
            if (IBFilePath != "")
            {
                TextBox_CategoryBufferFilePath.Text = IBFilePath;
            }
        }



        private void Menu_DeleteCategoryBufFilePath_Click(object sender, RoutedEventArgs e)
        {
            int index = DataGrid_CategoryBufferFile.SelectedIndex;
            CategoryBufferItemList.RemoveAt(index);
        }

        private void Button_AddToCategoryBufferList_Click(object sender, RoutedEventArgs e)
        {

            string CategoryBufFilePath = TextBox_CategoryBufferFilePath.Text;

            if (CategoryBufFilePath.Trim() == "")
            {
                _ = SSMTMessageHelper.Show("CategoryBuffer文件路径不能为空");
                return;
            }

            if (!File.Exists(CategoryBufFilePath))
            {
                _ = SSMTMessageHelper.Show("当前填写的CategoryBuffer文件不存在");
                return;
            }

            string Category = ComboBox_Category.Text;

            CategoryBufferItemList.Add(new CategoryBufferItem { BufFilePath = CategoryBufFilePath, Category = Category });
        }



        private void TextBox_CategoryBufferFilePath_DragOver(object sender, DragEventArgs e)
        {
            // 检查拖拽的数据是否包含文件路径
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        private async void TextBox_CategoryBufferFilePath_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                // 获取拖拽的文件
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file)
                    {
                        // 获取文件路径
                        string filePath = file.Path;
                        // 将文件路径添加到TextBox中
                        TextBox_CategoryBufferFilePath.Text = filePath;
                    }
                }
            }
        }
        private void DataGrid_CategoryBufferFile_DragOver(object sender, DragEventArgs e)
        {
            // 检查拖拽的数据是否包含文件路径
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        private async void DataGrid_CategoryBufferFile_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                // 获取拖拽的文件
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file)
                    {
                        // 获取文件路径
                        string filePath = file.Path;

                        CategoryBufferItem cbItem = new CategoryBufferItem();

                        cbItem.BufFilePath = filePath;
                        cbItem.Category = ComboBox_Category.Text;

                        CategoryBufferItemList.Add(cbItem);
                    }
                }
            }
        }

        private void DataGrid_IBFile_DragOver(object sender, DragEventArgs e)
        {
            // 检查拖拽的数据是否包含文件路径
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        private async void DataGrid_IBFile_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                // 获取拖拽的文件
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file)
                    {
                        // 获取文件路径
                        string filePath = file.Path;

                        IndexBufferItem ibItem = new IndexBufferItem();

                        ibItem.IBFilePath = filePath;
                        ibItem.Format = ComboBox_IBFormat.Text;

                        IndexBufferItemList.Add(ibItem);
                    }
                }
            }
        }


        private void DataGrid_ShapeKeyPositionBufferFile_DragOver(object sender, DragEventArgs e)
        {
            // 检查拖拽的数据是否包含文件路径
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        private async void DataGrid_ShapeKeyPositionBufferFile_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                // 获取拖拽的文件
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file)
                    {
                        // 获取文件路径
                        string filePath = file.Path;

                        ShapeKeyPositionBufferItem shapeKeyItem = new ShapeKeyPositionBufferItem();

                        shapeKeyItem.BufFilePath = filePath;

                        string Category = ComboBox_ShapeKeyPositionCategory.Text;
                        if (Category.Trim() == "")
                        {
                            Category = Path.GetFileNameWithoutExtension(filePath);
                        }

                        shapeKeyItem.Category = Category;

                        ShapeKeyPositionBufferItemList.Add(shapeKeyItem);
                    }
                }
            }
        }

        private void Menu_DeleteShapeKeyPositionBufFilePath_Click(object sender, RoutedEventArgs e)
        {
            int index = DataGrid_ShapeKeyPositionBufferFile.SelectedIndex;
            ShapeKeyPositionBufferItemList.RemoveAt(index);
        }

        private void TextBox_ShapeKeyPositionBufferFilePath_DragOver(object sender, DragEventArgs e)
        {
            // 检查拖拽的数据是否包含文件路径
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        private async void TextBox_ShapeKeyPositionBufferFilePath_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                // 获取拖拽的文件
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file)
                    {
                        // 获取文件路径
                        string filePath = file.Path;
                        // 将文件路径添加到TextBox中
                        TextBox_ShapeKeyPositionBufferFilePath.Text = filePath;
                    }
                }
            }
        }


        private async void Button_ChooseShapeKeyPositionBufferFile_Click(object sender, RoutedEventArgs e)
        {
            List<string> SuffixList = [];
            SuffixList.Add(".buf");
            SuffixList.Add("*");
            string ShapeKeyPositionBufferFilePath = await SSMTCommandHelper.ChooseFileAndGetPath(SuffixList);
            if (ShapeKeyPositionBufferFilePath != "")
            {
                TextBox_ShapeKeyPositionBufferFilePath.Text = ShapeKeyPositionBufferFilePath;
            }
        }

        private void Button_AddToShapeKeyPositionBufferList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string CategoryBufFilePath = TextBox_ShapeKeyPositionBufferFilePath.Text;

                if (CategoryBufFilePath.Trim() == "")
                {
                    _ = SSMTMessageHelper.Show("CategoryBuffer文件路径不能为空");
                    return;
                }

                if (!File.Exists(CategoryBufFilePath))
                {
                    _ = SSMTMessageHelper.Show("当前填写的CategoryBuffer文件不存在");
                    return;
                }

                string Category = ComboBox_ShapeKeyPositionCategory.Text;

                if (Category.Trim() == "")
                {
                    Category = Path.GetFileNameWithoutExtension(CategoryBufFilePath);
                }

                ShapeKeyPositionBufferItemList.Add(new ShapeKeyPositionBufferItem { BufFilePath = CategoryBufFilePath, Category = Category });

            }
            catch (Exception ex)
            {
                _ = SSMTMessageHelper.Show(ex.ToString());
            }
        }


    }
}
