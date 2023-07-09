using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowDragDebug : MonoBehaviour
{

    public float moveSpeed;
    public string direction;
    public bool isEnemy = false;
    private float enemyArrowDeletion;
    public string enemyArrowName;
    public Sprite arrowDragNormal;
    public Sprite arrowDragEnd;
    private GameObject enemyObject;
    private GameObject enemyArrowsObject;

    void Start() {
        enemyObject = GameObject.Find("Enemy");        
        if (isEnemy) {
            Transform enemyArrow = enemyArrowsObject.transform.GetChild(0);
            enemyArrowDeletion = enemyArrow.transform.position.y;
        }
    }

    public void Initialize() {
        enemyArrowsObject = GameObject.Find(enemyArrowName);
    }

    public void EndBar(bool shouldSwitch) {
        if (!shouldSwitch) { 
            GetComponent<Image>().sprite = arrowDragNormal;
            return;
        }
        GetComponent<Image>().sprite = arrowDragEnd;
    }

    void Update()
    {   
        transform.position += transform.up * moveSpeed * Time.deltaTime;
        if (isEnemy && transform.position.y > enemyArrowDeletion) {
            Transform enemyArrows = enemyArrowsObject.GetComponent<Transform>();
            switch (direction) {
                case "up":
                    enemyArrows.GetChild(1).GetComponent<Image>().color = new Color(0.3f, 1f, 0.3f);
                    if (enemyObject != null) {
                        enemyObject.GetComponent<SpriteHandler>().UpNote();
                    }
                    StartCoroutine(resetColor(1));
                    break;
                case "down":
                    enemyArrows.GetChild(2).GetComponent<Image>().color = new Color(0.3f, 0.9f, 1);
                    if (enemyObject != null) {
                        enemyObject.GetComponent<SpriteHandler>().DownNote();
                    }
                    StartCoroutine(resetColor(2));
                    break;
                case "left":
                    enemyArrows.GetChild(0).GetComponent<Image>().color = new Color(0.8f, 0.3f, 0.8f);
                    if (enemyObject != null) {
                        enemyObject.GetComponent<SpriteHandler>().LeftNote();
                    }
                    StartCoroutine(resetColor(0));
                    break;
                case "right":
                    enemyArrows.GetChild(3).GetComponent<Image>().color = new Color(1f, 0.3f, 0.3f);
                    if (enemyObject != null) {
                        enemyObject.GetComponent<SpriteHandler>().RightNote();
                    }
                    StartCoroutine(resetColor(3));
                    break;
            }
            gameObject.GetComponent<Image>().enabled = false;
        }
    }

    IEnumerator resetColor(int arrow) {
        yield return new WaitForSeconds(0.1f);
        Transform enemyArrows = enemyArrowsObject.GetComponent<Transform>();
        enemyArrows.GetChild(arrow).GetComponent<Image>().color = new Color(1, 1, 1);
        gameObject.SetActive(false);
    }
    
}
