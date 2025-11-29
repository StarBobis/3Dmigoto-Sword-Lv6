using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSMT_Core;

namespace SSMT
{
    /// <summary>
    /// 包装起来更方便使用
    /// </summary>
    public class D3D11GameTypeLv2
    {

        public List<D3D11GameType> d3D11GameTypeList { get; set; } = [];

        public Dictionary<string, D3D11GameType> GameTypeName_D3D11GameType_Dict = new Dictionary<string, D3D11GameType>();

        public List<D3D11GameType> Ordered_GPU_CPU_D3D11GameTypeList = [];

        public D3D11GameTypeLv2(string GameTypeFolderName)
        {
            LOG.Info("初始化D3D11GameTypeLv2::Start");
            LOG.Info("读取GameType分类: " + GameTypeFolderName);
            string GameTypeCategory_Path = Path.Combine(PathManager.Path_GameTypeConfigsFolder, GameTypeFolderName + "\\");

            string[] GameTypeCategory_Files = Directory.GetFiles(GameTypeCategory_Path);

            foreach (string file_path in GameTypeCategory_Files)
            {

                if (file_path.EndsWith(".json"))
                {
                    D3D11GameType d3D11GameType = new D3D11GameType(file_path);

                    this.d3D11GameTypeList.Add(d3D11GameType);
                    LOG.Info("读取数据类型:" + d3D11GameType.GameTypeName + " PreSkinning: " + d3D11GameType.GPUPreSkinning.ToString());
                    this.GameTypeName_D3D11GameType_Dict[d3D11GameType.GameTypeName] = d3D11GameType;
                }
            }

            //先加入GPU类型
            foreach (D3D11GameType d3D11GameType1 in this.d3D11GameTypeList)
            {
                if (d3D11GameType1.GPUPreSkinning)
                {
                    this.Ordered_GPU_CPU_D3D11GameTypeList.Add(d3D11GameType1);
                    LOG.Info("加入GPU类型:" + d3D11GameType1.GameTypeName);
                }
            }

            //再加入CPU类型，这样匹配时根据这个顺序自然就能先匹配到GPU再匹配到CPU。
            foreach (D3D11GameType d3D11GameType1 in this.d3D11GameTypeList)
            {
                if (!d3D11GameType1.GPUPreSkinning)
                {
                    this.Ordered_GPU_CPU_D3D11GameTypeList.Add(d3D11GameType1);
                    LOG.Info("加入CPU类型:" + d3D11GameType1.GameTypeName);
                }
            }
            LOG.Info("初始化D3D11GameTypeLv2::End");
            LOG.NewLine();

        }

        


    }
}
