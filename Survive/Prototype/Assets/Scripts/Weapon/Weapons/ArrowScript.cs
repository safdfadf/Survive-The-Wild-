using System;
using Player;
using UnityEngine;

public class ArrowScript : Obj<ObjSo>
{
    [SerializeField] private GameObject TestHitPoint;
    [SerializeField] private Vector3 offset;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Transform rayOrigin;
    private Vector3 _velocity;
    public float gravity = -9.81f;
   
    private bool _isStuck;
    private bool _canMove;
    private int _dmg;

    protected override void Awake()
    {
        Gm = gameObject;
        canBeCollected = false;
        outlineMe = false;
    }

    private void Start()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
    }

    public void InitDamage(int damage)
    {
      _dmg = damage;
    }

    private void Update()
    {
        MoveArrow();
    }

    private void StickArrow(RaycastHit hit) // use trigger enter 
    {
        _canMove = false;
        Debug.Log(hit.collider.gameObject.name);
        transform.position = hit.point;
        transform.rotation = Quaternion.LookRotation(-hit.normal);

        transform.SetParent(hit.collider.transform, true);
        PlayerAttack atk = new PlayerAttack(_dmg,null,null,hitPos);//ToDo Add effects for player
        
        ItakeDamage combatant = hit.collider.GetComponent<ItakeDamage>();
        if (combatant != null&& !combatant.IsEnvironment)
            combatant.TakeDamage(atk);
    }

    public void ShootArrow(Vector3 shootDirection, float arrowSpeed)
    {
        Debug.Log(arrowSpeed + " arrow Speed");
        _canMove = true;
        _velocity = shootDirection.normalized * arrowSpeed;
    }

    private void MoveArrow()
    {
        if (!_canMove) return;

        float dt = Time.deltaTime;


        _velocity.y += gravity * dt;


        transform.position += _velocity * dt;

        if (_velocity.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(_velocity);
        RaycastHit hit;
        float distance = _velocity.magnitude * Time.deltaTime;
        Debug.DrawLine(rayOrigin.position, rayOrigin.position + _velocity.normalized * distance, Color.red);
        if (Physics.Raycast(rayOrigin.position, _velocity.normalized, out hit, _velocity.magnitude * dt, mask))
        {
            StickArrow(hit);
        }
    }
}