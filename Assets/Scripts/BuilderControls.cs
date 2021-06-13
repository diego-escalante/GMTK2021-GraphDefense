using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BuilderControls : MonoBehaviour {

    [SerializeField]
    private GameObject turretPrefab;
    [SerializeField]
    private GameObject extractorPrefab;
    [SerializeField]
    private GameObject transferNodePrefab;
    [SerializeField]
    private LayerMask structureMask;
    [SerializeField]
    private Button extractorButton;
    [SerializeField]
    private Button turretButton;
    [SerializeField]
    private Button transferNodeButton;
    [SerializeField]
    private int maxBuildCapacity = 3;

    private Camera cam;
    private Controls controls;

    private bool building = false;

    private void Awake() {
        controls = GetComponent<Controls>();
    }

    private void OnEnable() {
        SetButtonsEnableState(true);
    }

    private void OnDisable() {
        SetButtonsEnableState(false);
    }

    private void Update() {
        if (!building) {
            SetButtonsEnableState(!AtBuildCapacity());
        }
    }

    private void Start() {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }
    
    public void BuildExtractor() {
        if (this.enabled) {
            StartCoroutine(BuildingCoroutine(extractorPrefab));
        }
    }

    public void BuildTurret() {
        if (this.enabled) {
            StartCoroutine(BuildingCoroutine(turretPrefab));
        }
    }

    public void BuildTransferNode() {
        if (this.enabled) {
            StartCoroutine(BuildingCoroutine(transferNodePrefab));
        }
    }

    private IEnumerator BuildingCoroutine(GameObject structurePrefab) {

        building = true;
        controls.enabled = false;
        SetButtonsEnableState(false);

        Transform buildSite = new GameObject("Build Site").transform;
        buildSite.localScale = structurePrefab.transform.localScale;
        SpriteRenderer prefabRenderer = structurePrefab.GetComponent<SpriteRenderer>();
        SpriteRenderer buildSpriteRenderer = buildSite.gameObject.AddComponent<SpriteRenderer>();
        buildSpriteRenderer.sprite = prefabRenderer.sprite;

        while (true) {
            buildSpriteRenderer.color = new Color(prefabRenderer.color.r, prefabRenderer.color.g, prefabRenderer.color.b, 0.5f);
            buildSite.position = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
            if (Physics2D.OverlapCircle(buildSite.position, 0.75f, structureMask) != null) {
                buildSpriteRenderer.color = Color.red;
            } else if (Input.GetMouseButtonDown(1)) {
                break;
            } else if (Input.GetMouseButtonDown(0)) {
                Instantiate(structurePrefab, buildSite.transform.position, Quaternion.identity);
                break;
            }
            yield return null;
        }

        Destroy(buildSite.gameObject);
        controls.enabled = true;
        building = false;
        SetButtonsEnableState(true);

    }

    private void SetButtonsEnableState(bool state) {
        extractorButton.interactable = state;
        turretButton.interactable = state;
        transferNodeButton.interactable = state;
    }

    private bool AtBuildCapacity() {
        return GameObject.FindGameObjectsWithTag("Structure").Where(go => !go.GetComponent<BaseStructure>().isBuilt).Count() >= maxBuildCapacity;
    }

}
