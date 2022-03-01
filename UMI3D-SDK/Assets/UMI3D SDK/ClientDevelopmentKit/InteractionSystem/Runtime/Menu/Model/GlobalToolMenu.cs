﻿/*
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

using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.common;
using UnityEngine;

namespace umi3d.cdk.menu.interaction
{
    public class GlobalToolMenu : Menu
    {
        public AbstractMenu parent;

        public GlobalTool tool { get; private set; }

        public void Setup(GlobalTool tool)
        {
            this.tool = tool;
            Name = tool.name;
            icon2D = new Texture2D(0, 0);

            if (tool.icon2D != null)
            {
                FileDto icon2DFile = UMI3DEnvironmentLoader.Parameters.ChooseVariant(tool.icon2D.variants);

                if ((icon2DFile != null) && (icon2DFile.url != null) && (icon2DFile.url != ""))
                    UMI3DResourcesManager.GetFile(
                        icon2DFile.url,
                        rawData => this.icon2D.LoadRawTextureData(rawData),
                        e => Debug.LogError(e));
            }

            tool.interactions.ForEach(inter => this.Add(GlobalToolMenuManager.GetMenuForInteraction(inter, tool.id)));

            Subscribe(() =>
            {
                AbstractInteractionMapper.Instance.SelectTool(tool.id, true, this.tool.id, new RequestedFromMenu());
            });
        }
    }
}