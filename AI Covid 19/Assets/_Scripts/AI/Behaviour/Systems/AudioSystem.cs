using UnityEngine;

namespace Covid19.AI.Behaviour.Systems
{
    public class AudioSystem
    {
        
        private AudioClip _lastAudioClip;
        private readonly AudioClip[] _coughSoundArray;
        private readonly AudioSource _audioSource;
        
        public AudioSystem(AudioClip[] coughSoundArray,AudioSource audioSource)
        {
            _coughSoundArray = coughSoundArray;
            _audioSource = audioSource;
        }
        
        private AudioClip RandomCoughClip()
        {
            AudioClip clip = _coughSoundArray[Random.Range(0, _coughSoundArray.Length - 1)];
            var tries = 0;
            while (clip == _lastAudioClip && tries <= 3)
            {
                clip = _coughSoundArray[Random.Range(0, _coughSoundArray.Length - 1)];
                tries++;
            }

            _lastAudioClip = clip;
            return clip;
        }

        public float PlayRandomCough()
        {
            AudioClip clip = RandomCoughClip();
            _audioSource.clip = clip;
            _audioSource.Play();
            return clip.length;
        }
    }
}