using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SSMT_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword
{
    public partial class AutoReversePage
    {

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
                TextBlock_AutoReverse.Text = "一键逆向";

                SettingsCard_GameName.Header = "游戏名称";
                SettingsCard_GameName.Description = "选择你要逆向的Mod所属的游戏名称，一般是首字母简写或常用代号";

                SettingsCard_WWMIReverseStyle.Header = "鸣潮Mod逆向的风格";
                SettingsCard_WWMIReverseStyle.Description = "选择WWMI则逆向出来的模型命名为WWMI-Tools风格，适合WWMI-Tools工作流，选择SSMT则逆向出来的模型命名为SSMT风格，适合使用SSMT + TheHerta3的工作流";

                Button_ReverseSingleIni.Content = "一键逆向Mod的ini";
                Button_ReverseBufferBasedToggleIni.Content = "一键逆向基于DrawIndexed的分支Mod的ini(常用)";
                Button_ReverseDrawIndexedBasedToggleIni.Content = "一键逆向基于Buffer的分支Mod的ini(很少用)";

            }
            else
            {
                TextBlock_AutoReverse.Text = "Auto Reverse";

                SettingsCard_GameName.Header = "Game Name";
                SettingsCard_GameName.Description = "Select the game name to which the Mod you want to reverse belongs, usually the initials or common code name.";

                SettingsCard_WWMIReverseStyle.Header = "WWMI Mod Reverse Style";
                SettingsCard_WWMIReverseStyle.Description = "Select WWMI to reverse the model naming in WWMI-Tools style, suitable for WWMI-Tools workflow. Select SSMT to reverse the model naming in SSMT style, suitable for SSMT + TheHerta3 workflow.";

                Button_ReverseSingleIni.Content = "Reverse Single Mod's ini";
                Button_ReverseBufferBasedToggleIni.Content = "Reverse DrawIndexed Based Toggle Mod's ini";
                Button_ReverseDrawIndexedBasedToggleIni.Content = "Reverse Buffer Based Toggle Mod's ini";

            }
        }
    }
}
