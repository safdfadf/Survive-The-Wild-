using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    [Header("Materials")] [SerializeField] private Material validMat; // Ghost
    [SerializeField] private Material invalidMat; // Red
    [SerializeField] private GameObject test;
    [Header("Settings")] [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask collisionMask;

    private GameObject ghostObject;
    private BaseStructure _currentStructure;
    private MeshRenderer[] ghostRenderers;
    [SerializeField] private float socketSearchRadius = 2f;
    [SerializeField] private LayerMask structureMask;
    private Sockets _targetSocket;
    private bool _canMove = false;
    private Camera cam;

    private bool canPlace;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (_currentStructure == null || !_canMove) return;
        UpdateGhostPosition();
        CheckPlacementValidity();
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            FinalizeMovement();
        }
    }

    public void SetGHostObject(CraftingSO so)
    {
        GameObject obj = Instantiate(so.resSo.prefab);
        _currentStructure = obj.GetComponent<BaseStructure>();
        _currentStructure.InitializeStructure(so);
        _currentStructure.SetValidMat(validMat);
        _currentStructure.SetInvalidMat(invalidMat);
        _currentStructure.ToggleMat(true);
        _currentStructure.DisabelAllColliders();
        _canMove = true;
    }

    private void UpdateGhostPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask))
        {
            Vector3 pos = hit.point;

            // Snap to grid
            pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
            pos.z = Mathf.Round(pos.z / gridSize) * gridSize;


            _currentStructure.transform.position = pos;
        }
    }

    private void CheckPlacementValidity()
    {
        if (_currentStructure.craftingRecipe.isChildStructure)
        {
            TrySocketPlacement();
            return;
        }

        TryFreePlacement();
    }

    private void TrySocketPlacement()
    {
        Debug.Log("hit with Frame structure");
        _targetSocket = null;

        Collider[] hits = Physics.OverlapSphere(
            _currentStructure.transform.position,
            socketSearchRadius,
            collisionMask
        );

        foreach (var col in hits)
        {
            FrameStructure frame = col.GetComponentInParent<FrameStructure>();
            if (frame == null) continue;

            Sockets socket = frame.GetClosestValidSocket(
                _currentStructure.craftingRecipe.structureType,
                _currentStructure.transform.position
            );

            if (socket != null)
            {
                _targetSocket = socket;

                _currentStructure.transform.position = socket.transform.position;
                _currentStructure.transform.rotation = socket.transform.rotation;

                canPlace = true;
                _currentStructure.ToggleMat(true);
                return;
            }
        }

        // No socket found → cannot place child structure
        canPlace = false;
        _currentStructure.ToggleMat(false);
    }

    private void TryFreePlacement()
    {
        Collider[] hits = Physics.OverlapBox(
            _currentStructure.transform.position,
            _currentStructure.transform.localScale,
            _currentStructure.transform.rotation,
            collisionMask
        );
        foreach (var hit in hits)
        {
            Debug.Log(hit.gameObject.name);
        }

        canPlace = hits.Length == 0;
        _currentStructure.ToggleMat(canPlace);
    }

    private void FinalizeMovement()
    {
        if (_targetSocket != null)
        {
            _targetSocket.Attach(_currentStructure);
            FrameStructure parent = _targetSocket.GetComponentInParent<FrameStructure>();
            parent.RegisterChild(_currentStructure);
        }
        _currentStructure.EnableAllColliders();
        canPlace = false;
        _currentStructure = null;
    }
}