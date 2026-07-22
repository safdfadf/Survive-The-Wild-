using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RegionRenderData
{
    public RegionType regionType;
    public Mesh mesh;
    public Material material;
}
public class GpuInstancting : MonoBehaviour
{
  [SerializeField]  private List<RegionRenderData> regionMeshes= new ();
  [SerializeField]  private Dictionary<Chunk, List<Matrix4x4>> chunkMatrices = new();
  [SerializeField] private Dictionary<RegionType, RegionRenderData> regionLookup = new();
  private int FunctCount = 0;
  

  private void OnEnable()
  {
      EventBus.OnGpuActivateInChunk += AddGpuChunk;
      EventBus.OnGpuDeactivateInChunk += RemoveGpuChunk;
  }
  private void OnDisable()
  {
      EventBus.OnGpuActivateInChunk -= AddGpuChunk;
      EventBus.OnGpuDeactivateInChunk -= RemoveGpuChunk;
  }
  private void Awake()
    {
        foreach (var mesh in regionMeshes)
        {
            regionLookup[mesh.regionType]= mesh;
        }
    }

    private void AddGpuChunk(Chunk chunk)
    {
        if (!regionLookup.ContainsKey(chunk.regionType))
            return;

        var renderData = regionLookup[chunk.regionType];

        List<Matrix4x4> matrices = new();

        foreach (var pos in chunk.cashedPos)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(
                pos.Position,
                Quaternion.identity,
                Vector3.one
            );

            matrices.Add(matrix);
        }

        chunkMatrices[chunk] = matrices;
      
    }

    private void RemoveGpuChunk(Chunk chunk)
    {
        FunctCount++;
        if (chunkMatrices.ContainsKey(chunk))
            chunkMatrices.Remove(chunk);
    }

    private void Update()
    {
        foreach (var kvp in chunkMatrices)
        {
            Chunk chunk = kvp.Key;

            if (!regionLookup.ContainsKey(chunk.regionType))
                continue;

            var renderData = regionLookup[chunk.regionType];
            var matrices = kvp.Value;
            var treeSo = SoProvider.instance.GetTreeSo();
            if (treeSo == null)
            {
                Debug.LogWarning("No Tree So");
            }
            int Count = Mathf.Min(treeSo.amount, matrices.Count);
            // Unity limit: 1023 per draw call
            for (int i = 0; i <Count; i += Count)
            {
               int count = Mathf.Min(1023, matrices.Count - i);
               
                Graphics.DrawMeshInstanced(
                    renderData.mesh,
                    0,
                    renderData.material,
                    matrices.GetRange(i, Count));
                Count--;
            }
        }
    }
}
