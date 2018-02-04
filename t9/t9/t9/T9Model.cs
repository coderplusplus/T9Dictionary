using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;

namespace t9
{
    /// <summary>
    /// This class is responsible for all the business logic
    /// Represents Model of MVVM
    /// </summary>
    public class T9Model
    {
        //variables used to handle t9 predictive app
        static int numberOfClicks = 0;
        static int countOfNumbers = 0;
        bool isPredictiveModeOn = false;
        string previousNumber = "";
        DateTime currentTime;
        DateTime previousTime;
        int currentCharacterIndex = 0;
        string[] currentWordLetters; 
        int totalNumberOfWords = 0;
        string[] words;
        string[] numbersMakingUpWords;
        Dictionary<string, string[]> keyPad;
        bool islastWordInsertASpace = false;
        TernarySearchTree tree;
        List<string>combinations;
        List<string> prefixButNotAWord = new List<string>();
        List<string> predictiveWords = new List<string>();
        List<string> wholeWordsAsIS = new List<string>();
        static int nextWordCount = 0;
        private bool wordPrevToDeletedWordIsASpace = false;

        /// <summary>
        /// This is the constructor to initialise the tree 
        /// and other variables of the model
        /// </summary>
        public T9Model(TernarySearchTree tree) {
            this.tree = tree;
            words = new string[100];
            currentWordLetters = new string[100];
            keyPad = new Dictionary<string, string[]>();
            createKeyPad();
            previousTime = DateTime.Now;
            combinations = new List<string>();
            numbersMakingUpWords = new string[100];
        }

        /// <summary>
        /// This functions creates a dictionary to replicate the keypad
        /// </summary>
        private void createKeyPad()
        {
            keyPad.Add("2",new string[] { "a", "b", "c" });
            keyPad.Add("3", new string[] { "d", "e", "f" });
            keyPad.Add("4", new string[] { "g", "h", "i" });
            keyPad.Add("5", new string[] { "j", "k", "l" });
            keyPad.Add("6", new string[] {"m","n","o"  });
            keyPad.Add("7", new string[] { "p", "q", "r", "s" });
            keyPad.Add("8", new string[] { "t", "u", "v" });
            keyPad.Add("9", new string[] { "w","x", "y", "z" });
        }

