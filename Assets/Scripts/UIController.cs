using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI equationText;

    [SerializeField]
    TextMeshProUGUI goalText;

    [SerializeField]
    TextMeshProUGUI scoreText;

    [SerializeField]
    Image damageImage;

    PlayerController player;

    bool survive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (SceneManager.GetActiveScene().name.ToLower() == "lvl4")
        {
            survive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        equationText.SetText(player.HitResultsBuilder.ToString());
        goalText.SetText($"Create: {player.GoalManager.NextGoal().ToString()}");
        damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, 1 - (player.GetHealth() / player.GetMaxHealth()));

        if (!survive)
        {
            scoreText.SetText($"Score: {player.GoalManager.GetScore()} / {player.GoalManager.ToNextLevel()}");
        }
        else
        {
            scoreText.SetText($"Score: {player.GoalManager.GetScore()} / SURVIVE");
        }


        if (player.GoalManager.GetScore() > player.GoalManager.ToNextLevel())
        {
            if (SceneManager.GetActiveScene().name.ToLower() == "lvl1")
            {
                SceneManager.LoadScene("lvl2");
            }

            if (SceneManager.GetActiveScene().name.ToLower() == "lvl2")
            {
                SceneManager.LoadScene("lvl3");
            }

            if (SceneManager.GetActiveScene().name.ToLower() == "lvl3")
            {
                SceneManager.LoadScene("lvl4");
            }
        }
    }
}
