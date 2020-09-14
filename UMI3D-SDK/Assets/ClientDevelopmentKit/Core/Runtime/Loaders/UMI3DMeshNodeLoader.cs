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

using System;
using System.Collections.Generic;
using umi3d.common;
using UnityEngine;

namespace umi3d.cdk
{
    /// <summary>
    /// Loader for UMI3D Mesh
    /// </summary>
    public class UMI3DMeshNodeLoader : UMI3DNodeLoader
    {
        public List<string> ignoredPrimitiveNameForSubObjectsLoading = new List<string>() { "Gltf_Primitive" };
        public UMI3DMeshNodeLoader(List<string> ignoredPrimitiveNameForSubObjectsLoading)
        {
            this.ignoredPrimitiveNameForSubObjectsLoading = ignoredPrimitiveNameForSubObjectsLoading;
        }
        public UMI3DMeshNodeLoader() { }


        /// <summary>
        /// Load a mesh node.
        /// </summary>
        /// <param name="dto">dto.</param>
        /// <param name="node">gameObject on which the abstract node will be loaded.</param>
        /// <param name="finished">Finish callback.</param>
        /// <param name="failed">error callback.</param>
        public override void ReadUMI3DExtension(UMI3DDto dto, GameObject node, Action finished, Action<string> failed)
        {
            var nodeDto = dto as UMI3DAbstractNodeDto;
            if (node == null)
            {
                failed.Invoke("dto should be an  UMI3DAbstractNodeDto");
                return;
            }

            base.ReadUMI3DExtension(dto, node, () =>
            {

                //MeshRenderer nodeMesh = node.AddComponent<MeshRenderer>();
                FileDto fileToLoad = UMI3DEnvironmentLoader.Parameters.ChooseVariante(((UMI3DMeshNodeDto)dto).mesh.variants);  // Peut etre ameliore

                string url = fileToLoad.url;
                string ext = fileToLoad.extension;
                string authorization = fileToLoad.authorization;
                string pathIfInBundle = fileToLoad.pathIfInBundle;
                IResourcesLoader loader = UMI3DEnvironmentLoader.Parameters.SelectLoader(ext);
                if (loader != null)
                    UMI3DResourcesManager.LoadFile(
                        nodeDto.id,
                        fileToLoad,
                        loader.UrlToObject,
                        loader.ObjectFromCache,
                        (o) =>
                        {
                            CallbackAfterLoadingForMesh((GameObject)o, (UMI3DMeshNodeDto)dto, node.transform);
                            finished.Invoke();
                        },
                        failed,
                        loader.DeleteObject
                        );
            }, failed);
        }


        /// <summary>
        ///  Set Sub Objects References.
        /// </summary>
        /// <param name="goInCache"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        private GameObject SetSubObjectsReferences(GameObject goInCache, UMI3DMeshNodeDto dto)
        {
            string url = UMI3DEnvironmentLoader.Parameters.ChooseVariante(dto.mesh.variants).url;
            if (!UMI3DResourcesManager.Instance.subModelsCache.ContainsKey(url))
            {
                GameObject copy = GameObject.Instantiate(goInCache, UMI3DResourcesManager.Instance.gameObject.transform);// goInCache.transform.parent);
                Dictionary<string, Transform> subObjectsReferences = new Dictionary<string, Transform>();
                foreach (Transform child in copy.GetComponentsInChildren<Transform>())
                {
                    if (!ignoredPrimitiveNameForSubObjectsLoading.Contains(child.name)) // ignore game objects created by the gltf importer or other importer 
                    {
                        child.SetParent(copy.transform.parent);
                        subObjectsReferences.Add(child.name, child);
                    }
                }
                UMI3DResourcesManager.Instance.subModelsCache.Add(url, subObjectsReferences);
                return copy;
            }
            else
            {
                return UMI3DResourcesManager.Instance.subModelsCache[url][goInCache.name + "(Clone)"].gameObject;
            }
        }

        private void CallbackAfterLoadingForMesh(GameObject go, UMI3DMeshNodeDto dto, Transform parent)
        {
            GameObject root = null;
            if (dto.areSubobjectsTracked)
            {
                root = SetSubObjectsReferences(go, dto);
            }
            else
            {
                root = go;
            }
            var instance = GameObject.Instantiate(root, parent, true);
            AbstractMeshDtoLoader.ShowModelRecursively(instance);
            instance.transform.localPosition = root.transform.localPosition;
            instance.transform.localScale = root.transform.localScale;
            instance.transform.localEulerAngles = root.transform.localEulerAngles;
            UMI3DNodeInstance nodeInstance = UMI3DEnvironmentLoader.GetNode(dto.id);
            ColliderDto colliderDto = ((UMI3DNodeDto)dto).colliderDto;
            SetCollider(nodeInstance, colliderDto);

            if (dto.overridedMaterials != null && dto.overridedMaterials.Count > 0)
            {
                //TODO a améliorer 
                foreach (UMI3DMeshNodeDto.MaterialOverrideDto mat in dto.overridedMaterials)
                {
                    var matEntity = UMI3DEnvironmentLoader.GetEntity(mat.newMaterialId);
                    if (matEntity != null)
                    {
                        if (mat.overridedMaterialsId.Contains("ANY_mat"))
                        {
                            OverrideMaterial(instance, (Material)matEntity.Object, (s) => true);
                        }
                        else
                        {
                            foreach (string matKey in mat.overridedMaterialsId)
                            {
                                OverrideMaterial(instance, (Material)matEntity.Object,
                                    (s) => s.Equals(matKey) || (s.Equals(matKey + " (Instance)")));

                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Material not found : " + mat.newMaterialId);
                    }
                }

            }
        }

        private void OverrideMaterial(GameObject go, Material newMat, Func<string, bool> filter)
        {
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {

                Material[] mats = renderer.sharedMaterials;
                bool modified = false;

                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (filter(renderer.sharedMaterials[i].name))
                    {
                        renderer.sharedMaterials.SetValue(newMat, i);

                        mats[i] = newMat;
                        modified = true;
                    }
                }
                if (modified)
                    renderer.materials = mats;
            }
        }

    }
}