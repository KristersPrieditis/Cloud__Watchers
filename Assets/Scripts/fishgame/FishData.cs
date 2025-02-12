using System;
using UnityEngine;
[Serializable] 
public class FishData
{
    public FishType FishType;
    public float SwimmSpeed;
    public float TurnSpeed;
    public float WobbleAmount;
    public float WobbleSpeed;
}

public enum FishType 
{
    Shark,
    GoldFish,
    Dolphine,
    Whale 
}