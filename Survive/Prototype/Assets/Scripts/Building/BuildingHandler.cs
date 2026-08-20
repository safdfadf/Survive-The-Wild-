using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
   [Header("Materials")]
   [SerializeField] private Material validMat;     // Ghost
   [SerializeField] private Material invalidMat;   // Red

   [Header("Settings")]
   [SerializeField] private float gridSize = 1f;
   [SerializeField] private LayerMask groundMask;
   [SerializeField] private LayerMask collisionMask;

   private GameObject ghostObject;
   private BaseStructure _currentStructure;
   private MeshRenderer[] ghostRenderers;
   
   private bool _canMove =false;
   private Camera cam;

   private bool canPlace;
   private void Awake()
   {
      cam = Camera.main;
   }
   private void Update()
   {
      if (_currentStructure == null ||!_canMove) return;
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

         if (_currentStructure == null)
         {
            Debug.Log(_currentStructure);
         }
         _currentStructure.transform.position = pos;
      }
   }
   private void CheckPlacementValidity()
   {
      // Check if ghost overlaps with anything
      Collider[] hits = Physics.OverlapBox(
         _currentStructure.transform.position,
         _currentStructure.transform.localScale / 2f,
         _currentStructure.transform.rotation,
         collisionMask
      );

      canPlace = hits.Length == 0;
      _currentStructure.ToggleMat(canPlace);
   }

   public void FinalizeMovement()
   {
      canPlace = false;
      _currentStructure = null;
   }
}
