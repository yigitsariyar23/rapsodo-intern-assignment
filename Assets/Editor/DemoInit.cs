using UnityEngine;

public class DemoInit : MonoBehaviour
{
    public static void InitScene(bool isGrouped)
    {
        CleanScene();

        // Create a parent object for organization
        GameObject root = new GameObject("AssessmentSceneObjects");

        // Create groups of objects
        CreateGameObjects("MeshRendererCubes", 5, CreateMeshRendererCube, root, isGrouped);
        CreateGameObjects("ColliderCubes", 5, CreateColliderCube, root, isGrouped);
        CreateGameObjects("RigidbodyObjects", 5, CreateRigidbodyObject, root, isGrouped);
        CreateGameObjects("SphereObjects", 5, CreateSphereWithColliderAndMesh, root, isGrouped);
        CreateGameObjects("FullComponentObjects", 5, CreateFullComponentObject, root, isGrouped);
        CreateGameObjects("InactiveObjects", 5, CreateInactiveObject, root, isGrouped);
        CreateGameObjects("TransformOnlyObjects", 5, CreateTransformOnlyObject, root, isGrouped);

        // Destroy the root object if not grouped
        if (!isGrouped)
        {
            DestroyImmediate(root);
        }
    }

    private static void CleanScene()
    {
        // Destroy all GameObjects in the scene, including inactive ones
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj != null && obj.name != "Main Camera" && obj.hideFlags == HideFlags.None)
            {
                DestroyImmediate(obj);
            }
        }
    }

    private static void CreateGameObjects(string groupName, int count, System.Action<GameObject> createMethod, GameObject parent, bool isGrouped)
    {
        GameObject group = null;
        if (isGrouped)
        {
            group = new GameObject(groupName);
            group.transform.SetParent(parent.transform);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = new GameObject($"{groupName}_{i}");
            createMethod(obj);
            if (isGrouped)
            {
                obj.transform.SetParent(group.transform);
            }
            else
            {
                continue;
            }
        }
    }

    private static void CreateMeshRendererCube(GameObject obj)
    {
        obj.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        obj.AddComponent<MeshRenderer>();
    }

    private static void CreateColliderCube(GameObject obj)
    {
        obj.AddComponent<BoxCollider>();
    }

    private static void CreateRigidbodyObject(GameObject obj)
    {
        obj.AddComponent<Rigidbody>();
    }

    private static void CreateSphereWithColliderAndMesh(GameObject obj)
    {
        obj.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<SphereCollider>();
    }

    private static void CreateFullComponentObject(GameObject obj)
    {
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<Rigidbody>();
        obj.AddComponent<BoxCollider>();
    }

    private static void CreateInactiveObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    private static void CreateTransformOnlyObject(GameObject obj)
    {
        // No additional components are added, Transform is already present
    }
}