        /// <summary>
        /// This function handles the backspace event
        /// updates the text to be displayed 
        /// it checks, if the element to be deleted is a space,
        /// if yes, it just removes it from the array,
        /// else, it remvoves the last character of the last word
        /// for predictive analysis, it makes sure that the word in consideration after deleting
        /// has its updated predictive words
        /// </summary>
        /// <returns>returns the final text</returns>
        internal string backspaceButtonClicked()
        {
            previousNumber = "-1";
            //for non-predictive
            if (!isPredictiveModeOn)
            {
                if (!(words[totalNumberOfWords].Equals("")))
                {
                    if (words[totalNumberOfWords].Equals(","))
                    {
                        islastWordInsertASpace = false;
                        wordPrevToDeletedWordIsASpace = false;
                        words[totalNumberOfWords] = null;
                        if (totalNumberOfWords != 0)
                        {
                            currentWordLetters = new string[100];
                            totalNumberOfWords--;
                            currentCharacterIndex = words[totalNumberOfWords].Length - 1;
                            for (int index = 0; index < currentCharacterIndex + 1; index++)
                            {
                                currentWordLetters[index] = words[totalNumberOfWords][index].ToString();
                            }
                        }
                        return appendString();
                    }
                    else
                    {
                        currentCharacterIndex = words[totalNumberOfWords].Length - 1;
                        currentWordLetters = new string[100];
                        for (int index = 0; index < currentCharacterIndex + 1; index++)
                        {
                            currentWordLetters[index] = words[totalNumberOfWords][index].ToString();
                        }
                        currentWordLetters[currentCharacterIndex] = null;
                        currentCharacterIndex--;
                        words[totalNumberOfWords] = string.Join("", currentWordLetters);
                        var result = appendString();

                        //if the last character deleted is end of the word,
                        //update the next word to be considered
                        if (currentCharacterIndex < 0)
                        {
                            if (totalNumberOfWords > 0)
                            {
                                if (words[totalNumberOfWords - 1].Equals(","))
                                {
                                    wordPrevToDeletedWordIsASpace = true;
                                }
                                else
                                {
                                    currentWordLetters = new string[100];
                                    words[totalNumberOfWords] = null;
                                    currentCharacterIndex = words[totalNumberOfWords].Length - 1;
                                    for (int index = 0; index < currentCharacterIndex + 1; index++)
                                    {
                                        currentWordLetters[index] = words[totalNumberOfWords][index].ToString();
                                    }
                                }
                                totalNumberOfWords--;
                            }
                        }
                        return result;
                    }
                }
            }
            //for predictive
            else
            {
                if (!(numbersMakingUpWords[totalNumberOfWords].Equals("")))
                {
                    if (numbersMakingUpWords[totalNumberOfWords].Equals(","))
                    {
                        islastWordInsertASpace = false;
                        wordPrevToDeletedWordIsASpace = false;
                        words[totalNumberOfWords] = null;
                        numbersMakingUpWords[totalNumberOfWords] = null;
                       
                        if (totalNumberOfWords != 0)
                        {
                            totalNumberOfWords--;
                            combinations.Clear();
                            predictiveWords.Clear();
                            prefixButNotAWord.Clear();
                            wholeWordsAsIS.Clear();
                            countOfNumbers = 0;
                            words[totalNumberOfWords] = null;
                            for (int index = 0; index < numbersMakingUpWords[totalNumberOfWords].Length; index++)
                            {
                                createCombinations(numbersMakingUpWords[totalNumberOfWords][index].ToString());
                                words[totalNumberOfWords] += keyPad[numbersMakingUpWords[totalNumberOfWords][index].ToString()][0];
                            }
                            startPredictioning();
                        }
                        return appendString();
                    }
                    else
                    {
                        numbersMakingUpWords[totalNumberOfWords] = numbersMakingUpWords[totalNumberOfWords].Remove(numbersMakingUpWords[totalNumberOfWords].Length - 1,1);
                        combinations.Clear();
                        predictiveWords.Clear();
                        prefixButNotAWord.Clear();
                        wholeWordsAsIS.Clear();
                        countOfNumbers = 0;
                        words[totalNumberOfWords] = "";
                        
                        for (int index = 0; index < numbersMakingUpWords[totalNumberOfWords].Length; index++)
                        {
                            createCombinations(numbersMakingUpWords[totalNumberOfWords][index].ToString());
                            words[totalNumberOfWords] += keyPad[numbersMakingUpWords[totalNumberOfWords][index].ToString()][0];
                        }
                        startPredictioning();
                        var result = appendString();

                        //if the last character deleted is end of the word,
                        //update the next word to be considered
                        if (numbersMakingUpWords[totalNumberOfWords].Length == 0)
                        {
                            if (totalNumberOfWords > 0)
                            {
                                if (numbersMakingUpWords[totalNumberOfWords - 1].Equals(","))
                                {
                                    wordPrevToDeletedWordIsASpace = true;

                                    totalNumberOfWords--;
                                }
                                else
                                {
                                    totalNumberOfWords--;
                                    combinations.Clear();
                                    predictiveWords.Clear();
                                    prefixButNotAWord.Clear();
                                    wholeWordsAsIS.Clear();
                                    countOfNumbers = 0;
                                    words[totalNumberOfWords] = "";

                                    for (int index = 0; index < numbersMakingUpWords[totalNumberOfWords].Length; index++)
                                    {
                                        createCombinations(numbersMakingUpWords[totalNumberOfWords][index].ToString());
                                        words[totalNumberOfWords] += keyPad[numbersMakingUpWords[totalNumberOfWords][index].ToString()][0];
                                    }
                                    startPredictioning();
                                }
                            }
                        }
                        return result;
                    }
                }
            }
            return appendString() ;
        }

        /// <summary>
        /// Whenever a number is pressed, it will check wheter to do predictive analysis or non-predictive
        /// and call the appropriate function
        /// </summary>
        /// <param name="number">value of which number is pressed</param>
        /// <returns>retunr the text to be displayed</returns>
        internal string numberClicked(string number)
        {
            if (!isPredictiveModeOn)
            {
                return doNonPredictiveAnalysis(number);
            }
            return doPredictiveAnalysis(number);
        }

        /// <summary>
        /// This function is used to toggle the Predictive mode
        /// Each toggle erases everything, and starts from scratch
        /// reinitializes all the arrays 
        /// sets the flag isPredictiveChecked
        /// The textbox would show an empty string
        /// </summary>
        /// <returns>An empty string to be displayed on the text box</returns>
        internal string isPredictiveChecked()
        {
            words = new string[100];
            totalNumberOfWords = 0;
            numbersMakingUpWords = new string[100];
            islastWordInsertASpace = false;
            wordPrevToDeletedWordIsASpace = false;
            numbersMakingUpWords = new string[100];
            isPredictiveModeOn = !isPredictiveModeOn;
            //a common function to be used to reset the arrays used
            clearAll();

            return "";
        }

