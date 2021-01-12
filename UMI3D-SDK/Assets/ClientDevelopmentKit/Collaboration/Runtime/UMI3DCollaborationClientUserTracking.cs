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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.userCapture;
using umi3d.common.collaboration;
using UnityEngine;

namespace umi3d.cdk.collaboration
{
    public class UMI3DCollaborationClientUserTracking : UMI3DClientUserTracking
    {
        ///<inheritdoc/>
        protected override IEnumerator DispatchTracking()
        {
            while (sendTracking)
            {
                if (targetTrackingFPS > 0)
                {
                    BonesIterator();
                    if(UMI3DCollaborationClientServer.Instance.ForgeClient != null && UMI3DCollaborationClientServer.Connected())
                        UMI3DCollaborationClientServer.Instance.ForgeClient.SendTrackingFrame(LastFrameDto);

                    yield return new WaitForSeconds(1f / targetTrackingFPS);
                }
                else
                    yield return new WaitUntil(() => targetTrackingFPS > 0 || !sendTracking);
            }
        }

        ///<inheritdoc/>
        protected override IEnumerator DispatchCamera()
        {
            yield return new WaitUntil(()=> UMI3DCollaborationClientServer.Instance.ForgeClient != null && UMI3DCollaborationClientServer.Connected());

            UMI3DCollaborationClientServer.Instance.ForgeClient.SendBrowserRequest(CameraPropertiesDto, true);
        }
    }
}
