using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource aux;

    public void PlaySwipeSound()
    {
        aux.Play();
    }
}