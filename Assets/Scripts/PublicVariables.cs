using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Achievement
{
    public int id;
    public string rawName;
    public string name;
    public string rawDescription;
    public string description;
    public int numLevels;
    public int[] levelRewards; // Reward for completing level X
    public int[] levelReqs; // Amount of times objective must be done for level X
    public int currentLevel = 1;
    public int currentValue; // How many times the objective has been done
    public int nextMilestone;
    public string specialModifier;
    public Achievement(string id, string name, string desc, string numLevels, int[] levelReqs, int[] numRewards, string specialModifier = "None")
    {
        this.id = int.Parse(id);
        this.rawName = name;
        this.rawDescription = desc;
        this.numLevels = int.Parse(numLevels);
        this.levelRewards = numRewards;
        this.levelReqs = levelReqs;
        this.currentValue = PlayerPrefs.GetInt("Achievement" + id);
        this.specialModifier = specialModifier;
        for (int i = 0; i < levelReqs.Length; i++)
        {
            if (this.currentValue >= levelReqs[i])
            {
                this.currentLevel++;
            }
        }
        UpdateLevelInfo();
    }
    public void Increment(int amount)
    {
        if (IsMaxLevel()) { return; }
        this.currentValue += amount;
        PlayerPrefs.SetInt("Achievement" + id, currentValue);
    }

    public void Set(int amount)
    {
        if (IsMaxLevel() || amount <= this.currentValue) { return; }
        this.currentValue = amount;
        PlayerPrefs.SetInt("Achievement" + id, amount);
    }

    public void IncrementIfValue(int amount, int mustEqualValue)
    {
        if (IsMaxLevel() || mustEqualValue != this.currentValue) { return; }
        this.currentValue = amount;
        PlayerPrefs.SetInt("Achievement" + id, amount);
    }

    public bool NextLevel()
    {
        if (IsMaxLevel()) { return false; }
        bool completedMilestone = this.currentValue >= this.nextMilestone;
        if (completedMilestone)
        {
            this.name = this.rawName + " (" + currentLevel + "/" + numLevels + ")";
            string rewardMsg = this.GetReward();
            this.description = this.rawDescription.Replace("{x}", this.levelReqs[currentLevel - 1].ToString()) + rewardMsg;
            PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money") + this.levelRewards[currentLevel - 1]);
            this.currentLevel++;
            if (this.specialModifier == "newsong")
            {
                PlayerPrefs.SetInt("UnlockedNull", 1);
            }
            if (!IsMaxLevel())
            {
                this.nextMilestone = this.levelReqs[currentLevel - 1];
            }
        }
        return completedMilestone;
    }

    public string GetReward()
    {
        if (this.specialModifier == "newsong")
        {
            return " (You'll unlock something special...)";
        }
        else
        {
            return " (+" + this.levelRewards[currentLevel - 1] + " GEMS)";
        }
    }

    // For when level could potentially be maxed and yield no data
    public void ForceGrabData()
    {
        if (!IsMaxLevel()) { return; }
        this.name = this.rawName + " (" + (currentLevel - 1) + "/" + numLevels + ")";
        this.description = this.rawDescription.Replace("{x}", this.levelReqs[currentLevel - 2].ToString()) + " (MAX)";
    }

    public bool IsMaxLevel()
    {
        return this.currentLevel > this.numLevels;
    }

    private void UpdateLevelInfo()
    {
        if (IsMaxLevel()) { return; }
        this.nextMilestone = this.levelReqs[currentLevel - 1];
        this.name = this.rawName + " (" + currentLevel + "/" + numLevels + ")";
        this.description = this.rawDescription.Replace("{x}", this.levelReqs[currentLevel - 1].ToString()) + this.GetReward();
    }
}

public class Song
{
    public List<string> movements;
    public string songName;
    public string bgName;
    public string playerName;
    public string enemyName;
    public string modifier;
    public string dialogue;
    public Song(string songName, string bgName, string playerName, string enemyName, string modifier = "None")
    {
        movements = new List<string>();
        this.songName = songName;
        this.bgName = bgName;
        this.playerName = playerName;
        this.enemyName = enemyName;
        if (modifier != null)
        {
            this.modifier = modifier;
        }
    }
}

public class PublicVariables : MonoBehaviour
{

