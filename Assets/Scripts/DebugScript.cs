using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DebugScript : MonoBehaviour
{

    public GameObject arrowPrefab;
    public GameObject arrowDragPrefab;
    public GameObject playerArrowGroup;
    public GameObject enemyArrowGroup;
    private Dropdown playerDrop;
    private Dropdown enemyDrop;
    public string movements;
    private int arrowSpeed;
    public bool fadeToBlack = false;
    private float lastFadeUpdate = 0;
    private int currentInstance = 0;
    public float lastBounce = 0;
    private float effectNoteDelay;
    private float tempo;
    private bool enemyPsychic;
    public Sprite normalArrow;
    public Sprite psychicArrow;
    public Sprite shadowArrow;
    public Sprite techArrow;
    public Sprite susArrow;
    public AudioClip clickSFX;
    private AudioSource musicPlayer;
    private PublicVariables p;
    private GameObject spawnedArrowsParentObject;
    public List<GameObject> pooledNotes; // Optimization
    public List<GameObject> pooledDragNotes; // Optimization    
    [Header("Pooled Objects")]
    public int pooledNoteCount;
    public int pooledDragNoteCount;
    private Dictionary<char, string> directionNames = new Dictionary<char, string>();
    
    void Awake() {
        directionNames.Add('L', "left");
        directionNames.Add('R', "right");
        directionNames.Add('U', "up");
        directionNames.Add('D', "down");
        spawnedArrowsParentObject = GameObject.Find("SpawnedArrows");
        playerDrop = GameObject.Find("PlayerDropdown").GetComponent<Dropdown>();
        enemyDrop = GameObject.Find("EnemyDropdown").GetComponent<Dropdown>();
        playerDrop.ClearOptions();
        enemyDrop.ClearOptions();
        p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        foreach (string c in p.characters) {
            Dropdown.OptionData o = new Dropdown.OptionData();
            o.text = c;
            playerDrop.options.Add(o);
            enemyDrop.options.Add(o);
            if (c == p.playerName) {
                playerDrop.value = playerDrop.options.IndexOf(o);
            } else if (c == p.enemyName) {
                enemyDrop.value = enemyDrop.options.IndexOf(o);
            }
        }
    }

    void Start() {
        GameObject.Find("Cover").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Cover").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        p.transitionSelection = -2;
        musicPlayer = GameObject.Find("Main Audio Source").GetComponent<AudioSource>();
        string currMovements = p.currentMovements;
        LoadPooledObjects();
        GameObject.Find("MovementInput").GetComponent<InputField>().text = currMovements.Substring(4, currMovements.Length - 4);
        GameObject.Find("SpeedInput").GetComponent<InputField>().text = currMovements.Substring(0, 2);
        GameObject.Find("TempoInput").GetComponent<InputField>().text = currMovements.Substring(2, 2);
    }

    void LoadPooledObjects() {
        pooledNotes = new List<GameObject>();
        GameObject newObject;
        for (int i = 0; i < pooledNoteCount; i++) {
            newObject = Instantiate(arrowPrefab);
            newObject.transform.SetParent(spawnedArrowsParentObject.transform);
            newObject.SetActive(false);
            newObject.GetComponent<ArrowMoveDebug>().dbs = this;
            pooledNotes.Add(newObject);
        }
        pooledDragNotes = new List<GameObject>();
        for (int i = 0; i < pooledDragNoteCount; i++) {
            newObject = Instantiate(arrowDragPrefab);
            newObject.transform.SetParent(spawnedArrowsParentObject.transform);
            newObject.SetActive(false);
            pooledDragNotes.Add(newObject);
        }
        Debug.Log("DebugScript.cs >> Successfully pooled " + pooledNotes.Count + " notes and " + pooledDragNotes.Count + " drag notes!");
    }

    IEnumerator Bump() {
        GameObject[] notes = GameObject.FindGameObjectsWithTag("UIDelete");
        foreach (GameObject note in notes) {
            note.transform.localScale += new Vector3(0.07f, 0.07f, 0.07f);
        }
        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < 7; i++) {
            foreach (GameObject note in notes) {
                note.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    void Update() {
        if (musicPlayer.isPlaying) {
            lastBounce += Time.deltaTime;
        }
        if (lastBounce > tempo) {
            StartCoroutine(Bump());
            lastBounce = 0;
        }
        if (Input.GetKeyDown(KeyCode.Return)) {
            fadeToBlack = true;
            string calcSpeed = GameObject.Find("SpeedInput").GetComponent<InputField>().text;
            string calcTempo = GameObject.Find("TempoInput").GetComponent<InputField>().text;
            if (calcSpeed.Length < 2) {
                calcSpeed = "0" + calcSpeed;
            }
            if (calcTempo.Length < 2) {
                calcTempo = "0" + calcTempo;
            }
            Debug.Log(calcSpeed + calcTempo + GameObject.Find("MovementInput").GetComponent<InputField>().text);
            musicPlayer.time = 0;
            musicPlayer.pitch = 1;
            Time.timeScale = 1;
            StartCoroutine(BackToGame());
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            if (musicPlayer.pitch == 1) {
                musicPlayer.pitch = 2;
                Time.timeScale = 2;
            } else {
                musicPlayer.pitch = 1;
                Time.timeScale = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            GameObject.Find("MovementInput").GetComponent<InputField>().text = GameObject.Find("MovementInput").GetComponent<InputField>().text.Replace(" ", "");
            GameObject.Find("PositionInput").GetComponent<InputField>().text = GameObject.Find("PositionInput").GetComponent<InputField>().text.Replace(" ", "");
            GameObject.Find("SpeedInput").GetComponent<InputField>().text = GameObject.Find("SpeedInput").GetComponent<InputField>().text.Replace(" ", "");
            GameObject.Find("TempoInput").GetComponent<InputField>().text = GameObject.Find("TempoInput").GetComponent<InputField>().text.Replace(" ", "");
            if (musicPlayer.pitch == 0) {
                musicPlayer.pitch = 1;
                Time.timeScale = 1;
                StartCoroutine(PlaySong());
            } else {
                musicPlayer.pitch = 0;
                Time.timeScale = 0;
            }
        }
    }

    IEnumerator BackToGame() {
        fadeToBlack = true;
        yield return new WaitForSeconds(0.9f);
        string calcSpeed = GameObject.Find("SpeedInput").GetComponent<InputField>().text;
        string calcTempo = GameObject.Find("TempoInput").GetComponent<InputField>().text;
        string pattern = @"\d\d";
        if (calcSpeed.Length < 2) {
            calcSpeed = "0" + calcSpeed;
        }
        if (calcTempo.Length < 2) {
            calcTempo = "0" + calcTempo;
        }
        if (!Regex.Match(calcSpeed, pattern).Success) {
            calcSpeed = "01";
        }
        if (!Regex.Match(calcTempo, pattern).Success) {
            calcTempo = "01";
        }
        p.songList[p.GetDifficulty()] = calcSpeed + calcTempo + GameObject.Find("MovementInput").GetComponent<InputField>().text;
        p.playerName = playerDrop.options[playerDrop.value].text;
        p.enemyName = enemyDrop.options[enemyDrop.value].text;
        p.cameFromDebug = true;
        SceneManager.LoadScene("Game");
    }

    void FixedUpdate() {
        GameObject.Find("SecondsCounter").GetComponent<Text>().text = Mathf.Round(musicPlayer.time * 10) / 10 + "/" + Mathf.Round(musicPlayer.clip.length * 10) / 10;
        lastFadeUpdate += Time.deltaTime;
        if (fadeToBlack && lastFadeUpdate > 0.03f) {
            lastFadeUpdate = 0;
            Color c = GameObject.Find("Cover").GetComponent<SpriteRenderer>().color;
            Vector3 fPos = GameObject.Find("Cover").transform.position;
            Vector3 cPos = GameObject.Find("Main Camera").transform.position;
            GameObject.Find("Cover").GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, c.a + 0.1f);
        }
    }

    IEnumerator QueueEffect(string str, float time) {
        while (true) {
            if (str[0] == 'P') {
                if (str[1] == '+') {
                    arrowSpeed += int.Parse("" + str[2]);
                } else if (str[1] == '-') {
                    arrowSpeed -= int.Parse("" + str[2]);
                } else {
                    arrowSpeed = int.Parse("" + str[1] + str[2]);
                }
                break;
            }
            if (str[0] == 'Y') {
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
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator StartDrag(float time, char side, string direction, int repeat) {
        yield return new WaitForSeconds(time);
        if (side == 'P') {
            if (repeat == 1) {
                DragP(direction, true);
            } else {
                for (int i = 0; i < repeat; i++) {
                    DragP(direction, i == repeat - 1);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        } else {
            if (repeat == 1) { 
                DragE(direction, true); 
            } else {
                for (int i = 0; i < repeat; i++) {
                    DragE(direction, i == repeat - 1);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

    public void PlayClickSound() {
        musicPlayer.PlayOneShot(clickSFX);
    }

    IEnumerator PlaySong() {
        for (int i = 0; i < 4; i++) {
            GameObject.Find("PlayerArrows").transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);
            GameObject.Find("EnemyArrows").transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);
        }
        currentInstance += 1;
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("Note");
        foreach (GameObject arrow in arrows) {
            arrow.SetActive(false);
        }
        musicPlayer.clip = p.currentSong;
        musicPlayer.Play();
        int nowInstance = currentInstance;
        string calcTempo = GameObject.Find("TempoInput").GetComponent<InputField>().text;
        if (calcTempo.Length < 2) {
            calcTempo = "0" + calcTempo;
        }
        string calcSpeed = GameObject.Find("SpeedInput").GetComponent<InputField>().text;
        if (calcSpeed.Length < 2) {
            calcSpeed = "0" + calcSpeed;
        }
        movements = calcSpeed + calcTempo + GameObject.Find("MovementInput").GetComponent<InputField>().text;
        Debug.Log(movements.Substring(4, movements.Length - 4));
        arrowSpeed = int.Parse(movements.Substring(0, 2));
        lastBounce = 0;
        tempo = -0.049f * float.Parse(movements.Substring(2, 2)) + 5;
        Vector2 d = p.GetDimensions();
        effectNoteDelay = 9 * 80 * (d.y / 720) * (arrowSpeed / 10) / (arrowSpeed * 50 * (d.y / 720));
        float waitingTime = 0;
        float startTime = 0;
        musicPlayer.time = 0;
        enemyPsychic = false;
        if (GameObject.Find("PositionInput").GetComponent<InputField>().text != "") {
            startTime = float.Parse(GameObject.Find("PositionInput").GetComponent<InputField>().text);
        }
        for (int i = 4; i < movements.Length; i += 4) {
            if (waitingTime < startTime) {
                if (movements[i] == 'S') {
                    continue;
                }
                float time;
                if (movements[i+1] == 'Q') {
                    i++;
                    continue;
                }
                if (movements[i+2] == '.') {
                    time = float.Parse(char.ToString(movements[i+3])) / 100;
                } else {
                    time = float.Parse(char.ToString(movements[i+2]) + char.ToString(movements[i+3])) / 10;
                }
                waitingTime += time;
                musicPlayer.time = waitingTime;
                continue;
            }
            if (currentInstance != nowInstance) {
                break;
            }
            if (movements[i] == 'P') {
                if (movements[i+1] == 'Q') {
                    SpawnP(directionNames[movements[i+2]]);
                    StartCoroutine(StartDrag(waitingTime - musicPlayer.time, 'P', directionNames[movements[i+2]], int.Parse(movements[i+3].ToString() + movements[i+4].ToString())));
                } else {
                    SpawnP(directionNames[movements[i+1]]);
                }                
            } else if (movements[i] == 'E') {
                if (movements[i+1] == 'Q') {
                    SpawnE(directionNames[movements[i+2]]);
                    StartCoroutine(StartDrag(waitingTime - musicPlayer.time, 'E', directionNames[movements[i+2]], int.Parse(movements[i+3].ToString() + movements[i+4].ToString())));
                } else {
                    SpawnE(directionNames[movements[i+1]]);
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
                yield return new WaitForSeconds(waitingTime - musicPlayer.time);
            } else if (movements[i+1] == 'Q') {
                i++;
            }
        }
    }

    private GameObject CreatePooledNote() {
        GameObject newArrow = Instantiate(arrowPrefab);
        newArrow.GetComponent<ArrowMoveDebug>().dbs = this;
        newArrow.transform.SetParent(spawnedArrowsParentObject.transform);
        newArrow.SetActive(false);
        pooledNotes.Add(newArrow);
        return newArrow;
    }

    private GameObject CreatePooledDragNote() {
        GameObject newArrow = Instantiate(arrowDragPrefab);
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
        Vector2 d = p.GetDimensions();
        newArrow.GetComponent<Image>().enabled = true;
        newArrow.GetComponent<ArrowMoveDebug>().disableUpdate = false;
        newArrow.SetActive(true);
        newArrow.GetComponent<ArrowMoveDebug>().moveSpeed = (float)(arrowSpeed) * 50 * (d.y / 720);
        Transform t = playerArrowGroup.transform;
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
            newArrow.GetComponent<Image>().sprite = normalArrow;
            newArrow.GetComponent<Image>().color = p.arrowColors[direction];
        }
        switch (direction) {
            case "left":
                newArrow.transform.position = t.GetChild(0).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)arrowSpeed / 10), 0);
                newArrow.transform.Rotate(0, 0, 90);
                break;
            case "up":
                newArrow.transform.position = t.GetChild(1).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)arrowSpeed / 10), 0);
                break;
            case "down":
                newArrow.transform.position = t.GetChild(2).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)arrowSpeed / 10), 0);
                newArrow.transform.Rotate(0, 0, 180);
                break;
            case "right":
                newArrow.transform.position = t.GetChild(3).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)arrowSpeed / 10), 0);
                newArrow.transform.Rotate(0, 0, 270);
                break;
        }
        newArrow.GetComponent<ArrowMoveDebug>().direction = direction;
        newArrow.GetComponent<ArrowMoveDebug>().isEnemy = true;
        newArrow.GetComponent<ArrowMoveDebug>().enemyArrowName = "PlayerArrows";
        newArrow.GetComponent<ArrowMoveDebug>().Initialize();
        newArrow.transform.localScale = new Vector3(1, 1, 1);
        return;
    }

    private void DragP(string direction, bool isLast) {
        GameObject newArrow = GetPooledDragNote();
        Vector2 d = p.GetDimensions();
        newArrow.SetActive(true);
        newArrow.GetComponent<ArrowDragDebug>().moveSpeed = (float)(arrowSpeed) * 50 * (d.y / 720);
        Transform t = playerArrowGroup.transform;
        Color c = p.arrowColors[direction];
        c.a = 0.7f;
        newArrow.GetComponent<Image>().color = c;
        switch (direction) {
            case "left":
                newArrow.transform.position = t.GetChild(0).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "up":
                newArrow.transform.position = t.GetChild(1).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "down":
                newArrow.transform.position = t.GetChild(2).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "right":
                newArrow.transform.position = t.GetChild(3).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
        }
        newArrow.GetComponent<ArrowDragDebug>().EndBar(isLast);
        newArrow.GetComponent<ArrowDragDebug>().isEnemy = true;
        newArrow.GetComponent<ArrowDragDebug>().enemyArrowName = "PlayerArrows";
        newArrow.GetComponent<ArrowDragDebug>().Initialize();
        newArrow.transform.localScale = new Vector3(1.05f, 1.05f, 1);
        newArrow.GetComponent<ArrowDragDebug>().direction = direction;
        return;
    }

    private void SpawnE(string direction, string special = "") {
        GameObject newArrow = GetPooledNote();
        Vector2 d = p.GetDimensions();
        newArrow.GetComponent<Image>().enabled = true;
        newArrow.GetComponent<ArrowMoveDebug>().disableUpdate = false;
        newArrow.SetActive(true);
        newArrow.GetComponent<ArrowMoveDebug>().moveSpeed = (float)(arrowSpeed) * 50 * (d.y / 720);
        Transform t = enemyArrowGroup.transform;
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
            newArrow.GetComponent<Image>().sprite = normalArrow;
            newArrow.GetComponent<Image>().color = p.arrowColors[direction];
        }
        switch (direction) {
            case "left":
                newArrow.transform.position = t.GetChild(0).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)arrowSpeed / 10), 0);
                newArrow.transform.Rotate(0, 0, 90);
                break;
            case "up":
                newArrow.transform.position = t.GetChild(1).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)arrowSpeed / 10), 0);
                break;
            case "down":
                newArrow.transform.position = t.GetChild(2).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)arrowSpeed / 10), 0);
                newArrow.transform.Rotate(0, 0, 180);
                break;
            case "right":
                newArrow.transform.position = t.GetChild(3).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)arrowSpeed / 10), 0);
                newArrow.transform.Rotate(0, 0, 270);
                break;
        }
        newArrow.GetComponent<ArrowMoveDebug>().direction = direction;
        newArrow.GetComponent<ArrowMoveDebug>().isEnemy = true;
        newArrow.GetComponent<ArrowMoveDebug>().enemyArrowName = "EnemyArrows";
        newArrow.GetComponent<ArrowMoveDebug>().Initialize();
        newArrow.transform.localScale = new Vector3(1, 1, 1);
        return;
    }

    private void DragE(string direction, bool isLast) {
        GameObject newArrow = GetPooledDragNote();
        Vector2 d = p.GetDimensions();
        newArrow.GetComponent<ArrowDragDebug>().moveSpeed = (float)(arrowSpeed) * 50 * (d.y / 720);
        Transform t = enemyArrowGroup.transform;
        Color c = p.arrowColors[direction];
        c.a = 0.7f;
        newArrow.SetActive(true);
        newArrow.GetComponent<Image>().color = c;
        switch (direction) {
            case "left":
                newArrow.transform.position = t.GetChild(0).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "up":
                newArrow.transform.position = t.GetChild(1).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "down":
                newArrow.transform.position = t.GetChild(2).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
            case "right":
                newArrow.transform.position = t.GetChild(3).position - new Vector3(0, 9 * 80 * (d.y / 720) * ((float)(arrowSpeed) / 10), 0);
                break;
        }
        newArrow.GetComponent<ArrowDragDebug>().EndBar(isLast);
        newArrow.GetComponent<ArrowDragDebug>().direction = direction;
        newArrow.GetComponent<ArrowDragDebug>().isEnemy = true;
        newArrow.GetComponent<ArrowDragDebug>().enemyArrowName = "EnemyArrows";
        newArrow.GetComponent<ArrowDragDebug>().Initialize();
        newArrow.transform.localScale = new Vector3(1.05f, 1.05f, 1);
        return;
    }
}
