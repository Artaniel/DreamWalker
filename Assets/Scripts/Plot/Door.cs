using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animator;

    public void OpenDoor()
    {
        animator.SetTrigger("Open");
    }
}