    [Header("In-Game Variables")]
    [HideInInspector] public string song = "";
    public string dialogue = "";
    public List<string> songQueue = new List<string>();
    public string enemyName = "Floof";
    public string playerName = "Man";
    public string background = "DesertBG";
    public int points = 0;
    public int misses = 0;
    public int notesSpawned = 0;
    public int health = 50; // 0 is dead, 100 is full
    public string songDifficulty = "normal";
    private Text score;
    private string preScoreText = "Score: ";
    public bool isZoomedIn = false;
    public bool playerLost = false;
    public float hitPerfection = 0; // For special accuracy calculator
    public bool cameFromDebug = false;
    public bool cameFromGallery = false;
    public bool noSpecialAccuracy = false; // To optimize speed; whether or not PlayerPrefs special accuracy is on
    public int transitionSelection = 0;
    [Header("Locally Stored Variables")]
    public int levelSelected = 1;
    public int weekHighScore = 0;
    public bool alreadyLoadedGame = false;
    [Header("All Characters")]
    public string[] characters;
    [Header("All Song Instantiations")]
    public Song[] songs;
    public Song mirageSong = new Song("mirage", "Desert", "Skylar", "Camo");
    public Song oasisSong = new Song("oasis", "Desert", "Skylar", "Camo");
    public Song parchedSong = new Song("parched", "Desert", "Skylar", "Camo");
    public Song rallySong = new Song("rally", "City", "Skylar", "Lone");
    public Song belligerentSong = new Song("belligerent", "City", "Skylar", "Lone");
    public Song overkillSong = new Song("overkill", "City", "Skylar", "LoneAngry");
    public Song sussySlideSong = new Song("sussyslide", "Spaceship", "Skylar", "AriongUs");
    public Song amidstUsSong = new Song("amidstus", "Spaceship", "Skylar", "AriongUs");
    public Song rantNVentSong = new Song("rant&vent", "Spaceship", "Skylar", "AriongUs");
    public Song firewallSong = new Song("firewall", "Dataroom", "Skylar", "Adrian");
    public Song overclockSong = new Song("overclock", "Dataroom", "Skylar", "Adrian");
    public Song breachedSong = new Song("breached", "Dataroom", "Skylar", "Adrian");
    public Song scribblesSong = new Song("scribbles", "Paper", "Man", "Domino", "Paper");
    public Song theZoneSong = new Song("thezone", "Paper", "Man", "Domino", "Paper");
    public Song scratchesSong = new Song("scratches", "Paper", "Man", "Domino", "Paper");
    public Song seashellSong = new Song("seashell", "Ocean", "Skylar", "Oliver", "Water");
    public Song yachtClubSong = new Song("yachtclub", "Ocean", "Skylar", "Oliver", "Water");
    public Song encounterSong = new Song("encounter", "Alley", "Skylar", "Mew");
    public Song psybeamSong = new Song("psybeam", "Alley", "Skylar", "Mew");
    public Song futureSightSong = new Song("futuresight", "Alley", "Skylar", "Mew");
    public Song endlessSong = new Song("endless", "Alley", "Lone", "MewAngry");
    public Song eternalSong = new Song("eternal", "Alley", "Lone", "MewAngry");
    public Song evolutionSong = new Song("evolution", "Alley", "Lone", "MewAngry");
    public Song rushinSong = new Song("rushin", "Alley", "Skylar", "Lone");
    public Song dateNightSong = new Song("datenight", "Alley", "Skylar", "Lone");
    public Song beatdownSong = new Song("beatdown", "Alley", "Skylar", "Camo");
    public Song svengoolieSong = new Song("svengoolie", "Alley", "Skylar", "NULL");
    public Song epilogueSong = new Song("epilogue", "Library", "Oliver", "Patches");
    public Song climaxSong = new Song("climax", "Library", "Oliver", "PatchesCrazy");
    public Song resolutionSong = new Song("resolution", "Library2", "Oliver", "PatchesCrazy");
    public Song nullSong = new Song("null", "Library2", "Skylar", "NULL");
    //public Song exceptionSong = new Song("exception", "Library2", "Skylar", "NULL");
    public Song unwelcomeSong = new Song("unwelcome", "Library", "Skylar", "Patches");
    public Dictionary<int, string[]> splitWeekSongs = new Dictionary<int, string[]>();
    public Dictionary<string, Color> arrowColors = new Dictionary<string, Color>();
    [Header("All Control Assignments")]
    public List<KeyCode> keycodes = new List<KeyCode>();
    private float lastLostUpdate = 0;
    private float lastSwitchUpdate = 0;
    private float lossScale = -1; // For scaling character during Loss
    [Header("External Assignments")]
    public AudioSource musicPlayer;
    public AudioClip scrollSound;
    public AudioClip confirmSound;
    public AudioClip achieveSound;
    public AudioClip missOne;
    public AudioClip missTwo;
    public AudioClip missThree;
    public AudioClip clickSFX;
    public TextAsset songData;
    public TextAsset achievementData;
    public string[] weekSongs;
    public Achievement[] achievements;
    [Header("Debug Menu")]
    public AudioClip currentSong; // For debug menu
    public string currentMovements; // For debug menu
    public List<string> songList; // For debug menu
    [Header("Other Object Assignments")]
    public GameObject achievementObject;
    private GameObject retryTextObject;
    private GameObject playerObject;
    private WaitForSeconds wfs;

