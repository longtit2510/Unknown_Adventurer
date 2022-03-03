using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    private Transform cam;
    private Vector3 prevCamPos;
    [SerializeField]
    private float parallaxEffectMultiplier;
    private void Awake()
    {
        cam = Camera.main.transform;
        prevCamPos = cam.position;
    }
    private void Start()
    {
        
    }
    private void LateUpdate()
    {
        Vector3 deltaMovement = cam.position - prevCamPos;
        transform.position = new Vector3(transform.position.x + deltaMovement.x * parallaxEffectMultiplier,transform.position.y,transform.position.z);
        prevCamPos = cam.position;
    }
}
