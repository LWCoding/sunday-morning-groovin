using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchieveController : MonoBehaviour
{

    private PublicVariables p;
    public GameObject achievementPrefab;

    public void Awake() {
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        p.LoadAchievements();
    }

    public void Start() {
        int i = 0;
        foreach (Achievement a in p.achievements) {
            GameObject achievementObj = Instantiate(achievementPrefab);
            achievementObj.transform.localScale = new Vector3(1, 1, 1);
            a.ForceGrabData();
            if (i < 4) {
                achievementObj.GetComponent<AchievementHandler>().SetStaticData(275, 620 - (i * 174), a);
            } else {
                achievementObj.GetComponent<AchievementHandler>().SetStaticData(905, 620 - ((i - 4) * 174), a);
            }
            if (a.IsMaxLevel()) {
                achievementObj.transform.GetChild(2).GetComponent<Image>().color += new Color(0, 0, 0, 1);
            }
            i++;
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)) {
            SceneManager.LoadScene("Start");
        }
    }

}
