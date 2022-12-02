using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionSettings : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    Resolution[] res;

    // Start is called before the first frame update
    void Start()
    {
        res = Screen.resolutions;
        List<string> strRes = new List<string> { };
        for (int i = 0; i < res.Length; i++)
        {
            strRes.Add(res[i].width.ToString() + "x" + res[i].height.ToString());
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(strRes);
        Screen.SetResolution(res[res.Length - 1].width, res[res.Length - 1].height, true);
    }

    public void SetRes()
    {
        Screen.SetResolution(res[dropdown.value].width, res[dropdown.value].height, true);
    }
}
