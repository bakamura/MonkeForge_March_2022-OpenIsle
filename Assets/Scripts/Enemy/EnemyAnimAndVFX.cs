using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimAndVFX : MonoBehaviour {
    public Animator _animator { get; private set; }
    private AudioSource _audio;
    [SerializeField] private EnemyAudio[] _audioDatas;
    private Dictionary<int, EnemyAudio> _audioDictionary = new Dictionary<int, EnemyAudio>();
    private float[] _soundCurrentCooldowns = new float[4];
    private EnemyAudio.soundTypes _currentSoundPlaying;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        //foreach (EnemyAudio data in _audioDatas) _audioDictionary.Add((int)data.soundType, data);
    }

    private void Update() {
        for (int i = 0; i < _soundCurrentCooldowns.Length; i++) _soundCurrentCooldowns[i] -= Time.deltaTime;        
    }

    public void AttackAnim(float atkSpeed) {
        _animator.SetFloat("ATKSPEED", Mathf.Clamp(atkSpeed, 1, float.MaxValue));
        bool attacking = atkSpeed >= 1;
        _animator.SetBool("ATTACK", attacking);        
        //PlaySoundEffect(EnemyAudio.soundTypes.Attack, _animator.GetCurrentAnimatorStateInfo(0).length / 2f);
    }

    public void DeathAnim() {
        _animator.SetTrigger("DEATH");
    }

    public void StunAnim() {
        _animator.SetTrigger("STUN");
        //PlaySoundEffect(EnemyAudio.soundTypes.Stun, 0f);
    }

    public void EndStunAnim() {
        _animator.SetTrigger("ENDSTUN");
    }

    public void MovmentAnim(float isMoving) {
        _animator.SetFloat("TARGET", isMoving);
        //if (isMoving > 0) PlaySoundEffect(EnemyAudio.soundTypes.Walk, 0f);
    }

    public void PlaySoundEffect(EnemyAudio.soundTypes sound, float soundDelay) {
        if (_audio.isPlaying && !_audioDictionary[(int)_currentSoundPlaying]._audioData.canLoop) return;
        if (_soundCurrentCooldowns[(int)sound] <= 0) {
            //Debug.Log(sound.ToString());
            AudioSetup(sound);
            _currentSoundPlaying = sound;
            //_audio.PlayDelayed(soundDelay);
        }
    }
    private void AudioSetup(EnemyAudio.soundTypes sound) {
        //_audio.clip = _audioDictionary[(int)sound]._audioData.audioClips[Random.Range(0, _audioDictionary[(int)sound]._audioData.audioClips.Length)];
        //_audio.outputAudioMixerGroup = _audioDictionary[(int)sound]._audioData.audioMixerGroup;
        //_audio.volume = _audioDictionary[(int)sound]._audioData.volume;
        //_audio.loop = _audioDictionary[(int)sound]._audioData.canLoop;
        _soundCurrentCooldowns[(int)sound] = _audioDictionary[(int)sound]._audioData.soundInterval;
        //if (_audioDictionary[(int)sound]._audioData.randomPitch) _audio.pitch = Random.Range(-_audioDictionary[(int)sound]._audioData.randomPicthRange, _audioDictionary[(int)sound]._audioData.randomPicthRange);
        //else _audio.pitch = _audioDictionary[(int)sound]._audioData.pitch;
    }


    [System.Serializable]
    public class EnemyAudio {
        public enum soundTypes {
            Attack,
            Idle,
            Walk,
            Stun,
            Die
        };
        public soundTypes soundType;
        public AudioData _audioData;
    }
}
