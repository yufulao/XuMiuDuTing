// ******************************************************************
//       /\ /|       @file       ToolBar
//       \ V/        @brief      工具栏扩展
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2022-08-26 19:50
//    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
// ******************************************************************

using System.IO;
using UnityEditor.SceneManagement;

namespace Rabi
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;

    [InitializeOnLoad]
    public static class ToolBar
    {
        private static readonly Type ToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject _currentToolbar;


        static ToolBar()
        {
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (_currentToolbar != null) return;
            var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            _currentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
            if (_currentToolbar == null) return;
            var root = _currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            if (root is null) return;
            var concreteRoot = root.GetValue(_currentToolbar) as VisualElement;
            var toolbarZone = concreteRoot.Q("ToolbarZoneRightAlign");
            var parent = new VisualElement()
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                }
            };
            var container = new IMGUIContainer();
            container.onGUIHandler += OnGUIHandler;
            parent.Add(container);
            toolbarZone.Add(parent);
        }

        private static void OnGUIHandler()
        {
            //自定义按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("主场景", EditorGUIUtility.FindTexture("PlayButton"))))
            {
                EditorSceneManager.OpenScene("Assets/Scenes/StartUp.unity");
            }

            if (GUILayout.Button(new GUIContent("存档目录", EditorGUIUtility.FindTexture("PlayButton"))))
            {
                OpenDirectory(Application.persistentDataPath);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// // *** 打开文件夹
        /// </summary>
        private static void OpenDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            path = path.Replace("/", "\\");
            if (!Directory.Exists(path))
            {
                Debug.LogError("No Directory: " + path);
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", path);
        }
    }
}