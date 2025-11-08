using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMT
{
    public class D3D11Element
    {
        [JsonProperty("SemanticName")]
        public string SemanticName { get; set; } = "";

        [JsonProperty("Format")]
        public string Format { get; set; } = "";

        [JsonProperty("ExtractSlot")]
        public string ExtractSlot { get; set; } = "";

        [JsonProperty("ExtractTechnique")]
        public string ExtractTechnique { get; set; } = "";

        [JsonProperty("Category")]
        public string Category { get; set; } = "";

        [JsonProperty("DrawCategory")]
        public string DrawCategory { get; set; } = "";

        /// <summary>
        /// 不能根据Format来动态ByteWidth计算长度
        /// 为了能够兼容新架构，必须手动指定长度
        /// </summary>
        [JsonProperty("ByteWidth")]
        public string ByteWidth { get; set; } = "";

        [JsonIgnore]
        public int ByteWidthInt { 
            get {

                // 尝试解析 ByteWidth 字符串为整数
                if (int.TryParse(ByteWidth, out int result))
                {
                    return result;
                }
                // 如果解析失败，返回 0 或其他默认值，也可抛出异常或记录日志
                return 0;
            } 
        }

        //public bool GPUPreSkinning { get; set; } = false;

        /// <summary>
        /// 根据在列表中出现的次数和顺序来赋值。
        /// </summary>
        [JsonIgnore]
        public int SemanticIndex { get; set; } = 0;



        [JsonIgnore]
        public string ElementName { get; set; } = "";

    }
}
