using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
        {
        anim.SetTrigger("Spawn");
        }

    private void OnTriggerEnter(Collider other)
        {
        if(other.tag == "Player")
            {
            GameManager.instance.GetCoin();
            anim.SetTrigger("Collected");
         
            }
       
        }
    }
