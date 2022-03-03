using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLookForPlayerStateData", menuName = "Data/State Data/Look For Player State Data")]
public class D_LookForPlayerStateData : ScriptableObject
{
    public int amountOfTurns = 2;
    public float timeBetweenTurns = 0.7f;
}
