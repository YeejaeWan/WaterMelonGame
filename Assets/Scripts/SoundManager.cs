using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX
{
    LevelUp,
    Next = 3, 
    Attach,
    Button,
    GameOver,
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;

    public static SoundManager Instance { get { return instance; } }

    [SerializeField] private AudioSource[] sfxPlayer;
    [SerializeField] private AudioClip[] sfxClip;

    private int sfxPlayerIndex;
    private const int Level_Up_Count = 3;
 

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void PlaySFX(SFX type)
    {
        switch(type)
        {
            case SFX.LevelUp:
                sfxPlayer[sfxPlayerIndex].clip = sfxClip[Random.Range(0, Level_Up_Count)];
                break;
            case SFX.Next:
                sfxPlayer[sfxPlayerIndex].clip = sfxClip[(int)SFX.Next];
                break;
            case SFX.Attach:
                sfxPlayer[sfxPlayerIndex].clip = sfxClip[(int)SFX.Attach];
                break;
            case SFX.Button:
                sfxPlayer[sfxPlayerIndex].clip = sfxClip[(int)SFX.Button];
                break;
            case SFX.GameOver:
                sfxPlayer[sfxPlayerIndex].clip = sfxClip[(int)SFX.GameOver];
                break;

        }
        sfxPlayer[sfxPlayerIndex].Play();
        // 0,1,2만 설정되도록
        sfxPlayerIndex = (sfxPlayerIndex+1) % sfxPlayer.Length;
    }
}
