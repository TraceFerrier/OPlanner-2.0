using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PlannerNameSpace
{
    public class MenuUtils
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a MenuItem to the given menu, using the specified parameters.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void AddContextMenuItem(ContextMenu menu, string title, string imageName, RoutedEventHandler handler)
        {
            MenuItem item = new MenuItem();
            item.Header = title;
            item.Click += handler;
            item.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri(@"/images/" + imageName, UriKind.RelativeOrAbsolute))
            };

            menu.Items.Add(item);
        }

        public static void OpenContextMenu(ContextMenu menu)
        {
            if (menu != null)
            {
                FrameworkElement frameworkElement = null;
                IInputElement element = Mouse.DirectlyOver;
                if (element != null && element is FrameworkElement)
                {
                    frameworkElement = (FrameworkElement)element;
                }

                menu.PlacementTarget = frameworkElement;
                menu.IsOpen = true;
            }
        }

    }
}
