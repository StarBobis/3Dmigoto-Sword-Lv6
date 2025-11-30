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

                // Save new settings
                if (ComboBox_PostReverseAction.SelectedIndex >=0)
                {
                    GlobalConfig.PostReverseAction = ComboBox_PostReverseAction.SelectedIndex;
                }

                if (ComboBox_TextureConversionFormat.SelectedItem is ComboBoxItem selectedTextureFormat)
                {
                    GlobalConfig.TextureConversionFormat = selectedTextureFormat.Content.ToString();
                }

                GlobalConfig.ConvertOriginalTextures = ToggleSwitch_ConvertOriginalTextures.IsOn;
                GlobalConfig.ConvertTexturesToOutputFolder = ToggleSwitch_ConvertTexturesToOutputFolder.IsOn;

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

            // Read new settings
            if (GlobalConfig.PostReverseAction >=0 && GlobalConfig.PostReverseAction < ComboBox_PostReverseAction.Items.Count)
            {
                ComboBox_PostReverseAction.SelectedIndex = GlobalConfig.PostReverseAction;
            }

            foreach (ComboBoxItem item in ComboBox_TextureConversionFormat.Items)
            {
                if (item.Content.ToString() == GlobalConfig.TextureConversionFormat)
                {
                    ComboBox_TextureConversionFormat.SelectedItem = item;
                    break;
                }
            }

            ToggleSwitch_ConvertOriginalTextures.IsOn = GlobalConfig.ConvertOriginalTextures;
            ToggleSwitch_ConvertTexturesToOutputFolder.IsOn = GlobalConfig.ConvertTexturesToOutputFolder;

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
                Frame.Navigate(typeof(SettingsPage));
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

                SettingsCard_Language.Header = "语言设置";
                SettingsCard_Language.Description = "切换软件显示语言";

                SettingsCard_Theme.Header = "主题颜色";
                SettingsCard_Theme.Description = "切换软件的主题颜色";

                ToggleSwitch_Theme.OnContent = "曜石黑";
                ToggleSwitch_Theme.OffContent = "晨曦白";

                TextBlock_BasicSettings.Text = "基础设置";
                ToggleSwitch_Chinese.OnContent = "简体中文";
                ToggleSwitch_Chinese.OffContent = "英语";

                TextBlock_About.Text = "关于";


                TextBlock_Help.Text = "帮助";
                HyperlinkButton_SSMTDocuments.Content = "SSMT使用文档";
                HyperlinkButton_SSMTPluginTheHerta.Content = "SSMT的Blender插件TheHerta";

                Run_SponsorSupport.Text = "赞助支持";
                HyperlinkButton_SSMTTechCommunity.Content = "SSMT技术社群";
                HyperlinkButton_AFDianNicoMico.Content = "爱发电:NicoMico";


                SettingsCard_LuminosityOpacity.Header = "透光度";
                SettingsCard_LuminosityOpacity.Description = "设置软件窗口的透光度";

                // New translations
                TextBlock_BehaviourSettings.Text = "行为设置";

                SettingsCard_TextureConversionFormat.Header = "贴图转换格式";
                SettingsCard_TextureConversionFormat.Description = "选择当前逆向后的贴图转换格式";

            
                SettingsCard_PostReverseAction.Header = "逆向完成后操作";
                SettingsCard_PostReverseAction.Description = "选择逆向完成后的操作";

                ComboBoxItem_OpenFolder.Content = "打开逆向好的文件夹";
                ComboBoxItem_ShowDialog.Content = "显示逆向成功对话框";

                SettingsCard_ConvertOriginalTextures.Header = "转换原始贴图";
                SettingsCard_ConvertOriginalTextures.Description = "控制是否转换原始Mod里的贴图文件";
                ToggleSwitch_ConvertOriginalTextures.OnContent = "启用";
                ToggleSwitch_ConvertOriginalTextures.OffContent = "禁用";

                SettingsCard_ConvertTexturesToOutputFolder.Header = "转换贴图到输出文件夹";
                SettingsCard_ConvertTexturesToOutputFolder.Description = "控制是否转换贴图到逆向输出文件夹";
                ToggleSwitch_ConvertTexturesToOutputFolder.OnContent = "启用";
                ToggleSwitch_ConvertTexturesToOutputFolder.OffContent = "禁用";
            }
            else
            {
                SettingsCard_Language.Header = "Language Settings";
                SettingsCard_Language.Description = "Switch the display language of the software";

                SettingsCard_Theme.Header = "Theme Color";
                SettingsCard_Theme.Description = "Switch the theme color of the software";

                SettingsCard_LuminosityOpacity.Header = "Window Opacity";
                SettingsCard_LuminosityOpacity.Description = "Set the opacity of the software window";


                ToggleSwitch_Theme.OnContent = "Dark";
                ToggleSwitch_Theme.OffContent = "Light";

                TextBlock_BasicSettings.Text = "Basic Settings";
                ToggleSwitch_Chinese.OnContent = "Chinese(zh-CN)";
                ToggleSwitch_Chinese.OffContent = "English(en-US)";

        

                TextBlock_About.Text = "About";


                TextBlock_Help.Text = "Help";
                HyperlinkButton_SSMTDocuments.Content = "SSMT Documents";
                HyperlinkButton_SSMTPluginTheHerta.Content = "SSMT's Blender Plugin: TheHerta";

                Run_SponsorSupport.Text = "Sponsor Support";
                HyperlinkButton_SSMTTechCommunity.Content = "SSMT Tech Community";
                HyperlinkButton_AFDianNicoMico.Content = "afdian: NicoMico";

                // New translations
                TextBlock_BehaviourSettings.Text = "Behavior Settings";

                SettingsCard_TextureConversionFormat.Header = "Texture Conversion Format";
                SettingsCard_TextureConversionFormat.Description = "Select the texture conversion format";

                SettingsCard_PostReverseAction.Header = "Post Reverse Action";
                SettingsCard_PostReverseAction.Description = "Select the action after reverse operation";

                ComboBoxItem_OpenFolder.Content = "Open Reversed Folder";
                ComboBoxItem_ShowDialog.Content = "Show Success Dialog";

                SettingsCard_ConvertOriginalTextures.Header = "Convert Original Textures";
                SettingsCard_ConvertOriginalTextures.Description = "Control whether to convert original textures in the mod";
                ToggleSwitch_ConvertOriginalTextures.OnContent = "Enable";
                ToggleSwitch_ConvertOriginalTextures.OffContent = "Disable";

                SettingsCard_ConvertTexturesToOutputFolder.Header = "Convert Textures to Output Folder";
                SettingsCard_ConvertTexturesToOutputFolder.Description = "Control whether to convert textures to the reverse output folder";
                ToggleSwitch_ConvertTexturesToOutputFolder.OnContent = "Enable";
                ToggleSwitch_ConvertTexturesToOutputFolder.OffContent = "Disable";
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

        private void ComboBox_TextureConversionFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReadOver)
            {
                SaveSettingsToConfig();
            }
        }

        private void ComboBox_PostReverseAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReadOver)
            {
                SaveSettingsToConfig();
            }
        }

        private void ToggleSwitch_ConvertOriginalTextures_Toggled(object sender, RoutedEventArgs e)
        {
            if (ReadOver)
            {
                SaveSettingsToConfig();
            }
        }

        private void ToggleSwitch_ConvertTexturesToOutputFolder_Toggled(object sender, RoutedEventArgs e)
        {
            if (ReadOver)
            {
                SaveSettingsToConfig();
            }
        }
    }
}
