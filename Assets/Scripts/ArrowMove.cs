using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowMove : MonoBehaviour
{

    public float moveSpeed;
    public string direction;
    public bool isEnemy = false;
    public int goingUp;
    private float arrowDeletion;
    private bool isAuto = false;
    public bool psychicEffect = false;
    public bool shadowEffect = false;
    public bool techEffect = false;
    public bool susEffect = false;
    private int quality = 0;
    private float lastParticle = 0;
    public GameObject shadowParticle;
    private GameObject canvasObject;
    private LevelHandler levelHandler;
    private PublicVariables p;
    private GameObject selObject;
    private GameObject enemyArrowsObject;

    void Start() {
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        levelHandler = GameObject.Find("GameController").GetComponent<LevelHandler>();
        canvasObject = GameObject.Find("Canvas");
        quality = PlayerPrefs.GetInt("Quality");
        isAuto = p.songDifficulty == "auto";
        Transform enemyArrow = GameObject.Find((isEnemy) ? "EnemyArrows" : "PlayerArrows").transform.GetChild(0);
        arrowDeletion = enemyArrow.transform.position.y;
    }

    public void Initialize() {
        selObject = GameObject.Find((isEnemy) ? "Enemy" : "Player");
        enemyArrowsObject = GameObject.Find((isEnemy) ? "EnemyArrows" : "PlayerArrows");
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "DeleteNote") {
            if (shadowEffect || techEffect || susEffect) {
                gameObject.SetActive(false);
                return;
            }
            levelHandler.playerNotes -= 1;
            p.Miss();
            p.misses += 1;
            p.updateScore();
            gameObject.SetActive(false);
        }
    }

    void Update()
    {   
        if (shadowEffect && quality != 1 && transform.position.y > 0 && Time.time - lastParticle > 0.08f) {
            GameObject s = Instantiate(shadowParticle);
            s.transform.SetParent(canvasObject.transform);
            s.transform.position = transform.position;
            s.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            lastParticle = Time.time;
        }
        if (psychicEffect && quality != 1 && transform.position.y > 0 && Time.time - lastParticle > 0.08f) {
            GameObject s = Instantiate(shadowParticle);
            s.transform.SetParent(canvasObject.transform);
            s.transform.position = transform.position;
            s.GetComponent<Image>().color = new Color(0.9f, 0.46f, 0.9f);
            lastParticle = Time.time;
        }
        switch (direction) {
            case "up":
                transform.position += transform.up * moveSpeed * Time.deltaTime * goingUp;
                break;
            case "down":
                transform.position -= transform.up * moveSpeed * Time.deltaTime * goingUp;
                break;
            case "left":
                transform.position += transform.right * moveSpeed * Time.deltaTime * goingUp;
                break;
            case "right":
                transform.position -= transform.right * moveSpeed * Time.deltaTime * goingUp;
                break;
        }
        bool canDelete = (goingUp == 1 && isAuto && !isEnemy && transform.position.y > arrowDeletion) || (goingUp == -1 && isAuto && !isEnemy && transform.position.y < arrowDeletion) || (goingUp == -1 && isEnemy && transform.position.y < arrowDeletion) || (goingUp == 1 && isEnemy && transform.position.y > arrowDeletion);
        if (!canDelete) { return; }
        if ((techEffect || shadowEffect || susEffect) && !isEnemy && isAuto) {
            return;
        }
        LevelHandler lvl = levelHandler;
        if (isEnemy && lvl.enemyRumble) {
            p.ShakeScreen();
        }
        if (isEnemy) {
            levelHandler.enemyNotes -= 1;
            if (psychicEffect) {
                p.Deplete(1, false);
            }
        } else {
            levelHandler.playerNotes -= 1;
        }
        if (shadowEffect || techEffect || susEffect) { gameObject.SetActive(false); return; }
        Transform enemyArrows = enemyArrowsObject.GetComponent<Transform>();
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
