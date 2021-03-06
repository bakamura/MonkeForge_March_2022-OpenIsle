using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class PlayerTools : MonoBehaviour {

    public static PlayerTools instance { get; private set; }

    [Header("Info")]

    [SerializeField] private float _actionInternalCoolDown;
    private float _currentActionCoolDown;
    [SerializeField] private MeshFilter _toolMeshFilter;
    [SerializeField] private MeshRenderer _toolMeshRenderer;

    [Header("Sword")]

    [SerializeField] private Mesh _swordMesh;
    [SerializeField] private Material _swordMaterial;
    [SerializeField] private float _swordActionDuration;

    public float swordDamage;
    [SerializeField] private Collider _swordCollider;
    [HideInInspector] public List<Collider> swordCollisions = new List<Collider>();

    [Header("Hook")]

    [SerializeField] private Mesh _hookMesh;
    [SerializeField] private Material _hookMaterial;
    [SerializeField] private CinemachineFreeLook cinemachineCam;
    [HideInInspector] public bool isAiming = false;
    // Vini
    [SerializeField] private ToolHookShot _hookScript;
    public Transform _hookCameraPoint;
    [SerializeField] private CanvasGroup _hookAimUI;
    [SerializeField] private MeshRenderer[] _hookChain = new MeshRenderer[2];
    // Naka (alternate script) UNUSED
    //[SerializeField] private AlternateToolHookShot _hookAlternateScript;
    //public float hookSpeed;
    //public float playerHookSpeed;
    //public float objectHookSpeed;
    //public float maxHookDistance; // In seconds, when going
    //public float hookReturnDuration = 1;
    //public float hookCorrectionDistance;

    [Header("Amulet")]

    [SerializeField] private Mesh _amuletMesh;
    [SerializeField] private Material _amuletMaterial;
    [SerializeField] private float _amuletActionDuration;
    [Tooltip("In % amount from max")]
    [SerializeField] private float _amuletHealthCost;

    public static float amuletDistance = 20;
    public static UnityAction onActivateAmulet;
    private PlayerAnim _animScript;

    private void Awake() {
        if (instance == null) {
            instance = this;
            _animScript = GetComponent<PlayerAnim>();
        }
        else if (instance != this) Destroy(gameObject);
    }


    // Checks each tool's input and the action cooldown (not exposed to the player)
    private void Update() {
        // Sword
        if (PlayerMovement.Instance.isGrounded && PlayerData.Instance.hasSword && _currentActionCoolDown <= 0 && PlayerInputs.swordKeyPressed > 0) {
            PlayerInputs.swordKeyPressed = 0;
            ChangeMesh(_swordMesh, _swordMaterial, _swordActionDuration);
            isAiming = false;
            ChangeCameraFollow(transform);
            _animScript.Sword();
            Invoke(nameof(SwordStart), 0.1f);
            Invoke(nameof(SwordEnd), 1f);
        }
        // Enter aiming (Hook)
        else if (PlayerData.Instance.hasHook && _currentActionCoolDown <= 0 && PlayerInputs.hookKeyPressed > 0 /*&& !AlternateToolHookShot.Instance.active*/) {
            PlayerInputs.hookKeyPressed = 0;
            isAiming = true;
            //ChangeCameraFollow(_toolMeshFilter.transform);
            ChangeCameraFollow(_hookCameraPoint);
            ChangeMesh(_hookMesh, _hookMaterial, 0);
            _animScript.Hook(true);
        }
        // Amulet
        else if (PlayerData.Instance.hasAmulet && _currentActionCoolDown <= 0 && PlayerInputs.amuletKeyPressed > 0) {
            PlayerInputs.amuletKeyPressed = 0;
            ChangeMesh(_amuletMesh, _amuletMaterial, _amuletActionDuration);
            isAiming = false;
            ChangeCameraFollow(transform);

            PlayerData.Instance.TakeDamage(_amuletHealthCost * PlayerData.Instance.maxHealth);
            if (onActivateAmulet != null) onActivateAmulet.Invoke(); //
            _animScript.Amulet();
        }
        // Shoots Hook
        if (isAiming) {
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            if (PlayerInputs.hookKeyReleased > 0) {
                PlayerInputs.hookKeyReleased = 0;

                isAiming = false;
                ChangeCameraFollow(transform);

                if (!_hookScript.isHookActive) {
                    _hookScript.SendHitDetection();
                    ChangeMesh(_hookMesh, _hookMaterial, 0);
                    _hookScript.StartHook();
                }

                //_hookAlternateScript.InitiateHook();
            }
        }
        else PlayerInputs.hookKeyReleased = 0;

        _currentActionCoolDown -= Time.deltaTime;

        if (_hookScript.isHookActive && (PlayerInputs.jumpKeyPressed > 0 || PlayerInputs.hookKeyPressed > 0 || PlayerInputs.dashKeyPressed > 0)) _hookScript.CancelHook();
    }

    // Changes the mesh in the tool object, to the current tool being used (tool stays there until another is used)
    private void ChangeMesh(Mesh mesh, Material material, float actionDuration) {
        if (actionDuration > 0) {
            //Debug.Log("Movement Lock");
            if (mesh != _swordMesh) PlayerMovement.Instance.movementLock = true;
            _currentActionCoolDown = actionDuration + _actionInternalCoolDown;
            Invoke(nameof(UnlockMovement), actionDuration);
        }
        //enables and disable the chains from the hook depending on the current mesh
        if (mesh == _hookMesh) foreach (MeshRenderer chain in _hookChain) chain.enabled = true;
        else foreach (MeshRenderer chain in _hookChain) chain.enabled = false;
        if (mesh != _swordMesh) PlayerData.rb.velocity = new Vector3(0, PlayerData.rb.velocity.y, 0);
        _toolMeshFilter.mesh = mesh;
        _toolMeshRenderer.material = material;
    }

    // Re-enables movement after performing action
    private void UnlockMovement() {
        // May have issues with hook function
        //Debug.Log("Movement Unlock");
        PlayerMovement.Instance.movementLock = false;
    }

    // Starts sword action
    private void SwordStart() {
        _swordCollider.enabled = true;
        swordCollisions.Clear();
    }

    // Ends sword action
    private void SwordEnd() {
        _swordCollider.enabled = false;
    }

    // Ends hook action
    public void EndHook() {
        UnlockMovement();
        _currentActionCoolDown = _actionInternalCoolDown;
    }

    // Sets the object the camera is following (for the hook aim)
    private void ChangeCameraFollow(Transform followObject) {
        _hookAimUI.alpha = (followObject == _hookCameraPoint) ? 1f : 0f;
        cinemachineCam.Follow = followObject;
        cinemachineCam.LookAt = followObject;
    }
}
