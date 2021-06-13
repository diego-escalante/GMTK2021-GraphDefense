using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnergyDeposit : MonoBehaviour {

    [SerializeField]
    private float energy = 100f;
    [SerializeField]
    private Color fullColor, emptyColor;
    private float startingEnergy;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        startingEnergy = energy;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.Lerp(emptyColor, fullColor, energy/startingEnergy);
    }

    public float Remove(float amount) {
        float result = 0;
        energy -= amount;
        
        if (energy < 0) {
            result = energy * -1;
            energy = 0;
        }
    
        spriteRenderer.color = Color.Lerp(emptyColor, fullColor, energy/startingEnergy);
        return result;
    }

    public float GetEnergy() {
        return energy;
    }

    public bool IsDepleted() {
        return energy <= 0;
    }
}
