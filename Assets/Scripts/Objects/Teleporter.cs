using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    [SerializeField] private Vector3 _pointToTeleport;
    [SerializeField, Range(0, 360)] private int _lookAngle;
    [SerializeField, Tooltip("the totakl amount of time that the fade out/in effect will take")] private float _transitionDuration;
    private bool _isTeleporting;
    private Transform _objToTeleport;
    private enum TransitionState {
        FadeOut,
        FadeIn
    };
    private TransitionState _currentTransitionState = TransitionState.FadeIn;

    [Header("LockedDoor")]

    [SerializeField] private PuzzleLightReceptor lightReceptor;
    private bool _isLocked = false;

    private void Start() {
        _isLocked = lightReceptor != null;
        if(_isLocked) lightReceptor.onActivate += OpenDoor;
    }

    private void Update() {
        //start teleport sequence
        if (_isTeleporting) {
            if (_currentTransitionState == TransitionState.FadeIn) {//start to make the screen black
                UserInterface.FadeCanvas(UserInterface.Instance.transitionImage, 1, _transitionDuration / 2f);
                if (UserInterface.Instance.transitionImage.alpha >= 1) {//when the screen is completely black, teleports the object to the position
                    _objToTeleport.SetPositionAndRotation(transform.position + _pointToTeleport, Quaternion.Euler(_objToTeleport.rotation.x, _lookAngle, _objToTeleport.rotation.z));
                    _currentTransitionState = TransitionState.FadeOut;
                }
            }
            else if (_currentTransitionState == TransitionState.FadeOut) {//start to take out the black from the screen
                UserInterface.FadeCanvas(UserInterface.Instance.transitionImage, 0, _transitionDuration / 2f);
                if (UserInterface.Instance.transitionImage.alpha <= 0) {//when the transition ends will make the player move again
                    _isTeleporting = false;
                    _currentTransitionState = TransitionState.FadeIn;
                    _objToTeleport.GetComponent<PlayerMovement>().movementLock = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !_isLocked) {
            _isTeleporting = true;
            _objToTeleport = other.transform;
            other.GetComponent<PlayerMovement>().movementLock = true;
            //other.transform.SetPositionAndRotation(transform.position + _pointToTeleport, Quaternion.Euler(other.transform.rotation.x, _lookAngle, other.transform.rotation.z));        
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + _pointToTeleport, .3f);
        Gizmos.color = Color.blue;
        Vector3 pos = transform.position + _pointToTeleport;
        Vector3 finalPoint = new Vector3(Mathf.Cos(_lookAngle * Mathf.PI / 180), 0, Mathf.Sin(_lookAngle * Mathf.PI / 180)) + pos;
        //Debug.Log(new Vector3(Mathf.Round(Mathf.Sin(_lookAngle) + pos.x), pos.y, Mathf.Round(Mathf.Cos(_lookAngle) + pos.z)));
        Gizmos.DrawLine(pos, finalPoint);//new Vector3(Mathf.Sin(_lookAngle), pos.y, Mathf.Cos(_lookAngle))*2
    }

    private void OpenDoor() {
        _isLocked = false;
    }
}
