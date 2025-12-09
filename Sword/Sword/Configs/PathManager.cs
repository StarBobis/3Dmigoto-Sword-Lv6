using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Configs
{
    public class PathManager
    {
       
        public static string Name_Texconv = "texconv.exe";
        public static string Name_GlobalConfigFileName = "Sword-Config.json";

        public static string Path_TexconvExe
        {
            get
            {
                return Path.Combine(PathManager.Path_AssetsFolder, Name_Texconv);
            }
        }
        
        public static string Path_AssetsFolder
        {
            get { return Path.Combine(PathManager.Path_BaseFolder, "Assets\\"); }
        }

        public static string Path_GameTypeConfigsFolder
        {
            get { return Path.Combine(PathManager.Path_BaseFolder, "GameTypeConfigs\\"); }
        }


        public static string Path_LogsFolder
        {
            get { return Path.Combine(PathManager.Path_BaseFolder, "Logs\\"); }
        }



        public static string Path_LatestDBMTLogFile
        {
            get
            {
                string logsPath = PathManager.Path_LogsFolder;
                if (!Directory.Exists(logsPath))
                {
                    return "";
                }
                string[] logFiles = Directory.GetFiles(logsPath); ;
                List<string> logFileList = new List<string>();
                foreach (string logFile in logFiles)
                {
                    string logfileName = Path.GetFileName(logFile);
                    if (logfileName.EndsWith(".log") && logfileName.Length > 15)
                    {
                        logFileList.Add(logfileName);
                    }
                }

                logFileList.Sort();


                if (logFileList.Count == 0)
                {
                    return "";
                }
                else
                {
                    string LogFilePath = logsPath + "\\" + logFileList[^1];
                    return LogFilePath;
                }
            }
        }

        //// 配置文件路径
        public static string Path_MainConfig
        {
            get { return Path.Combine(Path_ConfigsFolder, PathManager.Name_GlobalConfigFileName); }
        }

        public static string Path_MainConfig_Global
        {
            get { return Path.Combine(Path_AppDataLocal, PathManager.Name_GlobalConfigFileName); }
        }

        public static string Path_AppDataLocal
        {
            get
            { // 如果你需要非漫游配置文件路径（AppData\Local），可以这样做：
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return localAppDataPath;
            }
        }

        public static string Path_SwordGlobalConfigsFolder
        {
            get { return Path.Combine(PathManager.Path_AppDataLocal, "SwordGlobalConfigs\\"); }
        }

        //Path.Combine(PathManager.Path_ConfigsFolder, "ManuallyReversePageConfig.json");

        public static string Path_ManuallyReversePageConfig 
        {
            get { return Path.Combine(PathManager.Path_SwordGlobalConfigsFolder, "ManuallyReversePageConfig.json"); }
        }

        public static string Path_ConfigsFolder
        {
            get { return Path.Combine(PathManager.Path_BaseFolder, "Configs\\"); }
        }


        public static string Path_ModManageConfig
        {
            get { return Path.Combine(PathManager.Path_ConfigsFolder, "ModManageConfig.json"); }
        }

        public static string Path_TexturePageIndexConfig
        {
            get { return Path.Combine(PathManager.Path_ConfigsFolder, "TexturePageIndexConfig.json"); }
        }


        public static string Path_SwordLv5Exe
        {
            get { return Path.Combine(PathManager.Path_AssetsFolder, "3Dmigoto-Sword-Lv5.exe"); }
        }

        public static string Path_EncryptorExe
        {
            get { return Path.Combine(PathManager.Path_AssetsFolder, "DBMT-Encryptor.exe"); }
        }

        public static string Path_ProtectExe
        {
            get { return Path.Combine(PathManager.Path_AssetsFolder, "DBMT-Protect.exe"); }
        }
        public static string Path_RunResultJson
        {
            get { return Path.Combine(Path_ConfigsFolder, "RunResult.json"); }
        }

        public static string Path_RunInputJson
        {
            get { return Path.Combine(Path_ConfigsFolder, "RunInput.json"); }
        }

        public static string Path_ReversedFolder
        {
            get { return Path.Combine(PathManager.Path_BaseFolder, "Reversed\\"); }
        }

        public static string Path_BaseFolder
        {
            get
            {
                return AppContext.BaseDirectory;
            }
        }


    }
}
