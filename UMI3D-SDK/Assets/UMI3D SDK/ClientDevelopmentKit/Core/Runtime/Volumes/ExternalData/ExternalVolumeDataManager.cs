/*
Copyright 2019 - 2021 Inetum

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

using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.common;
using umi3d.common.volume;
using UnityEngine;
using UnityEngine.Events;

namespace umi3d.cdk.volumes
{
    /// <summary>
    /// Manager for volumes created with external data (.obj).
    /// </summary>
    public class ExternalVolumeDataManager : Singleton<ExternalVolumeDataManager>
    {
        private static readonly Dictionary<ulong, AbstractVolumeCell> cells = new Dictionary<ulong, AbstractVolumeCell>();

        private class ExternalVolumeEvent : UnityEvent<AbstractVolumeCell> { }
        private static readonly ExternalVolumeEvent onVolumeCreation = new ExternalVolumeEvent();
        private static readonly ExternalVolumeEvent onVolumeDelete = new ExternalVolumeEvent();

        /// <summary>
        /// Subscribe an action to a cell reception.
        /// </summary>
        /// <param name="catchUpWithPreviousCells">If true, the action will be called for each already received cells.</param>
        public static void SubscribeToExternalVolumeCreation(UnityAction<AbstractVolumeCell> callback, bool catchUpWithPreviousCells)
        {
            onVolumeCreation.AddListener(callback);

            if (catchUpWithPreviousCells)
                foreach (AbstractVolumeCell cell in cells.Values)
                    callback(cell);
        }

        /// <summary>
        /// Unsubscribe an action to a cell reception.
        /// </summary>
        public static void UnsubscribeToExternalVolumeCreation(UnityAction<AbstractVolumeCell> callback)
        {
            onVolumeCreation.RemoveListener(callback);
        }

        /// <summary>
        /// Subscribe an action to a cell delete.
        /// </summary>
        public static void SubscribeToExternalVolumeDelete(UnityAction<AbstractVolumeCell> callback)
        {
            onVolumeDelete.AddListener(callback);
        }

        /// <summary>
        /// Unsubscribe an action to a cell reception.
        /// </summary>
        public static void UnsubscribeToExternalVolumeDelete(UnityAction<AbstractVolumeCell> callback)
        {
            onVolumeDelete.RemoveListener(callback);
        }

        public void CreateOBJVolume(OBJVolumeDto dto, UnityAction<AbstractVolumeCell> finished)
        {
            var loader = new ObjMeshDtoLoader();

            Action<object> success = obj =>
            {
                var cell = new OBJVolumeCell()
                {
                    id = dto.id,
                    meshes = (obj as GameObject).GetComponentsInChildren<MeshFilter>().ToList().ConvertAll(filter => filter.mesh),
                    parts = new List<GameObject>() { obj as GameObject }
                };

                Matrix4x4 m = dto.rootNodeToLocalMatrix;
                foreach (Mesh mesh in cell.meshes)
                {
                    mesh.vertices = mesh.vertices.ToList().ConvertAll(v => Vector3.Scale(v, new Vector3(-1, 1, -1))).ToArray(); //asimpl right handed coordinate system dirty fix
                    mesh.vertices = mesh.vertices.ToList().ConvertAll(v => m.MultiplyPoint(v)).ToArray(); //apply dto transform
                }

                cell.isTraversable = dto.isTraversable;
                cells.Add(cell.id, cell);
                onVolumeCreation.Invoke(cell);
                finished.Invoke(cell);
            };

            Action<Umi3dException> failed = e =>
            {
                Debug.LogError("Failed to load obj file : " + e.Message);
            };

            loader.UrlToObject(dto.objFile, ".obj", UMI3DClientServer.getAuthorization(), success, failed);
        }

        public void DeleteOBJVolume(ulong id)
        {
            if (cells.ContainsKey(id))
            {
                var cell = cells[id] as OBJVolumeCell;
                if (cell != null)
                {
                    cell.parts.ForEach(p =>
                    {
                        if (p != null)
                            Destroy(p);
                    });
                }
                cells.Remove(id);
                onVolumeDelete.Invoke(cell);
            }
        }

        public static List<AbstractVolumeCell> GetCells()
        {
            return cells.Values.ToList();
        }
    }
}