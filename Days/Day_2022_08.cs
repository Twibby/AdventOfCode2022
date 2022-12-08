using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_08 : DayScript2022
{
    List<List<TreePoint>> grid = new List<List<TreePoint>>();
    public int gridHeight;
    public int gridWidth;

    enum Direction { Left, Right, Top, Bot }
    protected override string part_1()
    {
        grid = new List<List<TreePoint>>();
        string[] raws = _input.Split('\n');
        gridHeight = raws.Length;
        gridWidth = raws[0].Length;

        // init grid, reading input
        for (int i = 0; i < gridHeight; i++)
        {
            grid.Add(new List<TreePoint>());
            for (int j = 0; j < gridWidth; j++)
            {
                grid[i].Add(new TreePoint(int.Parse(raws[i][j].ToString())));
            }
        }

        // go through grid horizontally (from left to right and reverse)
        for (int i = 0; i < gridHeight; i++)
        {
            int heightVal = -1;
            for (int j = 0; j < gridWidth; j++)
            {
                // from left to right
                if (grid[i][j].height > heightVal)
                {
                    grid[i][j].isVisibleFromDirection[Direction.Left] = true;
                    heightVal = grid[i][j].height;
                }
            }

            heightVal = -1;
            for (int j = gridWidth-1; j >= 0; j--)
            {
                // from right to left
                if (grid[i][j].height > heightVal)
                {
                    grid[i][j].isVisibleFromDirection[Direction.Right] = true;
                    heightVal = grid[i][j].height;
                }
            }
        }

        // go through grid vertically (from top to bot and reverse)
        for (int j = 0; j < gridWidth; j++)
        {
            int heightVal = -1;
            for (int i = 0; i < gridHeight; i++)
            {
                // from top to bot
                if (grid[i][j].height > heightVal)
                {
                    grid[i][j].isVisibleFromDirection[Direction.Top] = true;
                    heightVal = grid[i][j].height;
                }
            }

            heightVal = -1;
            for (int i = gridHeight - 1; i >= 0; i--)
            {
                // from bot to top
                if (grid[i][j].height > heightVal)
                {
                    grid[i][j].isVisibleFromDirection[Direction.Bot] = true;
                    heightVal = grid[i][j].height;
                }
            }
        }

        int count = 0;
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                //Debug.Log("Tree at pos (" + i + "," + j + ") has height " + grid[i][j].height + " and visibility is : " + grid[i][j].GetVisibilityDebug());
                if (grid[i][j].isVisible)
                    count++;
            }
        }
        return count.ToString();
    }


    protected override string part_2()
    {
        grid = new List<List<TreePoint>>();
        string[] raws = _input.Split('\n');
        gridHeight = raws.Length;
        gridWidth = raws[0].Length;

        // init grid, reading input
        for (int i = 0; i < gridHeight; i++)
        {
            grid.Add(new List<TreePoint>());
            for (int j = 0; j < gridWidth; j++)
            {
                grid[i].Add(new TreePoint(int.Parse(raws[i][j].ToString())));
            }
        }

        double maxScenicScore = -1;
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                maxScenicScore = System.Math.Max(maxScenicScore, getScenicScore(i, j));
            }
        }
        return maxScenicScore.ToString();
    }

    double getScenicScore(int x, int y)
    {
        int curHeight = grid[x][y].height;

        double resultBot = 0;
        for (int i = x +1; i < gridHeight; i++)
        {
            resultBot++;
            if (grid[i][y].height >= curHeight)
                break;
        }

        double resultTop = 0;
        for (int i = x - 1; i >= 0; i--)
        {
            resultTop++;
            if (grid[i][y].height >= curHeight)
                break;
        }

        double resultRight = 0;
        for (int j = y + 1; j < gridWidth; j++)
        {
            resultRight++;
            if (grid[x][j].height >= curHeight)
                break;
        }

        double resultLeft = 0;
        for (int j = y -1; j >= 0; j--)
        {
            resultLeft++;
            if (grid[x][j].height >= curHeight)
                break;
        }

        double res = resultTop * resultLeft * resultBot * resultRight;
        //Debug.Log("ScenicScore for pos (" + x + ", " + y + ") are " + resultTop + ", " + resultLeft + ", " + resultBot + ", " + resultRight + " ==> " + res);
        return res;
    }


    class TreePoint
    {
        public int height;
        public Dictionary<Direction, bool> isVisibleFromDirection = new Dictionary<Direction, bool>();

        public bool isVisible { get { return isVisibleFromDirection.Any(x => x.Value); } }

        public int scenicSCore = -1;

        public TreePoint(int p_height)
        {
            this.height = p_height;
            this.isVisibleFromDirection = new Dictionary<Direction, bool>();
            isVisibleFromDirection.Add(Direction.Left, false);
            isVisibleFromDirection.Add(Direction.Right, false);
            isVisibleFromDirection.Add(Direction.Top, false);
            isVisibleFromDirection.Add(Direction.Bot, false);
        }

        public string GetVisibilityDebug()
        {
            return System.String.Join(", ", isVisibleFromDirection.Where(x => x.Value).Select(x => x.Key));
        }
    }
}
