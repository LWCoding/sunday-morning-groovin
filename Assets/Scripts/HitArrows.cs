using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HitArrows : MonoBehaviour
{

    public bool isColliding = false;
    public string direction;
    public List<Collider2D> arrows = new List<Collider2D>();
    public List<Collider2D> drags = new List<Collider2D>();
    public Dictionary<int, int> count = new Dictionary<int, int>();
    public GameObject shadowArrowPrefab;
    public GameObject techArrowPrefab;
    public AudioClip zapSFX;
    public AudioClip cracklingSFX;
    public AudioClip sabotageSFX;
    private SpriteHandler playerSH;
    private PublicVariables p;
    private GameObject gameController;
    private GameObject canvasObject;
    private GameObject playerArrows;
    public GameObject particleObject;
    private float arrowHitOffset;
    private float arrowGreatHitOffset;
    private bool noSpecialAccuracy = false;
    private WaitForSeconds wfs;
    private bool isLenientPressEnabled;
    private int qualityLevel;

    void Start()
    {
        canvasObject = GameObject.Find("Canvas");
        arrowHitOffset = 55 * canvasObject.transform.localScale.x; // Distance away from GOOD and UGH
        arrowGreatHitOffset = 40 * canvasObject.transform.localScale.x; // Distance away from GREAT and GOOD
        gameController = GameObject.Find("GameController");
        playerArrows = GameObject.Find("PlayerArrows");
        playerSH = GameObject.Find("Player").GetComponent<SpriteHandler>();
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        noSpecialAccuracy = PlayerPrefs.GetInt("Accuracy") == 1;
        isLenientPressEnabled = PlayerPrefs.GetInt("LenientPress") == 1;
        qualityLevel = PlayerPrefs.GetInt("Quality");
        for (int i = 0; i < 4; i++)
        {
            count.Add(i, 0);
        }
    }

    IEnumerator holdColor(KeyCode button, int num, bool hit)
    {
        if (hit)
        {
            if (qualityLevel != 1)
            {
                GameObject particle = Instantiate(particleObject);
                particle.transform.position = transform.position;
                particle.transform.SetParent(playerArrows.transform);
                particle.GetComponent<Image>().color = GetComponent<Image>().color;
            }
            GetComponent<Image>().transform.localScale = new Vector3(1.1f, 1.1f, 1);
        }
        else
        {
            GetComponent<Image>().transform.localScale = new Vector3(1, 1, 1);
        }
        count[num] += 1;
        while (true)
        {
            if (Input.GetKeyUp(button))
            {
                count[num] -= 1;
                if (count[num] == 0)
                {
                    GetComponent<Image>().transform.localScale = new Vector3(1.05f, 1.05f, 1);
                    GetComponent<Image>().color = new Color(1, 1, 1, 0.75f);
                }
                yield break;
            }
            yield return null;
        }
    }

    void Update()
    {
        if (!Input.anyKey || p.playerLost || p.songDifficulty == "auto" || gameController.GetComponent<LevelHandler>() == null || gameController.GetComponent<LevelHandler>().fadeToBlack || !gameController.GetComponent<CutsceneHandler>().dialogueComplete) { return; }
        switch (direction)
        {
            case "left":
                if (Input.GetKey(p.keycodes[0]) || Input.GetKey(p.keycodes[1]))
                {
                    if (isColliding)
                    {
                        if (drags.Count > 0)
                        {
                            Collider2D collider = drags[0];
                            float calcOffset = Mathf.Abs(playerArrows.transform.GetChild(0).position.y - collider.gameObject.transform.position.y);
                            if (calcOffset < arrowHitOffset)
                            {
                                p.Hit(50);
                                drags.Remove(collider);
                                if (!noSpecialAccuracy) { p.hitPerfection += 1; }
                                gameController.GetComponent<LevelHandler>().playerNotes -= 1;
                                playerSH.LeftNote();
                                collider.gameObject.SetActive(false);
                            }
                        }
                        if (Input.GetKeyDown(p.keycodes[0]) || Input.GetKeyDown(p.keycodes[1]))
                        {
                            playerSH.LeftNote();
                            changePoints(true, "left");
                            StartCoroutine(holdColor((Input.GetKeyDown(p.keycodes[0])) ? p.keycodes[0] : p.keycodes[1], 0, true));
                        }
                    }
                    else if (Input.GetKeyDown(p.keycodes[0]) || Input.GetKeyDown(p.keycodes[1]))
                    {
                        changePoints(false, "leftmiss");
                        StartCoroutine(holdColor((Input.GetKeyDown(p.keycodes[0])) ? p.keycodes[0] : p.keycodes[1], 0, false));
                    }
                }
                break;
            case "up":
                if (Input.GetKey(p.keycodes[4]) || Input.GetKey(p.keycodes[5]))
                {
                    if (isColliding)
                    {
                        if (drags.Count > 0)
                        {
                            Collider2D collider = drags[0];
                            float calcOffset = Mathf.Abs(playerArrows.transform.GetChild(0).position.y - collider.gameObject.transform.position.y);
                            if (calcOffset < arrowHitOffset)
                            {
                                p.Hit(50);
                                drags.Remove(collider);
                                if (!noSpecialAccuracy) { p.hitPerfection += 1; }
                                gameController.GetComponent<LevelHandler>().playerNotes -= 1;
                                collider.gameObject.SetActive(false);
                                playerSH.UpNote();
                            }
                        }
                        if (Input.GetKeyDown(p.keycodes[4]) || Input.GetKeyDown(p.keycodes[5]))
                        {
                            playerSH.UpNote();
                            changePoints(true, "up");
                            StartCoroutine(holdColor((Input.GetKeyDown(p.keycodes[4])) ? p.keycodes[4] : p.keycodes[5], 1, true));
                        }
                    }
                    else if (Input.GetKeyDown(p.keycodes[4]) || Input.GetKeyDown(p.keycodes[5]))
                    {
                        changePoints(false, "upmiss");
                        StartCoroutine(holdColor((Input.GetKeyDown(p.keycodes[4])) ? p.keycodes[4] : p.keycodes[5], 1, false));
                    }
                }
                break;
            case "down":
                if (Input.GetKey(p.keycodes[6]) || Input.GetKey(p.keycodes[7]))
                {
                    if (isColliding)
                    {
                        if (drags.Count > 0)
                        {
                            Collider2D collider = drags[0];
                            float calcOffset = Mathf.Abs(playerArrows.transform.GetChild(0).position.y - collider.gameObject.transform.position.y);
                            if (calcOffset < arrowHitOffset)
                            {
                                p.Hit(50);
                                drags.Remove(collider);
                                if (!noSpecialAccuracy) { p.hitPerfection += 1; }
                                gameController.GetComponent<LevelHandler>().playerNotes -= 1;
                                collider.gameObject.SetActive(false);
                                playerSH.DownNote();
                            }
                        }
                        if (Input.GetKeyDown(p.keycodes[6]) || Input.GetKeyDown(p.keycodes[7]))
                        {
                            playerSH.DownNote();
                            changePoints(true, "down");
                            StartCoroutine(holdColor((Input.GetKeyDown(p.keycodes[6])) ? p.keycodes[6] : p.keycodes[7], 2, true));
                        }
                    }
                    else if (Input.GetKeyDown(p.keycodes[6]) || Input.GetKeyDown(p.keycodes[7]))
                    {
                        changePoints(false, "downmiss");
                        StartCoroutine(holdColor((Input.GetKeyDown(p.keycodes[6])) ? p.keycodes[6] : p.keycodes[7], 2, false));
                    }
                }
                break;
            case "right":
                if (Input.GetKey(p.keycodes[2]) || Input.GetKey(p.keycodes[3]))
                {
                    if (isColliding)
                    {
                        if (drags.Count > 0)
                        {
                            Collider2D collider = drags[0];
                            float calcOffset = Mathf.Abs(playerArrows.transform.GetChild(0).position.y - collider.gameObject.transform.position.y);
                            if (calcOffset < arrowHitOffset)
                            {
                                p.Hit(50);
                                drags.Remove(collider);
                                if (!noSpecialAccuracy) { p.hitPerfection += 1; }
                                gameController.GetComponent<LevelHandler>().playerNotes -= 1;
                                collider.gameObject.SetActive(false);
                                playerSH.RightNote();
                            }
                        }
                        if (Input.GetKeyDown(p.keycodes[2]) || Input.GetKeyDown(p.keycodes[3]))
                        {
                            playerSH.RightNote();
                            changePoints(true, "right");
                            StartCoroutine(holdColor((Input.GetKeyDown(p.keycodes[2])) ? p.keycodes[2] : p.keycodes[3], 3, true));
                        }
                    }
                    else if (Input.GetKeyDown(p.keycodes[2]) || Input.GetKeyDown(p.keycodes[3]))
                    {
                        changePoints(false, "rightmiss");
                        StartCoroutine(holdColor((Input.GetKeyDown(p.keycodes[2])) ? p.keycodes[2] : p.keycodes[3], 3, false));
                    }
                }
                break;
        }
    }

    void changePoints(bool isHit, string direction)
    {
        GetComponent<Image>().color = p.arrowColors[direction + ((direction.Contains("miss")) ? "" : "hit")];
        if (isHit && arrows.Count > 0)
        {
            if (arrows[0].gameObject.GetComponent<ArrowMove>().shadowEffect)
            {
                if (qualityLevel != 1)
                {
                    GameObject c = Instantiate(shadowArrowPrefab);
                    c.transform.SetParent(canvasObject.transform);
                    c.transform.position = gameObject.transform.position;
                    c.transform.rotation = gameObject.transform.rotation;
                }
                GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
                GetComponent<Image>().transform.localScale = new Vector3(1, 1, 1);
                p.Deplete(33);
                playerSH.Miss();
                p.musicPlayer.PlayOneShot(cracklingSFX);
                arrows[0].gameObject.SetActive(false);
                return;
            }
            if (arrows[0].gameObject.GetComponent<ArrowMove>().techEffect)
            {
                if (qualityLevel != 1)
                {
                    GameObject c = Instantiate(techArrowPrefab);
                    c.transform.SetParent(canvasObject.transform);
                    c.transform.position = gameObject.transform.position;
                    c.transform.rotation = gameObject.transform.rotation;
                }
                GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
                GetComponent<Image>().transform.localScale = new Vector3(1, 1, 1);
                playerSH.Miss();
                GameObject.Find("GameController").GetComponent<LevelHandler>().arrowSpeed += 1;
                p.musicPlayer.PlayOneShot(zapSFX);
                arrows[0].gameObject.SetActive(false);
                return;
            }
            if (arrows[0].gameObject.GetComponent<ArrowMove>().susEffect)
            {
                GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
                GetComponent<Image>().transform.localScale = new Vector3(1, 1, 1);
                playerSH.Miss();
                StartCoroutine(ArrowShift());
                arrows[0].gameObject.SetActive(false);
                return;
            }
            float calcOffset = Mathf.Abs(playerArrows.transform.GetChild(0).position.y - arrows[0].gameObject.transform.position.y);
            if (!noSpecialAccuracy)
            {
                p.hitPerfection += 1 - Mathf.Max(0, (calcOffset - 30)) / 100; // Remove 30 off of it to provide some leniency
            }
            if (calcOffset < arrowGreatHitOffset)
            {
                gameController.GetComponent<LevelHandler>().FeedbackText("GREAT!");
                p.Hit(350);
                p.achievements[4].Increment(1);
                if (p.achievements[4].NextLevel())
                {
                    p.RenderAchievement(p.achievements[4].name, p.achievements[4].description);
                }
            }
            else if (calcOffset < arrowHitOffset)
            {
                gameController.GetComponent<LevelHandler>().FeedbackText("GOOD!");
                p.Hit(250);
            }
            else
            {
                gameController.GetComponent<LevelHandler>().FeedbackText("UGH!");
                p.Hit(150);
            }
            gameController.GetComponent<LevelHandler>().ShowStreak();
            gameController.GetComponent<LevelHandler>().playerNotes -= 1;
            arrows[0].gameObject.SetActive(false);
        }
        else if (GameObject.Find("Cover").GetComponent<Image>().color.a <= 0)
        {
            if (isLenientPressEnabled)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (playerArrows.transform.GetChild(i).GetComponent<HitArrows>().isColliding)
                    {
                        p.points -= 350;
                    }
                }
                return;
            }
            p.Miss();
        }
    }

    IEnumerator BackgroundSabotage()
    {
        SpriteRenderer background = GameObject.Find(p.background).GetComponent<SpriteRenderer>();
        background.color = new Color(1, 0.5f, 0.5f);
        wfs = new WaitForSeconds(0.2f);
        yield return wfs;
        for (int i = 0; i < 20; i++)
        {
            background.color += new Color(0, 0.025f, 0.025f);
            wfs = new WaitForSeconds(0.04f);
            yield return wfs;
        }
    }

    IEnumerator ArrowShift()
    {
        GameObject arrows = GameObject.Find("ArrowRefs");
        GameObject arrowList = GameObject.Find("SpawnedArrows");
        for (int i = 0; i < 3; i++)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            float xRandom = Random.Range(6f, 7f) * ((Random.Range(0, 2) == 0) ? -1 : 1);
            float yRandom = Random.Range(3f, 4f) * ((Random.Range(0, 2) == 0) ? -1 : 1);
            p.musicPlayer.PlayOneShot(sabotageSFX);
            p.Deplete(4, false);
            StartCoroutine(BackgroundSabotage());
            for (int j = 0; j < 20; j++)
            {
                arrows.transform.position += new Vector3(xRandom, yRandom, 0);
                arrowList.transform.position += new Vector3(xRandom, yRandom, 0);
                wfs = new WaitForSeconds(0.01f);
                yield return wfs;
            }
            wfs = new WaitForSeconds(0.5f);
            yield return wfs;
            p.musicPlayer.PlayOneShot(sabotageSFX, 0.3f);
            p.Deplete(4, false);
            StartCoroutine(BackgroundSabotage());
            for (int j = 0; j < 20; j++)
            {
                arrows.transform.position -= new Vector3(xRandom, yRandom, 0);
                arrowList.transform.position -= new Vector3(xRandom, yRandom, 0);
                wfs = new WaitForSeconds(0.01f);
                yield return wfs;
            }
            wfs = new WaitForSeconds(0.5f);
            yield return wfs;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Arrow(Clone)" && !arrows.Contains(collision))
        {
            arrows.Add(collision);
            if (!collision.gameObject.GetComponent<ArrowMove>().shadowEffect)
            {
                p.notesSpawned += 1;
            }
        }
        if (collision.gameObject.name == "ArrowDrag(Clone)" && !drags.Contains(collision))
        {
            drags.Add(collision);
            p.notesSpawned += 1;
        }
        isColliding = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (arrows.Contains(collision))
        {
            arrows.Remove(collision);
        }
        if (drags.Contains(collision))
        {
            drags.Remove(collision);
        }
        if (drags.Count == 0 && arrows.Count == 0)
        {
            isColliding = false;
        }
    }
}
