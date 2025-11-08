using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SSMT_Core
{
    public class DBMTFileUtils
    {

        /// <summary>
        /// 计算指定文件的SHA-256哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>SHA-256哈希值（小写十六进制字符串）</returns>
        public static string ComputeFileSHA256(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "";

            if (!File.Exists(filePath))
                return "";

            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// 检查当前应用程序是否以管理员权限运行。
        /// </summary>
        /// <returns>如果是管理员权限，则返回true；否则返回false。</returns>
        public static bool IsRunAsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


        public static long GetFileSize(string filePath)
        {
            //TODO 开启Symlink特性后，这里获取的文件大小，确定是真正的文件大小吗？需要进行测试。

            if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }

            return 0;
        }

        /// <summary>
        /// 如非必要尽可能不要使用这个。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [Obsolete("Blend无法准确判断，如果有顶点索引是10 00 00 00 则顶点数会少一位")]
        public static ulong GetRealFileSize_NullTerminated(string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath);

            long end = data.LongLength - 1;
            while (end >= 0 && data[end] == 0)
            {
                end--;
            }

            ulong length = (ulong)(end + 1);
            return length;
        }

        /// <summary>
        /// 获取文件中所有有效的字节数据（去掉末尾连续的全为00的块）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="stride">每次读取的字节数</param>
        /// <returns>有效字节数据数组</returns>
        public static byte[] GetValidFileData_NullTerminated_ByStride(string filePath, int stride)
        {
            if (stride <= 0)
                throw new ArgumentException("Stride must be a positive integer.", nameof(stride));

            long fileSize = new FileInfo(filePath).Length;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                List<byte> buffer = new List<byte>();
                long position = fileSize;

                while (position > 0)
                {
                    long readSize = Math.Min(stride, position);
                    position -= readSize;

                    byte[] chunk = new byte[readSize];
                    fs.Seek(position, SeekOrigin.Begin);
                    fs.Read(chunk, 0, (int)readSize);

                    // 判断是否全是 0
                    bool isAllZero = true;
                    foreach (byte b in chunk)
                    {
                        if (b != 0)
                        {
                            isAllZero = false;
                            break;
                        }
                    }

                    if (!isAllZero)
                    {
                        // 如果不是全0，则保留这部分及其之前的所有内容
                        buffer.InsertRange(0, chunk);
                        break;
                    }
                    else
                    {
                        // 如果是全0，则继续往前找
                        continue;
                    }
                }

                // 将剩余部分全部读取出来
                if (position > 0)
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    byte[] remaining = new byte[position];
                    fs.Read(remaining, 0, remaining.Length);
                    buffer.InsertRange(0, remaining);
                }

                return buffer.ToArray();
            }
        }

        /// <summary>
        /// 获取文件中有效数据的大小（即去除末尾连续的全为00的数据后的长度）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="stride">每次读取的字节数</param>
        /// <returns>有效数据大小</returns>
        public static ulong GetRealFileSize_NullTerminated_ByStride(string filePath, int stride)
        {
            byte[] validData = GetValidFileData_NullTerminated_ByStride(filePath, stride);
            return (ulong)validData.Length;
        }
        public static string[] ReadWorkSpaceNameList(string WorkSpaceFolderPath)
        {
            List<string> WorkSpaceNameList = new List<string>();

            if (Directory.Exists(WorkSpaceFolderPath))
            {
                string[] WorkSpaceList = Directory.GetDirectories(WorkSpaceFolderPath);
                foreach (string WorkSpacePath in WorkSpaceList)
                {
                    string WorkSpaceName = Path.GetFileName(WorkSpacePath);
                    WorkSpaceNameList.Add(WorkSpaceName);
                }
            }

            return WorkSpaceNameList.ToArray();
        }


        private static void SearchDirectory(string currentDirectory, List<string> result)
        {
            // 获取当前目录下的所有文件
            string[] files = Directory.GetFiles(currentDirectory);

            // 检查是否有.dds或.png文件
            foreach (string file in files)
            {
                string extension = Path.GetExtension(file).ToLower();
                if (extension == ".dds" || extension == ".png")
                {
                    // 如果找到了目标文件，将目录加入结果列表
                    if (!result.Contains(currentDirectory))
                    {
                        result.Add(currentDirectory);
                    }
                    break; // 找到了一种类型的文件就不再继续查找当前目录中的其他文件
                }
            }

            // 递归搜索子目录
            string[] subdirectories = Directory.GetDirectories(currentDirectory);
            foreach (string subdirectory in subdirectories)
            {
                SearchDirectory(subdirectory, result);
            }

        }


        public static List<string> FindDirectoriesWithImages(string rootDirectory)
        {
            if (string.IsNullOrEmpty(rootDirectory) || !Directory.Exists(rootDirectory))
            {
                throw new ArgumentException("The specified directory does not exist or is invalid.", nameof(rootDirectory));
            }

            var directoriesWithImages = new List<string>();
            SearchDirectory(rootDirectory, directoriesWithImages);
            return directoriesWithImages;
        }

        public static List<string> GetFileNameListInFolder(string rootDirectory)
        {

            List<string> FileNameList = new List<string>();
            string[] FrameAnalysisFileNameArray = Directory.GetFiles(rootDirectory);
            foreach (string FrameAnalysisFileName in FrameAnalysisFileNameArray)
            {
                FileNameList.Add(Path.GetFileName(FrameAnalysisFileName));
            }
            return FileNameList;
        }

        public static bool CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = true,bool overwriteFiles = true)
        {
            // 检查源目录是否存在
            if (!Directory.Exists(sourceDirName))
            {
                return false;
            }

            // 如果目标目录不存在，则创建它
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // 获取源目录中的所有文件并复制它们
            var files = Directory.GetFiles(sourceDirName);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(destDirName, fileName);
                if (!overwriteFiles)
                {
                    if (!File.Exists(destFile))
                    {
                        File.Copy(file, destFile, overwriteFiles); // 第三个参数为true表示覆盖已存在的文件

                    }
                }
                else
                {
                    File.Copy(file, destFile, overwriteFiles); // 第三个参数为true表示覆盖已存在的文件
                }
            }

            // 如果需要复制子目录，则递归处理
            if (copySubDirs)
            {
                var directories = Directory.GetDirectories(sourceDirName);
                foreach (var subdir in directories)
                {
                    var dirName = Path.GetFileName(subdir);
                    var destSubDir = Path.Combine(destDirName, dirName);
                    CopyDirectory(subdir, destSubDir, copySubDirs);
                }
            }

            return true;
        }



        /// <summary>
        /// Finds the value of a specified attribute in a .ini file.
        /// </summary>
        /// <param name="filePath">The path to the .ini file.</param>
        /// <param name="attributeName">The name of the attribute to find.</param>
        /// <returns>The value of the attribute if found, otherwise an empty string.</returns>
        public static string FindMigotoIniAttributeInFile(string filePath, string attributeName)
        {
            string attributeValue = "";

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Trim leading and trailing whitespace
                        line = line.Trim();

                        // Check if the line contains the attribute name followed by a colon
                        if (line.StartsWith(attributeName + ":", StringComparison.OrdinalIgnoreCase))
                        {
                            int pos = line.IndexOf(':');
                            if (pos >= 0)
                            {
                                // Extract the substring after the colon and trim it
                                string var = line.Substring(pos + 1).Trim();
                                attributeValue = var;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions such as file not found or I/O errors
                Console.WriteLine($"Error reading file: {ex.Message}");
                throw;
            }

            return attributeValue;
        }

        public static void CleanLogFiles(string Path_LogsFolder,int FrameAnalysisFolderReserveNumber)
        {
            string logsPath = Path_LogsFolder;

            if (!Directory.Exists(logsPath))
            {
                return;
            }

            //移除log文件
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

            if (logFileList.Count == 0)
            {
                return;
            }

            logFileList.Sort();
            //int n = MainConfig.GetConfig<int>(MainConfig.ConfigFiles.Game_Setting,"FrameAnalysisFolderReserveNumber"); // 你想移除的元素数量
            int n = FrameAnalysisFolderReserveNumber; // 你想移除的元素数量
            if (n > 0 && logFileList.Count > n)
            {
                logFileList.RemoveRange(logFileList.Count - n, n);

            }
            else if (logFileList.Count <= n)
            {
                // 如果 n 大于等于列表的长度，就清空整个列表
                logFileList.Clear();
            }
            if (logFileList.Count > 0)
            {
                foreach (string logfileName in logFileList)
                {
                    File.Delete(logsPath + "\\" + logfileName);

                    //移动到回收站有时无法生效
                    //FileSystem.DeleteFile();
                    //Directory.Delete(latestFrameAnalysisFolder, true);
                }
            }
        }


        public static void CleanFrameAnalysisFiles(string Path_3DmigotoLoaderFolder,int FrameAnalysisFolderReserveNumber)
        {
            if (!Directory.Exists(Path_3DmigotoLoaderFolder))
            {
                return;
            }

            string[] directories = Directory.GetDirectories(Path_3DmigotoLoaderFolder);

            List<string> frameAnalysisFileList = new List<string>();
            foreach (string directory in directories)
            {
                string directoryName = Path.GetFileName(directory);

                if (directoryName.StartsWith("FrameAnalysis-"))
                {
                    frameAnalysisFileList.Add(directoryName);
                }
            }

            if (frameAnalysisFileList.Count == 0)
            {
                return;
            }

            //Get FA numbers to reserve
            frameAnalysisFileList.Sort();

            //int n = MainConfig.FrameAnalysisFolderReserveNumber; // 你想移除的元素数量
            //int n = MainConfig.GetConfig<int>(MainConfig.ConfigFiles.Game_Setting, "FrameAnalysisFolderReserveNumber"); // 你想移除的元素数量
            int n = FrameAnalysisFolderReserveNumber; // 你想移除的元素数量

            if (n > 0 && frameAnalysisFileList.Count > n)
            {
                frameAnalysisFileList.RemoveRange(frameAnalysisFileList.Count - n, n);

            }
            else if (frameAnalysisFileList.Count <= n)
            {
                // 如果 n 大于等于列表的长度，就清空整个列表
                frameAnalysisFileList.Clear();
            }
            if (frameAnalysisFileList.Count > 0)
            {
                foreach (string directoryName in frameAnalysisFileList)
                {
                    string latestFrameAnalysisFolder = Path.Combine(Path_3DmigotoLoaderFolder, directoryName);
                    if (Directory.Exists(latestFrameAnalysisFolder))
                    {
                        //移动到回收站不好用，所以换成下面得了
                        //FileSystem.DeleteDirectory(latestFrameAnalysisFolder, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                        try
                        {
                            Directory.Delete(latestFrameAnalysisFolder, true);
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }


                }
            }

        }

    }
}
