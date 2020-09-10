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

using System.Collections.Generic;
using umi3d.common;
using umi3d.common.collaboration;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.edk.collaboration
{
    public abstract class IdentifierApi : ScriptableObject
    {
        public abstract WebSocketSharp.Net.NetworkCredential GetPasswordFor(string login);

        Dictionary<string, bool> librariesUpdateStatus;

        public virtual StatusType UpdateIdentity(UMI3DCollaborationUser user, UserConnectionDto identity)
        {
            if (librariesUpdateStatus == null) librariesUpdateStatus = new Dictionary<string, bool>();
            librariesUpdateStatus[user.login] = identity.librariesUpdated;
            return librariesUpdateStatus[user.login] ? ((identity.status > StatusType.READY) ? identity.status : StatusType.READY) : StatusType.CREATED;
        }

        public virtual FormDto GetParameterDtosFor(string login)
        {
            return null;
        }

        public virtual bool getLibrariesUpdateSatus(string login)
        {
            if (librariesUpdateStatus == null) librariesUpdateStatus = new Dictionary<string, bool>();
            return librariesUpdateStatus.ContainsKey(login) ? librariesUpdateStatus[login] : false;
        }

    }
}