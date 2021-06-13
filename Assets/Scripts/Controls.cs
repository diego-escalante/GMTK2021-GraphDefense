using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

    [SerializeField]
    private float maxWireLength = 5f;
    [SerializeField]
    private LayerMask buildingsMask;

    private LineRenderer lineRenderer;
    private Camera cam;
    private BaseStructure selected, highlighted;
    private WireDrawer wireDrawer;

    private void Awake() {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        lineRenderer = GetComponent<LineRenderer>();
        wireDrawer = GetComponent<WireDrawer>();
    }

    private void Update() {

        if (selected == null) {
            List<BaseStructure> connection = wireDrawer.FindMousedOverConnection();
            if (connection != null) {
                if (Input.GetMouseButtonDown((int)MouseButton.right)) {
                    connection[0].DisconnectFrom(connection[1]);
                }
                lineRenderer.positionCount = 2;
                lineRenderer.SetColor(Color.red);
                lineRenderer.SetPositions(new Vector3[]{connection[0].transform.position + new Vector3(0, 0, 0.02f), connection[1].transform.position + new Vector3(0, 0, 0.02f)});
            } else {
                lineRenderer.positionCount = 0;
            }
        }

        if (Input.GetMouseButtonDown((int)MouseButton.right)) {
            if (selected != null) {
                selected.Deselect();
                selected = null;
                lineRenderer.positionCount = 0;
            }
        }

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0.02f);
        Collider2D coll = Physics2D.OverlapPoint(mousePos, buildingsMask);

        // If not mousing over an object
        if (coll == null) {
            // If a structure is selected, draw a line to the mouse.
            if (selected != null) {
                lineRenderer.SetPosition(1, mousePos);
                lineRenderer.SetColor(lineRenderer.Length() > maxWireLength ? Color.red : Color.yellow);
            }

            // If a structure is highlighted, deselect it.
            if (highlighted != null) {
                highlighted.Deselect();
                highlighted = null;
            }
        } else {
            // If mousing over an object
            BaseStructure current = coll.GetComponent<BaseStructure>();

            // If the current object is not a selected object, highlight it.
            if (selected == null || current != selected) {
                highlighted = current;
                highlighted.Highlight();
            }

            // If we are clicking the object
            if (Input.GetMouseButtonDown((int)MouseButton.left)) {
                if (selected == null) {
                    selected = current;
                    highlighted = null;
                    selected.Select();
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPositions(new Vector3[]{new Vector3(selected.transform.position.x, selected.transform.position.y, 0.02f), mousePos});
                } else if (selected != current && Vector2.Distance(selected.transform.position, current.transform.position) <= maxWireLength) {
                    // Connect the selected object and the current object together.
                    selected.ConnectTo(current);
                    selected.Deselect();
                    selected = null;
                    lineRenderer.positionCount = 0;
                }
            } else if (selected != null) {
                lineRenderer.SetPosition(1, new Vector3(current.transform.position.x, current.transform.position.y, 0.02f));
                lineRenderer.SetColor(lineRenderer.Length() > maxWireLength ? Color.red : Color.yellow);
            }
        }

        // if (coll != null) {
        //     if (Input.GetMouseButtonDown((int)MouseButton.left)) {
        //         // Clicking on something.
        //         if (selected != null) {
        //             // Deselect anything currently selected.
        //             selected.Deselect();
        //             selected.ConnectTo(coll.GetComponent<BaseStructure>());
        //             selected = null;
        //             lineRenderer.positionCount = 0;
        //         } else {
        //             // Select the new structure.
        //             highlighted = null;
        //             selected = coll.GetComponent<BaseStructure>();
        //             selected.Select();
        //             lineRenderer.positionCount = 2;
        //             Vector3[] positions = {new Vector3(selected.transform.position.x, selected.transform.position.y, 0.02f), mousePos};
        //             lineRenderer.SetPositions(positions);
        //         }
        //     } else {
        //         // Mousing over something.
        //         BaseStructure currentStructure = coll.GetComponent<BaseStructure>();
        //         if (selected != currentStructure) {
        //             if (highlighted != null) {
        //                 highlighted.GetComponent<BaseStructure>().Deselect();
        //             }
        //             currentStructure.Highlight();
        //             highlighted = coll;
        //         }
        //     }
        // } else {
        //     // Not mousing over anything, make sure there's nothing highlighted.
        //     if (highlighted != null) {
        //         highlighted.GetComponent<BaseStructure>().Deselect();
        //         highlighted = null;
        //     }
        // }
    }

    private enum MouseButton {
            left = 0,
            right = 1,
            middle = 2
    }
}



