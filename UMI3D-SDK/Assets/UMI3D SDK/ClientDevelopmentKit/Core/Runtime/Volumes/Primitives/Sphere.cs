/*
Copyright 2019 - 2022 Inetum

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
using umi3d.common.volume;
using UnityEngine;

namespace umi3d.cdk.volumes
{
    /// <summary>
    /// Box shaped primitive.
    /// </summary>
    public class Sphere : AbstractPrimitive
    {
        public Vector3 localCenterOffset = Vector3.zero;

        public float radius = 0;

        /// <inheritdoc/>
        public override void Delete() { }

        /// <inheritdoc/>
        public override void GetBase(System.Action<Mesh> onsuccess, float angleLimit)
        {
            Debug.LogError("Not implemented.");
        }

        /// <inheritdoc/>
        public override Mesh GetMesh()
        {
            return null;
        }

        /// <inheritdoc/>
        public override bool IsInside(Vector3 point, Space relativeTo)
        {
            if (relativeTo == Space.Self)
                return Vector3.Distance(localCenterOffset, point) <= radius;
            else
            {
                Vector3 center = rootNode?.TransformPoint(point) ?? Vector3.zero;
                return Vector3.Distance(center, point) <= radius;
            }
        }

        public void SetLocalCenterOffset(Vector3 offset)
        {
            localCenterOffset = offset;
            onUpdate.Invoke();
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
            onUpdate.Invoke();
        }
    }
}