using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

[System.Serializable]
public class SoundData {
    public string sound_id;
    public AudioClip sound_clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource audioSource;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    [Header("Sound Data")]
    [SerializeField] private List<SoundData> soundDataList = new List<SoundData>();

    public void PlaySound(string sound_id) {
        AudioClip soundClip = soundDataList.Find(data => data.sound_id == sound_id).sound_clip;
        if(soundClip != null) {
            Debug.Log("Update 사운드 재생");
            audioSource.PlayOneShot(soundClip);
        }
    }
}
