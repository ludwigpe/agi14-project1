using UnityEngine;
using System.Collections;

public class RandomColor : MonoBehaviour {
    private float time;
    
    // Update is called once per frame
	void Update () {
        transform.Rotate(Time.smoothDeltaTime * 200, 0, 0);
        transform.localScale += Mathf.Sin(Time.time) * new Vector3(50* Time.smoothDeltaTime, 50*Time.smoothDeltaTime, 50*Time.smoothDeltaTime);

        /*
        ParticleSystem m_currentParticleEffect = (ParticleSystem)GetComponent("ParticleSystem");
        ParticleSystem.Particle[] ParticleList = new ParticleSystem.Particle[m_currentParticleEffect.particleCount];
        m_currentParticleEffect.GetParticles(ParticleList);
        
        Color[] colors = new Color[3];
        colors[0] = Color.cyan;
        colors[1] = Color.white;
        colors[2] = Color.blue;

        Color prevColor = Color.white;
        for (int i = 0; i < ParticleList.Length; ++i)
        {
            Color currentColor = ParticleList[i].color;
            Color32 newColor = Color32.Lerp(colors[i % 3], colors[(i + 1) % 3], Mathf.Sin(i + Time.time));
            newColor.a = (byte)((255 * ParticleList[i].lifetime / ParticleList[i].startLifetime));
            ParticleList[i].color = newColor;
            prevColor = currentColor;
        }

        m_currentParticleEffect.SetParticles(ParticleList, m_currentParticleEffect.particleCount);
        */
	}

    void Start()
    {
    }
}
