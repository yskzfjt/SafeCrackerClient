using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundPlayer {

    GameObject soundPlayerObj;
    AudioSource audioSource;
    Dictionary<string, AudioClipInfo> audioClips = new Dictionary<string, AudioClipInfo>();

    // AudioClip information
    class AudioClipInfo {
        public string resourceName;
        public string name;
        public AudioClip clip;

        public AudioClipInfo( string resourceName, string name ) {
            this.resourceName = resourceName;
            this.name = name;
        }
    }

    public SoundPlayer() {
        audioClips.Add( "clear", new AudioClipInfo( "cancel2", "clear" ) );
        audioClips.Add( "push", new AudioClipInfo( "cursor8", "push" ) );
        audioClips.Add( "hit", new AudioClipInfo( "decision9", "hit" ) );
        audioClips.Add( "finale", new AudioClipInfo( "decision6", "finale" ) );
        audioClips.Add( "fail", new AudioClipInfo( "decision7", "fail" ) );
        audioClips.Add( "bgm", new AudioClipInfo( "kiken-na-otoko", "bgm" ) );
    }

    public bool Play( string seName, float vol = 1.0f ) {
        if ( audioClips.ContainsKey( seName ) == false )  return false; // not register

        AudioClipInfo info = audioClips[ seName ];

        // Load
        if ( info.clip == null )
            info.clip = (AudioClip)Resources.Load( info.resourceName );

        if ( soundPlayerObj == null ) {
            soundPlayerObj = new GameObject( "SoundPlayer" ); 
            audioSource = soundPlayerObj.AddComponent<AudioSource>();
        }

        // Play SE
        audioSource.PlayOneShot( info.clip, vol );

        return true;
    }
}