﻿/*
Copyright 2019 Gfi Informatique

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;
using umi3d.edk.interaction;

namespace umi3d.edk.editor
{
    [CustomEditor(typeof(AbstractTool), true)]
    public class UMI3DAbstractToolEditor : Editor
    {

        AbstractTool t;
        protected SerializedObject _target;
        SerializedProperty interactions;
        SerializedProperty display;
        ListDisplayer<AbstractInteraction> ListDisplayer;

        protected virtual void OnEnable()
        {
            t = (AbstractTool)target;
            _target = new SerializedObject(t);
            display = _target.FindProperty("Display");
            interactions = _target.FindProperty("Interactions");
            ListDisplayer = new ListDisplayer<AbstractInteraction>();
        }

        protected virtual void _OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(display);
            ListDisplayer.Display(ref showList, interactions, t.Interactions, 
                t => {
                    switch (t)
                    {
                        case AbstractInteraction i:
                            return new List<AbstractInteraction>() { i };
                        case GameObject g:
                            return g.GetComponents<AbstractInteraction>().ToList();
                        default:
                            return null;
                    }
                });
        }

        static bool showList = true;

        public override void OnInspectorGUI()
        {
            _target.Update();
            _OnInspectorGUI();
            _target.ApplyModifiedProperties();
        }
    }
}
#endif