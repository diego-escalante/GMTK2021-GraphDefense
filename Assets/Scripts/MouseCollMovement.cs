using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCollMovement : MonoBehaviour {

    private Camera cam;

    private void Start() {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update() {
        transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
    }
}
