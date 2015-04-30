using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// TODO: replace this with the type you want to read.
using TRead = LevelLibrary.Level;

namespace LevelLibrary
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class LevelReader : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {
            int rows = input.ReadInt32();
            int columns = input.ReadInt32();
            int opponents = input.ReadInt32();

            char[,] levelData = new char[rows, columns];
            int[,] AI = new int[opponents, 2];
            string[] AIC = new string[opponents];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    levelData[row, column] = input.ReadChar();
                }
            }
            for (int opponent = 0; opponent < opponents; opponent++)
            {
                for (int x = 0; x < 2; x++)
                {
                    AI[opponent, x] = input.ReadInt32();
                }
                AIC[opponent] = input.ReadString();
            }
            return new Level(levelData, AI, AIC);
        }
    }
}
