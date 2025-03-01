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
using umi3d.common;

namespace umi3d.edk
{
    /// <summary>
    /// An operation to start interpolatation on a property's entity
    /// </summary>
    public class StartInterpolationProperty : AbstractInterpolationProperty
    {
        /// <summary>
        /// The value with witch to start interpolation
        /// </summary>
        public object startValue;

        public override Bytable ToBytable(UMI3DUser user)
        {
            return UMI3DNetworkingHelper.Write(UMI3DOperationKeys.StartInterpolationProperty)
                + UMI3DNetworkingHelper.Write(entityId)
                + UMI3DNetworkingHelper.Write(property)
                + UMI3DNetworkingHelper.Write((uint)0)
                + UMI3DNetworkingHelper.Write(startValue);

        }

        public override AbstractOperationDto ToOperationDto(UMI3DUser user)
        {
            var startInterpolation = new StartInterpolationPropertyDto
            {
                property = property,
                entityId = entityId,
                startValue = startValue
            };
            return startInterpolation;
        }
    }
}
