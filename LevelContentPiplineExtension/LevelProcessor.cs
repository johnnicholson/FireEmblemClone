using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = LevelLibrary.Level;

namespace LevelContentPiplineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Level Processor")]
    public class LevelProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            string[] lines = input.Split(new char[] { '\n' });
            int rows = Convert.ToInt32(lines[0]);
            int columns = Convert.ToInt32(lines[1]);
            int opponents = Convert.ToInt32(lines[2]);

            char[,] levelData = new char[rows, columns];
            int[,] AI = new int[opponents,2];
            string[] AIC = new string[opponents];
            for (int row = 0; row < rows; row++)
            {
                string[] values = lines[row + 3].Split(new char[] { ' ' });
                for (int column = 0; column < columns; column++)
                {
                    levelData[row, column] = Convert.ToChar(values[column]);
                }
            }
            for(int row = 0; row < opponents; row++)
            {
                string[] values = lines[row + rows + 3].Split(new char[] { ' ' });
                for (int x = 0; x < 2; x++)
                {
                    AI[row,x] = Convert.ToInt32(values[x]);
                }
                AIC[row] = Convert.ToString(values[2]);
            }

            return new LevelLibrary.Level(levelData, AI, AIC);

        }
    }
}