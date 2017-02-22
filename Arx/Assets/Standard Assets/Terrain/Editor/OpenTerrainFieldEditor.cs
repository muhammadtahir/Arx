﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Extensions;
using Utils;
using CommonEditors;
using Assets.Standard_Assets.Terrain.Builder;

namespace Assets.Standard_Assets.Terrain.Editor
{
    [CustomEditor(typeof(OpenTerrainField))]
    public class OpenTerrainFieldEditor : TerrainFieldEditor<OpenTerrainField>
    {
        private OpenTerrainBuilder _builder;

        public OpenTerrainFieldEditor()
        {
            _builder = new OpenTerrainBuilder();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        protected override void OnNodePathAdded()
        {
        }
        
        protected override void NodePathChanged()
        {
        }

        protected override void OnNodePathRemoved()
        {
        }

        private void OnSceneGUI()
        {
            _builder.BuildMeshFor(TerrainField);
            TerrainColliderBuilder.BuildColliderFor(TerrainField);
            DrawNodePathEditors();
            DrawCollider();
            HandleInput();        

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }  
        }
    }
}
