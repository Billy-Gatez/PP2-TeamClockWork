using System;
using System.Runtime;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject model;
    [Range(0,10)] public int shootDmg;
    [Range(0, 1000)] public int shootDist;
    [Range(0, 2)] public float shootRate;
    [Range(0, 50)] public int ammoMax;
    [HideInInspector] public int ammoCurr;
    [Range(0, 2)] public float weaponSpeedMod;
    [Range(1, 100)] public int projectileAmount;



    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0,1)] public float shootVol;
}