    public void RenderAchievement(string name, string desc)
    {
        GameObject g = Instantiate(achievementObject);
        g.GetComponent<AchievementHandler>().LoadAchievement(name, desc);
    }

    private void ResetAchievements()
    {
        for (int i = 0; i < achievements.Length; i++)
        {
            PlayerPrefs.SetInt("Achievement" + i, 0);
        }
    }

    public void LoadAchievements()
    {
        List<string> rawAchievementData = new List<string>(achievementData.text.Split('\n'));
        achievements = new Achievement[rawAchievementData.Count - 1];
        rawAchievementData.RemoveAt(0);
        foreach (string achievement in rawAchievementData)
        {
            // CREATING ACHIEVEMENT LIST
            // num, title, desc, level, #levels, x values..., reward values for x...
            string[] data = achievement.Split('#')[1].Split(',');
            int[] levelReqs = new int[int.Parse(data[3])];
            int[] levelRewards = new int[int.Parse(data[3])];
            for (int i = 0; i < int.Parse(data[3]); i++)
            {
                levelReqs[i] = int.Parse(data[i + 4]);
            }
            for (int i = 0; i < int.Parse(data[3]); i++)
            {
                levelRewards[i] = int.Parse(data[i + 4 + int.Parse(data[3])]);
            }
            achievements[int.Parse(data[0]) - 1] = new Achievement(data[0], data[1], data[2], data[3], levelReqs, levelRewards, (data[data.Length - 1] == "(s)") ? data[data.Length - 2] : "None");
        }
    }

    public void RenderLoginAchievement()
    {
        // UPDATING ACHIEVEMENT ONE (LOG IN #S)
        achievements[0].Increment(1);
        if (achievements[0].NextLevel())
        {
            RenderAchievement(achievements[0].name, achievements[0].description);
        }
    }

    void LoadSongs()
    {
        string[] songNotes = songData.text.Split('\n');
        int currSong = 0;
        string[] chars = new string[] { "Oliver", "Camo", "Lone", "LoneAngry", "Camo&Lone", "Adrian", "Skylar",
        "Domino", "Jackob", "JackobAngry", "JackobShadow", "Mew", "MewAngry", "Man", "NULL", "Patches",
        "PatchesCrazy", "Ariel", "AriongUs", "JakobHuman", "Floof", "Henry" };
        characters = chars;
        songs = new Song[] { mirageSong, oasisSong, parchedSong, rallySong, belligerentSong,
        overkillSong, encounterSong, psybeamSong, futureSightSong,
        sussySlideSong, amidstUsSong, rantNVentSong, firewallSong, overclockSong, breachedSong,
        seashellSong, yachtClubSong, scribblesSong, theZoneSong,
        scratchesSong, unwelcomeSong, epilogueSong, climaxSong, resolutionSong,
        rushinSong, dateNightSong, beatdownSong, eternalSong, endlessSong, evolutionSong, svengoolieSong, nullSong };
        for (int i = 0; i < songNotes.Length - 2; i += 5)
        {
            for (int j = i + 1; j < i + 4; j++)
            {
                songs[currSong].movements.Add(songNotes[j].Substring(0, songNotes[j].Length - 1));
            }
            if (!songNotes[i + 4].Contains("null"))
            {
                songs[currSong].dialogue = songNotes[i + 4].Substring(3, songNotes[i + 4].Length - 5);
            }
            currSong += 1;
        }
        for (int i = 0; i < weekSongs.Length; i++)
        {
            splitWeekSongs.Add(i + 1, weekSongs[i].Split('|'));
        }
#if UNITY_WEBGL
            for (int i = 0 ; i < weekSongs.Length + 1; i++) {
                PlayerPrefs.SetInt("Week" + i + "Complete", 1);
            }
#endif
    }

