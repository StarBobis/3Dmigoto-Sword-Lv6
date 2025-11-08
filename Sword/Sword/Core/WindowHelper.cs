using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics;

namespace WinUI3Helper
{
    public class WindowHelper
    {
        public static void SetTheme(Window window, ElementTheme theme)
        {
            if (window.Content is FrameworkElement root)
            {
                root.RequestedTheme = theme;
            }
        }

        public static void SetWindowSizeWithNavigationView(AppWindow appWindow,int Width,int Height)
        {
            appWindow.Resize(new SizeInt32(Width, Height));
        }

        public static void MoveWindowToCenter(AppWindow appWindow)
        {
            // 获取与窗口关联的DisplayArea
            var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Nearest);
            // 获取窗口当前的尺寸
            var windowSize = appWindow.Size;

            // 确保我们获取的是正确的显示器信息
            if (displayArea != null)
            {
                // 计算窗口居中所需的左上角坐标，考虑显示器的实际工作区（排除任务栏等）
                int x = (int)(displayArea.WorkArea.X + (displayArea.WorkArea.Width - windowSize.Width) / 2);
                int y = (int)(displayArea.WorkArea.Y + (displayArea.WorkArea.Height - windowSize.Height) / 2);

                // 设置窗口位置
                appWindow.Move(new PointInt32 { X = x, Y = y });
            }

            int window_pos_x = 0;
            int window_pos_y = 0;

            if (displayArea != null)
            {
                window_pos_x = (int)(displayArea.WorkArea.X + (displayArea.WorkArea.Width - windowSize.Width) / 2);
                window_pos_y = (int)(displayArea.WorkArea.Y + (displayArea.WorkArea.Height - windowSize.Height) / 2);
            }

            if (window_pos_x != -1 && window_pos_y != -1)
            {
                appWindow.Move(new PointInt32(window_pos_x, window_pos_y));
            }
        }
    }
}
