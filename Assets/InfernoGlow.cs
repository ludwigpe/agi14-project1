using UnityEngine;
using System.Collections;

public class InfernoGlow : MonoBehaviour {

    public Color glowColor1;
    public Color glowColor2;
    private float colorScale = 0.0F;
    public float colorSign = 1.0F;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        colorScale += Time.deltaTime * colorSign;
        if (colorScale >= 1 || colorScale <= 0)
        {
            colorSign *= -1;
        }
        this.gameObject.renderer.material.color = Color.Lerp(glowColor1, glowColor2, colorScale);
	}
}
