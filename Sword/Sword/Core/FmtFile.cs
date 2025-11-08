using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMT
{
    public class FmtFile
    {
        public string GameTypeName { get; set; } = "";
        public string Topology { get; set; } = "";

        public List<D3D11Element> d3D11ElementList = [];

        //Dictionary<string, D3D11Element> ElementName_D3D11Element_Dict;
        //下面这三个属性是如果你要手动拼接一个FMT文件并输出时必须设置的东西

        public string Format { get; set; } = "";
        public string Prefix { get; set; } = "";

        /// <summary>
        /// 导入模型时的大小比例，UE类游戏一般是0.01，普通游戏一般是1.0
        /// </summary>
        public string Scale { get; set; } = "1.0";


        public bool RotateAngle { get; set; } = false;
        public string RotateAngleX { get; set; } = "0";
        public string RotateAngleY { get; set; } = "0";
        public string RotateAngleZ { get; set; } = "0";

        public bool FlipFaceOrientation { get; set; } = false;




        public D3D11GameType d3D11GameType;

        public List<string> ElementNameList = [];

        public int Stride { get; set; } = 0;

        public FmtFile(D3D11GameType d3D11GameType)
        {
            this.ElementNameList = d3D11GameType.OrderedFullElementList;
            this.d3D11GameType = d3D11GameType;
            this.GameTypeName = d3D11GameType.GameTypeName;
            this.Format = "DXGI_FORMAT_R32_UINT";
            this.Stride = d3D11GameType.GetElementListTotalStride(this.ElementNameList);
            this.Scale = "1.0";

            this.FlipFaceOrientation = false;
            
            this.RotateAngleX = "0";
            this.RotateAngleY = "0";
            this.RotateAngleZ = "0";
            this.RotateAngle = false;
        }

        public void OutputFmtFile(string OutputFmtFilePath)
        {
            //防止手动初始化时忘记，这里再补一份
            if (this.Stride == 0)
            {
                this.Stride = d3D11GameType.GetElementListTotalStride(this.ElementNameList);
            }

            List<string> OutputContent = [];

            OutputContent.Add("stride: " + this.Stride.ToString());
            OutputContent.Add("topology: trianglelist");
            OutputContent.Add("format: " + this.Format);
            OutputContent.Add("gametypename: " + this.GameTypeName);
            OutputContent.Add("prefix: " + this.Prefix);
            OutputContent.Add("scale: " + this.Scale);
            OutputContent.Add("rotate_angle: " + this.RotateAngle.ToString());
            OutputContent.Add("rotate_angle_x: " + this.RotateAngleX);
            OutputContent.Add("rotate_angle_y: " + this.RotateAngleY);
            OutputContent.Add("rotate_angle_z: " + this.RotateAngleZ);
            OutputContent.Add("flip_face_orientation: " + this.FlipFaceOrientation.ToString());

            int ElementNumber = 0;
            int AlignedByteOffset = 0;
            foreach (string ElementName in this.ElementNameList)
            {
                string ElementNameUpper = ElementName.ToUpper();
                D3D11Element d3D11Element = this.d3D11GameType.ElementNameD3D11ElementDict[ElementName];
                OutputContent.Add("element[" + ElementNumber.ToString() + "]:");
                OutputContent.Add("  SemanticName: " + d3D11Element.SemanticName);
                OutputContent.Add("  SemanticIndex: " + d3D11Element.SemanticIndex);
                OutputContent.Add("  Format: " + d3D11Element.Format);
                OutputContent.Add("  ByteWidth: " + d3D11Element.ByteWidth);

                OutputContent.Add("  InputSlot: 0");//固定值，兼容其它插件

                OutputContent.Add("  AlignedByteOffset: " + AlignedByteOffset.ToString());

                AlignedByteOffset += d3D11Element.ByteWidthInt;
                OutputContent.Add("  InputSlotClass: per-vertex");//固定值，兼容其它插件
                OutputContent.Add("  InstanceDataStepRate: 0");//固定值，兼容其它插件

                ElementNumber += 1;
            }

            File.WriteAllLines(OutputFmtFilePath,OutputContent);
        }

        /// <summary>
        /// 目前只有WWMI的追加SHAPEKEY数据的用到了这个
        /// </summary>
        /// <param name="OutputFmtPath"></param>
        public void OutputFmtFileByD3D11ElementList(string OutputFmtPath)
        {
            int totalStride = 0;
            foreach (D3D11Element d3D11Element in this.d3D11ElementList)
            {
                totalStride = totalStride + d3D11Element.ByteWidthInt;
            }

            List<string> OutputContent = [];

            OutputContent.Add("stride: " + totalStride.ToString());
            OutputContent.Add("topology: trianglelist");
            OutputContent.Add("format: " + this.Format);
            OutputContent.Add("gametypename: " + this.GameTypeName);
            OutputContent.Add("prefix: " + this.Prefix);
            OutputContent.Add("scale: " + this.Scale);
            OutputContent.Add("rotate_angle: " + this.RotateAngle.ToString());
            OutputContent.Add("rotate_angle_x: " + this.RotateAngleX);
            OutputContent.Add("rotate_angle_y: " + this.RotateAngleY);
            OutputContent.Add("rotate_angle_z: " + this.RotateAngleZ);
            OutputContent.Add("flip_face_orientation: " + this.FlipFaceOrientation.ToString());

            int ElementNumber = 0;
            int AlignedByteOffset = 0;
            foreach (D3D11Element d3D11Element in this.d3D11ElementList)
            {
                OutputContent.Add("element[" + ElementNumber.ToString() + "]:");
                OutputContent.Add("  SemanticName: " + d3D11Element.SemanticName);
                OutputContent.Add("  SemanticIndex: " + d3D11Element.SemanticIndex);
                OutputContent.Add("  Format: " + d3D11Element.Format);
                OutputContent.Add("  ByteWidth: " + d3D11Element.ByteWidth);

                OutputContent.Add("  InputSlot: 0");//固定值，兼容其它插件

                OutputContent.Add("  AlignedByteOffset: " + AlignedByteOffset.ToString());

                AlignedByteOffset += d3D11Element.ByteWidthInt;
                OutputContent.Add("  InputSlotClass: per-vertex");//固定值，兼容其它插件
                OutputContent.Add("  InstanceDataStepRate: 0");//固定值，兼容其它插件

                ElementNumber += 1;
            }

            File.WriteAllLines(OutputFmtPath, OutputContent);

        }

    }
}
