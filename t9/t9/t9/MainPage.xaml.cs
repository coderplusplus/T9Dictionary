/*
 * Author: Hema Bahirwani
 * Application: T9 Predictive App
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
/// <summary>
/// This namespace is responsible for all the files required for t9 application
/// </summary>
namespace t9
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        //object of the ViewModel
        public T9ViewModel viewModelInstance { get; set; }

        /// <summary>
        /// This constructor initialises the component, ViewModel Object
        /// and binds the datacontext witht the view model
        /// </summary>
        public MainPage()
        {
            viewModelInstance = T9ViewModel.getViewModel();
            this.InitializeComponent();
            this.DataContext = viewModelInstance;
        }

        /// <summary>
        /// On any number click from 1-9, this function calls the function of the view model
        /// which updates the textbox with the appropriate results
        /// </summary>
        /// <param name="sender">reference to the control that raised the event</param>
        /// <param name="e">contains the event data</param>
        private void numberButtonClicked(object sender, RoutedEventArgs e)
        {
            var myValue = ((Button)sender).Tag.ToString();
            
            viewModelInstance.numberClicked(myValue);
        }

        /// <summary>
        /// This function is called on press of "*",
        /// which inturn calls the view model function to delete the text by 1
        /// </summary>
        /// <param name="sender">reference to the control that raised the event</param>
        /// <param name="e">contains the event data</param>
        private void backspaceButtonClicked(object sender, RoutedEventArgs e)
        {
            viewModelInstance.backspaceButtonClicked();
        }

        /// <summary>
        /// This function is called on press of "0"
        /// Calls the view model function to iterate through the list of suggestive words
        /// </summary>
        /// <param name="sender">reference to the control that raised the event</param>
        /// <param name="e">contains the event data</param>
        private void nextButtonClicked(object sender, RoutedEventArgs e)
        {
            viewModelInstance.nextButtonClicked();
        }

        /// <summary>
        /// This is called on prress of "#"
        /// In both the modes it adds a space,
        /// in predictive mode it helps in selecting a suggested word, too
        /// </summary>
        /// <param name="sender">reference to the control that raised the event</param>
        /// <param name="e">contains the event data</param>
        private void spaceButtonClicked(object sender, RoutedEventArgs e)
        {
            viewModelInstance.spaceButtonClicked();
        }
    }
}