    void LoadColors()
    {
        arrowColors.Add("left", new Color(0.8f, 0.3f, 0.8f));
        arrowColors.Add("up", new Color(0.3f, 1, 0.3f));
        arrowColors.Add("down", new Color(0.3f, 0.9f, 1));
        arrowColors.Add("right", new Color(1f, 0.3f, 0.3f));
        arrowColors.Add("lefthit", new Color(1, 0.6f, 1));
        arrowColors.Add("uphit", new Color(0.5f, 1, 0.6f));
        arrowColors.Add("downhit", new Color(0.35f, 1, 0.95f));
        arrowColors.Add("righthit", new Color(1f, 0.47f, 0.47f));
        arrowColors.Add("leftmiss", new Color(0.7f, 0.5f, 0.7f, 0.8f));
        arrowColors.Add("upmiss", new Color(0.5f, 0.7f, 0.5f, 0.8f));
        arrowColors.Add("downmiss", new Color(0.5f, 0.7f, 0.8f, 0.8f));
        arrowColors.Add("rightmiss", new Color(0.8f, 0.5f, 0.5f, 0.8f));
    }

    // 0 = W A S D and arrow keys
    // 1 = D F J K and arrow keys
    public void UpdateKeys()
    {
        int kbOption = PlayerPrefs.GetInt("Keybinds");
        keycodes.Clear();
        keycodes.Add(KeyCode.LeftArrow); // 0
        keycodes.Add((kbOption == 0) ? KeyCode.A : KeyCode.D); // 1 (alt)
        keycodes.Add(KeyCode.RightArrow); // 2
        keycodes.Add((kbOption == 0) ? KeyCode.D : KeyCode.K); // 3 (alt)
        keycodes.Add(KeyCode.UpArrow); // 4
        keycodes.Add((kbOption == 0) ? KeyCode.W : KeyCode.J); // 5 (alt)
        keycodes.Add(KeyCode.DownArrow); // 6
        keycodes.Add((kbOption == 0) ? KeyCode.S : KeyCode.F); // 7 (alt)
    }

