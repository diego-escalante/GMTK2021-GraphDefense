using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class BaseStructure : MonoBehaviour {

    [Header("Storage")]
    [SerializeField]
    protected float capacity = 100f;
    [SerializeField]
    protected float storage = 0;

    [Header("Health")]
    [SerializeField]
    private float totalHealth = 100f;
    [SerializeField]
    private float currentHealth = 100f;
    
    [Header("Highlighting and Selecting")]
    [SerializeField]
    private Color highlightedColor = Color.white;
    [SerializeField]
    private Color selectedColor = Color.green;
    [SerializeField]
    private float minScale = 1.1f;
    [SerializeField]
    private float maxScale = 1.2f;
    [SerializeField]
    private float selectedPulseSpeed = 1f;

    [Header("Build")]
    [SerializeField]
    public bool isBuilt = false;
    [SerializeField]
    private float totalBuildEnergy = 10f;
    [SerializeField]
    private float buildingRate = 1f;
    [SerializeField]
    private Transform buildMaskTrans;
    private float currentBuildEnergy = 0;

    [Header("Transfer")]
    [SerializeField]
    private float transferRate = 1f;

    private Coroutine selectPulseCoroutine;

    // Could be a dictionary just for performance reasons, but not likely to be needed.
    private List<BaseStructure> connectedToStructures = new List<BaseStructure>();
    private SelectState selectState = SelectState.Deselected;
    private SpriteRenderer spriteRenderer, selectRenderer;

    private Image healthBar, energyBar;

    private static WireDrawer wireDrawer;


    private void Awake() {
        if (wireDrawer == null) {
            wireDrawer = GameObject.FindWithTag("GameController").GetComponent<WireDrawer>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        BuildSelectRenderer();

        healthBar = transform.Find("Structure UI/Bar Group/Health Bar").GetComponent<Image>();
        healthBar.fillAmount = currentHealth/totalHealth;
        energyBar = transform.Find("Structure UI/Bar Group/Energy Bar").GetComponent<Image>();
        energyBar.fillAmount = storage/capacity;

        if (isBuilt) {
            Destroy(transform.Find("Building").gameObject);
        }
    }

    protected virtual void Update() {
        if (!isBuilt) {
            Build();
        } else {
            Transfer();
        }
    }

    public void Transfer() {
        if (connectedToStructures.Count == 0) {
            return;
        }

        float amountPerTransfer = transferRate/connectedToStructures.Count;
        foreach (BaseStructure connectedStructure in connectedToStructures) {
            float amountToTransfer = amountPerTransfer * Time.deltaTime;
            if (storage < amountToTransfer) {
                amountToTransfer = storage;
            }
            if(connectedStructure.capacity - connectedStructure.storage < amountToTransfer) {
                amountToTransfer = connectedStructure.capacity - connectedStructure.storage;
            }
            RemoveFromStorage(amountToTransfer);
            connectedStructure.AddToStorage(amountToTransfer);

            if (IsStorageEmpty()) {
                return;
            }
        }
    }

    public void GainHealth(float amount) {
        currentHealth += amount;

        if (currentHealth > totalHealth) {
            currentHealth = totalHealth;
        }

        healthBar.fillAmount = currentHealth/totalHealth;
    }

    public void LoseHealth(float amount) {
        currentHealth -= amount;

        if (currentHealth <= 0) {
            Demolish();
        }

        healthBar.fillAmount = currentHealth/totalHealth;
    }

    public void Deselect() {
        if (IsDeselected()) {
            return;
        }

        if (selectPulseCoroutine != null) {
            StopCoroutine(selectPulseCoroutine);
        }

        selectRenderer.gameObject.SetActive(false);
        selectRenderer.transform.localScale = new Vector3(1, 1, 0);
        selectState = SelectState.Deselected;
    }

    public void Highlight() {
        if (IsHighlighted()) {
            return;
        }

        if (selectPulseCoroutine != null) {
            StopCoroutine(selectPulseCoroutine);
        }

        selectRenderer.gameObject.SetActive(true);
        selectRenderer.color = highlightedColor;
        selectRenderer.transform.localScale = new Vector3(1, 1, 0) * minScale;
        selectState = SelectState.Highlighted;
    }

    public void Select() {
        if (IsSelected()) {
            return;
        }

        selectRenderer.gameObject.SetActive(true);
        selectRenderer.color = selectedColor;
        selectPulseCoroutine = StartCoroutine(SelectPulse());
        selectState = SelectState.Selected;
    }

    public bool IsDeselected() {
        return selectState == SelectState.Deselected;
    }

    public bool IsHighlighted() {
        return selectState == SelectState.Highlighted;
    }

    public bool IsSelected() {
        return selectState == SelectState.Selected;
    }

    public void ConnectTo(BaseStructure other) {
        // This
        if (!connectedToStructures.Contains(other)) {
            connectedToStructures.Add(other);
        }

        // Other
        if (!other.connectedToStructures.Contains(this)) {
            other.connectedToStructures.Add(this);
        }

        wireDrawer.AddConnection(this, other);
    }    

    public void DisconnectFrom(BaseStructure other) {
        // This
        connectedToStructures.Remove(other);

        // Other
        other.connectedToStructures.Remove(this);

        wireDrawer.RemoveConnection(this, other);
    }
    
    
    public bool IsConnectedTo(BaseStructure other) {
        return connectedToStructures.Contains(other);
    }

    public float GetCapacity() {
        return capacity;
    }

    public float GetStorage() {
        return storage;
    }

    // Adds amount to storage. If capacity is reached, storage is capped
    // and the excess amount is returned.
    public float AddToStorage(float amount) {
        storage += amount;

        if (storage > capacity) {
            float excess = storage - capacity;
            storage = capacity;
            return excess;
        }

        energyBar.fillAmount = storage/capacity;

        return 0;
    }

    // Removes amount from storage. If there's not enough to remove, storage is
    // emptied and the remaining amount that wasn't "removed" is returned.
    public float RemoveFromStorage(float amount) {
        storage -= amount;

        if (storage < 0) {
            float remainder = storage * -1;
            storage = 0;
            return remainder;
        }

        energyBar.fillAmount = storage/capacity;

        return 0;
    }

    public bool IsStorageEmpty() {
        return storage <= 0;
    }

    public bool IsStorageFull() {
        return storage == capacity;
    }

    private void Build() {
        if (buildMaskTrans == null) {
            transform.Find("Building").gameObject.SetActive(true);
            SpriteRenderer buildingSprite = transform.Find("Building/Sprite").GetComponent<SpriteRenderer>();
            buildingSprite.sprite = spriteRenderer.sprite;
            buildingSprite.color = new Color(0.25f, 0.25f, 0.25f);
            buildMaskTrans = transform.Find("Building/Mask");
            currentHealth = 0.001f;
        }

        float energyToSpend = buildingRate * Time.deltaTime;
        if (totalBuildEnergy - currentBuildEnergy < energyToSpend) {
            energyToSpend = totalBuildEnergy - currentBuildEnergy;
        }
        if (storage < energyToSpend) {
            energyToSpend = storage;
        }
        RemoveFromStorage(energyToSpend);
        currentBuildEnergy += energyToSpend;

        GainHealth(energyToSpend/totalBuildEnergy * totalHealth);

        buildMaskTrans.localScale = new Vector3(1, 1-currentBuildEnergy/totalBuildEnergy, 1);
        buildMaskTrans.localPosition = new Vector3(0, 0.5f-buildMaskTrans.localScale.y/2, 1);

        if (currentBuildEnergy >= totalBuildEnergy) {
            isBuilt = true;
            Destroy(buildMaskTrans.parent.gameObject);
            return;
        }
    }

    private void Demolish() {
        Destroy(gameObject);
    }

    private void BuildSelectRenderer() {
        if (selectRenderer != null) {
            return;
        }

        GameObject selectObject = new GameObject("select");
        selectObject.transform.SetParent(transform);
        selectObject.transform.localPosition = new Vector3(0, 0, 0.5f);
        selectRenderer = selectObject.AddComponent<SpriteRenderer>();
        selectRenderer.sprite = spriteRenderer.sprite;
        selectRenderer.gameObject.SetActive(false);
    }

    private IEnumerator SelectPulse() {
        bool growing = true;
        float duration = 0;

        while (true) {
            duration += Time.deltaTime;
            if (growing) {
                selectRenderer.transform.localScale = Vector3.Lerp(Vector3.one * minScale, Vector3.one * maxScale, duration/selectedPulseSpeed);
            } else {
                selectRenderer.transform.localScale = Vector3.Lerp(Vector3.one * maxScale, Vector3.one * minScale, duration/selectedPulseSpeed);
            }
            if (duration > selectedPulseSpeed) {
                duration -= selectedPulseSpeed;
                growing = !growing;
            }
            yield return null;
        }
    }
    
    private enum SelectState {
        Deselected,
        Highlighted,
        Selected
    }

}
