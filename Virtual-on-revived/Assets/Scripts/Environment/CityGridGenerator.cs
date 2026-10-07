using UnityEngine;

namespace VirtualOnRevived.Environment
{
    public class CityGridGenerator : MonoBehaviour
    {
        [Header("Grid Configuration")]
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private int gridSize = 5;
        [SerializeField] private float spacing = 20f;

        [Header("Spawn Settings")]
        [SerializeField] private bool generateOnStart = true;
        [SerializeField] private Transform parentContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateCityBlocks();
            }
        }

        public void GenerateCityBlocks()
        {
            if (buildingPrefab == null)
            {
                Debug.LogWarning("[CityGridGenerator] No buildingPrefab assigned.");
                return;
            }

            Transform container = parentContainer != null ? parentContainer : transform;
            Vector3 startPos = new Vector3(-gridSize * spacing / 2f, 0, -gridSize * spacing / 2f);

            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    Vector3 position = startPos + new Vector3(x * spacing, 0, z * spacing);
                    Instantiate(buildingPrefab, position, Quaternion.identity, container);
                }
            }
        }
    }
}
