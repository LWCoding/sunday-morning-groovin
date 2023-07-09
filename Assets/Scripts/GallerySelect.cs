using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GallerySelect : MonoBehaviour
{

    public int levelSelected = 1;
    public float predeterminedX;
    private float staticX;
    public int xSelectedOffset;
    private float lastUpdate = 0;
    public float updateCooldown;
    private float lastFlashUpdate = 0;
    public float flashCooldown;
    private bool flash = false;
    private int buttonCount = 0;
    private Vector2 resolution;
    private Vector2 initialPlayerPos;
    private Vector2 initialEnemyPos;
    public GameObject songText;
    public GameObject lockCover;
    private bool canPlay = true;

    void Awake() {
        Vector2 d = GameObject.Find("PublicVariables").GetComponent<PublicVariables>().GetDimensions();
        resolution = new Vector2(d.x, d.y);
        LoadText();
    }

    void Start() {
        #if UNITY_IPHONE
            GameObject.Find("MobileBack").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("Difficulty").transform.position -= new Vector3(-150, 60, 0);
        #endif
        initialPlayerPos = GameObject.Find("Player").transform.position;
        initialEnemyPos = GameObject.Find("Enemy").transform.position;
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().cameFromGallery = true;
        staticX = predeterminedX * GameObject.Find("Canvas").transform.localScale.x;
        buttonCount = GameObject.FindGameObjectsWithTag("Song").Length;
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().songDifficulty = "normal";
        UpdateButton();
        UpdateText();
    }

    private void MobileCheck() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began) { return; }
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            if (touchPos.x > -7 && touchPos.x < 0 && touchPos.y > 2.5f) {
                DifficultyRight();
            }
            if (touchPos.x < 2.5f) { return; }
            if (Mathf.Abs(touchPos.y) < 0.8f) {
                StartCoroutine(startLevel());
            } else if (touchPos.y >= 0.8f) {
                ScrollUp();
            } else {
                ScrollDown();
            }
        }
    }

    private void ScrollUp() {
        levelSelected -= 1;
        if (levelSelected < 1) {
            levelSelected = buttonCount;
        }
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ScrollSound();
        UpdateButton();
    }

    private void ScrollDown() {
        levelSelected += 1;
        if (levelSelected > buttonCount) {
            levelSelected = 1;
        }
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ScrollSound();
        UpdateButton();
    }

    private void DifficultyLeft() {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        p.ScrollSound();
        string d = p.songDifficulty;
        switch(d) {
            case "easy":
                p.songDifficulty = "auto";
                break;
            case "normal":
                p.songDifficulty = "easy";
                break;
            case "hard":
                p.songDifficulty = "normal";
                break;
        }
        GameObject.Find("Difficulty").GetComponent<Text>().text = "<- " + GameObject.Find("PublicVariables").GetComponent<PublicVariables>().songDifficulty.ToUpper() + " MODE ->";
        UpdateText();
    }

    private void DifficultyRight() {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        p.ScrollSound();
        string d = p.songDifficulty;
        switch(d) {
            case "auto":
                p.songDifficulty = "easy";
                break;
            case "normal":
                p.songDifficulty = "hard";
                break;
            case "easy":
                p.songDifficulty = "normal";
                break;
            case "hard":
                #if UNITY_IPHONE
                    p.songDifficulty = "easy";
                #endif
                break;
        }
        GameObject.Find("Difficulty").GetComponent<Text>().text = "<- " + GameObject.Find("PublicVariables").GetComponent<PublicVariables>().songDifficulty.ToUpper() + " MODE ->";
        UpdateText();
    }

    void Update() {
        GetComponent<AudioSource>().volume = GameObject.Find("PublicVariables").GetComponent<VolumeHandler>().volume / GameObject.Find("PublicVariables").GetComponent<VolumeHandler>().maxVolume * 0.5f;
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        Vector2 dim = p.GetDimensions();
        if (resolution.x != dim.x || resolution.y != dim.y) {
            resolution.x = dim.x;
            resolution.y = dim.y;
            UpdateButton();
        }
        if (flash) { return; }
        #if UNITY_IPHONE
            MobileCheck();
        #endif
        #if UNITY_STANDALONE || UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                ScrollUp();
            } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                ScrollDown();
            } else if (canPlay && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))) {
                StartCoroutine(startLevel());
            } else if (canPlay && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))) {
                DifficultyLeft();
            } else if (canPlay && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))) {
                DifficultyRight();
            } else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)) {
                SceneManager.LoadScene("Start");
            }
        #endif
    }

    IEnumerator startLevel() {
        flash = true;
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ConfirmSound();
        yield return new WaitForSeconds(1);
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().song = GameObject.Find("Song" + levelSelected).GetComponent<Data>().data;
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().levelSelected = levelSelected;
        SceneManager.LoadScene("Game");
    }

    void LoadText() {
        int currSong = 1;
        string[] songNotes = GameObject.Find("PublicVariables").GetComponent<PublicVariables>().songData.text.Split('\n');
        for (int i = 0; i < songNotes.Length - 2; i += 5) {
            if ((songNotes[i].Substring(0, songNotes[i].Length - 2) == "NULL" || songNotes[i].Substring(0, songNotes[i].Length - 2) == "EXCEPTION") && PlayerPrefs.GetInt("UnlockedNull") != 1) {
                return;
            }
            GameObject s = Instantiate(songText);
            s.gameObject.name = "Song" + currSong;
            s.GetComponent<Text>().text = songNotes[i].Substring(0, songNotes[i].Length - 2);
            s.GetComponent<Data>().data = songNotes[i].Substring(0, songNotes[i].Length - 2).ToLower();
            s.GetComponent<Data>().subData = songNotes[i+4].Substring(0, 2);
            s.transform.SetParent(GameObject.Find("Canvas").transform);
            currSong += 1;
        }
    }

    void FixedUpdate() {
        lastUpdate += Time.deltaTime;
        lastFlashUpdate += Time.deltaTime;
        if (flash) {
            GameObject stageButton = GameObject.Find("Song" + levelSelected);
            if (lastFlashUpdate > flashCooldown) {
                if (stageButton.GetComponent<Text>().color.a > 0.2f) {
                    stageButton.GetComponent<Text>().color = new Color(255, 255, 255, 0.2f);
                } else {
                    stageButton.GetComponent<Text>().color = new Color(255, 255, 255, 0.75f);
                }
                lastFlashUpdate = 0;
            }
        }
        if (lastUpdate > updateCooldown) {
            lastUpdate = 0;
            Vector3 pos = GameObject.Find("Song" + levelSelected).transform.position;
            if (pos.x < staticX + xSelectedOffset) {
                GameObject.Find("Song" + levelSelected).transform.position = new Vector3(pos.x + 4f, pos.y, pos.z);
            }
        }
    }

    void UpdateSprites() {
        SongData s = GameObject.Find("PublicVariables").GetComponent<SongData>();
        Song song = s.GetSong(GameObject.Find("Song" + levelSelected).GetComponent<Data>().data);
        Character[] characters = s.GetCharacterInfo(song);
        GameObject p = GameObject.Find("Player");
        GameObject e = GameObject.Find("Enemy");
        Character pC = characters[0];
        Character eC = characters[1];
        p.transform.position = initialPlayerPos + new Vector2(0, pC.yOffset);
        e.transform.position = initialEnemyPos + new Vector2(0, eC.yOffset);
        e.transform.position += new Vector3(0, 0, 1);
        if (pC.skin != "Normal" && pC.skin != "Chromatic") {
            p.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + song.playerName + "Sprite/" + pC.skin + "/Idle");
        } else {
            p.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + song.playerName + "Sprite/Idle");
        }
        if (eC.skin != "Normal" && eC.skin != "Chromatic") {
            e.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + song.enemyName + "Sprite/" + eC.skin + "/Idle");
        } else {
            e.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + song.enemyName + "Sprite/Idle");
        }
        p.transform.localScale = new Vector2(pC.spriteScale.x * -1, pC.spriteScale.y);
        e.transform.localScale = eC.spriteScale;
    }

    void UpdateButton() {
        staticX = predeterminedX * GameObject.Find("Canvas").transform.localScale.x;
        GameObject main = GameObject.Find("Song" + levelSelected);
        GetComponent<AudioSource>().clip = (AudioClip)Resources.Load("Audio/" + main.GetComponent<Data>().data.Replace(" ", ""));
        GetComponent<AudioSource>().Play();
        UpdateSprites();
        main.GetComponent<Text>().color = new Color(255, 255, 255, 0.9f);
        main.transform.position = new Vector3(main.transform.position.x, (GameObject.Find("Canvas").GetComponent<RectTransform>().rect.height * GameObject.Find("Canvas").transform.localScale.y) / 2, main.transform.position.z);
        for (int i = 1; i < buttonCount + 1; i++) {
            GameObject stageButton = GameObject.Find("Song" + i);
            Vector3 pos = GameObject.Find("Song" + i).transform.position;
            stageButton.transform.position = new Vector3(staticX, main.transform.position.y - ((i - levelSelected) * 150 * GameObject.Find("Canvas").transform.localScale.y), pos.z);
            if (i != levelSelected) {
                stageButton.GetComponent<Text>().color = new Color(255, 255, 255, 0.4f);
            }
        }
        UpdateText();
    }


    void UpdateText() {
        Song songName = GameObject.Find("PublicVariables").GetComponent<SongData>().GetSong(GameObject.Find("Song" + levelSelected).GetComponent<Data>().data);
        GameObject[] bgs = GameObject.FindGameObjectsWithTag("Background");
        foreach (GameObject bg in bgs) {
            if (bg.name != songName.bgName + "BG") {
                bg.GetComponent<SpriteRenderer>().enabled = false;
            } else {
                bg.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        if (GameObject.Find("PublicVariables").GetComponent<PublicVariables>().songDifficulty == "auto") {
            GameObject.Find("HighScore").GetComponent<Text>().text = "High Score: N/A";
            return;
        }
        lockCover.SetActive(false);
        canPlay = true;
        // The week the song is in, based on songdata.txt
        // string correlatedWeek = GameObject.Find("Song" + levelSelected).GetComponent<Data>().subData;
        // if (correlatedWeek == "NA" || int.Parse(correlatedWeek) == 1 || PlayerPrefs.GetInt("Week" + (correlatedWeek[0] == '0' ? correlatedWeek.Substring(1, 1) : correlatedWeek) + "Complete") == 1) {
        //     lockCover.SetActive(false);
        //     canPlay = true;
        // } else {
        //     lockCover.SetActive(true);
        //     canPlay = false;
        // }
        GameObject.Find("HighScore").GetComponent<Text>().text = "High Score: " + PlayerPrefs.GetInt(GameObject.Find("Song" + levelSelected).GetComponent<Data>().data + GameObject.Find("PublicVariables").GetComponent<PublicVariables>().GetDifficulty());
    }

}
