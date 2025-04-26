using TMPro;
using UnityEngine;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        equationText.SetText(player.HitResultsBuilder.ToString());
        goalText.SetText($"Create: {player.GoalManager.NextGoal().ToString()}");
        scoreText.SetText($"Score: {player.GoalManager.GetScore().ToString()}");
        damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, 1 - (player.GetHealth() / player.GetMaxHealth()));
    }
}
