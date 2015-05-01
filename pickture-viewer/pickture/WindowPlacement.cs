using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace pickture
{
    public static class WindowPlacement
    {
        public static bool Load(Window window)
        {
            var first_time_window = Properties.Settings.Default.Left < 0 || Properties.Settings.Default.Top < 0 || Properties.Settings.Default.Width < 0 || Properties.Settings.Default.Height < 0;

            window.Left = Properties.Settings.Default.Left;
            window.Top = Properties.Settings.Default.Top;

            window.Width = Properties.Settings.Default.Width;
            window.Height = Properties.Settings.Default.Height;

            return first_time_window;
        }

        public static void Save(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Left = window.RestoreBounds.Left;
                Properties.Settings.Default.Top = window.RestoreBounds.Top;

                Properties.Settings.Default.Width = window.RestoreBounds.Width;
                Properties.Settings.Default.Height = window.RestoreBounds.Height;

                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Left = window.Left;
                Properties.Settings.Default.Top = window.Top;

                Properties.Settings.Default.Width = window.Width;
                Properties.Settings.Default.Height = window.Height;

                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();
        }
    }
}
