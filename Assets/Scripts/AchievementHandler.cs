using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementHandler : MonoBehaviour
{
    
    public float xOffset;

    public void Awake() {
        Transform c = GameObject.Find("Canvas").transform;
        xOffset *= c.localScale.x;
        transform.SetParent(c, false);
    }

    public void SetStaticData(float x, float y, Achievement a) {
        Transform c = GameObject.Find("Canvas").transform;
        transform.position = new Vector2(x * c.localScale.x, y * c.localScale.y);
        transform.GetChild(0).GetComponent<Text>().text = a.rawName + " (" + (a.currentLevel - 1) + "/" + a.numLevels + ")";
        if (a.IsMaxLevel()) {
            transform.GetChild(1).GetComponent<Text>().text = a.description;
        } else {
            transform.GetChild(1).GetComponent<Text>().text = a.description + " (" + a.currentValue + "/" + a.nextMilestone + ")";
        }
    }

    public void LoadAchievement(string achievementName, string achievementDescription) {
        transform.position -= new Vector3(xOffset, 0, 0);
        transform.GetChild(0).GetComponent<Text>().text = achievementName;
        transform.GetChild(1).GetComponent<Text>().text = achievementDescription;
        StartCoroutine(ShowAchievementAndHide());
    }

    IEnumerator ShowAchievementAndHide() {
        int activeAchievements = GameObject.FindGameObjectsWithTag("Achievement").Length;
        yield return new WaitForSeconds((activeAchievements - 1) * 6.7f);
        float delay = 0.02f;
        for (int i = 0; i < 30; i++) {
            transform.position += new Vector3(xOffset / 30, 0, 0);
            yield return new WaitForSeconds(delay);
            delay += 0.0005f;
            if (i == 20) {
                GameObject.Find("PublicVariables").GetComponent<PublicVariables>().AchieveSound();
            }
        }
        delay = 0.03f;
        yield return new WaitForSeconds(5);
        for (int i = 0; i < 20; i++) {
            transform.position -= new Vector3(xOffset / 20, 0, 0);
            yield return new WaitForSeconds(delay);
            delay -= 0.0007f;
        }
        Destroy(gameObject);
    }

}
