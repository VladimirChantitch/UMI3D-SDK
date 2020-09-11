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
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.common.userCapture;
using UnityEngine;

namespace umi3d.cdk
{

    [CreateAssetMenu(fileName = "DefaultLoadingParameters", menuName = "UMI3D/Default Loading Parameters")]
    public class UMI3DLoadingParameters : AbstractUMI3DLoadingParameters
    {
        public virtual UMI3DNodeLoader nodeLoader { get; } = new UMI3DNodeLoader();
        public virtual UMI3DMeshNodeLoader meshLoader { get; } = new UMI3DMeshNodeLoader();
        public virtual UMI3DUINodeLoader UILoader { get; } = new UMI3DUINodeLoader();
        public virtual UMI3DAbstractAnchorLoader AnchorLoader { get; protected set; } = null;
        public virtual UMI3DAvatarNodeLoader avatarLoader { get; } = new UMI3DAvatarNodeLoader();

        public NotificationLoader notificationLoader;

        [SerializeField]
        Material _skyboxMaterial;
        public Material skyboxMaterial { get { if (_skyboxMaterial == null) { _skyboxMaterial = new Material(RenderSettings.skybox); RenderSettings.skybox = _skyboxMaterial; } return _skyboxMaterial; } }

        public List<IResourcesLoader> ResourcesLoaders { get; } = new List<IResourcesLoader>() { new ObjMeshDtoLoader(), new ImageDtoLoader(), new GlTFMeshDtoLoader(), new BundleDtoLoader(), new AudioLoader() };

        /// <summary>
        /// Load an UMI3DObject.
        /// </summary>
        /// <param name="dto">dto.</param>
        /// <param name="node">gameObject on which the abstract node will be loaded.</param>
        /// <param name="finished">Finish callback.</param>
        /// <param name="failed">error callback.</param>
        public override void ReadUMI3DExtension(UMI3DDto dto, GameObject node, Action finished, Action<string> failed)
        {
            Action callback = () => { if (AnchorLoader != null) AnchorLoader.ReadUMI3DExtension(dto, node, finished, failed); else finished.Invoke(); };
            switch (dto)
            {
                case UMI3DAbstractAnimationDto a:
                    UMI3DAnimationLoader.ReadUMI3DExtension(a, node, finished, failed);
                    break;
                case PreloadedSceneDto ps:
                    PreloadedSceneLoader.ReadUMI3DExtension(ps, node, finished, failed);
                    break;
                case InteractableDto i:
                    UMI3DInteractableLoader.ReadUMI3DExtension(i,node,finished,failed);
                    break;
                case ToolboxDto t:
                    UMI3DToolBoxLoader.ReadUMI3DExtension(t, node, finished, failed);
                    break;
                case UMI3DMeshNodeDto m:
                    meshLoader.ReadUMI3DExtension(dto, node, callback, failed);
                    break;
                case UIRectDto r:
                    UILoader.ReadUMI3DExtension(dto, node, callback, failed);
                    break;
                case UMI3DAvatarNodeDto a:
                    avatarLoader.ReadUMI3DExtension(dto, node, callback, failed);
                    break;
                case NotificationDto n:
                    notificationLoader.Load(n);
                    break;
                default:
                    nodeLoader.ReadUMI3DExtension(dto, node, callback, failed);
                    break;
            }
            
        }

        /// <summary>
        /// Update a property.
        /// </summary>
        /// <param name="entity">entity to be updated.</param>
        /// <param name="property">property containing the new value.</param>
        /// <returns></returns>
        public override bool SetUMI3DProperty(UMI3DEntityInstance entity, SetEntityPropertyDto property)
        {
            if (entity == null)
                throw new Exception($"no entity found for {property} [{property.entityId}]");
            if (UMI3DEnvironmentLoader.Exists && UMI3DEnvironmentLoader.Instance.sceneLoader.SetUMI3DProperty(entity, property))
                return true;
            if (UMI3DAnimationLoader.SetUMI3DProperty(entity, property))
                return true;
            if (PreloadedSceneLoader.SetUMI3DProperty(entity, property))
                return true;
            if (UMI3DInteractableLoader.SetUMI3DProperty(entity, property))
                return true;
            if (UMI3DToolLoader.SetUMI3DProperty(entity, property))
                return true;
            if (UMI3DToolBoxLoader.SetUMI3DProperty(entity, property))
                return true;
            if (notificationLoader != null && notificationLoader.SetUMI3DPorperty(entity,property))
                return true;
            if (meshLoader.SetUMI3DProperty(entity, property))
                return true;
            if (UILoader.SetUMI3DProperty(entity, property))
                return true;
            if (avatarLoader.SetUMI3DProperty(entity, property))
                return true;
            if (nodeLoader.SetUMI3DProperty(entity, property))
                return true;
            if (AnchorLoader != null && AnchorLoader.SetUMI3DPorperty(entity, property))
                return true;
            return GlTFNodeLoader.SetUMI3DProperty(entity, property);
        }

