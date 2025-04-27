using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager
{
    float currentScore = 0;
    List<int> goals;
    int GoalToMoveLevel = -1;

    public GoalManager()
    {
        goals = new List<int>();

        goals.Add(Random.Range(0, 5) + 1);
        goals.Add(Random.Range(0, 10) + 1);
        goals.Add(Random.Range(5, 15) + 1);
        goals.Add(Random.Range(10, 20) + 1);

        if (SceneManager.GetActiveScene().name.ToLower() == "lvl1")
        {
            GoalToMoveLevel = 50;
        }

        if (SceneManager.GetActiveScene().name.ToLower() == "lvl2")
        {
            GoalToMoveLevel = 100;
        }

        if (SceneManager.GetActiveScene().name.ToLower() == "lvl3")
        {
            GoalToMoveLevel = 150;
        }

        if (SceneManager.GetActiveScene().name.ToLower() == "lvl4")
        {
            GoalToMoveLevel = 200;
        }

        if (SceneManager.GetActiveScene().name.ToLower() == "lvl5")
        {
            GoalToMoveLevel = -1;
        }
    }

    public int ToNextLevel()
    {
        return GoalToMoveLevel;
    }

    public int NextGoal()
    {
        if (goals.Count == 0)
        {
            return 0;
        }

        return goals[0];
    }

    public float GetScore()
    {
        return Mathf.Round(currentScore);
    }

    public bool CheckGoal(int result)
    {
        if (goals.Count == 0)
        {
            return false;
        }

        int nextGoal = goals[0];
        float distance = Mathf.Abs((float)nextGoal - (float)result);

        // Allow 10% margin on the result to help with bigger numbers.
        if (distance <= nextGoal / 10)
        {
            // Reward the player a % of what they achieved
            currentScore += (int)Mathf.Round((float)nextGoal * (1.0f - (float)distance));

            Debug.Log($"Achieved: {goals[0]}");
            goals.RemoveAt(0);

            goals.Add(Random.Range(nextGoal - (int)Mathf.Round(nextGoal * 0.25f), nextGoal + (int)Mathf.Round(nextGoal * 2.0f)) + 1);

            return true;
        }

        return false;
    }
}
