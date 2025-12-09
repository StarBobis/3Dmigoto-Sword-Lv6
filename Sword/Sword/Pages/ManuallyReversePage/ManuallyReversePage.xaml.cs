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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Sword.DTO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sword
{
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


        // 复用的 DragOver 处理方法
        private void CommonFile_DragOver(DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }


        private void Button_ClearAllList_Click(object sender, RoutedEventArgs e)
        {
            IndexBufferItemList.Clear();
            CategoryBufferItemList.Clear();
            ShapeKeyPositionBufferItemList.Clear();
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
            CommonFile_DragOver(e);
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
            CommonFile_DragOver(e);
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
            CommonFile_DragOver(e);
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
            CommonFile_DragOver(e);
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
            CommonFile_DragOver(e);
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
            CommonFile_DragOver(e);
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
