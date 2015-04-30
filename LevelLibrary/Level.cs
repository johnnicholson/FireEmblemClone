using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelLibrary
{
    public class Level
    {
        public char[,] Values { get; set; }

        public Level(char[,] values, int[,] aI, string[] aIC)
        {
            Values = values;
            AI = aI;
            AIC = aIC;
        }

        public char GetValue(int row, int column)
        {
            return Values[row, column];
        }
        public int GetPosition(int opponent, int column)
        {
            return AI[opponent, column];
        }
        public string GetClass(int row)
        {
            return AIC[row];
        }

        public int Rows
        {
            get
            {
                return Values.GetLength(0);
            }
        }

        public int Columns
        {
            get
            {
                return Values.GetLength(1);
            }
        }
        public int[,] AI { get; set; }
        public string[] AIC { get; set; }
        public int Opponents
        {
            get
            {
                return AI.GetLength(0);
            }
        }
    }
}