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

using inetum.unityUtils;
using umi3d.common;
using umi3d.common.volume;
using UnityEngine;

namespace umi3d.edk.volume
{
    public class Sphere : AbstractPrimitive
    {
        #region Fields

        [SerializeField]
        private float Radius = 1;

        [SerializeField]
        private Vector3 LocalCenterOffset = Vector3.zero;

        [SerializeField]
        public UMI3DAsyncProperty<float> radius;

        public UMI3DAsyncProperty<Vector3> localCenterOffset;

        #endregion

        #region Methods

        protected override void Awake()
        {
            base.Awake();

            radius = new UMI3DAsyncProperty<float>(Id(), UMI3DPropertyKeys.VolumePrimitive_Sphere_Radius, Radius);
            localCenterOffset = new UMI3DAsyncProperty<Vector3>(Id(), UMI3DPropertyKeys.VolumePrimitive_Sphere_Offset, LocalCenterOffset);

            radius.OnValueChanged += r => Radius = r;
            localCenterOffset.OnValueChanged += offset => LocalCenterOffset = offset;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IEntity ToEntityDto(UMI3DUser user)
        {
            return new SphereDto()
            {
                id = Id(),
                radius = radius.GetValue(user),
                localCenterOffset = localCenterOffset.GetValue(user),
                rootNodeId = GetRootNode().Id(),
                isTraversable = IsTraversable()
            };
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.TransformPoint(LocalCenterOffset), Radius);
        }

        #endregion
    }
}