using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FallGuysMods
{
    public class SceneDebugger : ModBase
    {
        Int32 HierarchyWindowId;
        Int32 ProjectWindowId { get { return HierarchyWindowId + 1; } }
        Int32 InspectorWindowId { get { return ProjectWindowId + 1; } }
        Int32 Margin = 50;
        Rect HierarchyWindow;
        Int32 HierarchyWidth = 400;
        Vector2 HierarchyScrollPos;
        String SearchText = "";
        Vector2 PropertiesScrollPos;
        Transform SelectedGameObject;
        List<String> ExpandedObjs = new List<String>();

        Rect ProjectWindow;
        Int32 ProjectWidth = 400;
        Vector2 ProjectScrollPos;
        ConcurrentDictionary<object, Boolean> ExpandedObjects = new ConcurrentDictionary<object, Boolean>();

        Rect InspectorWindow;
        Int32 InspectorWidth = 350;

        public override void OnEnable()
        {
            ModName = "Scene Debugger";
            HasConfig = false;
            HierarchyWindowId = GetHashCode();

            HierarchyWindow = new Rect(Screen.width - HierarchyWidth - Margin, Margin, HierarchyWidth, Screen.height - Margin * 2);
            ProjectWindow = new Rect(HierarchyWindow.x - Margin - ProjectWidth, Margin, ProjectWidth, Screen.height - Margin * 2);
            InspectorWindow = new Rect(ProjectWindow.x - Margin - InspectorWidth, Margin, InspectorWidth, Screen.height - Margin * 2);
        }
        public override void OnGUI()
        {
            HierarchyWindow = GUILayout.Window(HierarchyWindowId, HierarchyWindow, (GUI.WindowFunction)HierarchyWindowMethod, "Hierarchy", new GUILayoutOption[0]);
            ProjectWindow = GUILayout.Window(ProjectWindowId, ProjectWindow, (GUI.WindowFunction)ProjectWindowMethod, "Project", new GUILayoutOption[0]);
        }
        #region Hierarchy GUI
        void DisplayGameObject(GameObject gameObj, Int32 level)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            {
                GUILayout.Space(level * 20);
                var color = GUI.color;
                if (SelectedGameObject == gameObj.transform)
                    GUI.color = Color.green;
                if (!gameObj.activeSelf && gameObj.transform.childCount == 0)
                    GUI.color = Color.magenta;
                else if (gameObj.transform.childCount == 0)
                    GUI.color = Color.yellow;
                else if (!gameObj.activeSelf)
                    GUI.color = Color.red;
                if (GUILayout.Toggle(ExpandedObjs.Contains(gameObj.name), gameObj.name, new GUILayoutOption[1] { GUILayout.ExpandWidth(false) }))
                {
                    if (!ExpandedObjs.Contains(gameObj.name))
                    {
                        ExpandedObjs.Add(gameObj.name);
                        SelectedGameObject = gameObj.transform;
                    }
                }
                else
                {
                    if (ExpandedObjs.Contains(gameObj.name))
                    {
                        ExpandedObjs.Remove(gameObj.name);
                        SelectedGameObject = gameObj.transform;
                    }
                }
                GUI.color = color;
            }
            GUILayout.EndHorizontal();
            if (ExpandedObjs.Contains(gameObj.name))
                for (var i = 0; i < gameObj.transform.childCount; ++i)
                    DisplayGameObject(gameObj.transform.GetChild(i).gameObject, level + 1);
        }
        void HierarchyWindowMethod(Int32 id)
        {
            GUILayout.BeginVertical(GUIContent.none, GUI.skin.box, new GUILayoutOption[0]);// { GUI.skin.box });
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                {
                    SearchText = GUILayout.TextField(SearchText, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
                    if (GUILayout.Button("Search", new GUILayoutOption[1] { GUILayout.ExpandWidth(false) }))
                    { }
                }
                GUILayout.EndHorizontal();
                var rootObjects = new List<GameObject>();
                foreach (Transform xform in GameObject.FindObjectsOfType<Transform>())
                    if (xform.parent == null)
                        rootObjects.Add(xform.gameObject);
                //var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                if (SelectedGameObject == null)
                    SelectedGameObject = rootObjects.First().transform;
                HierarchyScrollPos = GUILayout.BeginScrollView(HierarchyScrollPos, new GUILayoutOption[2] { GUILayout.Height(HierarchyWindow.height / 3), GUILayout.ExpandWidth(true) });
                {
                    foreach (var rootObject in rootObjects)
                        DisplayGameObject(rootObject, 0);
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUIContent.none, GUI.skin.box, new GUILayoutOption[0]);//GUI.skin.box);
            {
                PropertiesScrollPos = GUILayout.BeginScrollView(PropertiesScrollPos, new GUILayoutOption[0]);// GUI.skin.box);
                {
                    var fullName = SelectedGameObject.name;
                    var parentTransform = SelectedGameObject.parent;
                    while (parentTransform != null)
                    {
                        fullName = parentTransform.name + "/" + fullName;
                        parentTransform = parentTransform.parent;
                    }
                    GUILayout.Label(fullName, new GUILayoutOption[0]);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    {
                        GUILayout.Label(SelectedGameObject.gameObject.layer + " : " + LayerMask.LayerToName(SelectedGameObject.gameObject.layer), new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        SelectedGameObject.gameObject.SetActive(GUILayout.Toggle(SelectedGameObject.gameObject.activeSelf, "Active", new GUILayoutOption[1] { GUILayout.ExpandWidth(false) }));
                        if (GUILayout.Button("?", new GUILayoutOption[0]))
                            Console.WriteLine("?");
                        if (GUILayout.Button("X", new GUILayoutOption[0]))
                            GameObject.Destroy(SelectedGameObject.gameObject);
                    }
                    GUILayout.EndHorizontal();
                    foreach (var component in SelectedGameObject.GetComponents<Component>())
                    {
                        GUILayout.BeginHorizontal(GUIContent.none, GUI.skin.box, new GUILayoutOption[0]);// GUI.skin.box);
                        {

                            if (component is Behaviour)
                                (component as Behaviour).enabled = GUILayout.Toggle((component as Behaviour).enabled, "", new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });

                            GUILayout.Label(component.GetType().Name + " : " + component.GetType().Namespace, new GUILayoutOption[0]);
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("?", new GUILayoutOption[0]))
                                Console.WriteLine("?");
                            if (!(component is Transform))
                                if (GUILayout.Button("X", new GUILayoutOption[0]))
                                    GameObject.Destroy(component);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
        #endregion
        #region Project GUI
        void ProjectWindowMethod(Int32 id)
        {
            GUILayout.BeginVertical(GUIContent.none, GUI.skin.box, new GUILayoutOption[0]);// GUI.skin.box);
            {
                ProjectScrollPos = GUILayout.BeginScrollView(ProjectScrollPos, new GUILayoutOption[2] { GUILayout.Height(ProjectWindow.height / 3), GUILayout.ExpandWidth(true) });
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        ExpandedObjects[assembly] = GUILayout.Toggle(ExpandedObjects.ContainsKey(assembly) ? ExpandedObjects[assembly] : false, assembly.GetName().Name, new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
                        if (ExpandedObjects[assembly])
                        {
                            var types = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters).ToList();
                            foreach (var type in types)
                            {
                                var staticfields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy).Count(f => f.Name != "OffsetOfInstanceIDInCPlusPlusObject");
                                if (staticfields == 0)
                                    continue;
                                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                                {
                                    var color = GUI.color;
                                    GUILayout.Space(20);
                                    ExpandedObjects[type] = GUILayout.Toggle(ExpandedObjects.ContainsKey(type) ? ExpandedObjects[type] : false, type.Name, new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
                                    GUI.color = color;
                                }
                                GUILayout.EndHorizontal();
                                if (ExpandedObjects[type])
                                {
                                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                                    foreach (var field in fields)
                                    {
                                        if (field.Name == "OffsetOfInstanceIDInCPlusPlusObject") continue;
                                        //var val = field.GetValue(null);
                                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                                        {
                                            GUILayout.Space(40);
                                            ExpandedObjects[field] = GUILayout.Toggle(ExpandedObjects.ContainsKey(field) ? ExpandedObjects[field] : false, field.Name + " : " + field.FieldType, GUI.skin.label, new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                }
                            }
                        }
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
        #endregion
    }
}