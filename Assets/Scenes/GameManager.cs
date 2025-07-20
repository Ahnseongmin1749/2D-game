using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework.Constraints;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public Move player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public TextMeshProUGUI UIpoint;
    public TextMeshProUGUI UIStage;
    public GameObject RestartBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        UIpoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        //Change Stage
        if(stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE" + (stageIndex + 1);
        }
        else
        {
            //Game Clear
            //Player Control Lock
            Time.timeScale = 0;

            //Result UI
            Debug.Log("게임 클리어!");

            //Restart Button UI
            RestartBtn.SetActive(true);
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            RestartBtn.SetActive(true);
        }

            //Calcuate Point
            totalPoint += stagePoint;
        stagePoint = 0;
    }


    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //Player Reposition
            if(health > 1)
            {
                PlayerReposition();
            }

            //Health Dawn
            HealthDown();
        }
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else 
        {
            //All Health UI Off
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);

            //Player Die Effect
            player.OnDie();

            //Result UI
            Debug.Log("죽었습니다!");

            //Retry Button UI
            RestartBtn.SetActive(true);

        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-3.4f, 1, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
