using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [Range(1,10)] [SerializeField]
    private float smoothFactor;
    public Vector3  offset, minValues, maxValues;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioManager>().Play("Gameplay Theme");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        FollowPlayer();
    }
    public void FollowPlayer()
    {
        
        Vector3 targetPos = target.position + offset;
        Vector3 boundary = new Vector3(
            Mathf.Clamp(targetPos.x,minValues.x,maxValues.x),
            Mathf.Clamp(targetPos.y, minValues.y, maxValues.y),
            Mathf.Clamp(targetPos.z, minValues.z, maxValues.z));

        Vector3 smoothedPos = Vector3.Lerp(transform.position, boundary, smoothFactor * Time.fixedDeltaTime);
        transform.position = smoothedPos;
    }
}
