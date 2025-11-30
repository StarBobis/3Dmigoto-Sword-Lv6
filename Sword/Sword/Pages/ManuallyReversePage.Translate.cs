using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SSMT_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword
{
    public partial class ManuallyReversePage
    {


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



                Menu_Textures.Title = "贴图";
                Menu_Textures_ConvertJpg.Text = "批量转换.dds格式贴图到.jpg格式";
                Menu_Textures_ConvertPng.Text = "批量转换.dds格式贴图到.png格式";
                Menu_Textures_ConvertTga.Text = "批量转换.dds格式贴图到.tga格式";

                Menu_ManuallyReverseList.Title = "列表配置";
                Menu_ManuallyReverseList_SaveCurrentList.Text = "保存当前所有列表内容";
                Menu_ManuallyReverseList_ReadListFromConfig.Text = "读取上次保存的列表内容";


                //右侧菜单
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

       

                Menu_Textures.Title = "Textures";
                Menu_Textures_ConvertJpg.Text = "Batch Convert .dds Format Texture To .jpg Format";
                Menu_Textures_ConvertPng.Text = "Batch Convert .dds Format Texture To .png Format";
                Menu_Textures_ConvertTga.Text = "Batch Convert .dds Format Texture To .tga Format";


                Menu_ManuallyReverseList.Title = "List Config";
                Menu_ManuallyReverseList_SaveCurrentList.Text = "Save List To Config";
                Menu_ManuallyReverseList_ReadListFromConfig.Text = "Read List From Config";


                //右侧菜单
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
    }
}
