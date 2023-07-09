using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowDragMove : MonoBehaviour
{

    public float moveSpeed;
    public string direction;
    public bool isEnemy = false;
    public int goingUp;
    private float arrowDeletion;
    public Sprite arrowDragEnd;
    public Sprite arrowDragNormal;
    private bool isAuto = false;
    private LevelHandler levelHandler;
    private PublicVariables p;
    private GameObject selObject;

    void Start() {
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        levelHandler = GameObject.Find("GameController").GetComponent<LevelHandler>();
        isAuto = p.songDifficulty == "auto";
        if (isEnemy || isAuto) {
            Transform enemyArrow = GameObject.Find((isEnemy) ? "EnemyArrows" : "PlayerArrows").transform.GetChild(0);
            arrowDeletion = enemyArrow.transform.position.y;
        }
    }

    public void Initialize() {
        selObject = GameObject.Find((isEnemy) ? "Enemy" : "Player");
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "DeleteNote") {
            p.Miss();
            p.misses += 1;
            levelHandler.playerNotes -= 1;
            p.updateScore();
            gameObject.SetActive(false);
        }
    }

    public void EndBar(bool shouldSwitch) {
        if (!shouldSwitch) { 
            GetComponent<Image>().sprite = arrowDragNormal;
            return; 
        }
        if (goingUp == -1) {
            transform.localScale *= new Vector2(1, -1);
        }
        GetComponent<Image>().sprite = arrowDragEnd;
    }

    void Update()
    {   
        transform.position += transform.up * moveSpeed * Time.deltaTime * goingUp;
        bool canDelete = (goingUp == 1 && isAuto && !isEnemy && transform.position.y > arrowDeletion) || (goingUp == -1 && isAuto && !isEnemy && transform.position.y < arrowDeletion) || (goingUp == -1 && isEnemy && transform.position.y < arrowDeletion && gameObject.GetComponent<Image>().enabled) || (goingUp == 1 && isEnemy && transform.position.y > arrowDeletion && gameObject.GetComponent<Image>().enabled);
        if (canDelete) {
            string sel = (isEnemy) ? "Enemy" : "Player";
            Transform enemyArrows = GameObject.Find(sel + "Arrows").GetComponent<Transform>();
            if (isEnemy) {
                levelHandler.enemyNotes -= 1;
            } else {
                levelHandler.playerNotes -= 1;
            }
            switch (direction) {
                case "up":
                    if (selObject != null) {
                        selObject.GetComponent<SpriteHandler>().UpNote();
                    }
                    break;
                case "down":
                    if (selObject != null) {
                        selObject.GetComponent<SpriteHandler>().DownNote();
                    }
                    break;
                case "left":
                    if (selObject != null) {
                        selObject.GetComponent<SpriteHandler>().LeftNote();
                    }
                    break;
                case "right":
                    if (selObject != null) {
                        selObject.GetComponent<SpriteHandler>().RightNote();
                    }
                    break;
            }
            gameObject.SetActive(false);
        }
    }
    
}
