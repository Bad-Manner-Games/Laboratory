﻿using System;
using Reactor;
using Reactor.Extensions;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Debugging
{
    [RegisterInIl2Cpp]
    public class DebugWindow : MonoBehaviour
    {
        /// <summary>
        /// Default empty GUILayoutOptions array
        /// </summary>
        public static Il2CppReferenceArray<GUILayoutOption> EmptyOptions { get; } = new(0);

        /// <summary>
        /// Current Instance of the debug window
        /// </summary>
        public static DebugWindow? Instance { get; set; }

        public DebugWindow(IntPtr ptr) : base(ptr)
        {
            PrimaryWindow = new(PrimaryRect, "Debug", BuildPrimaryWindow);
        }

        /// <summary>
        /// If the debug window is enabled
        /// </summary>
        public bool Enabled { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }

        /// <summary>
        /// Index into the tabs array which is currently active
        /// </summary>
        public int SelectedTab { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }

        /// <summary>
        /// Rect of the debug window
        /// </summary>
        private Rect PrimaryRect { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; } = new(20, 20, 100, 100);

        /// <summary>
        /// Draw window used to draw debug components
        /// </summary>
        private DragWindow PrimaryWindow { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) Enabled = !Enabled;
        }

        private void OnGUI()
        {
            if (!Enabled) return;

            PrimaryWindow.OnGUI();

            bool anyActive = false;
            for (int index = 0; index < Debugger.Tabs.Count; index++)
            {
                DebugTab debugTab = Debugger.Tabs[index];
                if (debugTab.Visible != null && !debugTab.Visible()) continue;
                anyActive = SelectedTab == index;
            }

            if (anyActive) Debugger.Tabs[SelectedTab].OnGUI?.Invoke();
        }

        [HideFromIl2Cpp]
        private void BuildPrimaryWindow(int windowId)
        {
            try
            {
                GUILayout.BeginVertical(EmptyOptions);

                bool anyActive = false;
                GUILayout.BeginHorizontal(EmptyOptions);
                for (int index = 0; index < Debugger.Tabs.Count; index++)
                {
                    DebugTab debugTab = Debugger.Tabs[index];
                    if (debugTab.Visible != null && !debugTab.Visible()) continue;
                    if (GUILayout.Toggle(SelectedTab == index, debugTab.TabName, GUI.skin.button, EmptyOptions)) SelectedTab = index;
                    anyActive = true;
                }

                GUILayout.EndHorizontal();

                if (anyActive)
                {
                    CustomGUILayout.Divider();
                    Debugger.Tabs[SelectedTab].BuildUI?.Invoke();
                }

                GUILayout.EndVertical();
                GUI.DragWindow();
            }
            catch (Exception e)
            {
                Logger<LaboratoryPlugin>.Error(e);
            }
        }
    }
}