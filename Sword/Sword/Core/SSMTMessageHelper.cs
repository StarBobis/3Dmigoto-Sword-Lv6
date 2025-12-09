using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using WinRT.Interop;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using SSMT_Core;
using WinUI3Helper;
using Sword;
using Sword.Configs;

namespace SSMT
{
    
    public static class SSMTMessageHelper
    {

        public static async Task<bool> ShowConfirm(string ContentChinese, string ContentEnglish = "")
        {
            try
            {
                string TipContent = ContentChinese;
                ContentDialog subscribeDialog = new ContentDialog
                {
                    Title = "Tips",
                    Content = TipContent,
                    PrimaryButtonText = "OK", // 更改为确认
                    CloseButtonText = "Cancel", // 添加取消按钮
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = App._window.Content.XamlRoot // 确保设置 XamlRoot
                };

                ContentDialogResult result = await subscribeDialog.ShowAsync();

                // 根据用户点击的按钮返回相应的结果
                return result == ContentDialogResult.Primary; // 如果点击的是确认按钮，则返回true；否则返回false
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }


        public static async Task<bool> Show(string ContentChinese,string ContentEnglish="")
        {
            try
            {
                string TipContent = ContentChinese;

                if (!GlobalConfig.Chinese && ContentEnglish != "")
                {
                    TipContent = ContentEnglish;
                }
                LOG.Info("Show::" + TipContent);


                bool result = await MessageHelper.Show(App._window.Content.XamlRoot, TipContent);
                
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }

        }

    }
}
