using UnityEngine;

public class LineIndicator : MonoBehaviour
{
    public int[,] line_data = new int[8, 8]
    {
        {0, 1, 2, 3, 4, 5, 6, 7 },
        {8, 9, 10, 11, 12, 13, 14, 15 },
        {16, 17, 18, 19, 20, 21, 22, 23 },
        {24, 25, 26, 27, 28, 29, 30, 31 },
        {32, 33, 34, 35, 36, 37, 38, 39 },
        {40, 41, 42, 43, 44, 45, 46, 47 },
        {48, 49, 50, 51, 52, 53, 54, 55 },
        {56, 57, 58, 59, 60, 61, 62, 63 }
    };

    [HideInInspector]
    public int[] columnIndexes = new int[8]
    {
        0, 1, 2, 3, 4, 5, 6, 7
    };
        
    public (int, int) GetSquarePosition(int square_index)
    {
        int pos_row = -1;
        int pos_col = -1;

        for(int row = 0; row < 8; row++)
        {
            for(int col = 0; col < 8; col++)
            {
                if (line_data[row, col] == square_index)
                {
                    pos_row = row;
                    pos_col = col;
                }
            }
        }

        return (pos_row, pos_col);
    }

    public int[] GetVerticalLine(int square_index)
    {
        int[] line = new int[8];

        var square_position_column = GetSquarePosition(square_index).Item2;

        for(int index = 0; index < 8; index++)
        {
            line[index] = line_data[index, square_position_column];
        }

        return line;
    }

}
