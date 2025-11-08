using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSMT_Core;

namespace SSMT
{
    public class IndexBufferBufFile
    {
        public string Index { get; set; } = "";
        public string MatchFirstIndex { get; set; } = "";

        public UInt32 ReadDrawNumber { get; set; } = 0; //作用未知，但保留，好像是逆向的遗留产物？

        //最小的顶点索引值
        public UInt32 MinNumber { get; set; }= 0;
        //最大的顶点索引值
        public UInt32 MaxNumber { get; set; } = 0;
        //总共有几个索引值
        public UInt32 NumberCount { get; set; } = 0;
        //完全不同的顶点索引值数量，也代表实际用到的顶点数
        public UInt32 UniqueVertexCount { get; set; } = 0;

        //顶点索引值列表，多个IB文件组合在一起时，要用这个组合而不是二进制组合，避免格式不一致的问题。
        public List<UInt32> NumberList { get; set; } = [];

        public void SaveToFile_UInt32(string OutputIBBufFilePath,int Offset)
        {
            List<int> WriteNumberList = [];
            foreach (int number in this.NumberList)
            {
                WriteNumberList.Add(number + Offset);
            }
            DBMTBinaryUtils.WriteAsR32_UINT(WriteNumberList, OutputIBBufFilePath);
        }

        public void SaveToFile_UInt16(string OutputIBBufFilePath, int Offset)
        {
            List<int> WriteNumberList = [];
            foreach (int number in this.NumberList)
            {
                WriteNumberList.Add(number + Offset);
            }
            DBMTBinaryUtils.WriteAsR16_UINT(WriteNumberList, OutputIBBufFilePath);
        }

        /// <summary>
        /// Clean是把超过顶点数的部分，全部变为00 而不是删掉。
        /// 删掉虽然方便，但是如果遇到drawindexed类型的IB膨胀，就会出问题
        /// </summary>
        /// <param name="MeshVertexCount"></param>
        public void SelfCleanExtraVertexIndex(int MeshVertexCount)
        {
            LOG.Info("IBBufFile::SelfCleanExtraVertexIndex::Start");
            LOG.Info("MeshVertexCount: " + MeshVertexCount.ToString());

            LOG.Info("当前IB文件处理前总顶点数: " + NumberList.Count);

            List<UInt32> NewNumberList = [];
            HashSet<UInt32> NumberSet = new HashSet<UInt32>();

            UInt32 SubCount = 0;

            UInt32 TmpMaxNumber = 0;
            UInt32 TmpMinNumber = 9999999;

            for (int i = 0; i < NumberList.Count; i++)
            {


                UInt32 TmpNumber = NumberList[i];
                if (i < 30)
                {
                    LOG.Info("TmpNumber: " + TmpNumber.ToString());
                }
                if (TmpNumber >= MeshVertexCount)
                {
                    if (i < 30)
                    {
                        LOG.Info("Remove TmpNumber: " + TmpNumber.ToString());
                    }
                    //大于等于模型顶点数的顶点索引都是假的，都要去除。
                    TmpNumber = 0;
                }
                NewNumberList.Add(TmpNumber);
                NumberSet.Add(TmpNumber);
                //LOG.Info("TmpNumber: " + TmpNumber.ToString());

                SubCount++;


                if (TmpNumber > TmpMaxNumber)
                {
                    TmpMaxNumber = TmpNumber;
                }

                if (TmpNumber < TmpMinNumber)
                {
                    TmpMinNumber = TmpNumber;
                }

            }

            MinNumber = TmpMinNumber;
            MaxNumber = TmpMaxNumber;
            NumberCount = (UInt32)NewNumberList.Count;
            UniqueVertexCount = (UInt32)NumberSet.Count;
            NumberList = NewNumberList;

            LOG.Info("当前IB文件处理后总顶点数: " + NumberList.Count);
            LOG.NewLine("IBBufFile::SelfCleanExtraVertexIndex::End");
        }
        public void SelfDivide(int FirstIndex, int IndexCount)
        {
            LOG.Info("IBBufFile::SelfDivide::Start");
            LOG.Info("FirstIndex: " + FirstIndex.ToString());
            LOG.Info("IndexCount: " + IndexCount.ToString());
            LOG.Info("当前IB文件总顶点数: " + this.NumberList.Count);
            List<UInt32> NewNumberList = [];
            HashSet<UInt32> NumberSet = new HashSet<UInt32>();

            UInt32 SubCount = 0;

            UInt32 TmpMaxNumber = 0;
            UInt32 TmpMinNumber = 9999999;

            for (int i = 0; i < this.NumberList.Count; i++)
            {
                if (i < FirstIndex)
                {
                    continue;
                }

                UInt32 TmpNumber = this.NumberList[i];
                NewNumberList.Add(TmpNumber);
                NumberSet.Add(TmpNumber);
                //LOG.Info("TmpNumber: " + TmpNumber.ToString());

                SubCount++;


                if (TmpNumber > TmpMaxNumber)
                {
                    TmpMaxNumber = TmpNumber;
                }

                if (TmpNumber < TmpMinNumber)
                {
                    TmpMinNumber = TmpNumber;
                }


                if (SubCount == IndexCount)
                {
                    break;
                }
            }

            this.MinNumber = TmpMinNumber;
            this.MaxNumber = TmpMaxNumber;
            this.NumberCount = (UInt32)NewNumberList.Count;
            this.UniqueVertexCount = (UInt32)NumberSet.Count;
            this.NumberList = NewNumberList;
            LOG.NewLine("IBBufFile::SelfDivide::End");

        }


        public IndexBufferBufFile()
        {
            //啥也不干但是得有，方便凭空构造
        }


        public IndexBufferBufFile(string IndexBufferFilePath,string Format)
        {
            string FileName = Path.GetFileName(IndexBufferFilePath);
            this.Index = FileName.Substring(0,6);
            

            string FormatLower = Format.ToLower();

            List<UInt32> TmpNumberList = [];

            if (FormatLower == "dxgi_format_r32_uint")
            {
                TmpNumberList = DBMTBinaryUtils.ReadAsR32_UINT(IndexBufferFilePath);
            }
            else if (FormatLower == "dxgi_format_r16_uint") 
            {
                TmpNumberList = DBMTBinaryUtils.ReadAsR16_UINT(IndexBufferFilePath);
            }

            this.NumberList = TmpNumberList;
            //分别求出最大值最小值，读取数量，独特顶点数

            UInt32 MaxNumber = 0;
            UInt32 MinNumber = 9999999;
            foreach (UInt32 TmpNumber in TmpNumberList)
            {
                if (MaxNumber < TmpNumber)
                {
                    MaxNumber = TmpNumber;
                }

                if (MinNumber > TmpNumber)
                {
                    MinNumber = TmpNumber;
                }
            }
            this.MaxNumber = MaxNumber;
            this.MinNumber = MinNumber;

            this.NumberCount = (UInt32)TmpNumberList.Count;

            HashSet<UInt32> uniqueNumbers = new HashSet<UInt32>(TmpNumberList);

            this.UniqueVertexCount =(UInt32)uniqueNumbers.Count;
        }

    }
}
