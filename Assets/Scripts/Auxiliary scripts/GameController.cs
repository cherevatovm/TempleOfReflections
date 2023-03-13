using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Awake() => SoundManager.InitSoundTimerDict();
}
