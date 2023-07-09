using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public bool setVars = false;
    void Update() {
        if (Input.touchCount == 0) { return; }
        Vector3 pos = Camera.main.ScreenToWorldPoint (Input.GetTouch(0).position);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null && hit.collider.name == "MobileBack") {
            if (setVars) {
                GameObject.Find("PublicVariables").GetComponent<PublicVariables>().BackToMenu();
            } else {
                SceneManager.LoadScene("Start");
            }
        }
    }
}
