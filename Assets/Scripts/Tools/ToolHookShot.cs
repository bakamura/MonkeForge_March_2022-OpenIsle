using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHookShot : MonoBehaviour {

    [SerializeField] private float _hookDistance;
    [SerializeField] private float _hookSpeed;
    [SerializeField] private LayerMask[] _checkLayers = new LayerMask[3]; // 0 = hookPoint, 1 = enemies, 2 = movable objects
    [SerializeField] private AudioClip[] _hitAudios = new AudioClip[4]; // 0 = hookShot, 1 = hit Something Pullable, 2 = hit something unpullable, 3 = pulling
    [SerializeField] private Transform _hookTransform;
    [SerializeField] private float _hookShotSizeIncrease;
    [System.NonSerialized] public bool isHookActive = false;
    private bool _canStartPulling = false;
    private RaycastHit _hitinfo;
    private AudioSource _audio;
    private float _targetDistance;
    private float _initialTargetDistance;
    private float _objectSizeDifference;
    private bool _willHapenMovment;
    //[SerializeField] private float _distanceToStop = 1;

    private void Awake() {
        _audio = GetComponent<AudioSource>();
    }

    //private void Update() {
    //    // if (PlayerInputs.hookKeyPressed > 0 && PlayerData.Instance.hasHook && !isHookActive) HookStart();
    //    // if (PlayerMovement.Instance.movementLock && isHookActive && (PlayerInputs.jumpKeyPressed > 0 || PlayerInputs.hookKeyPressed > 0 || PlayerInputs.dashKeyPressed > 0)) EndHookMovment();
    //}

    private void FixedUpdate() {
        if (isHookActive) {
            HookMeshLine();
            if (_canStartPulling) {
                HookPosibilities();
            }
        }
    }

    public void SendHitDetection() {
        Physics.Raycast(PlayerMovement.Instance.transform.position, Camera.main.transform.forward, out _hitinfo, _hookDistance);
        _hookTransform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        UpdateTargetDistance();
    }
    private void UpdateTargetDistance() {
        if (_hitinfo.collider != null) {
            if (_hitinfo.rigidbody != null) {
                if (_objectSizeDifference == 0) _objectSizeDifference = Vector3.Distance(_hitinfo.point, _hitinfo.transform.position);
                _targetDistance = Vector3.Distance(_hitinfo.transform.position, PlayerMovement.Instance.transform.position) - _objectSizeDifference;
            }
            else _targetDistance = Vector3.Distance(_hitinfo.point, PlayerMovement.Instance.transform.position);
        }
        else _targetDistance = _hookDistance;
        if (_initialTargetDistance == 0) {
            _initialTargetDistance = _targetDistance;
            Mathf.Clamp(_initialTargetDistance, 1, _hookDistance);
        }
    }

    public float Duration() {
        float result;
        if (_targetDistance != _hookDistance) {
            if (_hitinfo.rigidbody != null)
                result = _targetDistance / ObjectvelocityCalc().magnitude;//ObjectvelocityCalc().magnitude * Time.fixedDeltaTime * _initialTargetDistance;
            else result = _targetDistance / PlayerVelocityCalc().magnitude;
        }
        else result = _targetDistance / _hookShotSizeIncrease;
        return  (_targetDistance / _hookShotSizeIncrease + result) * Time.fixedDeltaTime;
    }

    public void StartHook() {
        isHookActive = true;
        //_audio.clip = _hitAudios[0];
        //_audio.Play();
    }

    private void HookMeshLine() {
        UpdateTargetDistance();
        if (_hookTransform.localScale.z < _targetDistance && !_canStartPulling) {
            _hookTransform.localScale += new Vector3(0, 0, _hookShotSizeIncrease);
        }
        else {
            if (!_canStartPulling) {
                //_audio.clip = _hitAudios[2];
                //_audio.Play();
            }
            _canStartPulling = true;
            _hookTransform.localScale = _targetDistance != _hookDistance && _willHapenMovment ? new Vector3(1, 1, Mathf.Clamp(_targetDistance, 1, _hookDistance)) : new Vector3(1, 1, Mathf.Clamp(_hookTransform.localScale.z - _hookShotSizeIncrease * 2, 1, _hookDistance));
        }
    }

    private void HookPosibilities() {
        if (_hitinfo.collider != null) {
            if (Contains(_checkLayers[0], _hitinfo.collider.gameObject.layer)) { // hit a hook point
                //Debug.Log("hook point");
                MovePlayerToPoint();
                //_currentRuningMethodName = nameof(MovePlayerToPoint);
                //InvokeRepeating(_currentRuningMethodName, 0f, Time.fixedDeltaTime);
            }
            else if (Contains(_checkLayers[1], _hitinfo.collider.gameObject.layer)) { // hit an enemy                                                                                  
                //Debug.Log("enemy");
                //EndHookMovment();
            }
            else if (Contains(_checkLayers[2], _hitinfo.collider.gameObject.layer)) { // hit a movable object
                MovePointToPlayer();
            }
        }
        EndHookMovment();
    }

    private bool Contains(LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }


    private void MovePlayerToPoint() {
        _willHapenMovment = true;
        PlayerData.rb.velocity = PlayerVelocityCalc();
        // Weird behaviour: when playmode enterer, casting hook may "lock" the player into the ground
    }

    private void MovePointToPlayer() {
        _willHapenMovment = true;
        _hitinfo.rigidbody.velocity = ObjectvelocityCalc();
    }

    private Vector3 PlayerVelocityCalc() {
        return _hookSpeed * _initialTargetDistance * (_hitinfo.point - PlayerMovement.Instance.transform.position).normalized;
    }

    private Vector3 ObjectvelocityCalc() {
        return _hookSpeed * _initialTargetDistance * (PlayerMovement.Instance.transform.position - _hitinfo.transform.position).normalized / _hitinfo.rigidbody.mass * _initialTargetDistance;
    }

    public void EndHookMovment() {
        if (_hookTransform.localScale.z <= 1) {
            _canStartPulling = false;
            isHookActive = false;
            _willHapenMovment = false;
            _initialTargetDistance = 0;
            _objectSizeDifference = 0;
            _hookTransform.localRotation = Quaternion.identity;
            if (_hitinfo.rigidbody != null) _hitinfo.rigidbody.velocity = Vector3.zero;
            // PlayerMovement.Instance.movementLock = false;
            //PlayerData.rb.useGravity = true;
        }
    }
}