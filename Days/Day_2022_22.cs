using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_22 : DayScript2022
{
    List<string> grid = new List<string>();
    protected override string part_1()
    {
        grid = new List<string>(_input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split('\n').ToList());
        string instruction = _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[1];

        Vector2Int startingPoint = new(grid[0].IndexOf('.'), 0);
        FacingDirection startDirection = FacingDirection.Right;

        Debug.Log("Starting at " + startingPoint.ToString() + ", facing " + startDirection.ToString());

        Vector2Int curPos = startingPoint;
        FacingDirection curDir = startDirection;
        int index = 0;
        int directionsCount = System.Enum.GetValues(typeof(FacingDirection)).Length;

        int safetyCount = 10000;
        while (index < instruction.Length && safetyCount > 0)
        {
            if (instruction[index] == 'R' || instruction[index] == 'L')
            {
                curDir = (FacingDirection)(((int)curDir + (instruction[index] == 'R' ? 1 : -1) + directionsCount) % directionsCount);
                
                Debug.Log("Turning " + instruction[index] + " - Now facing " + curDir.ToString());
                index++;
            }
            else
            {
                int firstNonDigitIndex = Mathf.Min( (instruction.IndexOf('R', index) < 0 ? instruction.Length : instruction.IndexOf('R', index)),
                                                    (instruction.IndexOf('L', index) < 0 ? instruction.Length : instruction.IndexOf('L', index)) );

                //Debug.Log("index is " + index + " | fisrtnondigit is " + firstNonDigitIndex);
                //Debug.Log(", resulting substring is" + instruction.Substring(index, firstNonDigitIndex - index));
                int val = int.Parse(instruction.Substring(index, firstNonDigitIndex - index));
                curPos = DoMove(curDir, curPos, val);

                Debug.Log("Moving " + curDir.ToString() + ", " + val + " times - Now current position is " + curPos.ToString());
                index = firstNonDigitIndex;
            }

            safetyCount--;
        }

        Debug.Log("Finishing at " + curPos.ToString() + ", facing " + curDir.ToString());

        int total = 1000 * (curPos.y+1) + 4 * (curPos.x+1) + (int)curDir;
        return total.ToString();
    }

    public Vector2Int DoMove(FacingDirection direction, Vector2Int startPos, int moves)
    {
        Vector2Int result = Vector2Int.zero;

        switch (direction)
        {
            case FacingDirection.Right: result = DoMoveRight(startPos, moves);  break;
            case FacingDirection.Down:  result = DoMoveDown(startPos, moves);   break;
            case FacingDirection.Left:  result = DoMoveLeft(startPos, moves);   break;
            case FacingDirection.Up:    result = DoMoveUp(startPos, moves);     break;
        }

        return result;
    }

    public Vector2Int DoMoveRight(Vector2Int startPos, int moves)
    {
        Vector2Int result = new Vector2Int(startPos.x, startPos.y);

        for (int i = 0; i < moves; i ++)
        {
            if (grid[result.y].Length > result.x + 1 && grid[result.y][result.x +1] != ' ')
            {
                if (grid[result.y][result.x + 1] == '#')
                {
                    Debug.Log("BONK, after " + i.ToString() + " moves");
                    break;
                }

                result.x += 1;
            }
            else
            {
                int firstIndexNotEmpty = grid[result.y].IndexOfAny(new char[] { '.', '#' });
                if (firstIndexNotEmpty < 0)
                    Debug.LogError("wtf ?? '" + grid[result.y] + "'");

                if (grid[result.y][firstIndexNotEmpty] == '#')
                {
                    Debug.Log("BONK, on wrapping.. after " + i.ToString() + " moves");
                    break;
                }

                result.x = firstIndexNotEmpty;
            }
        }       

        return result;
    }

    public Vector2Int DoMoveLeft(Vector2Int startPos, int moves)
    {
        Vector2Int result = new Vector2Int(startPos.x, startPos.y);

        for (int i = 0; i < moves; i++)
        {
            if (result.x > 0 && grid[result.y][result.x-1] != ' ')
            {
                if (grid[result.y][result.x - 1] == '#')
                {
                    Debug.Log("BONK, after " + i.ToString() + " moves");
                    break;
                }

                result.x -= 1;
            }
            else
            {
                int lastIndexNotEmpty = grid[result.y].LastIndexOfAny(new char[] { '.', '#' });
                if (lastIndexNotEmpty < 0)
                    Debug.LogError("wtf ?? '" + grid[result.y] + "'");

                if (grid[result.y][lastIndexNotEmpty] == '#')
                {
                    Debug.Log("BONK, on wrapping.. after " + i.ToString() + " moves");
                    break;
                }

                result.x = lastIndexNotEmpty;
            }
        }

        return result;
    }

    public Vector2Int DoMoveUp(Vector2Int startPos, int moves)
    {
        Vector2Int result = new Vector2Int(startPos.x, startPos.y);

        for (int i = 0; i < moves; i++)
        {
            if (result.y > 0 && grid[result.y-1].Length > result.x && grid[result.y -1][result.x] != ' ')
            {
                if (grid[result.y -1][result.x] == '#')
                {
                    Debug.Log("BONK, after " + i.ToString() + "moves");
                    break;
                }

                result.y -= 1;
            }
            else
            {
                int lastIndexNotEmpty = -1;
                for (int j= grid.Count-1; j > 0; j--)
                {
                    if (grid[j].Length > result.x && grid[j][result.x] != ' ')
                    {
                        lastIndexNotEmpty = j;
                        break;
                    }
                }

                if (lastIndexNotEmpty < 0)
                    Debug.LogError("wtf ?? '" + System.String.Join("", grid.Select(x => x.Length > result.x ? x[result.x].ToString() : " ")) + "'");

                if (grid[lastIndexNotEmpty][result.x] == '#')
                {
                    Debug.Log("BONK, on wrapping.. after " + i.ToString() + "moves");
                    break;
                }

                result.y = lastIndexNotEmpty;
            }
        }

        return result;
    }

    public Vector2Int DoMoveDown(Vector2Int startPos, int moves)
    {
        Vector2Int result = new Vector2Int(startPos.x, startPos.y);

        for (int i = 0; i < moves; i++)
        {
            if (result.y < grid.Count -1  && grid[result.y + 1].Length > result.x && grid[result.y + 1][result.x] != ' ')
            {
                if (grid[result.y + 1][result.x] == '#')
                {
                    Debug.Log("BONK, after " + i.ToString() + "moves");
                    break;
                }

                result.y += 1;
            }
            else
            {
                int firstIndexNotEmpty = -1;
                for (int j = 0; j < grid.Count; j++)
                {
                    if (grid[j].Length > result.x && grid[j][result.x] != ' ')
                    {
                        firstIndexNotEmpty = j;
                        break;
                    }
                }

                if (firstIndexNotEmpty < 0)
                    Debug.LogError("wtf ?? '" + System.String.Join("", grid.Select(x => x.Length > result.x ? x[result.x].ToString() : " ")) + "'");

                if (grid[firstIndexNotEmpty][result.x] == '#')
                {
                    Debug.Log("BONK, on wrapping.. after " + i.ToString() + "moves");
                    break;
                }

                result.y = firstIndexNotEmpty;
            }
        }

        return result;
    }

    protected override string part_2()
    {


        return base.part_2();
    }

    public enum FacingDirection { Right, Down, Left, Up }
}
