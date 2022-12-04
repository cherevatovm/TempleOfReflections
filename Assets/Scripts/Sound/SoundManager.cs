using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SoundManager
{
    public enum Sound
    {
        WeaponSwing,
        WeaponSwingWithHit,
        EnterCombat,
        Step,
        StepsOnDryTerrain,
        StepsOnGrass,
        OpenContainer,
        TakingDamage,
        PsiSkill,
        ElectraSkill,
        FiraSkill,
        SomethingsGoingOn,
        Mystery,
        MentalBattle
    }

    static Dictionary<Sound, float> soundTimerDictionary;

    public static void InitSoundTimerDict()
    {
        soundTimerDictionary = new()
        {
            [Sound.Step] = 0
        };
    }

    public static void PlaySound(Sound sound)
    {
        if (IsPossibleToPlaySound(sound))
        {
            GameObject soundObject = ObjectPool.instance.GetPooledObject();
            if (soundObject != null)
            {
                AudioSource audioSource = soundObject.GetComponent<AudioSource>();
                soundObject.SetActive(true);
                if ((int)sound < 12)
                    audioSource.PlayOneShot(GetAudioClip(sound));
                else
                {
                    audioSource.loop = true;
                    audioSource.clip = GetAudioClip(sound);
                    audioSource.Play();
                }
            }
        }
    }

    public static void StopLoopedSound()
    {
        GameObject obj = ObjectPool.instance.GetLoopedPooledObject();
        obj.GetComponent<AudioSource>().clip = null;
        obj.SetActive(false);
    }

    static bool IsPossibleToPlaySound(Sound sound)
    {
        switch (sound)
        {
            case Sound.Step:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float stepMaxTimer = 0.3f;
                    if (lastTimePlayed + stepMaxTimer < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return true;
            default:
                return true;
        }
    }

    static AudioClip GetAudioClip(Sound sound)
    {
        foreach (var soundAudioClip in AssetManager.Instance.soundAudioClips)
            if (soundAudioClip.sound.Equals(sound))
                return soundAudioClip.audioClip;
        Debug.LogError("The following sound " + sound + "not found.");
        return null;
    }
}
