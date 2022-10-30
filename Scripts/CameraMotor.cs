using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Transform penguin;
    [SerializeField] Vector3 offset = new Vector3(0, 4f, -10f );

    public bool IsMoving { set; get; }
    public Vector3 rotation = new Vector3(35, 0, 0); 
    // Start is called before the first frame update
    void Start()
    {
        penguin = GameObject.Find("Penguin").transform;
       
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!IsMoving) return;
        Vector3 desiredPos = penguin.position + offset;
        desiredPos.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), 0.1f);
    }
}
