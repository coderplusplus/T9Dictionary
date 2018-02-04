using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace t9
{
    /// <summary>
    /// This class creates the tree datastructure for the word list.
    /// </summary>
    public class TernarySearchTree
    {
        Node root;
        List<string> wordList;

        /// <summary>
        /// Initialises the tree, 
        /// and initiatiates the reading of all words fromt the file
        /// </summary>
        public TernarySearchTree()
        {
            root = null;
            readWordsFromFile();
            wordList = new List<string>();
        }

        /// <summary>
        /// This function reads the given file
        /// it reads each line, that is,each word and call the addWordsToTheTree()
        /// to add to the datastructure
        /// </summary>
        private void readWordsFromFile()
        {
            string fileLocation = Path.Combine(Directory.GetCurrentDirectory(), "english-words.txt");
            StreamReader sr = File.OpenText(fileLocation);

            while (true)
            {
                String line = sr.ReadLine();

                if (line == null)
                {
                    break;
                }
                addWordsToTheTree(line);
            }
        }

        /// <summary>
        /// This function adds each character to the tree
        /// </summary>
        /// <param name="word">Word to be added</param>
        private void addWordsToTheTree(string word)
        {
            word = word.ToLower();
            addLetters(word, 0, ref root);
        }
        
        /// <summary>
        /// This method adds the letter to its position
        /// </summary>
        /// <param name="character">Character to be added</param>
        ///  <param name="node">node to which it could be attached</param>
        ///  <param name="isEndOfWord">node to which it could be attached</param>
        private void addLetters(string word, int index, ref Node node)
        {
            if (node == null)
            {
                node = new Node(word[index]);
            }

            //if the character is smaller, go left
            if (word[index] < node.letter)
            {
                addLetters(word, index, ref node.left);
            }
            //if the character is greater, go right
            else if (word[index] > node.letter)
            {
                addLetters(word, index, ref node.right);
            }
            else
            {
                //check if its the last letter of the word, if yes, set isAword==true, and return
                if (index + 1 == word.Length)
                {
                    node.isAWord = true;
                    return;
                }
                addLetters(word, index + 1, ref node.middle);
            }
        }

        /// <summary>
        /// This functions looks for the words corresponding to the given string
        /// adds all words to the lists and returns
        /// </summary>
        /// <param name="word">word to be added</param>
        public List<string> predictiveSearch(string word)
        {
            wordList.Clear();
            word = word.ToLower();
            List<string> list = new List<string>(searchString(word, 0));
            return list;
        }

        /// <summary>
        /// This is a standard ternary search tree used to route through 
        /// all the words the given word could possibly generate
        /// </summary>
        /// <param name="word">word to be looked</param>
        /// <param name="index">index where to start the search</param>
        /// <returns></returns>
        private List<string> searchString(string word, int index)
        {
            Node node = root;

            if (word == null || word == "")
            {
                return null;
            }
            while (node != null)
            {
                if (word[index] < node.letter)
                {
                    node = node.left;
                }
                else if (word[index] > node.letter)
                {
                    node = node.right;
                }
                else
                {
                    index++;
                    if (index == word.Length)
                    {
                        if (node.isAWord)
                        {
                            wordList.Add(word);
                        }
                        searchChildren(word, node.middle, wordList);

                        return wordList;
                    }
                    node = node.middle;
                }
            }
            return wordList;
        }

        /// <summary>
        /// This function is responsible for generating all the predictive words 
        /// with the given word as prefix
        /// </summary>
        /// <param name="wordAsPrefix">WOrd to be considered</param>
        /// <param name="node">which node does the tree route to</param>
        /// <param name="wordList">list of all the words</param>
        private void searchChildren(string wordAsPrefix, Node node, List<string> wordList)
        {
            if (node == null)
            {
                return;
            }
            if (node.isAWord)
            {
                wordList.Add(wordAsPrefix + node.letter);
            }
            searchChildren(wordAsPrefix, node.left, wordList);
            searchChildren(wordAsPrefix + node.letter, node.middle, wordList);
            searchChildren(wordAsPrefix, node.right, wordList);
        }
    }
    
    /// <summary>
    /// This class creates a node for the tree
    /// It stores the left, right and middle element information about the current node in consideration
    /// It also stores the letter and a boolean value indicating if its the end of node.
    /// </summary>
    public class Node
    {

        public Node left;
        public Node right;
        public Node middle;
        public char letter;
        public bool isAWord;

        /// <summary>
        /// This constructor initialises the values for a node
        /// </summary>
        /// <param name="letter"> the letter to be stored</param>
        public Node(char letter)
        {
            this.letter = letter;
            this.left = null;
            this.right = null;
            this.middle = null;
            this.isAWord = false;
        }
    }
}





