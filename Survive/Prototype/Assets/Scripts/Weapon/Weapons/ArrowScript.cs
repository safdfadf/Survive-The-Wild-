using System;
using UnityEngine;

public class ArrowScript : MonoBehaviour, ICollectable
{
    [SerializeField] private GameObject TestHitPoint;
    [SerializeField] private Vector3 offset;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Transform rayOrigin;
    private Vector3 _velocity;
    public float gravity = -9.81f;
    private ResourceType type;
    public ResourceSo So { get; set; }
    public bool canBeCollected { get; set; }
    public GameObject Gm { get; private set; }
    private bool _isStuck;
    private bool _canMove;
    private Vector3 lastPosition;

    private void Awake()
    {
        Gm = gameObject;
        canBeCollected = true;
    }

    private void Start()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
    }

    public void Init(Collider PlayerCollider, Collider bowCol)
    {
        Collider arrowCollider = GetComponent<Collider>();
        Physics.IgnoreCollision(PlayerCollider, arrowCollider);
        Physics.IgnoreCollision(bowCol, arrowCollider);
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

        transform.SetParent(hit.collider.transform,true);

        ItakeDamage combatant = hit.collider.GetComponent<ItakeDamage>();
        if (combatant != null)
            combatant.TakeDamage(0,hit.point);
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

        lastPosition = transform.position;

        transform.position += _velocity * dt;

        if (_velocity.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(_velocity);
        RaycastHit hit;
        //  Vector3 rayOrigin = lastPosition + _velocity.normalized * 0.1f;
        float distance = _velocity.magnitude * Time.deltaTime;
        Debug.DrawLine(rayOrigin.position, rayOrigin.position + _velocity.normalized * distance, Color.red);
        if (Physics.Raycast(rayOrigin.position, _velocity.normalized, out hit, _velocity.magnitude * dt, mask))
        {
            StickArrow(hit);
        }
    }


    public void Collect(PlayerInventory collector)
    {
        canBeCollected = false;
        collector.AddResource(this);
    }

    public void ToggleMenu()
    {
    }
}