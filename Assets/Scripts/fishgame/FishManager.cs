using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public List<FishData> FishList=new List<FishData>();
    public FishMovement FishMovement;
    public void Start()
    {
        FishMovement.swimSpeed = FishList[3].SwimmSpeed;
        FishMovement.turnSpeed = FishList[3].TurnSpeed;
        FishMovement.wobbleSpeed = FishList[3].WobbleSpeed;
        FishMovement.wobbleAmount = FishList[3].WobbleAmount;
        FishMovement.type = FishList[3].FishType;
    }
}
