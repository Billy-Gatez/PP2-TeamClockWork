using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject model;
    [Range(1,10)] public int shootDmg;
    [Range(5, 1000)] public int shootDist;
    [Range(.1f, 2)] public float shootRate;
    [Range(5, 50)] public int ammoMax;
    [HideInInspector] public int ammoCurr;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0,1)] public float shootVol;
}
