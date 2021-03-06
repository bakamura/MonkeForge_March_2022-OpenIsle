using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmuletObject : MonoBehaviour {

    [SerializeField] private Material _materialToChange;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private bool _updateCollider = true;
    [SerializeField] private bool _updateMeshVisibility = true;
    private Material _standarMaterial;
    private MeshRenderer _meshRender;
    private Collider _collider;

    private void Awake() {
        _meshRender = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
        _standarMaterial = _meshRender.material;
    }

    // Assign it's method to the amulet delegate
    private void Start() {
        PlayerTools.onActivateAmulet += Changedimension;
    }

    // (De)Activates a wall collision and visibility when called
    public void Changedimension() {
        if (Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) < PlayerTools.amuletDistance) {
            if(_updateCollider) _collider.enabled = !_collider.enabled;
            if (_updateMeshVisibility) _meshRender.enabled = !_meshRender.enabled;
            //if (_collider.enabled) DetectForEnemy();
            if (_meshRender.material == _standarMaterial) _meshRender.material = _materialToChange;
            else _meshRender.material = _standarMaterial;
        }
    }

    // May not be used
    // When a wall collider is enabled, it instantly defeats any enemies 'inside' it
    private void DetectForEnemy() {
        //new Vector3(_meshRender.bounds.size.x * transform.localScale.x, _meshRender.bounds.size.y * transform.localScale.y, _meshRender.bounds.size.z * transform.localScale.z)
        Collider[] hits = Physics.OverlapBox(transform.position, transform.lossyScale / 2, Quaternion.identity, _enemyLayerMask);
        foreach (Collider enemy in hits)  enemy.GetComponent<EnemyData>().TakeDamage(999, 0, 0);        
    }
}
