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

   

            ComboBox_IBFormat.SelectedIndex = 0;
            ComboBox_Category.SelectedIndex = 0;


            ComboBox_GameTypeName.Items.Clear();
            string[] GameTypeFolderPathList = Directory.GetDirectories(PathManager.Path_GameTypeConfigsFolder);

            foreach (string GameTypeFolderPath in GameTypeFolderPathList)
            {
                string GameTypeFolderName = Path.GetFileName(GameTypeFolderPath);
                ComboBox_GameTypeName.Items.Add(GameTypeFolderName);
            }
            ComboBox_GameTypeName.SelectedIndex = 0;
        }









        private void Menu_ReverseIniNew_Click(object sender, RoutedEventArgs e)
        {

        }


  




        private void Button_ClearAllList_Click(object sender, RoutedEventArgs e)
        {
            IndexBufferItemList.Clear();
            CategoryBufferItemList.Clear();
            ShapeKeyPositionBufferItemList.Clear();
        }


        private void Menu_ReversedFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(PathManager.Path_ReversedFolder))
            {
                SSMTCommandHelper.ShellOpenFolder(PathManager.Path_ReversedFolder);
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
            SSMTCommandHelper.ShellOpenFolder(PathManager.Path_AssetsFolder);
        }

        private void Menu_OpenLogsFolder_Click(object sender, RoutedEventArgs e)
        {
            SSMTCommandHelper.ShellOpenFolder(PathManager.Path_LogsFolder);
        }

        private async void Menu_OpenLatestLogFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SSMTCommandHelper.ShellOpenFile(PathManager.Path_LatestDBMTLogFile);
            }
            catch (Exception ex)
            {
                await SSMTMessageHelper.Show("Error: " + ex.ToString());
            }
        }

        private void Menu_OpenConfigsFolder_Click(object sender, RoutedEventArgs e)
        {
            SSMTCommandHelper.ShellOpenFolder(PathManager.Path_ConfigsFolder);
        }

        private void Menu_GameTypeFolder_Click(object sender, RoutedEventArgs e)
        {
            SSMTCommandHelper.ShellOpenFolder(PathManager.Path_GameTypeConfigsFolder);
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

              
                DBMTJsonUtils.SaveJObjectToFile(ManuallyReversePageConfigJOBJ, PathManager.Path_ManuallyReversePageConfig);

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
                

                if (!File.Exists(PathManager.Path_ManuallyReversePageConfig))
                {
                    _ = SSMTMessageHelper.Show("当前暂无任何保存过的配置文件");
                    return;
                }

                JObject ManuallyReversePageConfigJOBJ = DBMTJsonUtils.ReadJObjectFromFile(PathManager.Path_ManuallyReversePageConfig);

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
                if (Directory.Exists(PathManager.Path_ReversedFolder))
                {


                    string[] ReversedFileList = Directory.GetFiles(PathManager.Path_ReversedFolder);

                    if (ReversedFileList.Length != 0)
                    {
                        bool IfDeleteReversedFiles = await SSMTMessageHelper.ShowConfirm("检测到您当前Reversed目录下含有之前的逆向文件，是否清除？", "Your Reversed folder is not empty,do you want to delete all files in it?");
                        if (IfDeleteReversedFiles)
                        {
                            Directory.GetFiles(PathManager.Path_ReversedFolder).ToList().ForEach(file =>
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



                LOG.Initialize(PathManager.Path_LogsFolder);

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

                string ModReverseOutputFolderPath = Path.Combine(PathManager.Path_ReversedFolder);
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
                LOG.SaveFile(PathManager.Path_LogsFolder);

                GlobalConfig.ReverseOutputFolder = ModReverseOutputFolderPath;
                GlobalConfig.SaveConfig();

                if (GlobalConfig.PostReverseAction == 0)
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
                LOG.SaveFile(PathManager.Path_LogsFolder);
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
