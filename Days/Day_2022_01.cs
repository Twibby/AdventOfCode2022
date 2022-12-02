using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_01 : DayScript2022
{
    protected override string part_1()
    {
        List<int> inputs = _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split('\n', System.StringSplitOptions.RemoveEmptyEntries)
                            .Select(y => int.Parse(y)).Sum()
                     ).ToList();

        inputs.Sort();

        return inputs[inputs.Count - 1].ToString();

        // Readable and pleasant way \\
        //
        //  int maxCal = 0;
        //  foreach (string elfSnacks in _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries))
        //  {
        //      int totalCalory = 0;
        //      foreach (string snack in elfSnacks.Split('\n', System.StringSplitOptions.RemoveEmptyEntries))
        //      {
        //          totalCalory += int.Parse(snack);
        //      }
        //      maxCal = Mathf.Max(totalCalory, maxCal);
        //  }
        //  return maxCal.ToString();
    }

    protected override string part_2()
    {
        List<int> inputs = _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split('\n', System.StringSplitOptions.RemoveEmptyEntries).Select(y => int.Parse(y)).Sum()).ToList();

        inputs.Sort();
        inputs.Reverse();

        return (inputs[0] + inputs[1] + inputs[2]).ToString();
    }
}
