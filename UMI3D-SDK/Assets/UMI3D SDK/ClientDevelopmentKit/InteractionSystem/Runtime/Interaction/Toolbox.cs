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
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.menu.interaction;
using umi3d.common.interaction;

namespace umi3d.cdk.interaction
{
    /// <summary>
    /// Client's side interactable object.
    /// </summary>
    /// <see cref="InteractableDto"/>
    public class Toolbox
    {
        public static Dictionary<ulong, Toolbox> instances = new Dictionary<ulong, Toolbox>();

        /// <summary>
        /// Interactable dto describing this object.
        /// </summary>
        public ToolboxDto dto;
        public List<Tool> tools = new List<Tool>();

        public bool Active { get => dto?.Active ?? false; }

        public Toolbox(ToolboxDto dto)
        {
            instances.Add(dto.id, this);
            this.dto = dto;
            UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, this, Destroy);
        }

        public void Destroy()
        {
            instances.Remove(dto.id);
            tools.ForEach(t => UMI3DEnvironmentLoader.DeleteEntity(t.dto.id, null));
        }

    }
}