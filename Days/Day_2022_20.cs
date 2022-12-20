using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_20 : DayScript2022
{
    protected override string part_1()
    {
        List<IndexedElement> elements = new List<IndexedElement>();
        int index = 0;
        foreach (int val in _input.Split('\n').Select(x => int.Parse(x)))
        {
            elements.Add(new IndexedElement() { id = index, value = val, currentIndex = index });
            
            index++;
        }

        int ec = elements.Count;

        Debug.Log(System.String.Join(", ", elements.Select(x => x.value)));
        Debug.Log("Count : " + ec);

        for (int i = 0; i < elements.Count; i++)
        {
            IndexedElement elmt = elements.Find(x => x.id == i);

            int oldIndex = elmt.currentIndex;
            double newIndex = oldIndex + elmt.value;
            if (newIndex >= ec)
                newIndex = (newIndex + 1) % ec;
            if (newIndex < 0)
                newIndex = ((newIndex-1)%ec + ec)%ec;

            Debug.Log("Moving element " + elmt.value  + " | old Index : " + oldIndex + " | " + newIndex);


            if (oldIndex < newIndex)
            {
                for (int k = oldIndex + 1; k <= newIndex; k++)
                {
                    elements[k].currentIndex -= 1;
                }
            }
            else if (oldIndex > newIndex)
            {
                for (int k = oldIndex - 1; k >= newIndex; k--)
                {
                    elements[k].currentIndex += 1;
                }
            }
            elmt.currentIndex = (int)newIndex;


            elements.Sort(delegate (IndexedElement e1, IndexedElement e2) { return e1.currentIndex.CompareTo(e2.currentIndex); });

            //elements[(oldIndex + 1) % ec].prev = elements[(oldIndex + ec - 1) % ec];

            //Debug.Log(System.String.Join(", ", elements.Select(x => x.value)));
        }

        int zeroIndex = elements.Find(x => x.value == 0).currentIndex;
        double first = elements.Find(x => x.currentIndex == (zeroIndex + 1000) % ec).value;
        double second = elements.Find(x => x.currentIndex == (zeroIndex + 2000) % ec).value;
        double third = elements.Find(x => x.currentIndex == (zeroIndex + 3000) % ec).value;

        double total = first + second + third;
        Debug.Log("1 : " + first + " | 2 : " + second + " | 3 : " + third + "  ==> " + total.ToString());

        return "";
    }

    protected override string part_2()
    {
        int multiplier = 811589153;
        List<IndexedElement> elements = new List<IndexedElement>();
        int index = 0;
        foreach (double val in _input.Split('\n').Select(x => double.Parse(x)))
        {
            elements.Add(new IndexedElement() { id = index, value = val*multiplier, currentIndex = index });
            index++;
        }

        int ec = elements.Count;

        Debug.Log(System.String.Join(", ", elements.Select(x => x.value)));
        Debug.Log("Count : " + ec);

        for (int mixer = 0; mixer < 10; mixer++)
        {

            for (int i = 0; i < elements.Count; i++)
            {
                IndexedElement elmt = elements.Find(x => x.id == i);

                int oldIndex = elmt.currentIndex;
                double newIndex = oldIndex + elmt.value;
                if (newIndex >= ec)
                    newIndex = (newIndex + 1) % ec;
                if (newIndex < 0)
                    newIndex = ((newIndex - 1) % ec + ec) % ec;

                //Debug.Log("Moving element " + elmt.value + " | old Index : " + oldIndex + " | " + newIndex);


                if (oldIndex < newIndex)
                {
                    for (int k = oldIndex + 1; k <= newIndex; k++)
                    {
                        elements[k].currentIndex -= 1;
                    }
                }
                else if (oldIndex > newIndex)
                {
                    for (int k = oldIndex - 1; k >= newIndex; k--)
                    {
                        elements[k].currentIndex += 1;
                    }
                }
                elmt.currentIndex = (int)newIndex;


                elements.Sort(delegate (IndexedElement e1, IndexedElement e2) { return e1.currentIndex.CompareTo(e2.currentIndex); });

                //elements[(oldIndex + 1) % ec].prev = elements[(oldIndex + ec - 1) % ec];

            }
            Debug.Log("After " + (mixer + 1).ToString() + " arrangement:");
            Debug.Log(System.String.Join(", ", elements.Select(x => x.value)));
        }

        int zeroIndex = elements.Find(x => x.value == 0).currentIndex;
        double first = elements.Find(x => x.currentIndex == (zeroIndex + 1000) % ec).value;
        double second = elements.Find(x => x.currentIndex == (zeroIndex + 2000) % ec).value;
        double third = elements.Find(x => x.currentIndex == (zeroIndex + 3000) % ec).value;

        double total = first + second + third;
        Debug.Log("1 : " + first + " | 2 : " + second + " | 3 : " + third + "  ==> " + total.ToString());

        return total.ToString();
    }

    public class IndexedElement
    {
        public int id;
        public double value;
        public int currentIndex;

        public override string ToString()
        {
            return "Element " + value + " has original index " + id + " and current index " + currentIndex;
        }
    }
}
