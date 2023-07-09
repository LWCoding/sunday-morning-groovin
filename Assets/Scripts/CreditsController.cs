using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    public float scrollSpeed;
    private PublicVariables p;
    void Start() {
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)) {
            SceneManager.LoadScene("Start");
        }  
        float canvasHeight = GameObject.Find("Canvas").GetComponent<RectTransform>().rect.height * GameObject.Find("Canvas").transform.localScale.y;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
            GameObject.Find("Sentinel").transform.position += new Vector3(0, scrollSpeed * (canvasHeight / 720), 0);
            GameObject.Find("CreditText").transform.position += new Vector3(0, scrollSpeed * (canvasHeight / 720), 0);
        }
        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && GameObject.Find("CreditText").transform.position.y > GameObject.Find("CreditTextDefault").transform.position.y) {
            GameObject.Find("Sentinel").transform.position -= new Vector3(0, 2 * scrollSpeed * (canvasHeight / 720), 0);
            GameObject.Find("CreditText").transform.position -= new Vector3(0, 2 * scrollSpeed * (canvasHeight / 720), 0);
        }
        if (GameObject.Find("Sentinel").transform.position.y > canvasHeight) {
            GameObject.Find("CreditText").transform.position = GameObject.Find("CreditTextDefault").transform.position;
            GameObject.Find("Sentinel").transform.position = GameObject.Find("SentinelDefault").transform.position;
            p.achievements[7].Increment(1);
            if (p.achievements[7].NextLevel()) {
                p.RenderAchievement(p.achievements[7].name, p.achievements[7].description);
            }
        }
    }
    void FixedUpdate() {
        GameObject.Find("Sentinel").transform.position += new Vector3(0, scrollSpeed * (GameObject.Find("Canvas").GetComponent<RectTransform>().rect.height * GameObject.Find("Canvas").transform.localScale.y / 720), 0);
        GameObject.Find("CreditText").transform.position += new Vector3(0, scrollSpeed * (GameObject.Find("Canvas").GetComponent<RectTransform>().rect.height * GameObject.Find("Canvas").transform.localScale.y / 720), 0);
    }
}
