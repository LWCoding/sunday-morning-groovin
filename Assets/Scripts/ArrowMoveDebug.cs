using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowMoveDebug : MonoBehaviour
{

    public float moveSpeed;
    public string direction;
    public bool isEnemy = false;    
    private GameObject enemyArrowsObject;
    private float enemyArrowDeletion;
    public string enemyArrowName;
    public bool disableUpdate = false;
    public DebugScript dbs = null;

    void Start() {
        if (isEnemy) {
            Transform enemyArrow = enemyArrowsObject.transform.GetChild(0);
            enemyArrowDeletion = enemyArrow.transform.position.y;
        }
    }

    public void Initialize() {
        enemyArrowsObject = GameObject.Find(enemyArrowName);
    }

    void Update() {   
        if (disableUpdate) { return; }
        switch (direction) {
            case "up":
                transform.position += transform.up * moveSpeed * Time.deltaTime;
                break;
            case "down":
                transform.position -= transform.up * moveSpeed * Time.deltaTime;
                break;
            case "left":
                transform.position += transform.right * moveSpeed * Time.deltaTime;
                break;
            case "right":
                transform.position -= transform.right * moveSpeed * Time.deltaTime;
                break;
        }
        if (isEnemy && transform.position.y > enemyArrowDeletion) {
            disableUpdate = true;
            Transform enemyArrows = enemyArrowsObject.GetComponent<Transform>();
            switch (direction) {
                case "up":
                    enemyArrows.GetChild(1).GetComponent<Image>().color = new Color(0.3f, 1f, 0.3f);
                    StartCoroutine(resetColor(1));
                    break;
                case "down":
                    enemyArrows.GetChild(2).GetComponent<Image>().color = new Color(0.3f, 0.9f, 1);
                    StartCoroutine(resetColor(2));
                    break;
                case "left":
                    enemyArrows.GetChild(0).GetComponent<Image>().color = new Color(0.8f, 0.3f, 0.8f);
                    StartCoroutine(resetColor(0));
                    break;
                case "right":
                    enemyArrows.GetChild(3).GetComponent<Image>().color = new Color(1f, 0.3f, 0.3f);
                    StartCoroutine(resetColor(3));
                    break;
            }
            gameObject.GetComponent<Image>().enabled = false;
        }
    }

    IEnumerator resetColor(int arrow) {
        if (dbs != null) { dbs.PlayClickSound(); }
        yield return new WaitForSeconds(0.1f);
        Transform enemyArrows = enemyArrowsObject.GetComponent<Transform>();
        enemyArrows.GetChild(arrow).GetComponent<Image>().color = new Color(1, 1, 1);
        gameObject.SetActive(false);
    }
    
}
