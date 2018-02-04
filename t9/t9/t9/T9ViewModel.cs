using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t9
{
    /// <summary>
    /// This class represents the view model of the MVVM architecture
    /// It is kept relatively thin
    /// It is responsible for generating the Ternary Tree Datastructure that is used to keep all the words
    /// It implements INotifyPropertyChanged, to raise events in case of any property change
    /// </summary>
    public class T9ViewModel : INotifyPropertyChanged
    {
        private T9Model t9model_;
        private string resultText_ = "";
        private bool isPredictiveChecked_ = false;

        //creating the tree data structure in the background, to load the file
        public static TernarySearchTree tree = new TernarySearchTree();

        /// <summary>
        /// This constructor initialises the T9model object,
        /// where all the business logic is
        /// </summary>
        public T9ViewModel()
        {
            t9model_ = new T9Model(tree);
        }

        //creating a static instance of the View Model, so that it could be a singleton
        private static T9ViewModel viewModelInstance = new T9ViewModel();
             
        /// <summary>
        /// This property is bound to the textbox that displays the results
        /// </summary>
        public string resultText
        {
            get
            {
                return resultText_;
            }
            set
            {
                if (value != resultText_)
                {
                    resultText_ = value;
                    onPropertyChanged("resultText");
                }
            }
        }

        /// <summary>
        /// This method is called on any button click from 1 to 9
        /// Calls the model's numberClicked() to process the words
        /// As soon as the method returns, the resultText property is raised
        /// </summary>
        /// <param name="myValue">Value to be displayed</param>
        internal void numberClicked(string myValue)
        {
            resultText = t9model_.numberClicked(myValue);
           
        }

        /// <summary>
        /// This method is called on "*"
        /// Calls the model's backspaceButtonClicked() to process the words
        /// As soon as the method returns, the resultText property is raised
        /// </summary>
        /// <param name="myValue">Value to be displayed</param>
        internal void backspaceButtonClicked()
        {
            resultText = t9model_.backspaceButtonClicked();
        }

        /// <summary>
        /// This method is called on "0"
        /// Calls the model's nextButtonClicked() to iterate through the suggested word list
        /// This is for predictive mode
        /// As soon as the method returns, the resultText property is raised
        /// </summary>
        /// <param name="myValue">Value to be displayed</param>
        internal void nextButtonClicked()
        {
            resultText = t9model_.nextButtonClicked();
        }

        /// <summary>
        /// This method is called on "#"
        /// Calls the model's backspaceButtonClicked() add a space, 
        /// and select the word in case of predictive mode
        /// As soon as the method returns, the resultText property is raised
        /// </summary>
        /// <param name="myValue">Value to be displayed</param>
        internal void spaceButtonClicked()
        {
            resultText = t9model_.spaceButtonClicked();
        }

        /// <summary>
        /// This function is responsible to send the ViewModel Instance to the View
        /// </summary>
        /// <returns> ViewModel instance </returns>
        public static T9ViewModel getViewModel()
        {
            return viewModelInstance;
        }

        /// <summary>
        /// This is the property used to set the two modes
        /// its bound with the checkbox using twoway data binding
        /// </summary>
        public bool isPredictiveChecked
        {
            get
            {
                return isPredictiveChecked_;
            }
            set
            {
                if (value != isPredictiveChecked_)
                {
                    isPredictiveChecked_ = value;
                    resultText = t9model_.isPredictiveChecked();
                    onPropertyChanged("isPredictiveChecked");
                }
            }
        }

        /// <summary>
        /// This is used to handle any property change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This is the handler to handle any property change raised
        /// </summary>
        /// <param name="property">Name of the property changed</param>
        protected virtual void onPropertyChanged(string property)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
