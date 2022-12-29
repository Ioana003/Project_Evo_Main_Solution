using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Evo_Main_Solution
{
    class TextWriter
    {
        private Rectangle textPos;
        private char characterInputted = ' ';
        private string inputtedString = "";
        private bool stopTyping = false;
        private string[] charactersArrayForWriting = new string[8];

        public TextWriter()
        {

        }

        public TextWriter(Rectangle inPosition)
        {
            textPos = inPosition;
        }

        public char GetKeyCharacter(string characterString)
        {
            if(Keyboard.GetState().GetPressedKeys().Length > 0)
            {
                characterString = Keyboard.GetState().GetPressedKeys()[0].ToString();
                foreach(char c in characterString)
                {
                        characterInputted = c;
                }
            }

            return characterInputted;
        }

        public void WriteText(SpriteBatch spriteBatch, SpriteFont spritefont, string characterString)
        {
            int i = 0;
            for(int j = 0; j < charactersArrayForWriting.Length; j++)
            {
                if(charactersArrayForWriting[j] != null)
                {
                    i++;
                }
            }
            //i is the pointer at which number we are going to type
            //So if we have types 4 numbers, it only shows 4 numbers, but it points at spot 5
            //It always checks which number we were left at



            if (stopTyping == false) //As long as you can freely type, then you can type; This is so you can input 1 character at a time
            {
                if (Keyboard.GetState().GetPressedKeys().Length > 0 && Keyboard.GetState().GetPressedKeys().Length <= 1) //Only look at the first number in the array given by the keyboard
                {
                    if (char.IsDigit(GetKeyCharacter(characterString))) //In this case, if it is a digit, then write it
                    {
                        if(i >= 0 && i < charactersArrayForWriting.Length) // Makes sure you only look at numbers within the bounds of the array
                        {
                            charactersArrayForWriting[i] = GetKeyCharacter(characterString).ToString(); // Add the character to the array
                            inputtedString = inputtedString + charactersArrayForWriting[i]; // Add that character to the string that will be used for creation
                            stopTyping = true; // Don't allow further typing until no keys are being pressed
                        }

                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Back))
                    {
                        i--;
                        // Since i always looks at the NEXT number, to delete the current number, you must look one number BEHIND
                        // However, that can lead to errors without the following if statement (e.g. i being -1)
                        if (i >= 0 && i < charactersArrayForWriting.Length)
                        {
                            charactersArrayForWriting[i] = null; // Makes the latest number a null value that holds nothing
                            inputtedString = ""; // Resets the string so it may be rewritten
                            for (int j = 0; j < charactersArrayForWriting.Length; j++)
                            {
                                if (charactersArrayForWriting[j] != null) // As long as the array has any values, then add them to the reset string
                                {
                                    inputtedString = inputtedString + charactersArrayForWriting[j];
                                    // This makes a string that is 1 character less whenever you press backspace
                                }
                            }
                            stopTyping = true;
                            // If you hold backspace (or even just tap it), it would absolutely break everything as it erases FAR too much
                            // So you can only erase 1 character at a time; It's a small, 10-character string, it's not that bad
                        }
                    }
                }
            }
            else
            {
                if (Keyboard.GetState().GetPressedKeys().Length == 0)
                {
                    stopTyping = false;
                    // If no keys are being held down, then allow typing again
                }
            }


            spriteBatch.DrawString(spritefont, inputtedString, new Vector2(textPos.X, textPos.Y), Color.Black);
        }

        public string GetInputtedString()
        {
            return inputtedString;
        }

        public void SetInputtedString(string input)
        {
            inputtedString = input;
        }

        public void SetCharacterArray(string[] charArray)
        {
            charactersArrayForWriting = charArray;
        }

    }
}
