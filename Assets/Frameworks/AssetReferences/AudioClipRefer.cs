using System;
using UnityEngine;

namespace GoPlay
{
    [Serializable]
    public class AudioClipRefer : AssetRefer
    {
#if UNITY_EDITOR
        private AudioClip _audioClip;
#endif
        
        public static implicit operator AudioClip(AudioClipRefer data)
        {
            if (data == null) return null;
            
#if UNITY_EDITOR
            if (data._audioClip) return data._audioClip;
#endif

            var audioClip = data.Load<AudioClip>();
#if UNITY_EDITOR
            data._audioClip = audioClip;
#endif
            return audioClip;
        }
    }
}