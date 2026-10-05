using UnityEngine;

namespace VirtualOnRevived.Core
{
    public class ArenaBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private int gridSize = 5;
        [SerializeField] private float spacing = 20f;

        private void Start()
        {
            GenerateCityBlocks();
            ConfigurePhysicsMatrix();
        }

        private void GenerateCityBlocks()
        {
            if (buildingPrefab == null) return;

            Vector3 startPos = new Vector3(-gridSize * spacing / 2f, 0, -gridSize * spacing / 2f);

            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    Vector3 position = startPos + new Vector3(x * spacing, 0, z * spacing);
                    Instantiate(buildingPrefab, position, Quaternion.identity, transform);
                }
            }
        }

        private void ConfigurePhysicsMatrix()
        {
            // Dynamically set physics matrix ignoring Debris vs Debris to save CPU
            int debrisLayer = LayerMask.NameToLayer("Debris");
            if (debrisLayer != -1)
            {
                Physics.IgnoreLayerCollision(debrisLayer, debrisLayer, true);
            }
            else
            {
                Debug.LogWarning("Debris layer not found. Please add a 'Debris' layer in Project Settings -> Tags and Layers.");
            }
        }
    }
}
