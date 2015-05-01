using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace pickture
{
    public static class WindowCommands
    {
        public static void Initialize(Window window)
        {
            window.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));

            // Very quick and dirty - but it does the job
            if (Properties.Settings.Default.Maximized)
            {
                window.WindowState = WindowState.Maximized;
            }
        }

        private static void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            var window = (Window)sender;
            e.CanExecute = window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private static void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            var window = (Window)sender;
            e.CanExecute = window.ResizeMode != ResizeMode.NoResize;
        }

        private static void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            var window = (Window)target;
            SystemCommands.CloseWindow(window);
        }

        private static void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            var window = (Window)target;
            SystemCommands.MaximizeWindow(window);
        }

        private static void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            var window = (Window)target;
            SystemCommands.MinimizeWindow(window);
        }

        private static void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            var window = (Window)target;
            SystemCommands.RestoreWindow(window);
        }
    }
}
