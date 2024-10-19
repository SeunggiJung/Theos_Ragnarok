using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    GameObject swordman;
    Sword_Man sm;
    // Start is called before the first frame update
    void Start()
    {
        swordman = GameObject.Find("sword_man");
        sm = swordman.GetComponent<Sword_Man>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LeftBtnDown()
    {
        sm.MoveLeft();
    }
    public void RightBtnDown()
    {
        sm.MoveRight();
    }
}