        /// <see cref="AbstractUMI3DLoadingParameters.ChooseVariant(AssetLibraryDto)"/>
        public override UMI3DLocalAssetDirectory ChooseVariant(AssetLibraryDto assetLibrary)
        {
            UMI3DLocalAssetDirectory res = null;
            foreach (var assetDir in assetLibrary.variants)
            {
                if ((res == null) || (assetDir.metrics.resolution > res.metrics.resolution))
                {
                    res = assetDir;
                }
            }
            return res;
        }

        /// <see cref="AbstractUMI3DLoadingParameters.ChooseVariante(List{FileDto})"/>
        public override FileDto ChooseVariante(List<FileDto> files)
        {
            FileDto res = null;
            foreach (var file in files)
            {
                if ((res == null) || (file.metrics.resolution > res.metrics.resolution))
                {
                    res = file;
                }
            }
            return res;
        }

        /// <see cref="AbstractUMI3DLoadingParameters.SelectLoader(string)"/>
        public override IResourcesLoader SelectLoader(string extension)
        {
            foreach (IResourcesLoader loader in ResourcesLoaders)
            {
                if (loader.IsSuitableFor(extension))
                    return loader;
                if (loader.IsToBeIgnored(extension))
                    return null;
            }
            Debug.LogError("there is no compatible loader for this extention : " + extension);
            return null;
        }

        /// <see cref="AbstractUMI3DLoadingParameters.loadSkybox(ResourceDto)"/>
        public override void loadSkybox(ResourceDto skybox)
        {
            FileDto fileToLoad = ChooseVariante(skybox.variants);
            if (fileToLoad == null) return;
            string url = fileToLoad.url;
            string ext = fileToLoad.extension;
            string authorization = fileToLoad.authorization;
            IResourcesLoader loader = SelectLoader(ext);
            if (loader != null)
                UMI3DResourcesManager.LoadFile(
                    UMI3DGlobalID.EnvironementId,
                    fileToLoad,
                    loader.UrlToObject,
                    loader.ObjectFromCache,
                    (o) =>
                    {

                        var tex = (Texture2D)o;
                        if (tex != null)
                        {

                            Cubemap cube;
                            Color[] imageColors;

                            //prerequises: 
                            // 1) image is in format
                            //     +y
                            //  -x +z +x -z
                            //     -Y
                            // 2) faces are cubes

                            int size = tex.width / 4;
                            cube = new Cubemap(size, TextureFormat.RGB24, false);

                            //Need to invert y ? Oo
                            var buffer = new Texture2D(tex.width, tex.height);
                            buffer.SetPixels(tex.GetPixels());
                            for (int x = 0; x < tex.width; x++)
                                for (int y = 0; y < tex.height; y++)
                                    tex.SetPixel(x, y, buffer.GetPixel(x, tex.height - 1 - y));

                            imageColors = tex.GetPixels(size, 0, size, size);
                            cube.SetPixels(imageColors, CubemapFace.PositiveY);

                            imageColors = tex.GetPixels(0, size, size, size);
                            cube.SetPixels(imageColors, CubemapFace.NegativeX);

                            imageColors = tex.GetPixels(size, size, size, size);
                            cube.SetPixels(imageColors, CubemapFace.PositiveZ);

                            imageColors = tex.GetPixels(size * 2, size, size, size);
                            cube.SetPixels(imageColors, CubemapFace.PositiveX);

                            imageColors = tex.GetPixels(size * 3, size, size, size);
                            cube.SetPixels(imageColors, CubemapFace.NegativeZ);

                            imageColors = tex.GetPixels(size, size * 2, size, size);
                            cube.SetPixels(imageColors, CubemapFace.NegativeY);

                            cube.Apply();
                            skyboxMaterial.SetTexture("_Tex", cube);
                            RenderSettings.skybox = skyboxMaterial;
                        }
                        else
                            Debug.LogWarning($"invalid cast from {o.GetType()} to {typeof(Texture2D)}");
                    },
                    Debug.LogWarning,
                    loader.DeleteObject
                    );
        }

        /// <see cref="AbstractUMI3DLoadingParameters.UnknownOperationHandler(AbstractOperationDto, Action)"/>
        public override void UnknownOperationHandler(AbstractOperationDto operation, Action performed)
        {
            switch (operation)
            {
                case SwitchToolDto switchTool:
                    AbstractInteractionMapper.Instance.SwitchTools(switchTool.replacedToolId, switchTool.toolId, new interaction.RequestedByEnvironment());
                    performed.Invoke();
                    break;
                case ProjectToolDto projection:
                    AbstractInteractionMapper.Instance.SelectTool(projection.toolId, new interaction.RequestedByEnvironment());
                    performed.Invoke();
                    break;
                case ReleaseToolDto release:
                    AbstractInteractionMapper.Instance.ReleaseTool(release.toolId, new interaction.RequestedByEnvironment());
                    performed.Invoke();
                    break;
            }
        }
    }
}