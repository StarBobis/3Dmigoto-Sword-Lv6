using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SSMT;
using SSMT_Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Web.AtomPub;
using WinUI3Helper;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sword
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public DesktopAcrylicController _controller;

        public static MainWindow CurrentWindow;

        public MainWindow()
        {
            InitializeComponent();

            //确保全局配置文件夹存在
            if (!Directory.Exists(PathManager.Path_SwordGlobalConfigsFolder))
            {
                Directory.CreateDirectory(PathManager.Path_SwordGlobalConfigsFolder);
            }

            CurrentWindow = this;

            this.Title = "3Dmigoto-Swrod-Lv6 " + GlobalConfig.SwordVersion;
            this.ExtendsContentIntoTitleBar = true;
            //设置图标
            this.AppWindow.SetIcon("Assets/NuoKeSaSi.ico");

            // 1. 把窗口变成可以挂系统背景的目标
            var target = this.As<ICompositionSupportsSystemBackdrop>();

            // 2. 创建 Acrylic-Thin 控制器
            _controller = new DesktopAcrylicController()
            {

                Kind = DesktopAcrylicKind.Thin,   // ← 这就是“Thin透明”的关键
                LuminosityOpacity = 0.65f,  //遮挡光线程度
                TintOpacity = 0.1f //moh
            };

            // 3. 挂到窗口并激活
            _controller.AddSystemBackdropTarget(target);
            _controller.SetSystemBackdropConfiguration(new SystemBackdropConfiguration { IsInputActive = true });


            //读取配置文件
            GlobalConfig.ReadConfig();


            if (GlobalConfig.Theme)
            {
                WindowHelper.SetTheme(this, ElementTheme.Dark);
                _controller.TintColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);
            }
            else
            {
                WindowHelper.SetTheme(this, ElementTheme.Light);
                _controller.TintColor = Windows.UI.Color.FromArgb(255, 245, 245, 245);
            }

            _controller.LuminosityOpacity = (float)GlobalConfig.WindowLuminosityOpacity;

            if (!Directory.Exists(PathManager.Path_ConfigsFolder))
            {
                Directory.CreateDirectory(PathManager.Path_ConfigsFolder);
            }

            if (!Directory.Exists(PathManager.Path_LogsFolder))
            {
                Directory.CreateDirectory(PathManager.Path_LogsFolder);
            }

            
                

            double logicalWidth = GlobalConfig.WindowWidth;
            double logicalHeight = GlobalConfig.WindowHeight;

            int actualWidth = (int)(logicalWidth);
            int actualHeight = (int)(logicalHeight);

            if (actualHeight < 720)
            {
                actualHeight = 720;
            }

            if (actualWidth < 1280)
            {
                actualWidth = 1280;
            }

            WindowHelper.SetWindowSizeWithNavigationView(AppWindow, actualWidth, actualHeight);
            WindowHelper.MoveWindowToCenter(AppWindow);


            contentFrame.Navigate(typeof(ManuallyReversePage));
        }

        
        private void Window_Closed(object sender, WindowEventArgs args)
        {
            //保存窗口大小
            int WindowWidth = App._window.AppWindow.Size.Width;
            int WindowHeight = App._window.AppWindow.Size.Height;
            GlobalConfig.WindowWidth = WindowWidth;
            GlobalConfig.WindowHeight = WindowHeight;

            GlobalConfig.SaveConfig();

            //不释放资源就会出现那个0x0000005的内存访问异常
            //但是没有任何文档对此有所说明
            //可恶的WinUI3
            try
            {
                _controller?.RemoveAllSystemBackdropTargets();
                _controller?.Dispose();
                _controller = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Backdrop cleanup failed: {ex}");
            }
        }


        private void nvSample_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
            }
        }

        private void contentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            //此函数的作用是，在Frame每次Navigate调用后，自动设置选中项为当前跳转到的页面。
            //这样就不需要手动设置了

            // 确保返回按钮状态正确更新
            nvSample.IsBackEnabled = contentFrame.CanGoBack;

            // 假设页面类名和 Tag 有对应关系，如 "HomePage" -> "HomePage"
            // 为了让这种方法能够很方便的生效，以后都要符合这种命名约定
            string pageName = contentFrame.SourcePageType.Name;
            string tag = pageName; // 或者根据需要进行转换

            //if (tag != "HomePage")
            //{
            //    MainWindowImageBrush.Source = null;

            //}

            nvSample.SelectedItem = nvSample.MenuItems.OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == tag) ?? null;


        }

        private void nvSample_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // PxncAcd: 添加重复点击设置页则回退到前一页的功能
            if (args.IsSettingsInvoked)
            {
                contentFrame.Navigate(typeof(SettingsPage));
            }
            else if (args.InvokedItemContainer is NavigationViewItem item)
            {
                var pageTag = item.Tag.ToString();
                Type pageType = null;

                switch (pageTag)
                {
                    case "HomePage":
                        pageType = typeof(HomePage);
                        break;
                    case "AutoReversePage":
                        pageType = typeof(AutoReversePage);
                        break;
                    case "ManuallyReversePage":
                        pageType = typeof(ManuallyReversePage);
                        break;
                    case "ProtectPage":
                        pageType = typeof(ProtectPage);
                        break;

                }

                if (pageType != null && contentFrame.Content?.GetType() != pageType)
                {
                    contentFrame.Navigate(pageType);
                }
            }

        }
    }
}
