using NUnit.Framework;
using UnityEngine;
using VirtualOnRevived.Core;
using VirtualOnRevived.Environment;

namespace VirtualOnRevived.Tests
{
    public class ArenaTests
    {
        [Test]
        public void ArenaBootstrapper_GravityOverride_AndRestoration()
        {
            Vector3 originalGravity = Physics.gravity;
            Vector3 targetGravity = new Vector3(0, -4.905f, 0);

            GameObject bootstrapperObj = new GameObject("TestBootstrapper");
            ArenaBootstrapper bootstrapper = bootstrapperObj.AddComponent<ArenaBootstrapper>();

            // Use reflection or private field setup via SerializedObject in editor, or test component properties
            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(bootstrapper);
            so.FindProperty("overrideGravity").boolValue = true;
            so.FindProperty("customGravity").vector3Value = targetGravity;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Simulate Awake
            System.Reflection.MethodInfo awakeMethod = typeof(ArenaBootstrapper).GetMethod("Awake", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            awakeMethod?.Invoke(bootstrapper, null);

            Assert.AreEqual(targetGravity, Physics.gravity, "Physics.gravity should match custom gravity after Awake.");

            // Simulate OnDestroy
            System.Reflection.MethodInfo onDestroyMethod = typeof(ArenaBootstrapper).GetMethod("OnDestroy", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            onDestroyMethod?.Invoke(bootstrapper, null);

            Assert.AreEqual(originalGravity, Physics.gravity, "Physics.gravity should be restored after OnDestroy.");

            Object.DestroyImmediate(bootstrapperObj);
        }

        [Test]
        public void CityGridGenerator_SpawnsExpectedGrid()
        {
            GameObject genObj = new GameObject("TestCityGen");
            CityGridGenerator generator = genObj.AddComponent<CityGridGenerator>();

            GameObject dummyBuilding = new GameObject("DummyBuilding");
            GameObject container = new GameObject("Container");

            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(generator);
            so.FindProperty("buildingPrefab").objectReferenceValue = dummyBuilding;
            so.FindProperty("gridSize").intValue = 3;
            so.FindProperty("spacing").floatValue = 10f;
            so.FindProperty("generateOnStart").boolValue = false;
            so.FindProperty("parentContainer").objectReferenceValue = container.transform;
            so.ApplyModifiedPropertiesWithoutUndo();

            generator.GenerateCityBlocks();

            Assert.AreEqual(9, container.transform.childCount, "3x3 grid should generate 9 buildings.");

            Object.DestroyImmediate(dummyBuilding);
            Object.DestroyImmediate(container);
            Object.DestroyImmediate(genObj);
        }

        [Test]
        public void DestructibleCrystal_ExplosionDamagesNearbyMechs()
        {
            GameObject crystalObj = new GameObject("TestCrystal");
            crystalObj.transform.position = Vector3.zero;
            DestructibleCrystal crystal = crystalObj.AddComponent<DestructibleCrystal>();

            // Create mech within 4m
            GameObject mechObj = new GameObject("TestMech");
            mechObj.transform.position = new Vector3(3f, 0, 0);
            mechObj.AddComponent<SphereCollider>(); // Needs collider for Physics.OverlapSphere
            MechHealth health = mechObj.AddComponent<MechHealth>();

            // Setup crystal properties
            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(crystal);
            so.FindProperty("maxHealth").floatValue = 50f;
            so.FindProperty("explosionRadius").floatValue = 8f;
            so.FindProperty("explosionDamage").floatValue = 35f;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Simulate Start
            System.Reflection.MethodInfo startMethod = typeof(DestructibleCrystal).GetMethod("Start", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            startMethod?.Invoke(crystal, null);

            // Deal fatal damage
            crystal.TakeDamage(100f);

            Assert.IsTrue(crystal.IsDestroyed, "Crystal should be marked as destroyed.");

            Object.DestroyImmediate(crystalObj);
            Object.DestroyImmediate(mechObj);
        }

        [Test]
        public void HazardVent_DefaultStateIsIdle()
        {
            GameObject ventObj = new GameObject("TestVent");
            HazardVent vent = ventObj.AddComponent<HazardVent>();

            Assert.AreEqual(HazardVent.VentState.Idle, vent.CurrentState, "HazardVent should begin in Idle state.");

            Object.DestroyImmediate(ventObj);
        }
    }
}
