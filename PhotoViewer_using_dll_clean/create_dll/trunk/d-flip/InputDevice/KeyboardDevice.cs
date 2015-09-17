using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;


namespace dflip
{
    public class KeyboardDevice
    {
        KeyboardState keyboardState;
        public KeyboardDevice()
        {
        }
        KeyboardState oldState;
        public void getKeyboardState()
        {
            oldState = keyboardState;
            keyboardState = Keyboard.GetState();
        }
        public bool enterKey
        {
            get 
            {
                if (keyboardState.IsKeyDown(Keys.Enter))
                    return true;
                return false;
            }
        }
        public bool ctrlKey
        {
            get
            {
                if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl))
                {
                    return true;
                }
                return false;
            }
        }
        public bool altKey
        {
            get
            {
                if (keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt))
                {
                    return true;
                }
                return false;
            }
        }
        
        private bool add(Keys k)
        {
            if (oldState.GetPressedKeys().Contains<Keys>(k))
            {
                return false;
            }
            else {
                return true;
            }
            
        }

        public string getString()
        {
            string text = null;
            foreach (var k in keyboardState.GetPressedKeys())
            {
                switch (k)
                {
                    case Keys.A: if(add(Keys.A)) text += 'a'; break;
                    case Keys.B: if (add(Keys.B)) text += 'b'; break;
                    case Keys.C: if (add(Keys.C)) text += 'c'; break;
                    case Keys.D: if (add(Keys.D)) text += 'd'; break;
                    case Keys.E: if (add(Keys.E)) text += 'e'; break;
                    case Keys.F: if (add(Keys.F)) text += 'f'; break;
                    case Keys.G: if (add(Keys.G)) text += 'g'; break;
                    case Keys.H: if (add(Keys.H)) text += 'h'; break;
                    case Keys.I: if (add(Keys.I)) text += 'i'; break;
                    case Keys.J: if (add(Keys.J)) text += 'j'; break;
                    case Keys.K: if (add(Keys.K)) text += 'k'; break;
                    case Keys.L: if (add(Keys.L)) text += 'l'; break;
                    case Keys.M: if (add(Keys.M)) text += 'm'; break;
                    case Keys.N: if (add(Keys.N)) text += 'n'; break;
                    case Keys.O: if (add(Keys.O)) text += 'o'; break;
                    case Keys.P: if (add(Keys.P)) text += 'p'; break;
                    case Keys.Q: if (add(Keys.Q)) text += 'q'; break;
                    case Keys.R: if (add(Keys.R)) text += 'r'; break;
                    case Keys.S: if (add(Keys.S)) text += 's'; break;
                    case Keys.T: if (add(Keys.T)) text += 't'; break;
                    case Keys.U: if (add(Keys.U)) text += 'u'; break;
                    case Keys.V: if (add(Keys.V)) text += 'v'; break;
                    case Keys.W: if (add(Keys.W)) text += 'w'; break;
                    case Keys.X: if (add(Keys.X)) text += 'x'; break;
                    case Keys.Y: if (add(Keys.Y)) text += 'y'; break;
                    case Keys.Z: if (add(Keys.Z)) text += 'z'; break;
                    case Keys.Space: if (add(Keys.Space)) text += ' '; break;
                    case Keys.OemComma: if (add(Keys.OemComma)) text += ','; break;
                    case Keys.OemPeriod: if (add(Keys.OemPeriod)) text += '.'; break;
                    case Keys.OemSemicolon: if (add(Keys.OemSemicolon)) text += ';'; break;
                 }
            }
            //Console.WriteLine(text);
            return text;
        }
    }
}
