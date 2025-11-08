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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sword
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        bool ReadOver = false;


        public SettingsPage()
        {
            InitializeComponent();

            HyperlinkButton_SSMTVersion.Content ="3Dmigoto-Sword-Lv6 " +  GlobalConfig.SwordVersion;

            try
            {
                ReadSettingsFromConfig();
            }
            catch (Exception ex)
            {
                _ = SSMTMessageHelper.Show("Error: " + ex.ToString());
            }

        }


        public void SaveSettingsToConfig()
        {
            try
            {
                Debug.WriteLine("保存配置");


                GlobalConfig.WindowLuminosityOpacity = Slider_LuminosityOpacity.Value;

                GlobalConfig.Chinese = ToggleSwitch_Chinese.IsOn;
                GlobalConfig.Theme = ToggleSwitch_Theme.IsOn;


                GlobalConfig.SaveConfig();
            }
            catch (Exception ex)
            {
                _ = SSMTMessageHelper.Show(ex.ToString());
            }

        }

        public void ReadSettingsFromConfig()
        {
            ReadOver = false;
            //防止程序启动时没正确读取，这里冗余读取一次，后面看情况可以去掉
            GlobalConfig.ReadConfig();

            Slider_LuminosityOpacity.Value = GlobalConfig.WindowLuminosityOpacity;

            ToggleSwitch_Theme.IsOn = GlobalConfig.Theme;
            ToggleSwitch_Chinese.IsOn = GlobalConfig.Chinese;


            ReadOver = true;
        }


       


        /// <summary>
        /// 任何设置项被改变后，都应该立刻调用这个方法，否则无法同步状态。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshSettings(object sender, RoutedEventArgs e)
        {
            if (ReadOver)
            {
                SaveSettingsToConfig();
            }
        }


        private void ToggleSwitch_Theme_Toggled(object sender, RoutedEventArgs e)
        {
            if (ReadOver)
            {
                if (ToggleSwitch_Theme.IsOn)
                {
                    if (this.XamlRoot?.Content is FrameworkElement root)
                    {
                        root.RequestedTheme = ElementTheme.Dark;
                        MainWindow.CurrentWindow._controller.TintColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);
                    }
                }
                else
                {
                    if (this.XamlRoot?.Content is FrameworkElement root)
                    {
                        root.RequestedTheme = ElementTheme.Light;
                        MainWindow.CurrentWindow._controller.TintColor = Windows.UI.Color.FromArgb(255, 245, 245, 245);
                    }

                }
                SaveSettingsToConfig();
            }
        }

        private void ToggleSwitch_Chinese_Toggled(object sender, RoutedEventArgs e)
        {
            if (ReadOver)
            {
                SaveSettingsToConfig();
                //Frame.Navigate(typeof(SettingsPage));
                TranslatePage();
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // ✅ 每次进入页面都会执行，适合刷新 UI
            // 因为开启了缓存模式之后，是无法刷新页面语言的，只能在这里执行来刷新
            TranslatePage();
        }

        public void TranslatePage()
        {

            if (GlobalConfig.Chinese)
            {
                
                TextBlock_Theme.Text = "主题颜色";
                ToggleSwitch_Theme.OnContent = "曜石黑";
                ToggleSwitch_Theme.OffContent = "晨曦白";

                TextBlock_Language.Text = "语言";
                ToggleSwitch_Chinese.OnContent = "简体中文";
                ToggleSwitch_Chinese.OffContent = "英语";

                TextBlock_About.Text = "关于";


                TextBlock_Help.Text = "帮助";
                HyperlinkButton_SSMTDocuments.Content = "SSMT使用文档";
                HyperlinkButton_SSMTPluginTheHerta.Content = "SSMT的Blender插件TheHerta";

                Run_SponsorSupport.Text = "赞助支持";
                HyperlinkButton_SSMTTechCommunity.Content = "SSMT技术社群";
                HyperlinkButton_AFDianNicoMico.Content = "爱发电:NicoMico";

              

                TextBlock_WindowOpacitySetting.Text = "窗口透明度调整";
                Slider_LuminosityOpacity.Header = "透光度";

                

            }
            else
            {
                

                TextBlock_Theme.Text = "Theme Color";

                ToggleSwitch_Theme.OnContent = "Dark";
                ToggleSwitch_Theme.OffContent = "Light";

                TextBlock_Language.Text = "Language";
                ToggleSwitch_Chinese.OnContent = "Chinese(zh-CN)";
                ToggleSwitch_Chinese.OffContent = "English(en-US)";

        

                TextBlock_About.Text = "About";


                TextBlock_Help.Text = "Help";
                HyperlinkButton_SSMTDocuments.Content = "SSMT Documents";
                HyperlinkButton_SSMTPluginTheHerta.Content = "SSMT's Blender Plugin: TheHerta";

                Run_SponsorSupport.Text = "Sponsor Support";
                HyperlinkButton_SSMTTechCommunity.Content = "SSMT Tech Community";
                HyperlinkButton_AFDianNicoMico.Content = "afdian: NicoMico";

        

                TextBlock_WindowOpacitySetting.Text = "Window Opacity";
                Slider_LuminosityOpacity.Header = "Luminosity Opacity";


            }

        }


        private void Slider_LuminosityOpacity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (ReadOver)
            {
                MainWindow.CurrentWindow._controller.LuminosityOpacity = (float)Slider_LuminosityOpacity.Value;
            }
        }






        private void Slider_LuminosityOpacity_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveSettingsToConfig();

        }



    }
}
