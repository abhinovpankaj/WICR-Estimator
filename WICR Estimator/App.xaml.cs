using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WICR_Estimator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent,
            new RoutedEventHandler(TextBox_GotFocus));
            //EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseDownEvent,
            //new RoutedEventHandler(TextBox_GotFocus));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseDownEvent, new MouseButtonEventHandler(TextBox_PreviewMouseDown));
            //EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewTextInputEvent, new RoutedEventHandler(TextBox_PreviewTextInput));
            base.OnStartup(e);
        }

        private void TextBox_PreviewTextInput(object sender, RoutedEventArgs e)
        {
            TextCompositionEventArgs textArgs = e as TextCompositionEventArgs;
            if (textArgs!=null)
            {
                int result=0;
                bool success=Int32.TryParse(textArgs.Text, out result);
                //Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
                //textArgs.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, textArgs.Text));
                if (!success || textArgs.Text == ".")
                {
                    e.Handled = true;
                }
            }
            
        }
    
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var TextBox = (TextBox)sender;
            if (!TextBox.IsKeyboardFocusWithin)
            {
                TextBox.Focus();
                e.Handled = true;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //MessageBox.Show(e.Args[0]);
            if (e.Args.Length == 1) //make sure an argument is passed
            {
                //MessageBox.Show(e.Args[0]);
                FileInfo file = new FileInfo(e.Args[0]);
                if (file.Exists) //make sure it's actually a file
                {
                    ViewModels.HomeViewModel.LoadedFile = file.FullName;
                }
            }
        }
    }
}
