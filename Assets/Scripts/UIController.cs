using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI equationText;

    [SerializeField]
    TextMeshProUGUI goalText;

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
        goalText.SetText(player.GoalManager.NextGoal().ToString());
    }
}
