using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class DayAnimation_2022_22 : DayAnimationScript
{

    public float timeBetweenInstructions = 2f;
    public float timeBetweenMoves = 0.5f;

    List<string> grid = new List<string>();

    static List<CubeFace> cubeFaces = new List<CubeFace>();
    static CubeFace currentFace;
    static int faceSize = 50;
    static FacingDirection curDir = FacingDirection.Right;

    public TMP_Text TopContent, FrontContent, BotContent, BackContent, WestContent, EastContent;

    public TMP_Text CurrentInfoLabel;

    public override IEnumerator part_2()
    {
        yield return new WaitForEndOfFrame();

        grid = new List<string>(input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split('\n').ToList());
        string instruction = input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[1];


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

        TopContent.text = cubeFaces.Find(x => x.MyFace == FaceName.Top).gridText;
        FrontContent.text = System.String.Join("\n", cubeFaces.Find(x => x.MyFace == FaceName.Front).grid);
        BotContent.text = System.String.Join("\n", cubeFaces.Find(x => x.MyFace == FaceName.Bot).grid);
        BackContent.text = System.String.Join("\n", cubeFaces.Find(x => x.MyFace == FaceName.Back).grid);
        WestContent.text = System.String.Join("\n", cubeFaces.Find(x => x.MyFace == FaceName.West).grid);
        EastContent.text = System.String.Join("\n", cubeFaces.Find(x => x.MyFace == FaceName.East).grid);

        StartCoroutine(coDay22_2(instruction));
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
        printCurrentChar(currentFace, curPos);

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
                yield return DoMoveP2(curDir, curPos, val);

                curPos = res;

                Debug.Log("Moving " + curDir.ToString() + ", " + val + " times - Now current position is " + curPos.ToString() + " on face " + currentFace.MyFace.ToString());
                index = firstNonDigitIndex;
            }

            printCurrentChar(currentFace, curPos);
            CurrentInfoLabel.text = "Face " + currentFace.MyFace.ToString() + "\nDir : " + curDir.ToString();
            yield return new WaitForSeconds(timeBetweenInstructions);
            safetyCount--;
        }

        if (safetyCount <= 0)
            Debug.LogError("EXITED EARLIER");

        Debug.Log("Finishing at " + curPos.ToString() + ", facing " + curDir.ToString() + " on FAce " + currentFace.MyFace.ToString());

        int total = 1000 * (curPos.y + 1 + faceSize) + 4 * (curPos.x + 1 + faceSize) + (int)curDir;
        Debug.LogWarning(total.ToString());
    }


    public IEnumerator DoMoveP2(FacingDirection direction, Vector2Int startPos, int moves)
    {
        yield return new WaitForEndOfFrame();

        switch (direction)
        {
            case FacingDirection.Right: yield return DoMoveRightP2(startPos, moves); break;
            case FacingDirection.Down: yield return DoMoveDownP2(startPos, moves); break;
            case FacingDirection.Left: yield return DoMoveLeftP2(startPos, moves); break;
            case FacingDirection.Up: yield return DoMoveUpP2(startPos, moves); break;
        }
    }

    static Vector2Int res = Vector2Int.zero;


    public IEnumerator DoMoveRightP2(Vector2Int startPos, int moves)
    {
        Debug.Log("On face " + currentFace.MyFace.ToString() + ", start moving RIGHT " + moves + " times from Pos " + startPos);

        Vector2Int result = new Vector2Int(startPos.x, startPos.y);
        hardPrintChar(result, curDir);

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
                hardPrintChar(result, FacingDirection.Right);
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
                    printCurrentChar(currentFace, null);
                    currentFace = newFace; curDir = newDir;
                    hardPrintChar(newPos, newDir);
                    yield return DoMoveP2(newDir, newPos, moves - 1 - i);
                    yield break;
                }
            }
            yield return new WaitForSeconds(timeBetweenMoves);

        }

        res = result;
    }

    public IEnumerator DoMoveLeftP2(Vector2Int startPos, int moves)
    {
        Debug.Log("On face " + currentFace.MyFace.ToString() + ", start moving LEFT " + moves + " times from Pos " + startPos);
        Vector2Int result = new Vector2Int(startPos.x, startPos.y);
        hardPrintChar(result, curDir);

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
                hardPrintChar(result, FacingDirection.Left);
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
                    printCurrentChar(currentFace, null);
                    currentFace = newFace; curDir = newDir;
                    hardPrintChar(newPos, newDir);
                    yield return DoMoveP2(newDir, newPos, moves - 1 - i);
                    yield break;
                }
            }
            yield return new WaitForSeconds(timeBetweenMoves);

        }

        res = result;
    }

    public IEnumerator DoMoveUpP2(Vector2Int startPos, int moves)
    {
        yield return new WaitForSeconds(timeBetweenMoves);

        Debug.Log("On face " + currentFace.MyFace.ToString() + ", start moving UP " + moves + " times from Pos " + startPos);

        Vector2Int result = new Vector2Int(startPos.x, startPos.y);
        hardPrintChar(result, curDir);

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
                hardPrintChar(result, FacingDirection.Up);
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
                    printCurrentChar(currentFace, null);
                    currentFace = newFace; curDir = newDir;
                    hardPrintChar(newPos, newDir);
                    yield return DoMoveP2(newDir, newPos, moves - 1 - i);
                    yield break;
                }
            }
            yield return new WaitForSeconds(timeBetweenMoves);

        }

        res = result;
    }

    public IEnumerator DoMoveDownP2(Vector2Int startPos, int moves)
    {
        Debug.Log("On face " + currentFace.MyFace.ToString() + ", start moving DOWN " + moves + " times from Pos " + startPos);

        Vector2Int result = new Vector2Int(startPos.x, startPos.y);
        hardPrintChar(result, curDir);

        for (int i = 0; i < moves; i++)
        {
            if (result.y + 1 < currentFace.grid.Count)
            {
                if (currentFace.grid[result.y + 1][result.x] == '#')
                {
                    Debug.Log("BONK, after " + i.ToString() + "moves");
                    break;
                }

                result.y += 1;
                hardPrintChar(result, FacingDirection.Down);
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
                    printCurrentChar(currentFace, null);

                    currentFace = newFace; curDir = newDir;
                    hardPrintChar(newPos, newDir);
                    yield return DoMoveP2(newDir, newPos, moves - 1 - i);
                    yield break;
                }
            }
            yield return new WaitForSeconds(timeBetweenMoves);
        }

        res = result;
    }

    void hardPrintChar(Vector2Int result, FacingDirection dir)
    {
        char c = 'x';
        switch (dir)
        {
            case FacingDirection.Down: c = 'v'; break;
            case FacingDirection.Up: c = '^'; break;
            case FacingDirection.Left: c = '<'; break;
            case FacingDirection.Right: c = '>'; break;
        }

        currentFace.grid[result.y] = currentFace.grid[result.y].Substring(0, result.x) + c + (currentFace.grid[result.y].Length > result.x + 1 ? currentFace.grid[result.y].Substring(result.x + 1) : "");
        printCurrentChar(currentFace, result);
    }
    void printCurrentChar(CubeFace face, Vector2Int? pos)
    {


        TMP_Text textLabel = null;
        switch (face.MyFace)
        {
            case FaceName.Top: textLabel = TopContent; break;
            case FaceName.Front: textLabel = FrontContent; break;
            case FaceName.Bot: textLabel = BotContent; break;
            case FaceName.Back: textLabel = BackContent; break;
            case FaceName.West: textLabel = WestContent; break;
            case FaceName.East: textLabel = EastContent; break;
        }
        if (pos == null)
        {
            textLabel.text = face.gridText;
        }
        else
        {
            int index = pos.Value.y * (faceSize + 1) + pos.Value.x;
            string rawText = face.gridText;
            textLabel.text = rawText.Substring(0, index) + "<color=red>@</color>" + (index + 1 < rawText.Length ? rawText.Substring(index + 1) : "");
        }
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

        public string gridText {  get { return System.String.Join("\n", grid); } }
    }

    public enum FaceName { Top, Bot, West, East, Front, Back }
    public enum FacingDirection { Right, Down, Left, Up }
}
