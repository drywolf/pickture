using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace pickture.Utilities.WPF
{
    public static class Find
    {
        public static T Parent<T>(DependencyObject child)
            where T : class
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
            {
                return null;
            }
            if (parent is T)
            {
                return (T)(object)parent;
            }

            return Parent<T>((DependencyObject)(object)parent);
        }

        public static T VisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = VisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        public static List<T> VisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            List<T> list = new List<T>();
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        list.Add((T)child);
                    }

                    List<T> childItems = VisualChildren<T>(child);
                    if (childItems != null && childItems.Count() > 0)
                    {
                        foreach (var item in childItems)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }
    }
}
