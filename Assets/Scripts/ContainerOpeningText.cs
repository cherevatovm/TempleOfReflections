using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerOpeningText : MonoBehaviour
{
    public GameObject OpenText;

    public GameObject DontOpenText;

    public static ContainerOpeningText instance;

    void Start()
    {
        instance = this;
        OpenText.gameObject.SetActive(false);
        DontOpenText.gameObject.SetActive(false);
    }

    void Update()
    {

    }
}
