using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyNumeric
{
    One,
    Two,
    Three,
    Four,
    Five,
    Add,
    Subtract,
    Multiply,
    Divide,
    Equals,
    EnumCount
}

public class HitResultsBuilder
{
    EnemyNumeric lastSymbol = EnemyNumeric.EnumCount;
    EnemyNumeric nextSymbol = EnemyNumeric.EnumCount;
    Queue<EnemyNumeric> symbolQueue = new Queue<EnemyNumeric>();
    int currentNum;
    int nextNum;
    bool firstPass = true;
    bool completedNum = false;

    public void Symbol(EnemyNumeric symbol)
    {
        symbolQueue.Enqueue(symbol);
    }

    public void HandleNumber(int num)
    {
        try
        {
            int tempNum = (firstPass) ? currentNum : nextNum;
            if (tempNum == 0)
            {
                tempNum = num;
            }
            else
            {
                string numStr = tempNum.ToString() + num.ToString();
                tempNum = int.Parse(numStr);
            }

            if (firstPass)
            {
                currentNum = tempNum;
            }
            else
            {
                nextNum = tempNum;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception thrown: {e.ToString()}");
        }
    }

    public void Process(EnemyNumeric symbol)
    {
        switch (symbol)
        {
            case EnemyNumeric.One:
                HandleNumber(1);
                break;
            case EnemyNumeric.Two:
                HandleNumber(2);
                break;
            case EnemyNumeric.Three:
                HandleNumber(3);
                break;
            case EnemyNumeric.Four:
                HandleNumber(4);
                break;
            case EnemyNumeric.Five:
                HandleNumber(5);
                break;
            case EnemyNumeric.Add:
            case EnemyNumeric.Subtract:
            case EnemyNumeric.Multiply:
            case EnemyNumeric.Divide:
                if (currentNum != 0 && nextNum != 0 && nextSymbol != EnemyNumeric.EnumCount && !firstPass)
                {
                    switch (nextSymbol)
                    {
                        case EnemyNumeric.Add:
                            currentNum += nextNum;
                            break;
                        case EnemyNumeric.Subtract:
                            currentNum -= nextNum;
                            break;
                        case EnemyNumeric.Multiply:
                            currentNum *= nextNum;
                            break;
                        case EnemyNumeric.Divide:
                            currentNum /= nextNum;
                            break;
                        default:
                            break;
                    }

                    nextSymbol = EnemyNumeric.EnumCount;
                    nextNum = 0;
                }
                lastSymbol = symbol;
                nextSymbol = symbol;
                if (firstPass)
                {
                    firstPass = false;
                }
                break;
            case EnemyNumeric.Equals:
            case EnemyNumeric.EnumCount:
            default:
                break;
        }
    }

    public int Result()
    {
        return Result(symbolQueue);
    }

    public int Result(Queue<EnemyNumeric> queue)
    {
        firstPass = true;

        bool ready = true;
        do
        {
            ready = true;
            if (queue.Count == 0)
            {
                break;
            }

            // Pop items off of the front until we are starting with a number.
            if (queue.Peek() >= EnemyNumeric.Add)
            {
                ready = false;
                queue.Dequeue();
            }
        }
        while (!ready);

        while (queue.Count > 0)
        {
            EnemyNumeric next;
            queue.TryDequeue(out next);
            Process(next);
        }

        switch (lastSymbol)
        {
            case EnemyNumeric.Add:
                currentNum += nextNum;
                break;
            case EnemyNumeric.Subtract:
                currentNum -= nextNum;
                break;
            case EnemyNumeric.Multiply:
                if (nextNum != 0)
                {
                    currentNum *= nextNum;
                }
                break;
            case EnemyNumeric.Divide:
                if (nextNum != 0)
                {
                    currentNum /= nextNum;
                }
                break;
            default:
                break;
        }

        int result = currentNum;
        currentNum = 0;
        nextNum = 0;
        nextSymbol = EnemyNumeric.EnumCount;

        return result;
    }

    private string EnumToString(EnemyNumeric numeric)
    {
        switch (numeric)
        {
            case EnemyNumeric.One:
                return "1";

            case EnemyNumeric.Two:
                return "2";

            case EnemyNumeric.Three:
                return "3";

            case EnemyNumeric.Four:
                return "4";

            case EnemyNumeric.Five:
                return "5";

            case EnemyNumeric.Add:
                return "+";

            case EnemyNumeric.Subtract:
                return "-";

            case EnemyNumeric.Multiply:
                return "x";

            case EnemyNumeric.Divide:
                return "÷";

            case EnemyNumeric.Equals:
                return "=";

            case EnemyNumeric.EnumCount:
            default:
                return "invalid";
        }
    }

    public override string ToString()
    {
        string result = "";

        Queue<EnemyNumeric> burnQueue = new Queue<EnemyNumeric>(symbolQueue);
        while (burnQueue.Count > 0)
        {
            EnemyNumeric next;
            burnQueue.TryDequeue(out next);

            result += EnumToString(next);
        }

        result += $"={Result(new Queue<EnemyNumeric>(symbolQueue))}";

        return result;
    }
}
