using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VolumeHandler : MonoBehaviour
{
    
    public float volume = 0;
    public float maxVolume = 8;
    [SerializeField] private GameObject bar;
    [SerializeField] private GameObject template;
    private AudioSource audioSource;
    [SerializeField] private AudioClip volumeClip;
    private float lastVolumeUpdate = 0;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < maxVolume; i++) {
            volume += 1;
            LoadBar(1, false);
        }
        AudioToggle(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().musicPlayer.Stop();
        if (template == null) {
            template = GameObject.Find("TemplateAudio");
        }
        float curr = volume;
        volume = 0;
        for (int i = 0; i < curr; i++) {
            volume += 1;
            LoadBar(1, false);
        }
        AudioToggle(false);
    }

    void Update()
    {
        #if UNITY_STANDALONE || UNITY_WEBGL
            if (Input.GetKeyDown("-")) {
                lastVolumeUpdate = Time.time;
                AudioToggle(true);
                StartCoroutine(Disappear());
                if (volume > 0) {
                    volume -= 1;
                    LoadBar(-1);
                }
            }
            if (Input.GetKeyDown("=")) {
                lastVolumeUpdate = Time.time;
                AudioToggle(true);
                StartCoroutine(Disappear());
                if (volume < maxVolume) {
                    volume += 1;
                    LoadBar(1);
                }
            }
        #endif
    }
    
    void AudioToggle(bool display) {
        GameObject[] g = GameObject.FindGameObjectsWithTag("Audio");
        foreach (GameObject elem in g) {
            if (elem.GetComponent<Image>() != null) {
                elem.GetComponent<Image>().enabled = display;
            } else {
                elem.GetComponent<Text>().enabled = display;
            }
        }
    }

    IEnumerator Disappear() {
        while (true) {
            if (Time.time > lastVolumeUpdate + 1.8f) {
                AudioToggle(false);
                yield break;
            }
            yield return null;
        }
    }

    void LoadBar(int direction, bool playSound = true) {
        GameObject.Find("Main Audio Source").GetComponent<AudioSource>().volume = volume / maxVolume;
        if (playSound) {
            audioSource.PlayOneShot(volumeClip, volume / maxVolume);
        }
        if (direction == -1) {
            GameObject.Destroy(GameObject.Find("Bar" + (volume + 1)));
        } else if (direction == 1) {
            GameObject b = Instantiate(bar);
            b.transform.SetParent(GameObject.Find("AudioGroup").transform);
            b.name = "Bar" + volume;
            b.transform.localScale = new Vector3(1, 1, 1);
            b.transform.position = template.transform.position + new Vector3((volume - 1) * 50 * GameObject.Find("Canvas").transform.localScale.x, 0, 0);
        }
    }
}
