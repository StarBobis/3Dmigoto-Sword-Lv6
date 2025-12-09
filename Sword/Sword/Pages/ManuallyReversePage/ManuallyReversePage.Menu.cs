using Microsoft.UI.Xaml;
using Newtonsoft.Json.Linq;
using SSMT;
using SSMT_Core;
using Sword.Configs;
using Sword.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword
{
    public partial class ManuallyReversePage
    {


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
                ManuallyReversePageConfig.IndexBufferItemList = IndexBufferItemList.ToList();
                ManuallyReversePageConfig.CategoryBufferItemList = CategoryBufferItemList.ToList();
                ManuallyReversePageConfig.ShapeKeyPositionBufferItemList = ShapeKeyPositionBufferItemList.ToList();
                ManuallyReversePageConfig.GameTypeName = ComboBox_GameTypeName.SelectedItem.ToString();

                ManuallyReversePageConfig.SaveConfig();
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
                ManuallyReversePageConfig.ReadConfig();

                ComboBox_GameTypeName.SelectedItem = ManuallyReversePageConfig.GameTypeName;

                IndexBufferItemList.Clear();
                foreach (var item in ManuallyReversePageConfig.IndexBufferItemList)
                {
                    IndexBufferItemList.Add(item);
                }

                CategoryBufferItemList.Clear();
                foreach (var item in ManuallyReversePageConfig.CategoryBufferItemList)
                {
                    CategoryBufferItemList.Add(item);
                }

                ShapeKeyPositionBufferItemList.Clear();
                foreach (var item in ManuallyReversePageConfig.ShapeKeyPositionBufferItemList)
                {
                    ShapeKeyPositionBufferItemList.Add(item);
                }

                _ = SSMTMessageHelper.Show("读取配置完成", "Read Config Success");
            }
            catch (Exception ex)
            {
                _ = SSMTMessageHelper.Show(ex.ToString());
            }

        }




    }
}
