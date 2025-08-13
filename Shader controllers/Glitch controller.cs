using UnityEngine;

public class Glitchcontroller : MonoBehaviour
{
    public Material material;
    public float noiseAmount;
    public float glitchStrenght;

    private void Update()
    {
        material.SetFloat("_NoiseAmount", noiseAmount);
        material.SetFloat("_GlitchStrenght", glitchStrenght);
    }
}
