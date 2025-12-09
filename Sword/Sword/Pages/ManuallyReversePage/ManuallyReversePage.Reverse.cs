using Microsoft.UI.Xaml;
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


    }
}
