using SurviveTheWild.Interface;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingHandler : MonoBehaviour, IRequestInput
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
    [SerializeField] protected float socketSearchRadius = 2f;
    [SerializeField] private LayerMask structureMask;
    private Sockets _targetSocket;
    private bool _canMove = false;
    private Camera cam;

    private bool canPlace;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update() // 
    {
        if (_currentStructure == null || !_canMove) return;
        UpdateGhostPosition();
        CheckPlacementValidity();
    }

    public void PlaceStructure()
    {
        if (_currentStructure == null || !_canMove || !canPlace) return;
        FinalizeMovement();
    }

    public void SetGHostObject(CraftingSO so)
    {
        SceneRoot.Instance.EventManager.playerEvents.DeliverRequestInput(this);
        var obj = Instantiate(so.resSo.prefab);
        _currentStructure = obj.GetComponent<BaseStructure>();
        _currentStructure.InitializeStructure(so);
        _currentStructure.SetValidMat(validMat);
        _currentStructure.SetInvalidMat(invalidMat);
        _currentStructure.ToggleMat(true);
        _currentStructure.DisabelAllColliders();
        _canMove = true;
    }

    public void DeconstructGhostObject() // only when we are in placement phase i want to 
    {
        if (_currentStructure == null) return;
        _canMove = false;
        Destroy(_currentStructure.gameObject);
        _currentStructure = null;
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
        canPlace = _currentStructure.CheckPlacement();
        _currentStructure.ToggleMat(canPlace);
    }
    
    private void FinalizeMovement()
    {
        _currentStructure.FinalizeMovement();
        _currentStructure.EnableAllColliders();
        canPlace = false;
        _currentStructure = null;
        SceneRoot.Instance.EventManager.playerEvents.ResetRequestedInput();
    }

    public void UseInput(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name == "Left")
        {
            PlaceStructure();
        }
    }

    public void ResetInput()
    {
        DeconstructGhostObject();
    }
}