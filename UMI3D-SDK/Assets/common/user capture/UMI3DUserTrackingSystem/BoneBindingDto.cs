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

namespace umi3d.common.userCapture
{
    /// <summary>
    /// Class to associate a bone to its representation.
    /// </summary>
    [Serializable]
    public class BoneBindingDto : UMI3DDto
    {
        /// <summary>
        /// An identifier defined by the designer.
        /// </summary>
        public string bindingId;

        /// <summary>
        /// Optional rig name. If null, the whole object is binded to the bone.
        /// </summary>
        public string rigName;

        /// <summary>
        /// Define if the binding is currently active or overrided by the media.
        /// </summary>
        public bool active;

        /// <summary>
        /// The binded BoneType.
        /// </summary>
        public string boneType;

        /// <summary>
        /// The identifier of the 3D object.
        /// </summary>
        public string objectId;

        public SerializableVector3 position;

        public SerializableVector4 rotation;

    }
}