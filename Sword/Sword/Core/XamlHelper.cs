using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI3Helper
{
    public class XamlHelper
    {

        // 辅助方法：查找特定类型的父元素
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is T parentOfType)
                    return parentOfType;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

    }
}
