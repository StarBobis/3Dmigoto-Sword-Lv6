using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMT_Core
{
    public class PathManager
    {
       
        public static string Name_Texconv = "texconv.exe";
        public static string Name_GlobalConfigFileName = "Sword-Config.json";

        public static string Path_TexconvExe
        {
            get
            {
                return Path.Combine(GlobalConfig.Path_AssetsFolder, Name_Texconv);
            }
        }

       

    }
}
