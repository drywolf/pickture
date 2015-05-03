using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace pickture.Utilities.WPF
{
    public static class ItemsControlHelper
    {
        public static void OnContainersGenerated<VisualT>(this ItemsControl ic, Action<VisualT> on_visual)
            where VisualT : DependencyObject
        {
            ic.ItemContainerGenerator.StatusChanged += (s, e) =>
            {
                if (ic.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    return;

                var items = ic.Items;

                if (items == null)
                    return;

                foreach (var new_item in items)
                {
                    var presenter = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(new_item);

                    if (presenter == null)
                        continue;

                    presenter.ApplyTemplate();

                    var visual = Find.VisualChild<VisualT>(presenter);

                    if (visual != null)
                        on_visual(visual);
                }
            };
        }

        public static VisualT VisualFromItem<VisualT>(this ItemsControl ic, object item)
            where VisualT : FrameworkElement
        {
            var presenter = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(item);

            if (presenter == null)
                return null;

            presenter.ApplyTemplate();

            var visual = Find.VisualChild<VisualT>(presenter);
            return visual;
        }
    }
}
