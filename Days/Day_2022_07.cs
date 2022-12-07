using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Linq;

public class Day_2022_07 : DayScript2022
{
    #region Classes 
        class File
        {
            public string name;
            public double size;
            public File(string n, double s) { name = n; size = s; }
            public string ToString(NumberFormatInfo nfi) { return "File '" + name + "' of size " + size.ToString("N", nfi); }
        }

        class Dir
        {
            public Dir parent;
            public string name;
            public List<File> files;
            public List<Dir> dirs;
            double size = -1;

            public Dir(Dir dad, string n)
            {
                parent = dad;
                name = n;
                files = new List<File>();
                dirs = new List<Dir>();
                size = -1;
            }

            public double getSize(int offset = 0)
            {
                // if size >= 0, that means size has already be computed so no need to do that again as we have no filesystem change after init
                if (size < 0)
                {
                    double res = 0;
                    files.ForEach(x => res += x.size);
                    dirs.ForEach(x => res += x.getSize(offset + 1));

                    size = res;
                    //Debug.Log(Tools2022.WriteOffset(offset) + "Dir " + name + " has size " + size);
                }

                return size;
            }
        }
    #endregion

    protected override string part_1()
    {
        Dir mainDir = initFilesystem(); // read input and init FS
        mainDir.getSize();  //calc size of all dirs/subdirs

        return getSumOfSmallDirs(mainDir, 100000).ToString();
    }

    protected override string part_2()
    {
        Dir mainDir = initFilesystem(); // read input and init FS
        mainDir.getSize();  //calc size of all dirs/subdirs

        Debug.Log("Main dir size is " + mainDir.getSize() + " so, we look for smallest > " + (30000000 - (70000000 - mainDir.getSize())).ToString());

        // // Display filesystem tree
        // Debug.Log(" ------------------------------------------- ");
        // drawFS(mainDir);
        // Debug.Log(" ------------------------------------------- ");

        return getSmallestDirSizeBiggerThan(30000000 - (70000000 - mainDir.getSize()), mainDir).ToString();
    }

    /// <summary>
    /// Read input and create filesystem tree
    /// </summary>
    /// <returns>Main dir contianing all others</returns>
    Dir initFilesystem()
    {
        Dir mainDir = new Dir(null, "/");
        Dir currentParent = mainDir;
        foreach (string line in _input.Substring(_input.IndexOf('\n') + 1).Split('\n', System.StringSplitOptions.RemoveEmptyEntries).ToList())
        {
            if (line.StartsWith('$'))
            {   // we are in command lines
                if (line.StartsWith("$ cd "))
                {
                    string dirName = line.Substring(5);
                    if (dirName == "..")
                    {
                        // back to parent
                        currentParent = currentParent.parent;
                    }
                    else
                    {
                        // Creating new directory
                        Dir newDir = new Dir(currentParent, dirName);
                        if (currentParent != null)
                        {
                            currentParent.dirs.Add(newDir);
                        }
                        currentParent = newDir;
                    }
                }
                // if "ls" line, do nothing
            }
            else
            {   // we are in listing lines
                if (!line.StartsWith("dir"))        // don't need to do anything when listing dir, they will be created on "cd <dirName>"
                {
                    string[] data = line.Split(' ');
                    File f = new File(data[1], double.Parse(data[0]));
                    currentParent.files.Add(f);
                }
            }
        }

        return mainDir;
    }
    
    /// <summary>
    /// get sum of all child dirs which are smaller than threshold
    /// Recursive function 
    /// </summary>
    /// <param name="currentDir">current directory of compute</param>
    /// <param name="threshold"> double that are size limit for dir size</param>
    /// <returns>sum of all small dirs</returns>
    double getSumOfSmallDirs(Dir currentDir, double threshold)
    {
        double res = 0;

        if (currentDir.dirs.Count > 0)
        {
            foreach (Dir d in currentDir.dirs)
            {
                res += getSumOfSmallDirs(d, threshold);
            }
        }

        if (currentDir.getSize() < threshold)
            res += currentDir.getSize();

        return res;
    }

    /// <summary>
    /// Give smallest dir size that is above a given threshold
    /// recursive function
    /// </summary>
    /// <param name="threshold">double that we compare to</param>
    /// <param name="currentDir">dir currently analyzed</param>
    /// <returns>return a double > threshold or -1 if no dir/subdir that is >threshold</returns>
    double getSmallestDirSizeBiggerThan(double threshold, Dir currentDir)
    {
        if (currentDir.getSize() < threshold)
            return -1;

        double res = double.MaxValue;

        if (currentDir.getSize() < res)
            res = currentDir.getSize();

        if (currentDir.dirs.Count > 0)
        {
            foreach (Dir d in currentDir.dirs)
            {
                double score = getSmallestDirSizeBiggerThan(threshold, d);
                if (score > 0 && score < res)
                    res = score;
            }
        }

        return res;
    }



    /// <summary>
    /// Debug function to draw filesystem tree
    /// </summary>
    /// <param name="currentDir">current dir to display</param>
    /// <param name="offset">line offset to have a pretty tree in logs</param>
    void drawFS(Dir currentDir, int offset = 0)
    {
        // Just defining a culture info to have pleasant way to display big numbers, with point every 3 digits like 1.234.567.890
        var nfi = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat;
        nfi.NumberDecimalDigits = 0;             //Keep the ".00" from appearing
        nfi.NumberGroupSeparator = ".";          //Set the group separator to a space
        nfi.NumberGroupSizes = new int[] { 3 };  //Groups of 3 digits


        Debug.Log(Tools2022.WriteOffset(offset) + " DIR " + currentDir.name + " (size=" + currentDir.getSize().ToString("N", nfi) + ")"
            + System.Environment.NewLine + "Files : " + currentDir.files.Select(x => x.size).Sum().ToString("N", nfi) + " | Dirs : " + currentDir.dirs.Select(x => x.getSize()).Sum().ToString("N", nfi)
            );

        foreach (File f in currentDir.files) { Debug.Log(Tools2022.WriteOffset(offset + 1) + f.ToString(nfi)); }
        foreach (Dir d in currentDir.dirs) { drawFS(d, offset + 1); }
    }

}
