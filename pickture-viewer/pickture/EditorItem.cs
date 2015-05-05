using pickture.Utilities.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace pickture
{
    public class EditorItemEx
    {
        public static void ActivateItem(FrameworkElement element)
        {
            element.PreviewMouseDown -= element_PreviewMouseDown;
            element.PreviewMouseDown += element_PreviewMouseDown;
        }

        static void element_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;
            
            if (fe == null)
                return;

            IEditorItem item = fe.DataContext as IEditorItem;

            if (item == null)
                return;

            var host = Find.Parent<IEditorHost>(fe);

            if (host != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    item.IsSelected = !item.IsSelected;
                }
                else
                {
                    if (!item.IsSelected)
                    {
                        host.Editor.DeselectAll();
                        item.IsSelected = true;
                    }
                }
            }

            e.Handled = false;
        }
    }
}
