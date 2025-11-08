using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace SSMT
{
    public class ClipboardImageHelper
    {
        /// <summary>
        /// 检查剪贴板中是否有图片
        /// </summary>
        public static bool HasImageAsync()
        {
            try
            {
                var dataPackageView = Clipboard.GetContent();
                return dataPackageView.Contains(StandardDataFormats.Bitmap);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 保存剪贴板图片到文件并返回BitmapImage
        /// </summary>
        public static async Task<bool> SaveClipboardImageToFileAsync(string filePath)
        {
            var dataPackageView = Clipboard.GetContent();

            // 检查剪贴板是否有图片
            if (!dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                return false;
            }

            // 获取图片流
            var imageStream = await dataPackageView.GetBitmapAsync();
            var streamWithContent = await imageStream.OpenReadAsync();

            // 确保目录存在
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }


            // 创建或替换文件
            var folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);

            // 创建文件
            var file = await folder.CreateFileAsync(
                Path.GetFileName(filePath),
                CreationCollisionOption.ReplaceExisting);

            // 编码为PNG并保存
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var decoder = await BitmapDecoder.CreateAsync(streamWithContent);
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                var pixelData = await decoder.GetPixelDataAsync();
                encoder.SetPixelData(
                    decoder.BitmapPixelFormat,
                    decoder.BitmapAlphaMode,
                    decoder.PixelWidth,
                    decoder.PixelHeight,
                    decoder.DpiX,
                    decoder.DpiY,
                    pixelData.DetachPixelData()
                );

                await encoder.FlushAsync();
            }

            return true;
        }

        /// <summary>
        /// 从文件路径加载BitmapImage
        /// </summary>
        private static async Task<BitmapImage> LoadImageFromFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return null;

                var file = await StorageFile.GetFileFromPathAsync(filePath);

                using (var stream = await file.OpenReadAsync())
                {
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(stream);
                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载图片失败: {ex.Message}");
                return null;
            }
        }

    }
}
