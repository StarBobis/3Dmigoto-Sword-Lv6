using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Sword.Configs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SSMT
{
    public class SSMTResourceUtils
    {
        public static void InitializeWorkFolder(bool OverwriteMigotoFiles = false)
        {
           

        }
        



        


        public static bool ExistsPluginReverse()
        {
            if (File.Exists(PathManager.Path_SwordLv5Exe))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ExistsPluginEncryption()
        {
            if (File.Exists(PathManager.Path_EncryptorExe))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ExistsPluginProtect()
        {
            if (File.Exists(PathManager.Path_ProtectExe))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
