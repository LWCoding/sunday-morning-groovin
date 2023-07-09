using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelHandler : MonoBehaviour
{

    public GameObject arrowPrefab;
    public GameObject arrowDragPrefab;
    public GameObject playerArrowGroup;
    public GameObject enemyArrowGroup;
    public Sprite paperArrow;
    public Sprite normalArrow;
    public Sprite waterArrow;
    public TMP_FontAsset normalStreakFont;
    public TMP_FontAsset paperStreakFont;
    public Font normalScoreFont;
    public Font paperScoreFont;
    public string movements;
    public int arrowSpeed;
    public float tempo;
    private string modifier;
    public AudioClip deathMusic;
    public bool fadeToBlack = false;
    public bool partialGray = false;
    private bool fadeToOpaque = false;
    private bool songFinished = false;
    private int goingUp = 1;
    private int bounceCount = 0;
    // To make different things "bounce" (see Coroutine Bump()) at different times
    private float lastFadeUpdate = 0;
    public int currentStreak = 0;
    public int enemyNotes = 0;
    public int playerNotes = 0;
    public float lastBounce = 0.0f;
    private float lastBarUpdate = 0.0f;
    private float playerBumpAmount;
    private float enemyBumpAmount;
    public bool enemyRumble = false;
    [SerializeField] private AudioClip countdown;
    private string songName;
    private CutsceneHandler c;
    private MidSongModifiers msm;
    private AudioSource musicPlayer;
    public GameObject streakTextPrefab;
    private bool duetOn = false;
    public Sprite psychicArrow; // For special effect psychic arrows
    public Sprite shadowArrow; // For special effect shadow arrows
    public Sprite techArrow; // For special effect tech arrows
    public Sprite susArrow; // For special effect sus arrows
    private bool enemyPsychic = false;
    private float effectNoteDelay; // Delay before note spawn and note hit.
    public int bumpFreq = 4; // How often the screen should bump (standard = 4)
    private int bumpCount = 0; // To make screen bump every 4
    public float bumpStrength = 0.3f; // How hard the screen bumps every 4
    private PublicVariables p;
    public List<GameObject> pooledNotes; // Optimization
    public List<GameObject> pooledDragNotes; // Optimization
    public List<GameObject> pooledTextObjects; // Optimization
    [Header("Pooled Objects")]
    public int pooledNoteCount;
    public int pooledDragNoteCount;
    public int pooledTextObjectCount;
    private GameObject coverObject;
    private GameObject playerObject;
    private GameObject enemyObject;
    private GameObject mainCameraObject;
    private GameObject spawnedArrowsParentObject;
    private GameObject streakTextParentObject;
    private GameObject feedbackPointObject;
    private GameObject percentageObject;
    private GameObject progressObject;
    private GameObject canvas;
    private Vector3 streakObjectSpawn;
    private WaitForSeconds wfs;
    private Dictionary<char, string> directionNames = new Dictionary<char, string>();
    
    void Awake() {
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        canvas = GameObject.Find("Canvas");
        coverObject = GameObject.Find("Cover");
        mainCameraObject = GameObject.Find("Main Camera");
        playerObject = GameObject.Find("Player");
        enemyObject = GameObject.Find("Enemy");
        feedbackPointObject = GameObject.Find("FeedbackPoint");
        percentageObject = GameObject.Find("Percentage");
        progressObject = GameObject.Find("Progress");
        streakObjectSpawn = GameObject.Find("StreakPoint").transform.position;;
        playerBumpAmount = GameObject.Find("Player").transform.localScale.x * 0.025f;
        enemyBumpAmount = GameObject.Find("Enemy").transform.localScale.x * 0.025f;
        directionNames.Add('L', "left");
        directionNames.Add('R', "right");
        directionNames.Add('U', "up");
        directionNames.Add('D', "down");
        spawnedArrowsParentObject = GameObject.Find("SpawnedArrows");
        streakTextParentObject = GameObject.Find("TextObjects");
        c = GetComponent<CutsceneHandler>();
        msm = GetComponent<MidSongModifiers>();
        duetOn = PlayerPrefs.GetInt("DuetMode") == 1;
    }

    void ChangeArrows(Sprite s) {
        Transform playerArrows = GameObject.Find("PlayerArrows").transform;
        for (int i = 0; i < playerArrows.childCount; i++) {
            playerArrows.GetChild(i).GetComponent<Image>().sprite = s;
        }
        Transform enemyArrows = GameObject.Find("EnemyArrows").transform;
        for (int i = 0; i < enemyArrows.childCount; i++) {
            enemyArrows.GetChild(i).GetComponent<Image>().sprite = s;
        }
        arrowPrefab.GetComponent<Image>().sprite = s;
    }

    // Loads proper coordinates if Downscroll/Upscroll
    void LoadScroll() {
        goingUp = PlayerPrefs.GetInt("Downscroll") == 0 ? 1 : -1;
        Vector3 pos;
        if (PlayerPrefs.GetInt("Downscroll") == 1) {
            pos = playerArrowGroup.transform.position;
            playerArrowGroup.transform.position = new Vector3(pos.x, GameObject.Find("D-Arrows").transform.position.y, pos.z);
            pos = enemyArrowGroup.transform.position;
            enemyArrowGroup.transform.position = new Vector3(pos.x, GameObject.Find("D-Arrows").transform.position.y, pos.z);
            pos = GameObject.Find("Deletion").transform.position;
            GameObject.Find("Deletion").transform.position = new Vector3(pos.x, GameObject.Find("D-Deletion").transform.position.y, pos.z);
            pos = GameObject.Find("BarBottom").transform.position;
            GameObject.Find("BarBottom").transform.position = new Vector3(pos.x, GameObject.Find("D-Bar").transform.position.y, pos.z);
        } else {
            pos = playerArrowGroup.transform.position;
            playerArrowGroup.transform.position = new Vector3(pos.x, GameObject.Find("N-Arrows").transform.position.y, pos.z);
            pos = enemyArrowGroup.transform.position;
            enemyArrowGroup.transform.position = new Vector3(pos.x, GameObject.Find("N-Arrows").transform.position.y, pos.z);
            pos = GameObject.Find("Deletion").transform.position;
            GameObject.Find("Deletion").transform.position = new Vector3(pos.x, GameObject.Find("N-Deletion").transform.position.y, pos.z);
            pos = GameObject.Find("BarBottom").transform.position;
            GameObject.Find("BarBottom").transform.position = new Vector3(pos.x, GameObject.Find("N-Bar").transform.position.y, pos.z);
        }
    }

    void LoadPooledObjects() {
        pooledNotes = new List<GameObject>();
        GameObject newObject;
        for (int i = 0; i < pooledNoteCount; i++) {
            newObject = Instantiate(arrowPrefab);
            newObject.transform.SetParent(spawnedArrowsParentObject.transform);
            newObject.GetComponent<ArrowMove>().goingUp = goingUp;
            newObject.SetActive(false);
            pooledNotes.Add(newObject);
        }
        pooledDragNotes = new List<GameObject>();
        for (int i = 0; i < pooledDragNoteCount; i++) {
            newObject = Instantiate(arrowDragPrefab);
            newObject.transform.SetParent(spawnedArrowsParentObject.transform);
            newObject.GetComponent<ArrowDragMove>().goingUp = goingUp;
            newObject.SetActive(false);
            pooledDragNotes.Add(newObject);
        }
        pooledTextObjects = new List<GameObject>();
        for (int i = 0; i < pooledTextObjectCount; i++) {
            newObject = Instantiate(streakTextPrefab);
            newObject.transform.SetParent(streakTextParentObject.transform);
            newObject.SetActive(false);
            pooledTextObjects.Add(newObject);
        }
        Debug.Log("LevelHandler.cs >> Successfully pooled " + pooledNotes.Count + " notes, " + pooledDragNotes.Count + " drag notes, and " + pooledTextObjects.Count + " streak text objects!");
    }

    void LoadCharacterUnlock(string name) {
        if (PlayerPrefs.GetInt("Unlocked" + name) == 0) {
            PlayerPrefs.SetInt("Unlocked" + name, 1);
        }
    }

    void LoadModifiers() {
        LoadScroll();
        string mod = GameObject.Find("PublicVariables").GetComponent<SongData>().currentMod;
        switch (mod) {
            case "None":
            case "Water":
                GameObject.Find("Points").GetComponent<Text>().color = new Color(1, 1, 1);
                GameObject.Find("Points").GetComponent<Text>().font = normalScoreFont;
                GameObject.Find("Points").GetComponent<Text>().fontSize = 23;
                GameObject.Find("Accuracy").GetComponent<Text>().color = new Color(1, 1, 1);
                GameObject.Find("Accuracy").GetComponent<Text>().font = normalScoreFont;
                GameObject.Find("Accuracy").GetComponent<Text>().fontSize = 23;
                GameObject.Find("SongName").GetComponent<Text>().color = new Color(1, 1, 1);
                streakTextPrefab.GetComponent<TextMeshPro>().font = normalStreakFont;
                streakTextPrefab.GetComponent<TextMeshPro>().fontSize = 10;
                streakTextPrefab.GetComponent<TextMeshPro>().color = new Color(1, 1, 1);
                GameObject.Find("PaperSpeakers").GetComponent<SpriteRenderer>().enabled = false;
                if (mod == "None") {
                    ChangeArrows(normalArrow);
                    GameObject.Find("PlayerBar").GetComponent<Image>().color = new Color32(23, 183, 35, 255);
                    GameObject.Find("EnemyBar").GetComponent<Image>().color = new Color32(212, 41, 41, 255);
                } else if (mod == "Water") {
                    ChangeArrows(waterArrow);
                    GameObject.Find("PlayerBar").GetComponent<Image>().color = new Color32(33, 150, 226, 255);
                    GameObject.Find("EnemyBar").GetComponent<Image>().color = new Color32(241, 76, 109, 255);
                }
                return; 
            case "Paper":
                GameObject.Find("Points").GetComponent<Text>().color = new Color(0, 0, 0);
                GameObject.Find("Points").GetComponent<Text>().font = paperScoreFont;
                GameObject.Find("Points").GetComponent<Text>().fontSize = 32;
                GameObject.Find("Accuracy").GetComponent<Text>().color = new Color(0, 0, 0);
                GameObject.Find("Accuracy").GetComponent<Text>().font = paperScoreFont;
                GameObject.Find("Accuracy").GetComponent<Text>().fontSize = 32;
                GameObject.Find("SongName").GetComponent<Text>().color = new Color(0, 0, 0);
                streakTextPrefab.GetComponent<TextMeshPro>().font = paperStreakFont;
                streakTextPrefab.GetComponent<TextMeshPro>().fontSize = 12;
                streakTextPrefab.GetComponent<TextMeshPro>().color = new Color(0, 0, 0);
                GameObject.Find("PlayerBar").GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                GameObject.Find("EnemyBar").GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
                GameObject.Find("PaperSpeakers").GetComponent<SpriteRenderer>().enabled = true;
                ChangeArrows(paperArrow);
                return;
        }
    }

    void Start() {
        Time.timeScale = 1;
        p.SetSongText();
        GameObject.Find("PublicVariables").GetComponent<SongData>().enemyOrigY = enemyObject.transform.position.y;
        p.CameraToCenter();
        p.points = 0;
        p.notesSpawned = 0;
        p.misses = 0;
        p.health = 50;
        p.hitPerfection = 0;
        p.noSpecialAccuracy = PlayerPrefs.GetInt("Accuracy") == 1;
        musicPlayer = GameObject.Find("Main Audio Source").GetComponent<AudioSource>();
        musicPlayer.loop = false;
        musicPlayer.Pause();
        coverObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        coverObject.GetComponent<Image>().enabled = true;
        GameObject.Find("EnemyBar").GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        string song = p.song.Replace(" ", "").ToLower();
        songName = song;
        musicPlayer.clip = GetSong(songName);
        GameObject.Find("PublicVariables").GetComponent<SongData>().SetSong(song);
        LoadModifiers();
        LoadPooledObjects();
        LoadCharacterUnlock(p.playerName);
        LoadCharacterUnlock(p.enemyName);
        p.StoreAssetsInPublicVariables();
        movements = p.songList[p.GetDifficulty()];
        arrowSpeed = int.Parse(movements.Substring(0, 2));
        if (PlayerPrefs.GetInt("GlobalSpeed") != 0) {
            arrowSpeed = PlayerPrefs.GetInt("GlobalSpeed");
        }
        float calcTempo = float.Parse(movements.Substring(2, 2));
        if (calcTempo == 0) {
            // New Tempo (00)
            tempo = 0.91f;
        } else {
            // Old Tempo
            tempo = -0.049f * float.Parse(movements.Substring(2, 2)) + 5;
        }
        GameObject.Find("TopBarBG").GetComponent<Image>().color = new Color32(94, 94, 94, 170);
        p.updateScore();
        StartCoroutine(DelayAndShow());
    }

    IEnumerator DelayAndShow() {
        wfs = new WaitForSeconds(0.3f);
        yield return wfs;
        fadeToOpaque = true;
    }

    AudioClip GetSong(string songName) {
        return (AudioClip)Resources.Load("Audio/" + songName);
    }

    public IEnumerator Bump() {
        if (p.background == "PaperBG") {
            GameObject.Find("PaperSpeakers").transform.localScale += new Vector3(0.014f, 0.014f, 0);
        }
        GameObject playerIcon = GameObject.Find("PlayerIcon");
        GameObject enemyIcon = GameObject.Find("EnemyIcon");
        bumpCount++;
        playerObject.transform.localScale += new Vector3(playerBumpAmount, -0.5f * playerBumpAmount, 0);
        enemyObject.transform.localScale += new Vector3(enemyBumpAmount, -0.5f * enemyBumpAmount, 0);
        Vector2 playerDelta = new Vector2(playerIcon.transform.localScale.x * 0.03f, playerIcon.transform.localScale.y * 0.03f);
        Vector2 enemyDelta = new Vector2(enemyIcon.transform.localScale.x * 0.03f, enemyIcon.transform.localScale.y * 0.03f);
        playerIcon.transform.localScale += new Vector3(playerDelta.x, playerDelta.y, 0);
        enemyIcon.transform.localScale += new Vector3(enemyDelta.x, enemyDelta.y, 0);
        float recovered = 0;
        if (bumpCount >= bumpFreq) {
            recovered = bumpStrength;
            mainCameraObject.GetComponent<CameraScale>().sceneWidth -= bumpStrength;
        }
        wfs = new WaitForSeconds(0.15f);
        yield return wfs;
        for (int j = 0; j < 7; j++) {
            if (p.background == "PaperBG") {
                GameObject.Find("PaperSpeakers").transform.localScale -= new Vector3(0.002f, 0.002f, 0);
            }
            playerIcon.transform.localScale -= new Vector3(playerDelta.x / 7, playerDelta.y / 7, 0);
            enemyIcon.transform.localScale -= new Vector3(enemyDelta.x / 7, enemyDelta.y / 7, 0);
            if (bumpCount >= bumpFreq) {
                float removed = bumpStrength / 7;
                mainCameraObject.GetComponent<CameraScale>().sceneWidth += removed;
                recovered -= removed;
            }
            playerObject.transform.localScale -= new Vector3(playerBumpAmount / 7, -0.5f * playerBumpAmount / 7, 0);
            enemyObject.transform.localScale -= new Vector3(enemyBumpAmount / 7, -0.5f * enemyBumpAmount / 7, 0);
            wfs = new WaitForSeconds(0.01f);
            yield return wfs;
        }
        mainCameraObject.GetComponent<CameraScale>().sceneWidth += recovered;
        mainCameraObject.GetComponent<CameraScale>().sceneWidth = float.Parse(mainCameraObject.GetComponent<CameraScale>().sceneWidth.ToString("0.00"));
        if (bumpCount >= bumpFreq) {
            bumpCount = 0;
        }
    }

    void Update() {
        if (p.playerLost) {
            return;
        }
        if (c.dialogueComplete && !partialGray) {
            lastBounce += Time.deltaTime;
        }
        if (bounceCount <= 3 && lastBounce > ((tempo > 0.6f) ? tempo / 2 : tempo)) {
            StartCoroutine(Bump());
            lastBounce = 0;
            if (bounceCount < 3) {
                musicPlayer.PlayOneShot((AudioClip)Resources.Load("Audio/Countdown/" + (3 - bounceCount)));
                bounceCount += 1;
                GameObject t = GetPooledText();
                t.GetComponent<StreakTextFade>().xMove = false;
                t.GetComponent<TextMeshPro>().text = (4 - bounceCount).ToString();
                t.GetComponent<TextMeshPro>().fontSize = 18;
                t.SetActive(true);
            } else if (bounceCount == 3) {
                musicPlayer.PlayOneShot((AudioClip)Resources.Load("Audio/Countdown/go"));
                bounceCount += 1;
                GameObject t = GetPooledText();
                t.GetComponent<StreakTextFade>().xMove = false;
                t.GetComponent<TextMeshPro>().fontSize = 18;
                t.GetComponent<TextMeshPro>().text = "GO!";
                t.SetActive(true);
                StartCoroutine(PlaySong());
            }
        }
        if (lastBounce > tempo) {
            StartCoroutine(Bump());
            lastBounce = 0;
        }
        if (Input.GetKeyDown(KeyCode.Return) && !p.playerLost && !songFinished && musicPlayer.time > 0) {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.R) && !p.playerLost && !songFinished && musicPlayer.time > 0 && partialGray) {
            p.RestartGame();
        }
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape) && partialGray) {
            Time.timeScale = 1;
            p.BackToMenu();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7) && !fadeToBlack) {
            GameObject.Find("Main Audio Source").GetComponent<AudioSource>().Stop();
            StartCoroutine(DebugScene());
        }
    }

    void OnApplicationFocus(bool focus) {
        if (!Application.isEditor && !focus && !p.playerLost && !fadeToBlack && musicPlayer.time > 0) {
            TogglePause();
        }
    }

    private void TogglePause() {
        if (!c.dialogueComplete) { return; }
        fadeToBlack = !fadeToBlack;
        partialGray = !partialGray;
        ResizeCover();
        Color col = coverObject.GetComponent<Image>().color;
        if (!fadeToBlack) {
            GameObject.Find("PauseText").GetComponent<Text>().enabled = false;
            coverObject.GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0);
            Time.timeScale = 1;
            if (musicPlayer.time > 0) {
                musicPlayer.Play();
            }
        } else {
            GameObject.Find("PauseText").GetComponent<Text>().enabled = true;
            coverObject.GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0.7f);
            musicPlayer.Pause();
            Time.timeScale = 0;
        }
    }

    private GameObject CreatePooledText() {
        GameObject newText = Instantiate(streakTextPrefab);
        newText.transform.SetParent(streakTextParentObject.transform);
        newText.SetActive(false);
        pooledTextObjects.Add(newText);
        return newText;
    }

    private GameObject GetPooledText() {
        for (int i = 0; i < pooledTextObjects.Count; i++) {
            if (!pooledTextObjects[i].activeInHierarchy) {
                pooledTextObjects[i].GetComponent<StreakTextFade>().timeAlive = 0;
                pooledTextObjects[i].GetComponent<StreakTextFade>().lastUpdate = 0;
                pooledTextObjects[i].GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 1);
                pooledTextObjects[i].transform.position = new Vector3(0, 0, 0);
                return pooledTextObjects[i];
            }
        }
        return CreatePooledText();
    }

    public void ShowStreak() {
        if (currentStreak < 5) { return; }
        GameObject[] texts = GameObject.FindGameObjectsWithTag("StreakNote");
        foreach (GameObject text in texts) {
            Color c = text.GetComponent<TextMeshPro>().color;
            text.GetComponent<TextMeshPro>().color = new Color(c.r, c.g, c.b, text.GetComponent<TextMeshPro>().color.a - 0.08f);
        }
        GameObject t = GetPooledText();
        t.GetComponent<TextMeshPro>().fontSize = 10;
        t.GetComponent<TextMeshPro>().text = currentStreak.ToString("000");
        t.transform.position = streakObjectSpawn;
        t.SetActive(true);
    }

    public void FeedbackText(string feedback) {
        Vector3 spawn = feedbackPointObject.transform.position;
        GameObject t = GetPooledText();
        t.GetComponent<TextMeshPro>().fontSize = 11;
        t.GetComponent<TextMeshPro>().text = currentStreak.ToString(feedback);
        t.transform.position = spawn;
        t.SetActive(true);
    }

    void ResizeCover() {
        coverObject.transform.localScale = new Vector3(canvas.GetComponent<RectTransform>().rect.x / coverObject.GetComponent<RectTransform>().rect.x, canvas.GetComponent<RectTransform>().rect.y / coverObject.GetComponent<RectTransform>().rect.y, 1);
    }

    IEnumerator DebugScene() {
        fadeToBlack = true;
        p.currentMovements = movements;
        p.currentSong = GetSong(songName);
        wfs = new WaitForSeconds(0.9f);
        yield return wfs;
        p.points = 0;
        p.transitionSelection = -2;
        p.playerLost = false;
        SceneManager.LoadScene("Debug");
    }

    private void UpdateBar() {
        if (musicPlayer.clip == null || Time.time - lastBarUpdate < 0.25f || !musicPlayer.isPlaying) { return; }
        float progress = musicPlayer.time / musicPlayer.clip.length;
        lastBarUpdate = Time.time;
        percentageObject.GetComponent<Text>().text = Mathf.RoundToInt(progress * 100) + "%";
        progressObject.GetComponent<RectTransform>().localScale = new Vector3(800 * progress, 1, 1);
    }

    void FixedUpdate() {
        UpdateBar();
        if ((enemyNotes > 0) == (playerNotes > 0)) {
            p.CameraToCenter();
        } else if (enemyNotes > 0) {
            p.CameraToEnemy();
        } else {
            p.CameraToPlayer();
        }
        lastFadeUpdate += Time.deltaTime;
        if (fadeToOpaque && lastFadeUpdate > 0.03f) {
            lastFadeUpdate = 0;
            Color c = coverObject.GetComponent<Image>().color;
            coverObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, c.a - 0.1f);
            if (c.a <= 0) {
                fadeToOpaque = false;
            }
        }
        if (fadeToBlack && lastFadeUpdate > 0.03f) {
            lastFadeUpdate = 0;
            Color c = coverObject.GetComponent<Image>().color;
            Vector3 fPos = coverObject.transform.position;
            Vector3 cPos = mainCameraObject.transform.position;
            if (partialGray) {
                if (coverObject.GetComponent<Image>().color.a < 0.75f) {
                    coverObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, c.a + 0.1f);
                }
            } else {
                coverObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, c.a + 0.1f);
            }
        }
    }

    IEnumerator StartDrag(float time, char side, string direction, int repeat) {
        int dragMoveSpeed = arrowSpeed;
        wfs = new WaitForSeconds(time);
        yield return wfs;
        if (side == 'P') {
            playerNotes += repeat;
            if (repeat == 1) {
                DragP(direction, true);
            } else {
                for (int i = 0; i < repeat; i++) {
                    DragP(direction, i == repeat - 1);
                    wfs = new WaitForSeconds(0.1f);
                    yield return wfs;
                }
            }
        } else {
            enemyNotes += repeat;
            if (repeat == 1) { 
                DragE(direction, true); 
            } else {
                for (int i = 0; i < repeat; i++) {
                    DragE(direction, i == repeat - 1);
                    wfs = new WaitForSeconds(0.1f);
                    yield return wfs;
                }
            }
        }
    }

    IEnumerator QueueEffect(string str, float time) {
        while (true) {
            if (str[0] == 'P') {
                if (str[1] == '+') {
                    arrowSpeed += int.Parse("" + str[2]);
                } else if (str[1] == '-') {
                    arrowSpeed -= int.Parse("" + str[2]);
                } else if (PlayerPrefs.GetInt("GlobalSpeed") != 0) {
                    arrowSpeed = int.Parse("" + str[1] + str[2]);
                }
                break;
            } else if (str[0] == 'Y') {
                enemyPsychic = !enemyPsychic;
                break;
            } else if (str[0] == 'D') {
                // SD PL
                if (str[1] == 'P') {
                    SpawnP(directionNames[str[2]], "shadow");
                } else if (str[1] == 'E') {
                    SpawnE(directionNames[str[2]], "shadow");
                }
                break;
            } else if (str[0] == 'T') {
                // ST PL
                if (str[1] == 'P') {
                    SpawnP(directionNames[str[2]], "tech");
                } else if (str[1] == 'E') {
                    SpawnE(directionNames[str[2]], "tech");
                }
                break;
            } else if (str[0] == 'S') {
                // ST PL
                if (str[1] == 'P') {
                    SpawnP(directionNames[str[2]], "sus");
                } else if (str[1] == 'E') {
                    SpawnE(directionNames[str[2]], "sus");
                }
                break;
            }
            if (musicPlayer.time > time) {
                switch (str[0]) {
                    // Rumble toggle (SR)
                    case 'R':
                        enemyRumble = !enemyRumble;
                        break;
                    // Enemy character switch (SW)
                    case 'W':
                        string character = str.Substring(1, 2);
                        if (character == "CA") {
                            GameObject.Find("PublicVariables").GetComponent<SongData>().SetCharacter("Camo", true);
                        } else if (character == "LO") {
                            GameObject.Find("PublicVariables").GetComponent<SongData>().SetCharacter("Lone", true);
                        } else if (character == "CL") {
                            GameObject.Find("PublicVariables").GetComponent<SongData>().SetCharacter("Camo&Lone", true);
                        } else if (character == "PC") {
                            GameObject.Find("PublicVariables").GetComponent<SongData>().SetCharacter("PatchesCrazy", true);
                        }
                        LoadCharacterUnlock(p.enemyName);
                        break;
                    // Enemy shadow effect (SH)
                    case 'H':
                        enemyObject.GetComponent<SpriteHandler>().shadowOpacity = float.Parse(str.Substring(1, 2)) / 100;
                        enemyObject.GetComponent<SpriteHandler>().shadowEffect = !(str.Substring(1, 2) == "00");
                        break;
                    // Player shadow effect (SA)
                    case 'A':
                        playerObject.GetComponent<SpriteHandler>().shadowOpacity = float.Parse(str.Substring(1, 2)) / 100;
                        playerObject.GetComponent<SpriteHandler>().shadowEffect = !(str.Substring(1, 2) == "00");
                        break;
                    // Refer to midsongmodifiers script (SM)
                    case 'M':
                        msm.LoadModifier(str.Substring(1, 2));
                        break;
                }
                break;
            }
            wfs = new WaitForSeconds(0.1f);
            yield return wfs;
        }
    }

    IEnumerator PlaySong() {
        musicPlayer.Play();
        musicPlayer.time = 0;
        float waitingTime = 0 + PlayerPrefs.GetFloat("ArrowOffset");
        Vector2 d = p.GetDimensions();
        effectNoteDelay = 9 * 80 * (d.y / 720) * ((float)arrowSpeed / 10) / ((float)arrowSpeed * 50 * (d.y / 720));
        for (int i = 4; i < movements.Length; i += 4) {
            if (p.playerLost) { break; }
            if (musicPlayer.time < waitingTime) {
                wfs = new WaitForSeconds(waitingTime - musicPlayer.time);
                yield return wfs;
            }
            if (movements[i] == 'P') {
                if (movements[i+1] == 'Q') {
                    SpawnP(directionNames[movements[i+2]]);
                    StartCoroutine(StartDrag(waitingTime - musicPlayer.time, 'P', directionNames[movements[i+2]], int.Parse(movements[i+3].ToString() + movements[i+4].ToString())));
                    if (duetOn) {
                        SpawnE(directionNames[movements[i+2]]);
                        StartCoroutine(StartDrag(waitingTime - musicPlayer.time, 'E', directionNames[movements[i+2]], int.Parse(movements[i+3].ToString() + movements[i+4].ToString())));
                        if (i+7 < movements.Length && (movements[i+2] == movements[i+7] || movements[i+2] == movements[i+6])) {
                            i += 5;
                        }
                    }
                } else {
                    SpawnP(directionNames[movements[i+1]]);
                    if (duetOn) {
                        if (!(movements[i+2] == '0' && movements[i+3] == '0')) {
                            SpawnE(directionNames[movements[i+1]]);
                        } else if (i+5 < movements.Length && movements[i+4] == 'P' && movements[i+5] != 'Q') {
                            SpawnE(directionNames[movements[i+1]]);
                            SpawnE(directionNames[movements[i+5]]);
                            SpawnP(directionNames[movements[i+5]]);
                            i += 4;
                        } else if (i+5 < movements.Length && movements[i+4] == 'E' && movements[i+5] != 'Q') {
                            SpawnE(directionNames[movements[i+5]]);
                            i += 4;
                        }
                    }
                }                
            } else if (movements[i] == 'E') {
                if (movements[i+1] == 'Q') {
                    SpawnE(directionNames[movements[i+2]]);
                    StartCoroutine(StartDrag(waitingTime - musicPlayer.time, 'E', directionNames[movements[i+2]], int.Parse(movements[i+3].ToString() + movements[i+4].ToString())));
                    if (duetOn) {
                        SpawnP(directionNames[movements[i+2]]);
                        StartCoroutine(StartDrag(waitingTime - musicPlayer.time, 'P', directionNames[movements[i+2]], int.Parse(movements[i+3].ToString() + movements[i+4].ToString())));
                        if (i+7 < movements.Length && (movements[i+2] == movements[i+7] || movements[i+2] == movements[i+6])) {
                            i += 5;
                        }
                    }
                } else {
                    SpawnE(directionNames[movements[i+1]]);
                    if (duetOn) {
                        if (!(movements[i+2] == '0' && movements[i+3] == '0')) {
                            SpawnP(directionNames[movements[i+1]]);
                        } else if (i+4 < movements.Length && movements[i+4] == 'E' && movements[i+5] != 'Q') {
                            SpawnP(directionNames[movements[i+1]]);
                            SpawnP(directionNames[movements[i+5]]);
                            SpawnE(directionNames[movements[i+5]]);
                            i += 4;
                        } else if (i+4 < movements.Length && movements[i+4] == 'P' && movements[i+5] != 'Q') {
                            SpawnP(directionNames[movements[i+5]]);
                            i += 4;
                        }
                    }
                }                
            } else if (movements[i] == 'S') {
                string specialStr = "" + movements[i+1] + movements[i+2] + movements[i+3];
                float specialTime = waitingTime + effectNoteDelay - 0.1f;
                StartCoroutine(QueueEffect(specialStr, specialTime));
            }
            if (movements[i] != 'S' && movements[i+1] != 'Q') {
                float waitSecs;
                if (movements[i+2] == '.') {
                    waitSecs = float.Parse(char.ToString(movements[i+3])) / 100;
                } else {
                    waitSecs = float.Parse(char.ToString(movements[i+2]) + char.ToString(movements[i+3])) / 10;
                }
                waitingTime += waitSecs;
                if (waitSecs != 0) {
                    wfs = new WaitForSeconds(waitingTime - musicPlayer.time);
                    yield return wfs;
                }
            } else if (movements[i+1] == 'Q') {
                i++;
            }
        }
        float timeLeft = musicPlayer.clip.length - musicPlayer.time;
        wfs = new WaitForSeconds((timeLeft < 1) ? 1 : timeLeft + 1);
        yield return wfs;
        songFinished = true;
        if (!p.playerLost) {
            fadeToBlack = true;
            partialGray = true;
            StartCoroutine(ShowContinueText());
            StartCoroutine(CheckForContinue());
        }
    }

    IEnumerator ShowContinueText() {
        GameObject continueText = GameObject.Find("ContinueText");
        GameObject gemImageObject = GameObject.Find("GemImage");
        StartCoroutine(GemCount());
        for (int i = 0; i < 10; i++) {
            continueText.GetComponent<Text>().color += new Color(0.1f, 0.1f, 0.1f);
            gemImageObject.GetComponent<Image>().color += new Color(0.1f, 0.1f, 0.1f);
            wfs = new WaitForSeconds(0.06f);
            yield return wfs;
        }
    }

    IEnumerator GemPulse() {
        GameObject gemImageObject = GameObject.Find("GemImage");
        for (int i = 0; i < 15; i++) {
            gemImageObject.transform.localScale += new Vector3(0.01f, 0.01f, 0);
            wfs = new WaitForSeconds(0.03f * (1 + 0.03f * i));
            yield return wfs;
        }
        gemImageObject.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        for (int i = 0; i < 15; i++) {
            gemImageObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0);
            wfs = new WaitForSeconds(0.05f * (1 + 0.03f * i));
            yield return wfs;
        }
        StartCoroutine(GemPulse());
    }

    IEnumerator GemCount() {
        GameObject rewardText = GameObject.Find("RewardText");
        rewardText.GetComponent<Text>().color = new Color(1, 1, 1, 1);
        rewardText.GetComponent<Text>().fontSize = 0;
        float delay = 0.02f;
        int fontChange = 6;
        int rewardAmount = (p.GetMoneyEarned() < 0) ? 0 : p.GetMoneyEarned();
        int earnedAmount = 0;
        int gemIncrement = (int)(rewardAmount / 30);
        for (int i = 0; i < 30; i++) {
            rewardText.GetComponent<Text>().fontSize += fontChange;
            earnedAmount += gemIncrement;
            rewardText.GetComponent<Text>().text = "+" + earnedAmount;
            if (i == 29) { rewardText.GetComponent<Text>().text = "+" + rewardAmount; }
            wfs = new WaitForSeconds(delay);
            yield return wfs;
            delay += 0.002f;
            if (i % 6 == 0) { fontChange--; }
        }
        StartCoroutine(GemPulse());
    }

    IEnumerator HideContinueScene() {
        GameObject endCoverObject = GameObject.Find("EndCover");
        while (endCoverObject.GetComponent<Image>().color.a < 1) {
            endCoverObject.GetComponent<Image>().color += new Color(0, 0, 0, 0.05f);
            wfs = new WaitForSeconds(0.035f);
            yield return wfs;
        }
    }

    IEnumerator CheckForContinue() {
        while (true) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                StartCoroutine(HideContinueScene());
                StartCoroutine(LoadNextScene());
                yield break;
            }  
            yield return null;
        }
    }

    // This can be pushed back later to show achievements beforehand!
    IEnumerator LoadNextScene() {
        List<string> queue = p.songQueue;
        wfs = new WaitForSeconds(2f);
        yield return wfs;
        p.UpdateHighScore();
        if (queue.Count == 0) {
            p.transitionSelection = -2;
            if (p.cameFromGallery) {
                SceneManager.LoadScene("Gallery");
            } else {
                SceneManager.LoadScene("StorySelect");
            }
        } else {
            p.song = queue[0];
            queue.RemoveAt(0);
            SceneManager.LoadScene("Game");
        }
    }

    private GameObject CreatePooledNote() {
        GameObject newArrow = Instantiate(arrowPrefab);
        newArrow.GetComponent<ArrowMove>().goingUp = goingUp;
        newArrow.transform.SetParent(spawnedArrowsParentObject.transform);
        newArrow.SetActive(false);
        pooledNotes.Add(newArrow);
        return newArrow;
    }

    private GameObject CreatePooledDragNote() {
        GameObject newArrow = Instantiate(arrowDragPrefab);
        newArrow.GetComponent<ArrowDragMove>().goingUp = goingUp;
        newArrow.transform.SetParent(spawnedArrowsParentObject.transform);
        newArrow.SetActive(false);
        pooledDragNotes.Add(newArrow);
        return newArrow;
    }

    private GameObject GetPooledNote() {
        for (int i = 0; i < pooledNotes.Count; i++) {
            if (!pooledNotes[i].activeInHierarchy) {
                pooledNotes[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                return pooledNotes[i];
            }
        }
        return CreatePooledNote();
    }

    private GameObject GetPooledDragNote() {
        for (int i = 0; i < pooledDragNotes.Count; i++) {
            if (!pooledDragNotes[i].activeInHierarchy) {
                pooledDragNotes[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                return pooledDragNotes[i];
            }
        }
        return CreatePooledDragNote();
    }

    private void SpawnP(string direction, string special = "") {
        GameObject newArrow = GetPooledNote();
        newArrow.SetActive(true);
        Vector2 d = p.GetDimensions();
        newArrow.GetComponent<ArrowMove>().moveSpeed = (float)(arrowSpeed) * 50 * (d.y / 720);
        Transform t = playerArrowGroup.transform;
        newArrow.GetComponent<ArrowMove>().shadowEffect = special == "shadow";
        newArrow.GetComponent<ArrowMove>().techEffect = special == "tech";
        newArrow.GetComponent<ArrowMove>().susEffect = special == "sus";
        newArrow.GetComponent<ArrowMove>().psychicEffect = false;
        if (special == "shadow") {
            newArrow.GetComponent<Image>().color = new Color(1, 1, 1);
            newArrow.GetComponent<Image>().sprite = shadowArrow;
        } else if (special == "tech") {
            newArrow.GetComponent<Image>().color = new Color(1, 1, 1);
            newArrow.GetComponent<Image>().sprite = techArrow;
        } else if (special == "sus") {
            newArrow.GetComponent<Image>().color = new Color(1, 1, 1);
            newArrow.GetComponent<Image>().sprite = susArrow;
        } else {
            playerNotes += 1;
            newArrow.GetComponent<Image>().sprite = normalArrow;
            newArrow.GetComponent<Image>().color = p.arrowColors[direction];
        }
        switch (direction) {
            case "left":
                newArrow.transform.position = t.GetChild(0).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                newArrow.transform.Rotate(0, 0, 90);
                break;
            case "up":
                newArrow.transform.position = t.GetChild(2).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "down":
                newArrow.transform.position = t.GetChild(1).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                newArrow.transform.Rotate(0, 0, 180);
                break;
            case "right":
                newArrow.transform.position = t.GetChild(3).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                newArrow.transform.Rotate(0, 0, 270);
                break;
        }
        newArrow.GetComponent<ArrowMove>().isEnemy = false;
        newArrow.GetComponent<ArrowMove>().direction = direction;
        newArrow.transform.localScale = new Vector3(1.05f, 1.05f, 1);
        newArrow.GetComponent<ArrowMove>().Initialize();
        return;
    }

    private void DragP(string direction, bool isLast) {
        GameObject newArrow = GetPooledDragNote();
        newArrow.SetActive(true);
        Vector2 d = p.GetDimensions();
        newArrow.GetComponent<ArrowDragMove>().moveSpeed = (float)(arrowSpeed) * 50 * (d.y / 720);
        Transform t = playerArrowGroup.transform;
        Color c = p.arrowColors[direction];
        c.a = 0.7f;
        newArrow.GetComponent<Image>().color = c;
        switch (direction) {
            case "left":
                newArrow.transform.position = t.GetChild(0).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "up":
                newArrow.transform.position = t.GetChild(2).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "down":
                newArrow.transform.position = t.GetChild(1).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "right":
                newArrow.transform.position = t.GetChild(3).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
        }
        newArrow.GetComponent<ArrowDragMove>().isEnemy = false;
        newArrow.GetComponent<ArrowDragMove>().direction = direction;
        newArrow.transform.localScale = new Vector3(1.05f, 1.05f, 1);
        newArrow.GetComponent<ArrowDragMove>().EndBar(isLast);
        newArrow.GetComponent<ArrowDragMove>().Initialize();
        return;
    }

    private void SpawnE(string direction, string special = "") {
        GameObject newArrow = GetPooledNote();
        newArrow.SetActive(true);
        enemyNotes += 1;
        Vector2 d = p.GetDimensions();
        newArrow.GetComponent<ArrowMove>().moveSpeed = (float)(arrowSpeed) * 50 * (d.y / 720);
        Transform t = enemyArrowGroup.transform;
        newArrow.GetComponent<ArrowMove>().shadowEffect = special == "shadow";
        newArrow.GetComponent<ArrowMove>().techEffect = special == "tech";
        newArrow.GetComponent<ArrowMove>().susEffect = special == "sus";
        newArrow.GetComponent<ArrowMove>().psychicEffect = enemyPsychic;
        if (special == "shadow") {
            newArrow.GetComponent<Image>().color = new Color(1, 1, 1);
            newArrow.GetComponent<Image>().sprite = shadowArrow;
        } else if (special == "tech") {
            newArrow.GetComponent<Image>().color = new Color(1, 1, 1);
            newArrow.GetComponent<Image>().sprite = techArrow;
        } else if (special == "sus") {
            newArrow.GetComponent<Image>().color = new Color(1, 1, 1);
            newArrow.GetComponent<Image>().sprite = susArrow;
        } else if (enemyPsychic) {
            newArrow.GetComponent<Image>().sprite = psychicArrow;
            newArrow.GetComponent<Image>().color = new Color(1, 1, 1);
        } else {
            newArrow.GetComponent<Image>().sprite = normalArrow;
            newArrow.GetComponent<Image>().color = p.arrowColors[direction];
        }
        switch (direction) {
            case "left":
                newArrow.transform.position = t.GetChild(0).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                newArrow.transform.Rotate(0, 0, 90);
                break;
            case "up":
                newArrow.transform.position = t.GetChild(2).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "down":
                newArrow.transform.position = t.GetChild(1).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                newArrow.transform.Rotate(0, 0, 180);
                break;
            case "right":
                newArrow.transform.position = t.GetChild(3).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                newArrow.transform.Rotate(0, 0, 270);
                break;
        }
        newArrow.GetComponent<ArrowMove>().direction = direction;
        newArrow.GetComponent<ArrowMove>().isEnemy = true;
        newArrow.transform.localScale = new Vector3(1.05f, 1.05f, 1);
        newArrow.GetComponent<ArrowMove>().Initialize();
        return;
    }

    private void DragE(string direction, bool isLast) {
        GameObject newArrow = GetPooledDragNote();
        newArrow.SetActive(true);
        Vector2 d = p.GetDimensions();
        newArrow.GetComponent<ArrowDragMove>().moveSpeed = (float)(arrowSpeed) * 50 * (d.y / 720);
        Transform t = enemyArrowGroup.transform;
        if (enemyPsychic) {
            newArrow.GetComponent<Image>().color = new Color32(255, 76, 211, 178);
        } else {
            Color c = p.arrowColors[direction];
            c.a = 0.7f;
            newArrow.GetComponent<Image>().color = c;
        }
        switch (direction) {
            case "left":
                newArrow.transform.position = t.GetChild(0).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "up":
                newArrow.transform.position = t.GetChild(2).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "down":
                newArrow.transform.position = t.GetChild(1).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "right":
                newArrow.transform.position = t.GetChild(3).position - new Vector3(0, goingUp * 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
        }
        newArrow.GetComponent<ArrowDragMove>().direction = direction;
        newArrow.GetComponent<ArrowDragMove>().isEnemy = true;
        newArrow.transform.localScale = new Vector3(1.05f, 1.05f, 1);
        newArrow.GetComponent<ArrowDragMove>().EndBar(isLast);
        newArrow.GetComponent<ArrowDragMove>().Initialize();
        return;
    }

    public void DestroyScript() {
        fadeToOpaque = false;
        musicPlayer.clip = deathMusic;
        musicPlayer.loop = true;
        musicPlayer.Play();
        string playerName = p.playerName;
        playerObject.GetComponent<SpriteHandler>().Loss();
        Vector3 playerPosition = playerObject.transform.position;
        playerObject.transform.position = new Vector3(playerPosition.x, playerPosition.y, -3);
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("Note");
        foreach (GameObject arrow in arrows) {
            Destroy(arrow);
        }
        GameObject[] icons = GameObject.FindGameObjectsWithTag("Icon");
        foreach (GameObject icon in icons) {
            Destroy(icon);
        }
        StartCoroutine(DelayDestroyScript());
    }

    IEnumerator DelayDestroyScript() {
        wfs = new WaitForSeconds(0.01f);
        yield return wfs;
        GameObject.Find("BarTop").SetActive(false);
        GameObject.Find("BarBottom").SetActive(false);
        GameObject.Find("PlayerArrows").SetActive(false);
        GameObject.Find("EnemyArrows").SetActive(false);
        wfs = new WaitForSeconds(2);
        yield return wfs;
        Destroy(GetComponent<LevelHandler>());
    }
}
