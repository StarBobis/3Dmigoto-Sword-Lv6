using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSMT_Core;
using Windows.Globalization;

namespace Sword.Configs
{

    public static class GlobalConfig
    {
        public static string SwordVersion { get; set; } = "V1.1.3";

        public static string ReverseOutputFolder { get; set; } = "";
        public static bool Theme { get; set; } = true;
        public static bool Chinese { get; set; } = true;

        //窗口大小
        public static double WindowWidth { get; set; } = 1280;
        public static double WindowHeight { get; set; } = 720;
        public static double WindowLuminosityOpacity { get; set; } = 0.65f;

        // New properties
        public static string TextureConversionFormat { get; set; } = "jpg";
        public static int PostReverseAction { get; set; } = 0; // Changed to int to store SelectedIndex
        public static bool ConvertOriginalTextures { get; set; } = true;
        public static bool ConvertTexturesToOutputFolder { get; set; } = true;

        public static string AutoReverseGameName { get; set; } = "GI";
        public static string WWMIReverseStyle { get; set; } = "WWMI";


        /// <summary>
        /// 使用古法读取，不要自作聪明用C#的某些语法糖特性实现全自动
        /// </summary>
        public static void ReadConfig()
        {
            try
            {
                //读取配置时优先读取全局的
                JObject SettingsJsonObject = DBMTJsonUtils.CreateJObject();

                if (File.Exists(PathManager.Path_MainConfig_Global))
                {
                    string json = File.ReadAllText(PathManager.Path_MainConfig_Global);
                    SettingsJsonObject = JObject.Parse(json);
                }
              
                //WindowWidth
                if (SettingsJsonObject.ContainsKey("WindowWidth"))
                {
                    WindowWidth = (double)SettingsJsonObject["WindowWidth"];
                }

                //WindowHeight
                if (SettingsJsonObject.ContainsKey("WindowHeight"))
                {
                    WindowHeight = (double)SettingsJsonObject["WindowHeight"];
                }

                //WindowLuminosityOpacity
                if (SettingsJsonObject.ContainsKey("WindowLuminosityOpacity"))
                {
                    WindowLuminosityOpacity = (double)SettingsJsonObject["WindowLuminosityOpacity"];
                }

                if (SettingsJsonObject.ContainsKey("Theme"))
                {
                    Theme = (bool)SettingsJsonObject["Theme"];
                }

                if (SettingsJsonObject.ContainsKey("Chinese"))
                {
                    Chinese = (bool)SettingsJsonObject["Chinese"];
                }

                //ReverseOutputFolder
                if (SettingsJsonObject.ContainsKey("ReverseOutputFolder"))
                {
                    ReverseOutputFolder = (string)SettingsJsonObject["ReverseOutputFolder"];
                }

                // TextureConversionFormat
                if (SettingsJsonObject.ContainsKey("TextureConversionFormat"))
                {
                    TextureConversionFormat = (string)SettingsJsonObject["TextureConversionFormat"];
                }

                //AutoReverseGameName
                if (SettingsJsonObject.ContainsKey("AutoReverseGameName"))
                {
                    AutoReverseGameName = (string)SettingsJsonObject["AutoReverseGameName"];
                }

                //WWMIReverseStyle
                if (SettingsJsonObject.ContainsKey("WWMIReverseStyle"))
                {
                    WWMIReverseStyle = (string)SettingsJsonObject["WWMIReverseStyle"];
                }

                // PostReverseAction
                if (SettingsJsonObject.ContainsKey("PostReverseAction"))
                {
                    PostReverseAction = (int)SettingsJsonObject["PostReverseAction"];
                }

                // ConvertOriginalTextures
                if (SettingsJsonObject.ContainsKey("ConvertOriginalTextures"))
                {
                    ConvertOriginalTextures = (bool)SettingsJsonObject["ConvertOriginalTextures"];
                }

                // ConvertTexturesToOutputFolder
                if (SettingsJsonObject.ContainsKey("ConvertTexturesToOutputFolder"))
                {
                    ConvertTexturesToOutputFolder = (bool)SettingsJsonObject["ConvertTexturesToOutputFolder"];
                }

            }
            catch (Exception ex) {
                ex.ToString();
            }
        }

        /// <summary>
        /// 使用古法保存，不要自作聪明用C#的某些语法糖特性实现全自动
        /// </summary>
        public static void SaveConfig()
        {
            //古法保存
            JObject SettingsJsonObject = new JObject();

            SettingsJsonObject["WindowWidth"] = WindowWidth;
            SettingsJsonObject["WindowHeight"] = WindowHeight;
            SettingsJsonObject["WindowLuminosityOpacity"] = WindowLuminosityOpacity;
            SettingsJsonObject["Theme"] = Theme;
            SettingsJsonObject["Chinese"] = Chinese;
            SettingsJsonObject["ReverseOutputFolder"] = ReverseOutputFolder;
            SettingsJsonObject["AutoReverseGameName"] = AutoReverseGameName;
            SettingsJsonObject["WWMIReverseStyle"] = WWMIReverseStyle;

            // New properties
            SettingsJsonObject["TextureConversionFormat"] = TextureConversionFormat;
            SettingsJsonObject["PostReverseAction"] = PostReverseAction;
            SettingsJsonObject["ConvertOriginalTextures"] = ConvertOriginalTextures;
            SettingsJsonObject["ConvertTexturesToOutputFolder"] = ConvertTexturesToOutputFolder;

            //写出内容
            string WirteStirng = SettingsJsonObject.ToString();

            //保存配置时，全局配置也顺便保存一份
            File.WriteAllText(PathManager.Path_MainConfig_Global, WirteStirng);
        }



    }
}
