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

    static List<CubeFace> cubeFaces = new List<CubeFace>();
    static CubeFace currentFace;
    static int faceSize = 50;
    static FacingDirection curDir = FacingDirection.Up;

    protected override string part_2()
    {
        grid = new List<string>(_input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split('\n').ToList());
        string instruction = _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[1];


        cubeFaces = new List<CubeFace>();

        // TOP
        CubeFace face = new CubeFace(FaceName.Top);
        for (int i = 0; i < faceSize; i++) { face.grid.Add(grid[i].Substring(faceSize, faceSize)); }
        face.neighboursRelationship.Add(FacingDirection.Right, new KeyValuePair<FaceName, int>(FaceName.East, 0));
        face.neighboursRelationship.Add(FacingDirection.Down, new KeyValuePair<FaceName, int>(FaceName.Front, 0));
        face.neighboursRelationship.Add(FacingDirection.Up, new KeyValuePair<FaceName, int>(FaceName.Back, 1));
        face.neighboursRelationship.Add(FacingDirection.Left, new KeyValuePair<FaceName, int>(FaceName.West, 2));
        cubeFaces.Add(face);

        // EAST
        face = new CubeFace(FaceName.East);
        for (int i = 0; i < faceSize; i++) { face.grid.Add(grid[i].Substring(2 * faceSize, faceSize)); }
        face.neighboursRelationship.Add(FacingDirection.Left, new KeyValuePair<FaceName, int>(FaceName.Top, 0));
        face.neighboursRelationship.Add(FacingDirection.Down, new KeyValuePair<FaceName, int>(FaceName.Front, 1));
        face.neighboursRelationship.Add(FacingDirection.Right, new KeyValuePair<FaceName, int>(FaceName.Bot, 2));
        face.neighboursRelationship.Add(FacingDirection.Up, new KeyValuePair<FaceName, int>(FaceName.Back, 0));
        cubeFaces.Add(face);

        // FRONT
        face = new CubeFace(FaceName.Front);
        for (int i = faceSize; i < 2 * faceSize; i++) { face.grid.Add(grid[i].Substring(faceSize, faceSize)); }
        face.neighboursRelationship.Add(FacingDirection.Up, new KeyValuePair<FaceName, int>(FaceName.Top, 0));
        face.neighboursRelationship.Add(FacingDirection.Down, new KeyValuePair<FaceName, int>(FaceName.Bot, 0));
        face.neighboursRelationship.Add(FacingDirection.Right, new KeyValuePair<FaceName, int>(FaceName.East, -1));
        face.neighboursRelationship.Add(FacingDirection.Left, new KeyValuePair<FaceName, int>(FaceName.West, -1));
        cubeFaces.Add(face);

        // BOT
        face = new CubeFace(FaceName.Bot);
        for (int i = 2 * faceSize; i < 3 * faceSize; i++) { face.grid.Add(grid[i].Substring(faceSize, faceSize)); }
        face.neighboursRelationship.Add(FacingDirection.Up, new KeyValuePair<FaceName, int>(FaceName.Front, 0));
        face.neighboursRelationship.Add(FacingDirection.Left, new KeyValuePair<FaceName, int>(FaceName.West, 0));
        face.neighboursRelationship.Add(FacingDirection.Right, new KeyValuePair<FaceName, int>(FaceName.East, 2));
        face.neighboursRelationship.Add(FacingDirection.Down, new KeyValuePair<FaceName, int>(FaceName.Back, 1));
        cubeFaces.Add(face);

        // WEST
        face = new CubeFace(FaceName.West);
        for (int i = 2 * faceSize; i < 3 * faceSize; i++) { face.grid.Add(grid[i].Substring(0, faceSize)); }
        face.neighboursRelationship.Add(FacingDirection.Right, new KeyValuePair<FaceName, int>(FaceName.Bot, 0));
        face.neighboursRelationship.Add(FacingDirection.Up, new KeyValuePair<FaceName, int>(FaceName.Front, 1));
        face.neighboursRelationship.Add(FacingDirection.Down, new KeyValuePair<FaceName, int>(FaceName.Back, 0));
        face.neighboursRelationship.Add(FacingDirection.Left, new KeyValuePair<FaceName, int>(FaceName.Top, 2));
        cubeFaces.Add(face);

        // BACK
        face = new CubeFace(FaceName.Back);
        for (int i = 3 * faceSize; i < 4 * faceSize; i++) { face.grid.Add(grid[i].Substring(0, faceSize)); }
        face.neighboursRelationship.Add(FacingDirection.Right, new KeyValuePair<FaceName, int>(FaceName.Bot, -1));
        face.neighboursRelationship.Add(FacingDirection.Up, new KeyValuePair<FaceName, int>(FaceName.West, 0));
        face.neighboursRelationship.Add(FacingDirection.Down, new KeyValuePair<FaceName, int>(FaceName.East, 0));
        face.neighboursRelationship.Add(FacingDirection.Left, new KeyValuePair<FaceName, int>(FaceName.Top, -1));
        cubeFaces.Add(face);

        StartCoroutine(coDay22_2(instruction));
        return "";
    }

    IEnumerator coDay22_2(string instruction)
    { 
        CubeFace startFace = cubeFaces.Find(x => x.MyFace == FaceName.Top);
        Vector2Int startingPoint = new(startFace.grid[0].IndexOf('.'), 0); ;
        FacingDirection startDirection = FacingDirection.Right;

        Debug.Log("Starting at " + startingPoint.ToString() + ", facing " + startDirection.ToString());

        Vector2Int curPos = startingPoint;
        curDir = startDirection;
        currentFace = startFace;

        int index = 0;
        int directionsCount = System.Enum.GetValues(typeof(FacingDirection)).Length;

        int safetyCount = 100000;
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
                int firstNonDigitIndex = Mathf.Min((instruction.IndexOf('R', index) < 0 ? instruction.Length : instruction.IndexOf('R', index)),
                                                    (instruction.IndexOf('L', index) < 0 ? instruction.Length : instruction.IndexOf('L', index)));

                //Debug.Log("index is " + index + " | fisrtnondigit is " + firstNonDigitIndex);
                //Debug.Log(", resulting substring is" + instruction.Substring(index, firstNonDigitIndex - index));
                int val = int.Parse(instruction.Substring(index, firstNonDigitIndex - index));
                curPos = DoMoveP2(curDir, curPos, val);

                Debug.Log("Moving " + curDir.ToString() + ", " + val + " times - Now current position is " + curPos.ToString() + " on face " + currentFace.MyFace.ToString());
                index = firstNonDigitIndex;
            }

            yield return new WaitForSeconds(0.2f);
            safetyCount--;
        }

        if (safetyCount <= 0)
            Debug.LogError("EXITED EARLIER");

        Debug.LogWarning("Finishing at " + curPos.ToString() + ", facing " + curDir.ToString() + " on FAce " + currentFace.MyFace.ToString());

        int total = 1000 * (curPos.y + 1 + faceSize) + 4 * (curPos.x + 1 + faceSize) + (int)curDir;
        Debug.LogWarning(total.ToString());
    }


    public Vector2Int DoMoveP2(FacingDirection direction, Vector2Int startPos, int moves)
    {
        Vector2Int result = Vector2Int.zero;

        switch (direction)
        {
            case FacingDirection.Right: result = DoMoveRightP2(startPos, moves); break;
            case FacingDirection.Down: result = DoMoveDownP2(startPos, moves); break;
            case FacingDirection.Left: result = DoMoveLeftP2(startPos, moves); break;
            case FacingDirection.Up: result = DoMoveUpP2(startPos, moves); break;
        }

        return result;
    }

    public Vector2Int DoMoveRightP2(Vector2Int startPos, int moves)
    {
        Debug.Log("On face " + currentFace.MyFace.ToString() + ", start moving RIGHT " + moves + " times from Pos " + startPos);
        Vector2Int result = new Vector2Int(startPos.x, startPos.y);

        for (int i = 0; i < moves; i++)
        {
            if (currentFace.grid[result.y].Length > result.x + 1)
            {
                if (currentFace.grid[result.y][result.x + 1] == '#')
                {
                    Debug.Log("BONK, after " + i.ToString() + " moves");
                    break;
                }

                result.x += 1;
            }
            else
            {
                CubeFace newFace = cubeFaces.Find(x => x.MyFace == currentFace.neighboursRelationship[FacingDirection.Right].Key);
                int faceModifier = (4 + currentFace.neighboursRelationship[FacingDirection.Right].Value) % 4;
                FacingDirection newDir = (FacingDirection)(((int)FacingDirection.Right + faceModifier) % 4);

                Vector2Int newPos = result;
                switch (faceModifier)
                {
                    case 0: newPos.x = 0; break;
                    case 1: newPos.x = result.y; newPos.y = 0; break;
                    case 2: newPos.y = (faceSize - 1) - result.y; break;
                    case 3: newPos.x = result.y; newPos.y = (faceSize - 1); break;
                    default: Debug.LogError("IMpossible, " + faceModifier); break;
                }

                if (newFace.grid[newPos.y][newPos.x] == '#')
                {
                    Debug.Log("BONK, on wrapping.. after " + i.ToString() + " moves");
                    break;
                }
                else
                {
                    currentFace = newFace; curDir = newDir;
                    return DoMoveP2(newDir, newPos, moves - 1 - i);
                }
            }
        }

        return result;
    }

    public Vector2Int DoMoveLeftP2(Vector2Int startPos, int moves)
    {
        Debug.Log("On face " + currentFace.MyFace.ToString() + ", start moving LEFT " + moves + " times from Pos " + startPos);
        Vector2Int result = new Vector2Int(startPos.x, startPos.y);

        for (int i = 0; i < moves; i++)
        {
            if (result.x > 0)
            {
                if (currentFace.grid[result.y][result.x - 1] == '#')
                {
                    Debug.Log("BONK, after " + i.ToString() + " moves");
                    break;
                }

                result.x -= 1;
            }
            else
            {
                CubeFace newFace = cubeFaces.Find(x => x.MyFace == currentFace.neighboursRelationship[FacingDirection.Left].Key);
                int faceModifier = (4 + currentFace.neighboursRelationship[FacingDirection.Left].Value) % 4;
                FacingDirection newDir = (FacingDirection)(((int)FacingDirection.Left + faceModifier) % 4);

                Vector2Int newPos = result;
                switch (faceModifier)
                {
                    case 0: newPos.x = (faceSize - 1); break;
                    case 1: newPos.x = result.y; newPos.y = (faceSize - 1); break;
                    case 2: newPos.y = (faceSize - 1) - result.y; break;
                    case 3: newPos.x = result.y; newPos.y = 0; break;
                    default: Debug.LogError("IMpossible, " + faceModifier); break;
                }

                if (newFace.grid[newPos.y][newPos.x] == '#')
                {
                    Debug.Log("BONK, on wrapping.. after " + i.ToString() + " moves");
                    break;
                }
                else
                {
                    currentFace = newFace; curDir = newDir;
                    return DoMoveP2(newDir, newPos, moves - 1 - i);
                }
            }
        }

        return result;
    }

    public Vector2Int DoMoveUpP2(Vector2Int startPos, int moves)
    {
        Debug.Log("On face " + currentFace.MyFace.ToString() + ", start moving UP " + moves + " times from Pos " + startPos);

        Vector2Int result = new Vector2Int(startPos.x, startPos.y);

        for (int i = 0; i < moves; i++)
        {
            if (result.y > 0)
            {
                if (currentFace.grid[result.y - 1][result.x] == '#')
                {
                    Debug.Log("BONK, after " + i.ToString() + "moves");
                    break;
                }

                result.y -= 1;
            }
            else
            {
                CubeFace newFace = cubeFaces.Find(x => x.MyFace == currentFace.neighboursRelationship[FacingDirection.Up].Key);
                int faceModifier = (4 + currentFace.neighboursRelationship[FacingDirection.Up].Value) % 4;
                FacingDirection newDir = (FacingDirection)(((int)FacingDirection.Up + faceModifier) % 4);

                Vector2Int newPos = result;
                switch (faceModifier)
                {
                    case 0: newPos.y = (faceSize - 1); break;
                    case 1: newPos.x = 0; newPos.y = result.x; break;
                    case 2: newPos.x = (faceSize - 1) - result.x; break;
                    case 3: newPos.x = (faceSize - 1); newPos.y = (faceSize - 1) - result.x; break;
                    default: Debug.LogError("IMpossible, " + faceModifier); break;
                }

                if (newFace.grid[newPos.y][newPos.x] == '#')
                {
                    Debug.Log("BONK, on wrapping.. after " + i.ToString() + " moves");
                    break;
                }
                else
                {
                    currentFace = newFace; curDir = newDir;
                    return DoMoveP2(newDir, newPos, moves - 1 - i);
                }
            }
        }

        return result;
    }

    public Vector2Int DoMoveDownP2(Vector2Int startPos, int moves)
    {
        Debug.Log("On face " + currentFace.MyFace.ToString() + ", start moving DOWN " + moves + " times from Pos " + startPos);

        Vector2Int result = new Vector2Int(startPos.x, startPos.y);

        for (int i = 0; i < moves; i++)
        {
            if (result.y + 1 < currentFace.grid.Count )
            {
                if (currentFace.grid[result.y + 1][result.x] == '#')
                {
                    Debug.Log("BONK, after " + i.ToString() + "moves");
                    break;
                }

                result.y += 1;
            }
            else
            {
                CubeFace newFace = cubeFaces.Find(x => x.MyFace == currentFace.neighboursRelationship[FacingDirection.Down].Key);
                int faceModifier = (4 + currentFace.neighboursRelationship[FacingDirection.Down].Value) % 4;
                FacingDirection newDir = (FacingDirection)(((int)FacingDirection.Down + faceModifier) % 4);

                Vector2Int newPos = result;
                switch (faceModifier)
                {
                    case 0: newPos.y = 0; break;
                    case 1: newPos.x = (faceSize - 1); newPos.y = result.x; break;
                    case 2: newPos.x = (faceSize - 1) - result.x; break;
                    case 3: newPos.x = 0; newPos.y = (faceSize - 1) - result.x; break;
                    default: Debug.LogError("IMpossible, " + faceModifier); break;
                }

                if (newFace.grid[newPos.y][newPos.x] == '#')
                {
                    Debug.Log("BONK, on wrapping.. after " + i.ToString() + " moves");
                    break;
                }
                else
                {
                    currentFace = newFace; curDir = newDir;
                    return DoMoveP2(newDir, newPos, moves - 1 - i);
                }
            }
        }

        return result;
    }


    public class CubeFace
    {
        public List<string> grid;
        public FaceName MyFace;
        public Dictionary<FacingDirection, KeyValuePair<FaceName, int>> neighboursRelationship;

        public CubeFace(FaceName side)
        {
            this.grid = new List<string>();
            this.MyFace = side;
            this.neighboursRelationship = new Dictionary<FacingDirection, KeyValuePair<FaceName, int>>();
        }
    }

    public enum FaceName { Top, Bot, West, East, Front, Back }
    public enum FacingDirection { Right, Down, Left, Up }
}
