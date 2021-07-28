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

using System;
using umi3d.common;
using UnityEngine;

namespace umi3d.cdk
{
    [CreateAssetMenu(fileName = "DefaultNotificationLoader", menuName = "UMI3D/Default Notification Loader")]
    public class NotificationLoader : ScriptableObject
    {
        /// <summary>
        /// Load a notification
        /// </summary>
        /// <param name="dto"></param>
        public virtual void Load(NotificationDto dto)
        {
            UMI3DEnvironmentLoader.RegisterEntityInstance(dto.id, dto, null);
        }

        /// <summary>
        /// Update a property.
        /// </summary>
        /// <param name="entity">entity to be updated.</param>
        /// <param name="property">property containing the new value.</param>
        /// <returns></returns>
        public virtual bool SetUMI3DProperty(UMI3DEntityInstance entity, SetEntityPropertyDto property)
        {
            var dto = entity.dto as NotificationDto;
            if (dto == null) return false;
            switch (property.property)
            {
                case UMI3DPropertyKeys.NotificationTitle:
                    dto.title = (string)property.value;
                    break;
                case UMI3DPropertyKeys.NotificationContent:
                    dto.content = (string)property.value;
                    break;
                case UMI3DPropertyKeys.NotificationDuration:
                    dto.duration = (float)(Double)property.value;
                    break;
                case UMI3DPropertyKeys.NotificationIcon2D:
                    dto.icon2D = (ResourceDto)property.value;
                    break;
                case UMI3DPropertyKeys.NotificationIcon3D:
                    dto.icon3D = (ResourceDto)property.value;
                    break;
                case UMI3DPropertyKeys.NotificationObjectId:
                    var Odto = dto as NotificationOnObjectDto;
                    if (Odto == null) return false;
                    Odto.objectId = (string)property.value;
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}