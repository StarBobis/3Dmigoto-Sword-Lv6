using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSMT_Core;

namespace SSMT
{
    public static class ProtectConfig
    {
        public static string DBMT_Protect_ACLFolderPath = "";
        public static string DBMT_Protect_TargetModPath = "";

        public static void ReadConfig()
        {
            if (File.Exists(Path_SettingsJson))
            {
                string json = File.ReadAllText(Path_SettingsJson); // 读取文件内容
                JObject SettingsJsonObject = JObject.Parse(json);
                DBMT_Protect_ACLFolderPath = (string)SettingsJsonObject["DBMT_Protect_ACLFolderPath"];
                DBMT_Protect_TargetModPath = (string)SettingsJsonObject["DBMT_Protect_TargetModPath"];
            }
        }

        public static void SaveConfig()
        {
            JObject SettingsJsonObject = new JObject();
            SettingsJsonObject["DBMT_Protect_ACLFolderPath"] = DBMT_Protect_ACLFolderPath;
            SettingsJsonObject["DBMT_Protect_TargetModPath"] = DBMT_Protect_TargetModPath;
            File.WriteAllText(Path_SettingsJson, SettingsJsonObject.ToString());
        }
        public static string Path_Base
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string Path_ConfigsFolder
        {
            get { return Path.Combine(Path_Base, "Configs\\"); }
        }
        public static string Path_PluginsFolder
        {
            get { return Path.Combine(Path_Base, "Plugins\\"); }
        }

        public static string Path_LogsFolder
        {
            get { return Path.Combine(Path_Base, "Logs\\"); }
        }

        public static string Path_ReversedFolder
        {
            get { return Path.Combine(Path_Base, "Reversed\\"); }
        }

        public static string Path_SettingsJson
        {
            get { return Path.Combine(Path_ConfigsFolder, "Settings.json"); }
        }

        public static string Path_ACLSettingJson
        {
            get { return Path.Combine(Path_ConfigsFolder, "ACLSetting.json"); }
        }

        public static string Path_DeviceKeySetting
        {
            get { return Path.Combine(Path_ConfigsFolder, "DeviceKeySetting.json"); }
        }

        public static string Path_GeneratedAESKeyFolder
        {
            get { return Path.Combine(Path_Base, "GeneratedAESKey\\"); }
        }


    }
}
