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
using UnityEngine;
using umi3d.edk;
using umi3d.common;
using umi3d.common.userCapture;
using System;

namespace umi3d.edk.userCapture
{
    public class UMI3DHandAnimation : UMI3DNodeAnimation
    {
        public bool isRight;

        public Vector3 HandLocalPosition;
        public Vector3 HandLocalEulerRotation;

        [Serializable]
        public class PhalanxRotations
        {
            [ConstStringEnum(typeof(BoneType))]
            public string Phalanx;
            public Vector3 PhalanxEulerRotation;
        }

        // set up with gizmos
        public List<PhalanxRotations> Phalanxes;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.02f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(new Vector3(0.03788812f, -0.02166999f, 0.03003085f)), 0.01f);
            Gizmos.DrawSphere(transform.TransformPoint(new Vector3(0.1226662f, -0.002316732f, 0.02822055f)), 0.01f);
            Gizmos.DrawSphere(transform.TransformPoint(new Vector3(0.1277552f, 8.945774e-08f, 1.801164e-07f)), 0.01f);
            Gizmos.DrawSphere(transform.TransformPoint(new Vector3(0.12147f, 9.894557e-05f, -0.02216627f)), 0.01f);
            Gizmos.DrawSphere(transform.TransformPoint(new Vector3(0.1090819f, -0.002263658f, -0.04725829f)), 0.01f);
        }
    }
}