        /// <summary>
        /// This function is responsible for all non-predictive analysis
        /// It checks the number of clickes within 1000 millisecond to have the appropriate character
        /// </summary>
        /// <param name="number">the number key pressed</param>
        /// <returns>returns the updated string to be displayed</returns>
        private string doNonPredictiveAnalysis(string number)
        {
            currentTime = DateTime.Now;
            if(previousNumber == "")
            {
                previousNumber = number;
                previousTime = DateTime.Now;
            }
            if((previousTime.AddMilliseconds(1000) >= currentTime) && (previousNumber.Equals(number)))
            {
                numberOfClicks++;
            }
            else
            {
                numberOfClicks = 1;
                currentCharacterIndex++;
            }
            previousNumber = number;
            previousTime = currentTime;
            return addCharacterToString(number);
        }
        
        /// <summary>
        /// This functions updates the letters of the current word, and adds it to the resultant array of words.
        /// </summary>
        /// <param name="currentNumberPressed">value of the button pressed</param>
        /// <returns></returns>
        private string addCharacterToString(string currentNumberPressed)
        {
            //if the clicks exceed the number of chars on the numpads with the number of chars on it
            if(currentNumberPressed.Equals("7") || currentNumberPressed.Equals("9"))
            {
                if(numberOfClicks > 4)
                {
                    numberOfClicks = (numberOfClicks+1) % 5;
                }
            }
            else
            {
                if (numberOfClicks > 3)
                {
                    numberOfClicks = (numberOfClicks + 1) % 4;
                }
            }
            //add letters to the current word, and update the words' array
            currentWordLetters[currentCharacterIndex] = keyPad[currentNumberPressed][numberOfClicks - 1];
            if (wordPrevToDeletedWordIsASpace || islastWordInsertASpace)
            {
                totalNumberOfWords++;
                if (islastWordInsertASpace)
                {
                    islastWordInsertASpace = false;
                }
                if (wordPrevToDeletedWordIsASpace)
                {
                    wordPrevToDeletedWordIsASpace = false;
                    //totalNumberOfWords++;

                }
            }
            words[totalNumberOfWords] = string.Join("",currentWordLetters);
            return appendString();
        }
        
        /// <summary>
        /// This function handles the space button clicked
        /// for non-predictive mode, it adds a space and returns the updated string
        /// for predictive mode, it selects the word, if present
        /// else, it checks if any possible word could be made from the give combinations, 
        /// and selects the 1st in the ascending order
        /// it also resets all the arrays for new iteration
        /// </summary>
        /// <returns> return the updated string</returns>
        public string spaceButtonClicked()
        {
            if (isPredictiveModeOn)
            {
                if(!(wholeWordsAsIS.Contains(words[totalNumberOfWords])))
                {
                    if(prefixButNotAWord.Count() == 0 || countOfNumbers<3)
                    {
                        words[totalNumberOfWords] = "";
                        int temp = countOfNumbers;
                        while (temp > 0)
                        {
                            words[totalNumberOfWords] += "-";
                            temp--;
                        }
                    }
                    else
                    {
                        if (wholeWordsAsIS.Count() > 0)
                        {
                            predictiveWords.AddRange(wholeWordsAsIS);
                        }
                        predictiveWords.Sort();
                        words[totalNumberOfWords] = predictiveWords[0];
                    }
                }
            }
            totalNumberOfWords++;
            numbersMakingUpWords[totalNumberOfWords] = ",";
            words[totalNumberOfWords] = ",";
            islastWordInsertASpace = true;
            clearAll();
            
            return appendString();
        }

        /// <summary>
        /// This is a common function made to reset all the word arrays used
        /// </summary>
        private void clearAll()
        {
            currentWordLetters = new string[100];
            currentCharacterIndex = 0;
            previousNumber = " ";
            predictiveWords.Clear();
            combinations.Clear();
            wholeWordsAsIS.Clear();
            prefixButNotAWord.Clear();
            countOfNumbers = 0;
            nextWordCount = 0;
        }

