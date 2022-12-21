using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_21 : DayScript2022
{

    protected override string part_1()
    {
        List<Monkey> monkeys = new List<Monkey>();

        foreach (string instruction in _input.Split('\n'))
        {
            monkeys.Add(new Monkey(instruction));
        }

        Monkey root = monkeys.Find(x => x.name == "root");
        if (root != null)
            root.Solve(monkeys);

        return root.result.ToString();
    }

    protected override string part_2()
    {
        List<Monkey> monkeys = new List<Monkey>();

        foreach (string instruction in _input.Split('\n'))
        {
            monkeys.Add(new Monkey(instruction));
        }

        Monkey root = monkeys.Find(x => x.name == "root");
        Monkey mk1 = monkeys.Find(x => x.name == root.Monkey1);
        Monkey mk2 = monkeys.Find(x => x.name == root.Monkey2);

        mk1.SolveP2(monkeys);
        mk2.SolveP2(monkeys);

        Monkey mkToEval = mk1.isSolved ? mk2 : mk1;
        double mkResult = mk1.isSolved ? mk1.result : mk2.result;

        double result = mkToEval.EvalWithResult(monkeys, mkResult);

        return result.ToString();
    }

    class Monkey
    {
        public string name;
        public bool isSolved;
        public double result;
        public char Operande;
        public string Monkey1;
        public string Monkey2;

        public Monkey(string input)
        {
            this.name = input.Substring(0, 4);
            if (double.TryParse(input.Substring(6), out this.result))
            {
                this.isSolved = true;
            }
            else
            {
                this.isSolved = false;
                this.Monkey1 = input.Substring(6, 4);
                this.Operande = input[11];
                this.Monkey2 = input.Substring(13);
            }
        }

        public Monkey(Monkey copy)
        {
            this.name = copy.name;
            this.isSolved = copy.isSolved;
            this.result = copy.result;
            this.Monkey1 = copy.Monkey1;
            this.Monkey2 = copy.Monkey2;
            this.Operande = copy.Operande;
        }

        public override string ToString()
        {
            if (isSolved)
                return "Monkey " + this.name + " is solved and yells " + this.result;
            else
                return "Monkey " + this.name + " is NOT solved and is yelling " + this.Monkey1 + this.Operande.ToString() + this.Monkey2;
        }

        public double Solve(List<Monkey> monkeys)
        {
            if (this.isSolved)
                return this.result;

            double mk1 = monkeys.Find(x => x.name == this.Monkey1).Solve(monkeys);
            double mk2 = monkeys.Find(x => x.name == this.Monkey2).Solve(monkeys);

            this.isSolved = true;
            switch (this.Operande)
            {
                case '+': this.result = mk1 + mk2; break;
                case '-': this.result = mk1 - mk2; break;
                case '*': this.result = mk1 * mk2; break;
                case '/': this.result = mk1 / mk2; break;
                default:
                    Debug.LogError("wtf ! " + this.ToString()); break;
            }
            
            this.isSolved = true;
            return this.result;
        }

        public string SolveP2(List<Monkey> monkeys)
        {
            if (this.name == "humn")
            {
                this.isSolved = false;
                return "X";
            }
            
            if (this.isSolved)
                return this.result.ToString();

            string mk1 = monkeys.Find(x => x.name == this.Monkey1).SolveP2(monkeys);
            string mk2 = monkeys.Find(x => x.name == this.Monkey2).SolveP2(monkeys);

            double m1 = -1, m2 = -1;
            if (double.TryParse(mk1, out m1) && double.TryParse(mk2, out m2))
            {
                this.isSolved = true;
                switch (this.Operande)
                {
                    case '+': this.result = m1 + m2; break;
                    case '-': this.result = m1 - m2; break;
                    case '*': this.result = m1 * m2; break;
                    case '/': this.result = m1 / m2; break;
                    default:
                        Debug.LogError("wtf ! " + this.ToString()); break;
                }
                return this.result.ToString();
            }
            else
            {
                this.isSolved = false;
                return "(" + mk1 + this.Operande + mk2 + ")";
            }
        }

        public double EvalWithResult(List<Monkey> monkeys, double targetVal)
        {
            if (this.name == "humn")
                return targetVal;

            Monkey mk1 = monkeys.Find(x => x.name == this.Monkey1);
            Monkey mk2 = monkeys.Find(x => x.name == this.Monkey2);

            Monkey mkToEval = mk1.isSolved ? mk2 : mk1;
            double mkResult = mk1.isSolved ? mk1.result : mk2.result;

            double newTargetVal = 0;
            switch (this.Operande)
            {
                case '+': newTargetVal = targetVal - mkResult; break;
                case '-': newTargetVal = (mk1.isSolved ? mkResult - targetVal : targetVal + mkResult) ; break;
                case '*': newTargetVal = targetVal / mkResult; break;
                case '/': newTargetVal = (mk1.isSolved ? mkResult / targetVal : targetVal * mkResult); break;
                default:
                    Debug.LogError("wtf ! " + this.ToString()); break;
            }

            return mkToEval.EvalWithResult(monkeys, newTargetVal);
        }
    }
}
