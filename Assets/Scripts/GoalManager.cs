using System.Collections.Generic;
using UnityEngine;

public class GoalManager
{
    List<int> goals;

    public GoalManager()
    {
        goals = new List<int>();

        goals.Add(Random.Range(0, 5) + 1);
        goals.Add(Random.Range(0, 10) + 1);
        goals.Add(Random.Range(5, 15) + 1);
        goals.Add(Random.Range(5, 50) + 1);
        goals.Add(Random.Range(0, 100) + 1);
        goals.Add(Random.Range(0, 150) + 1);
        goals.Add(Random.Range(0, 200) + 1);
        goals.Add(Random.Range(0, 300) + 1);
        goals.Add(Random.Range(0, 400) + 1);
        goals.Add(Random.Range(0, 500) + 1);
    }

    public int NextGoal()
    {
        return goals[0];
    }

    public bool CheckGoal(int result)
    {
        int nextGoal = goals[0];
        if (nextGoal == result)
        {
            Debug.Log($"Achieved: {goals[0]}");
            goals.RemoveAt(0);
            return true;
        }

        return false;
    }
}
