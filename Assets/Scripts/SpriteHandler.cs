using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    
    public Sprite left;
    public Sprite right;
    public Sprite down;
    public Sprite up;
    public Sprite miss;
    public Sprite idle;
    public float resetDelay = 0.6f;

    private float lastUpdate;
    public bool isPlayer;
    public bool lost = false;
    private float shadowEffectOffset = 0.25f;
    public bool shadowEffect = false;
    public bool isClone = false;
    public float shadowOpacity = 0.3f;
    private bool isSquishVert = false;
    private bool isSquishHor = false;

    IEnumerator Squish(bool squishVert, bool squishHor) {
        if ((squishVert && isSquishVert) || (squishHor && isSquishHor)) { yield break; }
        if (squishVert) {
            isSquishVert = true;
        }
        if (squishHor) {
            isSquishHor = true;
        }
        transform.localScale -= new Vector3((squishVert) ? 0.03f : 0, (squishHor) ? 0.03f : 0, 0);
        yield return new WaitForSeconds(0.1f);
        transform.localScale += new Vector3((squishVert) ? 0.03f : 0, (squishHor) ? 0.03f : 0, 0);
        if (squishVert) {
            isSquishVert = false;
        }
        if (squishHor) {
            isSquishHor = false;
        }
    }

    IEnumerator Reset() {
        if (shadowEffect && PlayerPrefs.GetInt("Quality") != 1) {
            MakeShadow();
        }
        yield return new WaitForSeconds(resetDelay);
        if (Time.time - lastUpdate > resetDelay && !lost) {
            GetComponent<SpriteRenderer>().sprite = idle;
        }
    }

    IEnumerator Delete() {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void MakeShadow() {
        GameObject clone = gameObject;
        clone.GetComponent<SpriteHandler>().isClone = true;
        Instantiate(clone);
    }

    void Start() {
        if (isClone) {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, shadowOpacity);
            transform.position += new Vector3(Random.Range(-1 * shadowEffectOffset, shadowEffectOffset), Random.Range(-1 * shadowEffectOffset, shadowEffectOffset), 1);
            StartCoroutine(Delete());
        }
        lastUpdate = Time.time;
    }

    public IEnumerator ChangeColor() {
        float h, s, v;
        Color.RGBToHSV(GetComponent<SpriteRenderer>().color, out h, out s, out v);
        if (h >= 1) {
            h = 0;
        }
        GetComponent<SpriteRenderer>().color = Color.HSVToRGB(h + 0.01f, 0.45f, v);
        yield return new WaitForSeconds(0.06f);
        StartCoroutine(ChangeColor());
    }
    
    public void Miss() {
        GetComponent<SpriteRenderer>().sprite = miss;
        StartCoroutine(Reset());
        lastUpdate = Time.time;
    }

    public void Loss() {
        lost = true;
        GetComponent<SpriteRenderer>().sprite = miss;
    }

    public void LeftNote() {
        if (lost) { return; }
        StartCoroutine(Squish(true, false));
        if (isPlayer) {
            GetComponent<SpriteRenderer>().sprite = right;
        } else {
            GetComponent<SpriteRenderer>().sprite = left;
        }
        StartCoroutine(Reset());
        lastUpdate = Time.time;
    }

    public void RightNote() {
        if (lost) { return; }
        StartCoroutine(Squish(true, false));
        if (isPlayer) {
            GetComponent<SpriteRenderer>().sprite = left;
        } else {
            GetComponent<SpriteRenderer>().sprite = right;
        }
        StartCoroutine(Reset());
        lastUpdate = Time.time;
    }

    public void UpNote() {
        if (lost) { return; }
        StartCoroutine(Squish(false, true));
        GetComponent<SpriteRenderer>().sprite = up;
        StartCoroutine(Reset());
        lastUpdate = Time.time;
    }

    public void DownNote() {
        if (lost) { return; }
        StartCoroutine(Squish(false, true));
        GetComponent<SpriteRenderer>().sprite = down;
        StartCoroutine(Reset());
        lastUpdate = Time.time;
    }

}