        /// <summary>
        /// This function merely appends all the words in a string to be displayed
        /// </summary>
        /// <returns>return the result string</returns>
        private string appendString()
        {
            StringBuilder sb = new StringBuilder();
            for(int index = 0; index < (totalNumberOfWords+1); index++)
            {
                if (words[index].Equals(","))
                {
                    sb.Append(' ', 1);
                }
                else
                    sb.Append(words[index]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// This function is responsible for all the predictive analysis
        ///  It creates all the possible combinations of the given number keys
        ///  Generates a list of all possibilities of words
        /// </summary>
        /// <returns>Updated text to be displayed</returns>
        private string doPredictiveAnalysis(string number)
        {
            var keyCharacters = keyPad[number];
            currentWordLetters[currentCharacterIndex] = keyCharacters[0];
            currentCharacterIndex++;
            if ( wordPrevToDeletedWordIsASpace || islastWordInsertASpace)
            {
                totalNumberOfWords++;
                if (islastWordInsertASpace)
                {
                    islastWordInsertASpace = false;
                }
                if (wordPrevToDeletedWordIsASpace)
                {
                    wordPrevToDeletedWordIsASpace = false; 
                }
            }
            numbersMakingUpWords[totalNumberOfWords] += number;
            words[totalNumberOfWords] = null;
            for (int index = 0; index < numbersMakingUpWords[totalNumberOfWords].Length; index++)
            {
                words[totalNumberOfWords] += keyPad[numbersMakingUpWords[totalNumberOfWords][index].ToString()][0];
            }
            //create all sorts of combinations of the num keys pressed
            createCombinations(number);
            //generate all the possible words
            startPredictioning();
            
            return appendString();
        }

        /// <summary>
        /// This function is responsible for generating all sorts
        /// combinations of the num keys pressed
        /// </summary>
        /// <param name="number">Num key pressed</param>
        private void createCombinations(string number)
        {
            countOfNumbers++;
            var keyCharacters = keyPad[number];
            if (countOfNumbers == 1)
            {
                for (int index = 0; index < keyCharacters.Length; index++)
                {
                    combinations.Add(keyCharacters[index]);
                }
            }
            else
            {
                List<string> tempList = new List<string>(combinations);
                combinations.Clear();
                for (int index = 0; index < tempList.Count; index++)
                {
                    for (int keyIndex = 0; keyIndex < keyCharacters.Length; keyIndex++)
                    {
                        combinations.Add(tempList[index] + keyCharacters[keyIndex]);
                    }
                }
            }
        }

        /// <summary>
        /// This function generates all the possible words from a set of combinations
        /// it creates a set of valid words of the given length, 
        /// set of valid prefixes, set of valid predictive words
        /// </summary>
        private void startPredictioning()
        {
            wholeWordsAsIS.Clear();
            prefixButNotAWord.Clear();
            predictiveWords.Clear();
            List<string> tempCombinations = new List<string>(combinations);
            for(int index = 0; index < tempCombinations.Count; index++)
            {
                var tpWPrd = tempCombinations[index];
                var temp = tree.predictiveSearch(tempCombinations[index]);
                
                if (temp.Count > 0)
                {
                    if (temp[0].Equals(tempCombinations[index]))
                    {
                        wholeWordsAsIS.Add(temp[0]);
                    }
                    else
                    {
                        prefixButNotAWord.Add(tempCombinations[index]);
                        predictiveWords.AddRange(temp);
                    }
                }
                else
                {
                    combinations.Remove(tempCombinations[index]);
                }
            }
        }

        /// <summary>
        /// This function is responsible for handling the iteration of the predictive words
        /// </summary>
        /// <returns>updated sting to be displayed</returns>
        internal string nextButtonClicked()
        {
            if (isPredictiveModeOn)
            {
                if(countOfNumbers<3)
                {
                    if (wholeWordsAsIS.Count > 0)
                    {
                        words[totalNumberOfWords] = wholeWordsAsIS[nextWordCount % wholeWordsAsIS.Count];
                        nextWordCount++;
                    }
                    else
                    {
                        words[totalNumberOfWords] = "";
                        int temp = countOfNumbers;
                        while (temp > 0)
                        {
                            words[totalNumberOfWords] += "-";
                            temp--;
                        }
                    }
                }
                else
                {
                    if (wholeWordsAsIS.Count > 0)
                    {
                        words[totalNumberOfWords] = wholeWordsAsIS[nextWordCount % wholeWordsAsIS.Count];
                        nextWordCount++;
                    }
                    else if (prefixButNotAWord.Count > 0)
                    {
                        words[totalNumberOfWords] = predictiveWords[nextWordCount % predictiveWords.Count];
                        nextWordCount++;
                    }
                    else
                    {
                        words[totalNumberOfWords] = "";
                        int temp = countOfNumbers;
                        while (temp > 0)
                        {
                            words[totalNumberOfWords] += "-";
                            temp--;
                        }
                    }
                }
            }
            return appendString();
        }
    }
}
