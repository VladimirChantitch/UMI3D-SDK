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

namespace umi3d.cdk.collaboration
{
    public interface IWebRTCClient : IAbstractWebRtcClient
    {
        bool ExistServer(bool reliable, DataType dataType, out List<DataChannel> dataChannels);
        void sendAudio(AudioDto dto);
        void sendAudio(List<DataChannel> channels, AudioDto dto);
        void SendServer(UMI3DDto dto, bool reliable);
    }
}