    void Awake()
    {
        if (PlayerPrefs.GetInt("money") < 0)
        {
            PlayerPrefs.SetInt("money", 0);
        }
        transitionSelection = -2;
        if (FindObjectsOfType<PublicVariables>().Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        if (GameObject.FindGameObjectsWithTag("AudioPlayer").Length > 1)
        {
            Destroy(musicPlayer);
        }
        else
        {
            DontDestroyOnLoad(musicPlayer);
        }
        DontDestroyOnLoad(transform.gameObject);
        LoadSongs();
        LoadColors();
        UpdateKeys();
        LoadAchievements();
    }

    void Lose()
    {
        GameObject.Find("Darkness").SetActive(false);
        GameObject.Find("DeathCover").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("DeathCover").GetComponent<SpriteRenderer>().color = new Color(20, 20, 20, 1);
        playerLost = true;
        GameObject.Find("GameController").GetComponent<LevelHandler>().DestroyScript();
    }

    public void StoreAssetsInPublicVariables()
    {
        playerObject = GameObject.Find("Player");
        retryTextObject = GameObject.Find("RetryText");
    }

    void Update()
    {
        if (playerLost)
        {
            transitionSelection = 0;
            if (lossScale == -1)
            {
                retryTextObject = GameObject.Find("RetryText");
                lossScale = 0;
                retryTextObject.GetComponent<Text>().enabled = true;
                retryTextObject.GetComponent<Text>().color = new Color(1, 1, 1, 0f);
            }
            lastLostUpdate += Time.deltaTime;
            if (lastLostUpdate > 0.08f)
            {
                lastLostUpdate = 0;
                Vector3 cPos = GameObject.Find("Main Camera").transform.position;
                Vector3 pPos = playerObject.transform.position;
                // Change transparency of the RETRY text.
                Color retryColor = retryTextObject.GetComponent<Text>().color;
                if (retryColor.a < 1)
                {
                    retryTextObject.GetComponent<Text>().color = new Color(1, 1, 1, retryColor.a + 0.025f);
                }
                // Change scale of the PLAYER sprite.
                if (Mathf.Abs(lossScale) < 0.4f)
                {
                    Vector3 currScale = playerObject.transform.localScale;
                    lossScale += currScale.x * 0.03f;
                    playerObject.transform.localScale = new Vector3(currScale.x + (currScale.x * 0.03f), currScale.y + (currScale.y * 0.03f), currScale.z);
                }
                // Move camera towards PLAYER position.
                float xDifference = 0;
                float yDifference = 0;
                if (pPos.x > cPos.x)
                {
                    xDifference = Mathf.Min((pPos.x - cPos.x) / 8, 0.96f);
                }
                if (pPos.y < cPos.y)
                {
                    yDifference = Mathf.Max((cPos.y - pPos.y) / 8, -0.15f);
                }
                GameObject.Find("Main Camera").transform.position = new Vector3(cPos.x + xDifference, cPos.y - yDifference, cPos.z);
            }
            // If SPACE key pressed, reset variables and reload scene.
            if (retryTextObject.GetComponent<Text>().color.a >= 0.1f && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                RestartGame();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            {
                BackToMenu();
            }
        }
        lastSwitchUpdate += Time.deltaTime;
        if (transitionSelection == 1 && !playerLost)
        {
            if (lastSwitchUpdate > 0.05f)
            {
                lastSwitchUpdate = 0;
                Vector3 cPos = GameObject.Find("Main Camera").transform.position;
                Vector3 pPos = playerObject.transform.position / 3;
                // Move camera towards PLAYER position.
                float xDifference = 0;
                if (pPos.x > cPos.x)
                {
                    xDifference = Mathf.Min((pPos.x - cPos.x) / 6, 0.96f);
                }
                GameObject.Find("Main Camera").transform.position = new Vector3(cPos.x + xDifference, cPos.y, cPos.z);
            }
        }
        if (transitionSelection == -1 && !playerLost)
        {
            if (lastSwitchUpdate > 0.05f)
            {
                lastSwitchUpdate = 0;
                Vector3 cPos = GameObject.Find("Main Camera").transform.position;
                Vector3 ePos = GameObject.Find("Enemy").transform.position / 3;
                // Move camera towards ENEMY position.
                float xDifference = 0;
                if (ePos.x < cPos.x)
                {
                    xDifference = Mathf.Min((ePos.x - cPos.x) / 6, 0.96f);
                }
                GameObject.Find("Main Camera").transform.position = new Vector3(cPos.x + xDifference, cPos.y, cPos.z);
            }
        }
        if (transitionSelection == 0 && !playerLost)
        {
            if (lastSwitchUpdate > 0.05f)
            {
                lastSwitchUpdate = 0;
                Vector3 cPos = GameObject.Find("Main Camera").transform.position;
                Vector3 ePos = Vector3.zero;
                // Move camera towards ENEMY position.
                float xDifference = 0;
                xDifference = Mathf.Min((ePos.x - cPos.x) / 6, 0.96f);
                GameObject.Find("Main Camera").transform.position = new Vector3(cPos.x + xDifference, cPos.y, cPos.z);
            }
        }
    }

    public void RestartGame()
    {
        playerLost = false;
        musicPlayer.loop = false;
        musicPlayer.Stop();
        StartCoroutine(ResetScale());
        SceneManager.LoadScene("Game");
    }

    public void BackToMenu()
    {
        playerLost = false;
        musicPlayer.loop = false;
        musicPlayer.Stop();
        lossScale = -1;
        songQueue = new List<string>();
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().cameFromDebug = false;
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().transitionSelection = -2;
        if (cameFromGallery)
        {
            SceneManager.LoadScene("Gallery");
        }
        else
        {
            SceneManager.LoadScene("StorySelect");
        }
    }

    IEnumerator ResetScale()
    {
        wfs = new WaitForSeconds(0.1f);
        yield return wfs;
        GameObject.Find("PublicVariables").GetComponent<SongData>().SetCharacter(playerName, false);
        lossScale = -1;
    }

    public void IconAdjust()
    {
        // If health is under 0
        if (health <= 0)
        {
            Lose();
            return;
        }
        // Change icons at relative health
        if (health >= 80)
        {
            GameObject.Find("EnemyIcon").GetComponent<IconSprites>().ToLosing();
        }
        else
        {
            GameObject.Find("EnemyIcon").GetComponent<IconSprites>().ToNormal();
        }
        if (health < 20)
        {
            GameObject.Find("PlayerIcon").GetComponent<IconSprites>().ToLosing();
        }
        else
        {
            GameObject.Find("PlayerIcon").GetComponent<IconSprites>().ToNormal();
        }
        // Edit bar
        RectTransform enemyRect = (RectTransform)GameObject.Find("EnemyBar").transform;
        // Bar scale 0 is 100, 1 is 50, 2 is 0
        GameObject.Find("EnemyBar").transform.localScale = new Vector3((float)(100 - health) / 50.0f, 1, 1);
        float distance = GameObject.Find("BarMin").transform.position.x - GameObject.Find("BarMax").transform.position.x;
        Vector3 initPos = GameObject.Find("BarIcons").transform.position;
        GameObject.Find("BarIcons").transform.position = new Vector3(GameObject.Find("BarMin").transform.position.x + (-1 * distance * ((float)health / 100)), initPos.y, initPos.z);
    }

    public void Miss()
    {
        if (playerObject.GetComponent<SpriteHandler>().lost || songDifficulty == "auto") { return; }
        AudioClip missSound = null;
        switch (Mathf.Round(Random.Range(1, 4)))
        {
            case 1:
                missSound = missOne;
                break;
            case 2:
                missSound = missTwo;
                break;
            case 3:
                missSound = missThree;
                break;
        }
        musicPlayer.PlayOneShot(missSound, 0.7f);
        GameObject.Find("GameController").GetComponent<LevelHandler>().currentStreak = 0;
        points -= 10;
        health -= 3;
        updateScore();
        playerObject.GetComponent<SpriteHandler>().Miss();
        IconAdjust();
    }

    public void Deplete(int amount = 1, bool canKill = true)
    {
        if (playerObject.GetComponent<SpriteHandler>().lost || songDifficulty == "auto") { return; }
        RectTransform enemyRect = (RectTransform)GameObject.Find("EnemyBar").transform;
        if (!canKill && health == 1) { return; }
        health -= amount;
        IconAdjust();
    }

    public void Hit(int pointsEarned)
    {
        if (playerObject.GetComponent<SpriteHandler>().lost) { return; }
        // This means it's not a drag note
        if (pointsEarned != 50)
        {
            if (PlayerPrefs.GetInt("AudibleClick") == 1)
            {
                musicPlayer.PlayOneShot(clickSFX);
            }
            achievements[1].Increment(1);
            if (achievements[1].NextLevel())
            {
                RenderAchievement(achievements[1].name, achievements[1].description);
            }
            GameObject.Find("GameController").GetComponent<LevelHandler>().currentStreak += 1;
            achievements[5].Set(GameObject.Find("GameController").GetComponent<LevelHandler>().currentStreak);
            if (achievements[5].NextLevel())
            {
                RenderAchievement(achievements[5].name, achievements[5].description);
            }
        }
        points += pointsEarned;
        updateScore();
        if (pointsEarned == 50) { return; } // This means it's a drag note
        health += 2;
        health = Mathf.Min(health, 100);
        IconAdjust();
    }

    public void updateScore()
    {
        if (GameObject.Find("Points") != null)
        {
            score = GameObject.Find("Points").GetComponent<Text>();
            score.text = preScoreText + points.ToString();
        }
        if (GameObject.Find("Accuracy") != null && notesSpawned > 0)
        {
            if (noSpecialAccuracy)
            {
                GameObject.Find("Accuracy").GetComponent<Text>().text = "Accuracy: " + ((float)(notesSpawned - misses) / notesSpawned * 100).ToString("N1") + "% (-" + misses + ")";
            }
            else
            {
                GameObject.Find("Accuracy").GetComponent<Text>().text = "Accuracy: " + ((float)hitPerfection / notesSpawned * 100).ToString("N1") + "% (-" + misses + ")";
            }
        }
    }

    public void UpdateHighScore()
    {
        cameFromDebug = false;
        musicPlayer.loop = false;
        musicPlayer.Stop();
        if (PlayerPrefs.GetInt("DuetMode") == 1) { return; }
        if (points > PlayerPrefs.GetInt(song + GetDifficulty()))
        {
            PlayerPrefs.SetInt(song + GetDifficulty(), points);
        }
        if (!cameFromGallery)
        {
            weekHighScore += points;
            string weekName = "Week" + levelSelected + "HS" + GetDifficulty();
            if (GetDifficulty() == 2)
            {
                achievements[6].IncrementIfValue(levelSelected, levelSelected - 1);
                if (achievements[6].NextLevel())
                {
                    RenderAchievementAfterTime(achievements[6].name, achievements[6].description, 1);
                }
            }
            if (songQueue.Count == 0)
            {
                PlayerPrefs.SetInt("Week" + levelSelected + "Complete", 1);
            }
            if (songQueue.Count == 0 && weekHighScore > PlayerPrefs.GetInt(weekName))
            {
                PlayerPrefs.SetInt(weekName, weekHighScore);
                weekHighScore = 0;
            }
        }
        if (points <= 0) { return; }
        int profit = GetMoneyEarned();
        PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money") + profit);
        achievements[3].Set((int)((float)hitPerfection / notesSpawned * 100));
        while (achievements[3].NextLevel())
        {
            StartCoroutine(RenderAchievementAfterTime(achievements[3].name, achievements[3].description, 0.5f));
        }
    }

    private IEnumerator RenderAchievementAfterTime(string name, string desc, float time)
    {
        wfs = new WaitForSeconds(time);
        yield return wfs;
        RenderAchievement(name, desc);
    }

    public int GetMoneyEarned()
    {
        // 0.35x for easy, 0.4x for normal, 0.45x for hard
        float multiplier = 0.35f + 0.05f * GetDifficulty();
        int profit = (int)(multiplier * Mathf.Sqrt((float)points));
        return profit;
    }

    public int GetDifficulty()
    {
        switch (songDifficulty)
        {
            case "easy":
                return 0;
            case "normal":
                return 1;
            case "hard":
            case "auto":
                return 2;
        }
        Debug.Log("ERROR FETCHING VALID DIFFICULTY (PUBLICVARIABLES)");
        return -1;
    }

    public void CameraToEnemy()
    {
        if (isZoomedIn) { return; }
        //if (GameObject.Find("Main Camera").GetComponent<CameraScale>().sceneWidth <= 19) { return; }
        if (!playerLost && transitionSelection != -3)
        {
            transitionSelection = -1;
        }
    }

    public void CameraToPlayer()
    {
        if (isZoomedIn) { return; }
        //if (GameObject.Find("Main Camera").GetComponent<CameraScale>().sceneWidth <= 19) { return; }
        if (!playerLost && transitionSelection != -3)
        {
            transitionSelection = 1;
        }
    }

    public void CameraToCenter()
    {
        if (isZoomedIn) { return; }
        //if (GameObject.Find("Main Camera").GetComponent<CameraScale>().sceneWidth <= 19) { return; }
        if (!playerLost && transitionSelection != -3)
        {
            transitionSelection = 0;
        }
    }

    public void ScrollSound()
    {
        musicPlayer.PlayOneShot(scrollSound);
    }

    public void ConfirmSound()
    {
        musicPlayer.PlayOneShot(confirmSound);
    }

    public void AchieveSound()
    {
        musicPlayer.PlayOneShot(achieveSound);
    }

    public void SetSongText()
    {
        GameObject.Find("SongName").GetComponent<Text>().text = (song + " (" + songDifficulty + ")").ToUpper();
    }

    public Vector2 GetDimensions()
    {
        Rect rect = GameObject.Find("Canvas").GetComponent<RectTransform>().rect;
        Vector2 scale = GameObject.Find("Canvas").transform.localScale;
        return new Vector2(rect.width * scale.x, rect.height * scale.y);
    }

    public void ShakeScreen()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float rand = Random.Range(-0.1f, 0.1f);
        GameObject bg = GameObject.Find(GameObject.Find("PublicVariables").GetComponent<PublicVariables>().background);
        bg.transform.position -= new Vector3(0.1f, rand, 0);
        wfs = new WaitForSeconds(0.03f);
        yield return wfs;
        bg.transform.position += new Vector3(0.2f, rand * 2, 0);
        wfs = new WaitForSeconds(0.03f);
        yield return wfs;
        bg.transform.position -= new Vector3(0.1f, rand, 0);
        // if (playerMiss) {
        //     playerObject.GetComponent<SpriteHandler>().Miss();
        // }
        StartCoroutine(GameObject.Find("GameController").GetComponent<LevelHandler>().Bump());
    }

}
