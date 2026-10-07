using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VirtualOnRevived.Core;
using VirtualOnRevived.Environment;

namespace VirtualOnRevived.Editor
{
    public static class LevelSceneGenerator
    {
        private const string LevelsFolder = "Assets/Scenes/Levels";

        [MenuItem("Virtual-On/Levels/Generate All Level Scenes")]
        public static void GenerateAllLevels()
        {
            EnsureLevelsDirectory();

            GenerateCityArenaScene();
            GenerateAsteroidOutpostScene();
            GenerateAlienTundraScene();
            GenerateOrbitalPlatformScene();

            UpdateEditorBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Virtual-On Level Generation",
                "Successfully generated all 4 level scenes:\n" +
                "- Level_00_CityArena.unity\n" +
                "- Level_01_AsteroidOutpost.unity\n" +
                "- Level_02_AlienTundra.unity\n" +
                "- Level_03_OrbitalPlatform.unity\n\n" +
                "Scenes have been registered in EditorBuildSettings.", "OK");
        }

        private static void EnsureLevelsDirectory()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
            if (!AssetDatabase.IsValidFolder(LevelsFolder))
            {
                AssetDatabase.CreateFolder("Assets/Scenes", "Levels");
            }
        }

        #region Level 0: City Arena
        [MenuItem("Virtual-On/Levels/Generate City Arena (Level 0)")]
        public static void GenerateCityArenaScene()
        {
            EnsureLevelsDirectory();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCommonEnvironment("City Arena", Color.white, out GameObject arenaMgr);
            
            // Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor_Asphalt";
            floor.transform.position = new Vector3(0, -0.5f, 0);
            floor.transform.localScale = new Vector3(120, 1, 120);
            SetColor(floor, new Color(0.2f, 0.2f, 0.22f));

            // City Grid Generator
            CityGridGenerator gridGen = arenaMgr.AddComponent<CityGridGenerator>();
            // Fallback building prefab or placeholder
            GameObject placeholderBuilding = CreateBuildingPlaceholder();
            SerializedObject serializedGen = new SerializedObject(gridGen);
            serializedGen.FindProperty("buildingPrefab").objectReferenceValue = placeholderBuilding;
            serializedGen.ApplyModifiedPropertiesWithoutUndo();

            // Perimeter walls
            CreatePerimeterBoundaries(120, 120, 25);

            // Spawns
            CreateSpawnPoints(75f);

            string path = $"{LevelsFolder}/Level_00_CityArena.unity";
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[LevelSceneGenerator] Saved {path}");
        }
        #endregion

        #region Level 1: Asteroid Outpost
        [MenuItem("Virtual-On/Levels/Generate Asteroid Outpost (Level 1)")]
        public static void GenerateAsteroidOutpostScene()
        {
            EnsureLevelsDirectory();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCommonEnvironment("Asteroid Outpost", new Color(0.6f, 0.7f, 0.85f), out GameObject arenaMgr);

            // Low-G configuration in ArenaBootstrapper
            ArenaBootstrapper bootstrapper = arenaMgr.GetComponent<ArenaBootstrapper>();
            SerializedObject serializedBoot = new SerializedObject(bootstrapper);
            serializedBoot.FindProperty("overrideGravity").boolValue = true;
            serializedBoot.FindProperty("customGravity").vector3Value = new Vector3(0, -4.905f, 0);
            serializedBoot.ApplyModifiedPropertiesWithoutUndo();

            // Crater Floor (Recessed center)
            GameObject basinFloor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            basinFloor.name = "Crater_Basin_Floor";
            basinFloor.transform.position = new Vector3(0, -1.5f, 0);
            basinFloor.transform.localScale = new Vector3(70, 0.5f, 70);
            SetColor(basinFloor, new Color(0.18f, 0.17f, 0.2f));

            // Outer Rim Ring (Elevated)
            GameObject outerRim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            outerRim.name = "Crater_Outer_Rim";
            outerRim.transform.position = new Vector3(0, 0f, 0);
            outerRim.transform.localScale = new Vector3(110, 0.5f, 110);
            SetColor(outerRim, new Color(0.28f, 0.26f, 0.3f));

            // Perimeter Mining Silos (Indestructible LoS cover)
            GameObject coverRoot = new GameObject("_MiningSilos");
            int siloCount = 6;
            float rimRadius = 45f;
            for (int i = 0; i < siloCount; i++)
            {
                float angle = i * (360f / siloCount) * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * rimRadius, 6f, Mathf.Sin(angle) * rimRadius);

                GameObject silo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                silo.name = $"Mining_Silo_{i + 1}";
                silo.transform.position = pos;
                silo.transform.localScale = new Vector3(8, 6, 8); // 12m height
                silo.transform.parent = coverRoot.transform;
                SetColor(silo, new Color(0.45f, 0.45f, 0.5f));
            }

            // Floating Asteroid Nodes (Dynamic Oscillating Cover)
            GameObject debrisRoot = new GameObject("_FloatingAsteroids");
            Vector3[] asteroidPositions = new Vector3[]
            {
                new Vector3(-18f, 4f, 12f),
                new Vector3(18f, 5f, -14f),
                new Vector3(15f, 3.5f, 20f),
                new Vector3(-20f, 6f, -18f),
                new Vector3(0f, 4.5f, 0f)
            };

            for (int i = 0; i < asteroidPositions.Length; i++)
            {
                GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rock.name = $"Floating_Asteroid_{i + 1}";
                rock.transform.position = asteroidPositions[i];
                rock.transform.localScale = new Vector3(7, 5, 6);
                rock.transform.parent = debrisRoot.transform;
                SetColor(rock, new Color(0.35f, 0.32f, 0.35f));

                Rigidbody rb = rock.AddComponent<Rigidbody>();
                rb.isKinematic = true;

                OscillatingObstacle osc = rock.AddComponent<OscillatingObstacle>();
                SerializedObject serializedOsc = new SerializedObject(osc);
                serializedOsc.FindProperty("movementOffset").vector3Value = new Vector3(0, 1.8f, 0);
                serializedOsc.FindProperty("frequency").floatValue = 0.3f + (i * 0.05f);
                serializedOsc.FindProperty("phaseShift").floatValue = i * 1.2f;
                serializedOsc.FindProperty("rotationSpeed").vector3Value = new Vector3(3f, 8f + i, 2f);
                serializedOsc.ApplyModifiedPropertiesWithoutUndo();
            }

            CreatePerimeterBoundaries(115, 115, 25);
            CreateSpawnPoints(75f);

            string path = $"{LevelsFolder}/Level_01_AsteroidOutpost.unity";
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[LevelSceneGenerator] Saved {path}");
        }
        #endregion

        #region Level 2: Alien Tundra
        [MenuItem("Virtual-On/Levels/Generate Alien Tundra (Level 2)")]
        public static void GenerateAlienTundraScene()
        {
            EnsureLevelsDirectory();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCommonEnvironment("Alien Tundra", new Color(0.4f, 0.85f, 0.7f), out GameObject arenaMgr);

            // Valley Ground Floor
            GameObject valley = GameObject.CreatePrimitive(PrimitiveType.Cube);
            valley.name = "Valley_Floor";
            valley.transform.position = new Vector3(0, -0.5f, 0);
            valley.transform.localScale = new Vector3(110, 1, 110);
            SetColor(valley, new Color(0.12f, 0.18f, 0.16f));

            // Stepped Terrace 1 (High Plateau +4m)
            GameObject terrace1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            terrace1.name = "Terrace_NorthPlateau";
            terrace1.transform.position = new Vector3(25, 2f, 25);
            terrace1.transform.localScale = new Vector3(45, 4, 45);
            SetColor(terrace1, new Color(0.18f, 0.28f, 0.24f));

            // Stepped Terrace 2 (Overlook Ridge +3m)
            GameObject terrace2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            terrace2.name = "Terrace_SouthRidge";
            terrace2.transform.position = new Vector3(-25, 1.5f, -25);
            terrace2.transform.localScale = new Vector3(40, 3, 40);
            SetColor(terrace2, new Color(0.18f, 0.28f, 0.24f));

            // Ramps connecting terraces
            CreateRamp(new Vector3(25, 1f, 0), new Vector3(15, 0.5f, 15), 15f);
            CreateRamp(new Vector3(-25, 0.75f, 0), new Vector3(15, 0.5f, 15), -15f);

            // Petrified Alien Trunks (Indestructible cover)
            GameObject trunkRoot = new GameObject("_AlienTrunks");
            Vector3[] trunkPositions = new Vector3[]
            {
                new Vector3(-15, 4, 15),
                new Vector3(15, 4, -15),
                new Vector3(-35, 4, 10),
                new Vector3(35, 4, -10)
            };
            foreach (var pos in trunkPositions)
            {
                GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                trunk.name = "Alien_Trunk";
                trunk.transform.position = pos;
                trunk.transform.localScale = new Vector3(4, 4, 4); // 8m height
                trunk.transform.parent = trunkRoot.transform;
                SetColor(trunk, new Color(0.35f, 0.25f, 0.35f));
            }

            // Destructible Explosive Crystals
            GameObject crystalRoot = new GameObject("_ExplosiveCrystals");
            Vector3[] crystalPositions = new Vector3[]
            {
                new Vector3(0, 2f, 15),
                new Vector3(0, 2f, -15),
                new Vector3(18, 5f, 18),
                new Vector3(-18, 4f, -18),
                new Vector3(-8, 2f, 0),
                new Vector3(8, 2f, 0)
            };

            foreach (var pos in crystalPositions)
            {
                GameObject crystalObj = new GameObject("Destructible_Crystal");
                crystalObj.transform.position = pos;
                crystalObj.transform.parent = crystalRoot.transform;

                // Visual mesh
                GameObject meshObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                meshObj.name = "Crystal_Mesh";
                meshObj.transform.SetParent(crystalObj.transform, false);
                meshObj.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
                SetColor(meshObj, new Color(0.85f, 0.1f, 0.95f));

                // Intact Collider
                CapsuleCollider intactCol = crystalObj.AddComponent<CapsuleCollider>();
                intactCol.radius = 1.2f;
                intactCol.height = 4f;

                // Stump Collider (inactive until destroyed)
                BoxCollider stumpCol = crystalObj.AddComponent<BoxCollider>();
                stumpCol.size = new Vector3(2f, 0.5f, 2f);
                stumpCol.center = new Vector3(0, -1.75f, 0);
                stumpCol.enabled = false;

                // Component
                DestructibleCrystal crystalComp = crystalObj.AddComponent<DestructibleCrystal>();
                SerializedObject serializedCryst = new SerializedObject(crystalComp);
                serializedCryst.FindProperty("crystalMesh").objectReferenceValue = meshObj;
                serializedCryst.FindProperty("intactCollider").objectReferenceValue = intactCol;
                serializedCryst.FindProperty("stumpCollider").objectReferenceValue = stumpCol;
                serializedCryst.FindProperty("explosionRadius").floatValue = 8f;
                serializedCryst.FindProperty("explosionDamage").floatValue = 35f;
                serializedCryst.ApplyModifiedPropertiesWithoutUndo();
            }

            CreatePerimeterBoundaries(110, 110, 25);
            CreateSpawnPoints(75f);

            string path = $"{LevelsFolder}/Level_02_AlienTundra.unity";
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[LevelSceneGenerator] Saved {path}");
        }
        #endregion

        #region Level 3: Orbital Platform
        [MenuItem("Virtual-On/Levels/Generate Orbital Platform (Level 3)")]
        public static void GenerateOrbitalPlatformScene()
        {
            EnsureLevelsDirectory();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCommonEnvironment("Orbital Platform", new Color(0.9f, 0.95f, 1f), out GameObject arenaMgr);

            // Octagonal Main Deck (Square Center + 4 Angled Flanks)
            GameObject deckCenter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            deckCenter.name = "Platform_Deck_Center";
            deckCenter.transform.position = new Vector3(0, -0.5f, 0);
            deckCenter.transform.localScale = new Vector3(70, 1, 70);
            SetColor(deckCenter, new Color(0.3f, 0.32f, 0.36f));

            // Angled extensions to form octagon
            float[] rotations = { 45f, -45f, 135f, -135f };
            for (int i = 0; i < rotations.Length; i++)
            {
                GameObject flank = GameObject.CreatePrimitive(PrimitiveType.Cube);
                flank.name = $"Platform_Flank_{i + 1}";
                flank.transform.position = new Vector3(0, -0.5f, 0);
                flank.transform.rotation = Quaternion.Euler(0, rotations[i], 0);
                flank.transform.localScale = new Vector3(70, 0.99f, 70);
                SetColor(flank, new Color(0.28f, 0.3f, 0.34f));
            }

            // Energy Barriers (Transparent LoS with Projectile Collision)
            GameObject barrierRoot = new GameObject("_EnergyBarriers");
            Vector3[] barrierPositions = new Vector3[]
            {
                new Vector3(-18, 3, 0),
                new Vector3(18, 3, 0),
                new Vector3(0, 3, -18),
                new Vector3(0, 3, 18)
            };
            Vector3[] barrierScales = new Vector3[]
            {
                new Vector3(0.5f, 6, 16),
                new Vector3(0.5f, 6, 16),
                new Vector3(16, 6, 0.5f),
                new Vector3(16, 6, 0.5f)
            };

            for (int i = 0; i < barrierPositions.Length; i++)
            {
                GameObject barrierObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                barrierObj.name = $"Energy_Barrier_{i + 1}";
                barrierObj.transform.position = barrierPositions[i];
                barrierObj.transform.localScale = barrierScales[i];
                barrierObj.transform.parent = barrierRoot.transform;

                SetColor(barrierObj, new Color(0f, 0.85f, 1f, 0.35f));
                barrierObj.AddComponent<EnergyBarrier>();
            }

            // Thermal Hazard Vents
            GameObject ventRoot = new GameObject("_ThermalVents");
            Vector3[] ventPositions = new Vector3[]
            {
                new Vector3(-12, 0.05f, -12),
                new Vector3(12, 0.05f, 12),
                new Vector3(-12, 0.05f, 12),
                new Vector3(12, 0.05f, -12)
            };

            for (int i = 0; i < ventPositions.Length; i++)
            {
                GameObject ventObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ventObj.name = $"Hazard_Vent_{i + 1}";
                ventObj.transform.position = ventPositions[i];
                ventObj.transform.localScale = new Vector3(8, 0.1f, 8);
                ventObj.transform.parent = ventRoot.transform;
                SetColor(ventObj, new Color(0.15f, 0.15f, 0.18f));

                // Trigger volume above the vent
                GameObject triggerObj = new GameObject("DamageTrigger");
                triggerObj.transform.SetParent(ventObj.transform, false);
                triggerObj.transform.localPosition = new Vector3(0, 15f, 0); // Scale is 0.1, so local 15 gives 1.5m height
                triggerObj.transform.localScale = new Vector3(1f, 30f, 1f);
                BoxCollider triggerCol = triggerObj.AddComponent<BoxCollider>();
                triggerCol.isTrigger = true;

                // Visual Indicator
                GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                indicator.name = "Warning_Light";
                indicator.transform.SetParent(ventObj.transform, false);
                indicator.transform.localPosition = new Vector3(0, 0.6f, 0);
                indicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                SetColor(indicator, Color.green);

                HazardVent ventComp = triggerObj.AddComponent<HazardVent>();
                SerializedObject serializedVent = new SerializedObject(ventComp);
                serializedVent.FindProperty("initialDelay").floatValue = i * 1.5f; // Stagger cycle
                serializedVent.FindProperty("idleDuration").floatValue = 4f;
                serializedVent.FindProperty("warningDuration").floatValue = 2f;
                serializedVent.FindProperty("activeDuration").floatValue = 3f;
                serializedVent.FindProperty("indicatorRenderer").objectReferenceValue = indicator.GetComponent<Renderer>();
                serializedVent.ApplyModifiedPropertiesWithoutUndo();
            }

            CreatePerimeterBoundaries(95, 95, 25);
            CreateSpawnPoints(75f);

            string path = $"{LevelsFolder}/Level_03_OrbitalPlatform.unity";
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[LevelSceneGenerator] Saved {path}");
        }
        #endregion

        #region Helpers
        private static void CreateCommonEnvironment(string sceneName, Color lightColor, out GameObject arenaMgr)
        {
            // Arena Manager
            arenaMgr = new GameObject("_ArenaManager");
            arenaMgr.AddComponent<ArenaBootstrapper>();

            // Directional Light
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = lightColor;
            light.intensity = 1.6f;
            light.shadows = LightShadows.Soft;
            lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Spawns Root
            new GameObject("_Spawns");
        }

        private static void CreateSpawnPoints(float distance)
        {
            GameObject spawnsRoot = GameObject.Find("_Spawns") ?? new GameObject("_Spawns");

            GameObject p1 = new GameObject("SpawnPoint_Player_1");
            p1.transform.position = new Vector3(0, 0.5f, -distance / 2f);
            p1.transform.rotation = Quaternion.Euler(0, 0, 0);
            p1.transform.parent = spawnsRoot.transform;

            GameObject p2 = new GameObject("SpawnPoint_Player_2");
            p2.transform.position = new Vector3(0, 0.5f, distance / 2f);
            p2.transform.rotation = Quaternion.Euler(0, 180, 0);
            p2.transform.parent = spawnsRoot.transform;
        }

        private static void CreatePerimeterBoundaries(float width, float depth, float height)
        {
            GameObject boundsRoot = new GameObject("_PerimeterColliders");

            CreateWall(boundsRoot.transform, "Wall_North", new Vector3(0, height / 2f, depth / 2f), new Vector3(width, height, 1));
            CreateWall(boundsRoot.transform, "Wall_South", new Vector3(0, height / 2f, -depth / 2f), new Vector3(width, height, 1));
            CreateWall(boundsRoot.transform, "Wall_East", new Vector3(width / 2f, height / 2f, 0), new Vector3(1, height, depth));
            CreateWall(boundsRoot.transform, "Wall_West", new Vector3(-width / 2f, height / 2f, 0), new Vector3(1, height, depth));
            CreateWall(boundsRoot.transform, "Ceiling_Limit", new Vector3(0, height, 0), new Vector3(width, 1, depth));
        }

        private static void CreateWall(Transform parent, string name, Vector3 pos, Vector3 size)
        {
            GameObject wall = new GameObject(name);
            wall.transform.position = pos;
            wall.transform.parent = parent;
            BoxCollider col = wall.AddComponent<BoxCollider>();
            col.size = size;
        }

        private static void CreateRamp(Vector3 pos, Vector3 size, float xRotation)
        {
            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Terrace_Ramp";
            ramp.transform.position = pos;
            ramp.transform.rotation = Quaternion.Euler(xRotation, 0, 0);
            ramp.transform.localScale = size;
            SetColor(ramp, new Color(0.2f, 0.25f, 0.22f));
        }

        private static GameObject CreateBuildingPlaceholder()
        {
            // Try to load DestructibleBuilding or create a runtime dummy prefab
            string[] guids = AssetDatabase.FindAssets("t:Prefab DestructibleBuilding");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }

            return null;
        }

        private static void SetColor(GameObject obj, Color color)
        {
            Renderer r = obj.GetComponent<Renderer>();
            if (r != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                mat.color = color;
                r.material = mat;
            }
        }

        private static void UpdateEditorBuildSettings()
        {
            string[] scenePaths = new string[]
            {
                "Assets/Scenes/SampleScene.unity",
                $"{LevelsFolder}/Level_00_CityArena.unity",
                $"{LevelsFolder}/Level_01_AsteroidOutpost.unity",
                $"{LevelsFolder}/Level_02_AlienTundra.unity",
                $"{LevelsFolder}/Level_03_OrbitalPlatform.unity"
            };

            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[scenePaths.Length];
            for (int i = 0; i < scenePaths.Length; i++)
            {
                buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }

            EditorBuildSettings.scenes = buildScenes;
        }
        #endregion
    }
}
