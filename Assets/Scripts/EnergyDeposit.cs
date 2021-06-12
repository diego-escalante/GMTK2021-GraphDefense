using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnergyDeposit : MonoBehaviour, IStorage {

    [SerializeField]
    private float capacity = 100f;
    [SerializeField]
    private float storage = 100f;
    [SerializeField]
    private Color fullColor, emptyColor;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.Lerp(emptyColor, fullColor, storage/capacity);
    }

    public void UpdateStorage(float delta) {
        storage += delta;
        if (storage < 0) {
            storage = 0;
            Debug.LogWarning("Tried to remove more than available from Storage!");
        } else if (storage > capacity) {
            storage = capacity;
            Debug.LogWarning("Tried to add more than Storage's capacity allows for!");
        }
        spriteRenderer.color = Color.Lerp(emptyColor, fullColor, storage/capacity);
    }

    public float GetStorage() {
        return storage;
    }

    public float GetCapacity() {
        return capacity;
    }
}
