using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.IMGUI.Controls;

public class CustomEditorWindow : EditorWindow
{
    private string searchString = "";
    private bool filterMeshRenderer = false;
    private bool filterCollider = false;
    private bool filterRigidbody = false;
    private AdvancedDropdownState dropdownState = new AdvancedDropdownState();
    private Type selectedComponentType = null;
    private List<Type> componentTypes;
    private Vector2 scrollPos;
    private List<GameObject> allGameObjects = new List<GameObject>();
    private Dictionary<GameObject, bool> selectionDict = new Dictionary<GameObject, bool>();

    [MenuItem("Tools/Custom Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<CustomEditorWindow>("GameObject Manager");
    }

    private void OnEnable()
    {
        RefreshGameObjectList();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Initialize w/ grouping"))
        {
            if (EditorUtility.DisplayDialog("Confirm Initialization", "Are you sure you want to initialize with grouping? This action will clear the current scene and irreversible.", "Yes", "No"))
            {
                DemoInit.InitScene(true);
                RefreshGameObjectList();
            }
        }
        if (GUILayout.Button("Initialize w/o grouping"))
        {
            if (EditorUtility.DisplayDialog("Confirm Initialization", "Are you sure you want to initialize without grouping? This action will clear the current scene and irreversible.", "Yes", "No"))
            {
                DemoInit.InitScene(false);
                RefreshGameObjectList();
            }
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Refresh List"))
        {
            RefreshGameObjectList();
        }
        DrawSearchBar();
        DrawFilterToggles();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawGameObjectList();
        DrawSelectionEditor();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
        string newSearch = EditorGUILayout.TextField(searchString);
        if (newSearch != searchString)
        {
            searchString = newSearch;
            // Optionally refresh something here, or just rely on dynamic filtering
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawFilterToggles()
    {
        EditorGUILayout.BeginHorizontal();
        filterMeshRenderer = GUILayout.Toggle(filterMeshRenderer, "Mesh Renderer");
        filterCollider = GUILayout.Toggle(filterCollider, "Collider");
        filterRigidbody = GUILayout.Toggle(filterRigidbody, "Rigidbody");
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGameObjectList()
    {
        // Start a scroll view
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        // Add headers for the columns
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", GUILayout.Width(200));
        EditorGUILayout.LabelField("Active State", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        // Filter the list based on user input
        var filteredList = FilterGameObjectList();

        foreach (GameObject go in filteredList)
        {
            // Check if the GameObject is null (destroyed)
            if (go == null)
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal();

            // Ensure the selectionDict contains the current GameObject
            if (!selectionDict.ContainsKey(go))
            {
                selectionDict[go] = false;
            }

            // Selection Toggle
            bool currentlySelected = selectionDict[go];
            bool newSelection = EditorGUILayout.ToggleLeft(go.name, currentlySelected, GUILayout.Width(200));
            if (newSelection != currentlySelected)
            {
                selectionDict[go] = newSelection;
            }

            // Active State Toggle
            bool newActive = EditorGUILayout.Toggle(go.activeSelf, GUILayout.Width(30));
            if (newActive != go.activeSelf)
            {
                // Record Undo
                Undo.RecordObject(go, "Toggle GameObject Active");
                go.SetActive(newActive);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private List<GameObject> FilterGameObjectList()
    {
        return allGameObjects.Where(go =>
        {
            // Check if the GameObject is null (destroyed)
            if (go == null)
            {
                return false;
            }

            // Search filter
            if (!string.IsNullOrEmpty(searchString) &&
                !go.name.ToLower().Contains(searchString.ToLower()))
            {
                return false;
            }

            // Component filters
            if (filterMeshRenderer && go.GetComponent<MeshRenderer>() == null) return false;
            if (filterCollider && go.GetComponent<Collider>() == null) return false;
            if (filterRigidbody && go.GetComponent<Rigidbody>() == null) return false;

            return true;
        }).ToList();
    }

    private void DrawSelectionEditor()
    {
        var selectedGOs = selectionDict.Where(kv => kv.Value).Select(kv => kv.Key).ToList();

        if (selectedGOs.Count == 0)
        {
            EditorGUILayout.HelpBox("No GameObjects selected.", MessageType.Info);
            return;
        }

        // Transform batch edit
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Batch Transform Edit", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        Vector3 newPosition = EditorGUILayout.Vector3Field("Position", selectedGOs[0].transform.localPosition);
        Vector3 newRotation = EditorGUILayout.Vector3Field("Rotation", selectedGOs[0].transform.localEulerAngles);
        Vector3 newScale = EditorGUILayout.Vector3Field("Scale", selectedGOs[0].transform.localScale);

        if (EditorGUI.EndChangeCheck())
        {
            foreach (var go in selectedGOs)
            {
                Undo.RecordObject(go.transform, "Batch Transform Change");
                go.transform.localPosition = newPosition;
                go.transform.localEulerAngles = newRotation;
                go.transform.localScale = newScale;
            }
        }

        // Add Component UI
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add or Remove Component to All Selected", EditorStyles.boldLabel);

        // Show selected component type (if any)
        string selectedComponentName = selectedComponentType != null ? selectedComponentType.Name : "Select Component";

        if (GUILayout.Button(selectedComponentName, EditorStyles.popup))
        {
            var dropdown = new ComponentDropdown(dropdownState, type => selectedComponentType = type);
            dropdown.Show(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        if (selectedComponentType != null)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Component"))
            {
                foreach (var go in selectedGOs)
                {
                    if (!go.GetComponent(selectedComponentType))
                    {
                        Undo.AddComponent(go, selectedComponentType);
                    }
                }
            }

            if (GUILayout.Button("Remove Component"))
            {
                foreach (var go in selectedGOs)
                {
                    var component = go.GetComponent(selectedComponentType);
                    if (component != null)
                    {
                        Undo.DestroyObjectImmediate(component);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void RefreshGameObjectList()
    {
        // Find all GameObjects in the active scene
        allGameObjects = UnityEngine.SceneManagement.SceneManager
            .GetActiveScene()
            .GetRootGameObjects()
            .SelectMany(g => g.GetComponentsInChildren<Transform>(true))
            .Select(t => t.gameObject)
            .Distinct()
            .ToList();

        // Reset selection dictionary
        selectionDict.Clear();

        componentTypes = TypeCache.GetTypesDerivedFrom<Component>().Where(t => !t.IsAbstract).ToList();
    }
}