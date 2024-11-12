using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField]
    private TextMeshProUGUI textGoal;
    [SerializeField]
    private int goal;

	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Awake()
	{
        if (instance == null)
        {
            instance = this;
        }
	}

	void Start()
    {
        SetGoalText();
	}

    void SetGoalText()
    {
		textGoal.SetText(goal.ToString());
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DecreaseGoal()
    {
        goal--;
        SetGoalText();
	}
}
