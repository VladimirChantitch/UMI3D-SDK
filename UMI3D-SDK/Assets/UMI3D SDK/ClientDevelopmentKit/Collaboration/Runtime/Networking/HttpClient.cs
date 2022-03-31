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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.collaboration;
using UnityEngine.Networking;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// Class to send Http Request.
    /// </summary>
    public class HttpClient
    {
        private const DebugScope scope = DebugScope.CDK | DebugScope.Collaboration | DebugScope.Networking;

        internal string HeaderToken;

        private string httpUrl => environmentClient.connectionDto.httpUrl;

        private readonly ThreadDeserializer deserializer;

        UMI3DEnvironmentClient environmentClient;

        /// <summary>
        /// Init HttpClient.
        /// </summary>
        /// <param name="UMI3DClientServer"></param>
        public HttpClient(UMI3DEnvironmentClient environmentClient)
        {
            this.environmentClient = environmentClient;
            UMI3DLogger.Log($"Init HttpClient", scope | DebugScope.Connection);
            deserializer = new ThreadDeserializer();
        }

        public void Stop()
        {
            deserializer?.Stop();
        }

        /// <summary>
        /// Renew token.
        /// </summary>
        /// <param name="token"></param>
        public void SetToken(string token)
        {
            UMI3DLogger.Log($"SetToken {token}", scope | DebugScope.Connection);
            HeaderToken = UMI3DNetworkingKeys.bearer + token;
        }

        private static bool DefaultShouldTryAgain(RequestFailedArgument argument)
        {
            return argument.count < 3;
        }

        #region user

        /// <summary>
        /// Connect to a media
        /// </summary>
        /// <param name="connectionDto"></param>
        public static async Task<UMI3DDto> Connect(UMI3DDto connectionDto, string MasterUrl, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            var uwr = await _PostRequest(null, MasterUrl + UMI3DNetworkingKeys.connect, connectionDto.ToBson(), onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), false);
            UMI3DLogger.Log($"Received answer to Connect", scope | DebugScope.Connection);
            var dto = uwr?.downloadHandler.data != null ? UMI3DDto.FromBson(uwr?.downloadHandler.data) : null;
            return dto;
        }

        /// <summary>
        /// Send request using GET method to get the user Identity.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<UserConnectionDto> SendGetIdentity(Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send Get Identity", scope | DebugScope.Connection);
            var uwr = await _GetRequest(HeaderToken, httpUrl + UMI3DNetworkingKeys.connectionInfo, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received Get Identity", scope | DebugScope.Connection);
            var dto = await deserializer.FromBson(uwr?.downloadHandler.data);
            return dto as UserConnectionDto;
        }


        /// <summary>
        /// Send request using POST method to update user Identity.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async void SendPostUpdateIdentityAsync(UserConnectionAnswerDto answer, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            await SendPostUpdateIdentity(answer, onError, shouldTryAgain);
        }

        /// <summary>
        /// Send request using POST method to update user Identity.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task SendPostUpdateIdentity(UserConnectionAnswerDto answer, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostUpdateIdentity", scope | DebugScope.Connection);
            await _PostRequest(HeaderToken, httpUrl + UMI3DNetworkingKeys.connection_information_update, answer.ToBson(), onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received PostUpdateIdentity", scope | DebugScope.Connection);
        }



        /// <summary>
        /// Send request using POST method to update user Identity.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async void SendPostUpdateStatusAsync(StatusType status, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            await SendPostUpdateStatus(status, onError, shouldTryAgain);
        }

        /// <summary>
        /// Send request using POST method to update user Identity.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task SendPostUpdateStatus(StatusType status, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostUpdateStatus", scope | DebugScope.Connection);
            await _PostRequest(HeaderToken, httpUrl + UMI3DNetworkingKeys.status_update, new StatusDto() { status = status }.ToBson(), onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received PostUpdateStatus", scope | DebugScope.Connection);
        }

        /// <summary>
        /// Send request using POST method to logout of the server.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task SendPostLogout(Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostLogout", scope | DebugScope.Connection);
            await _PostRequest(HeaderToken, httpUrl + UMI3DNetworkingKeys.logout, new UMI3DDto().ToBson(), onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received PostLogout", scope | DebugScope.Connection);
        }
        #endregion

        #region media
        /// <summary>
        /// Send request using GET method to get the server Media.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<MediaDto> SendGetMedia(Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain)
        {
            return await SendGetMedia(httpUrl + UMI3DNetworkingKeys.media, onError, shouldTryAgain);
        }

        /// <summary>
        /// Send request using GET method to get a Media at a specified url.
        /// </summary>
        /// <param name="url">Url to send the resquest to. For a vanilla server add '/media' at the end of the server url.</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public static async Task<MediaDto> SendGetMedia(string url, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetMedia", scope | DebugScope.Connection);
            var uwr = await _GetRequest(null, url, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e));
            UMI3DLogger.Log($"Received GetMedia", scope | DebugScope.Connection);
            var dto = uwr?.downloadHandler.data != null ? UMI3DDto.FromBson(uwr?.downloadHandler.data) : null;
            return dto as MediaDto;
        }

        #endregion

        #region resources

        /// <summary>
        /// Send request using GET.
        /// </summary>
        /// <param name="url">Url.</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<LibrariesDto> SendGetLibraries(Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetLibraries", scope | DebugScope.Connection);
            var uwr = await _GetRequest(HeaderToken, httpUrl + UMI3DNetworkingKeys.libraries, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received GetLibraries", scope | DebugScope.Connection);
            var dto = await deserializer.FromBson(uwr?.downloadHandler.data);
            return dto as LibrariesDto;
        }

        /// <summary>
        /// Get a LoadEntityDto
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="onError"></param>
        /// <param name="shouldTryAgain"></param>
        public async Task<LoadEntityDto> SendPostEntity(EntityRequestDto id, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostEntity", scope | DebugScope.Connection);
            var uwr = await _PostRequest(HeaderToken, httpUrl + UMI3DNetworkingKeys.entity, id.ToBson(), onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received PostEntity", scope | DebugScope.Connection);
            var dto = await deserializer.FromBson(uwr?.downloadHandler.data);
            return dto as LoadEntityDto;
        }

        /// <summary>
        /// Send request using GET
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<byte[]> SendGetPublic(string url, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetPublic {url}", scope | DebugScope.Connection);
            var uwr = await _GetRequest(HeaderToken, url, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), false);
            UMI3DLogger.Log($"received getPublic {url}", scope | DebugScope.Connection);
            return uwr?.downloadHandler.data;
        }

        /// <summary>
        /// Send request using GET method to get the a private file.
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<byte[]> SendGetPrivate(string url, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetPrivate {url}", scope | DebugScope.Connection);
            var uwr = await _GetRequest(HeaderToken, url, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received GetPrivate {url}", scope | DebugScope.Connection);
            return uwr?.downloadHandler.data;
        }
        #endregion

        #region environement
        /// <summary>
        /// Send request using GET method to get the Environement.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<GlTFEnvironmentDto> SendGetEnvironment(Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetEnvironment", scope | DebugScope.Connection);
            var uwr = await _GetRequest(HeaderToken, httpUrl + UMI3DNetworkingKeys.environment, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received GetEnvironment", scope | DebugScope.Connection);
            var dto = await deserializer.FromBson(uwr?.downloadHandler.data);
            return dto as GlTFEnvironmentDto;
        }

        /// <summary>
        /// Send request using POST method to Join server.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<EnterDto> SendPostJoin(JoinDto join, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostJoin", scope | DebugScope.Connection);
            var uwr = await _PostRequest(HeaderToken, httpUrl + UMI3DNetworkingKeys.join, join.ToBson(), onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received PostJoin", scope | DebugScope.Connection);
            var dto = await deserializer.FromBson(uwr?.downloadHandler.data);
            return dto as EnterDto;
        }

        /// <summary>
        /// Send request using POST method to request the server to send a Scene.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task SendPostSceneRequest(Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostSceneRequest", scope | DebugScope.Connection);
            await _PostRequest(HeaderToken, httpUrl + UMI3DNetworkingKeys.scene, null, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received PostSceneRequest", scope | DebugScope.Connection);
        }

        #endregion

        #region Local Info
        /// <summary>
        /// Send request using POST method to send to the server Local Info.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        /// <param name="key">Local data file key.</param>
        public async Task SendPostLocalInfo(Action<string> onError, string key, byte[] bytes, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostLocalInfo {key}", scope | DebugScope.Connection);
            string url = System.Text.RegularExpressions.Regex.Replace(httpUrl + UMI3DNetworkingKeys.localData, ":param", key);
            await _PostRequest(HeaderToken, url, bytes, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received PostLocalInfo {key}", scope | DebugScope.Connection);
        }

        /// <summary>
        /// Send request using GET method to get datas from server then save its in local file.
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<byte[]> SendGetLocalInfo(string key, Action<string> onError, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetLocalInfo {key}", scope | DebugScope.Connection);
            string url = System.Text.RegularExpressions.Regex.Replace(httpUrl + UMI3DNetworkingKeys.localData, ":param", key);
            var uwr = await _GetRequest(HeaderToken, url, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            UMI3DLogger.Log($"Received GetLocalInfo {key}", scope | DebugScope.Connection);
            return uwr?.downloadHandler.data;
        }

        #endregion

        #region upload
        /// <summary>
        /// Send request using POST method to send file to the server.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        /// <param name="token">Authorization token, given by the server.</param>
        /// <param name="fileName">Name of the uploaded file.</param>
        /// <param name="bytes">the file in bytes.</param>
        /// <param name="shouldTryAgain"></param>
        public async Task SendPostFile(Action<string> onError, string token, string fileName, byte[] bytes, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            string url = System.Text.RegularExpressions.Regex.Replace(httpUrl + UMI3DNetworkingKeys.uploadFile, ":param", token);
            var headers = new List<(string, string)>
            {
                (UMI3DNetworkingKeys.contentHeader, fileName)
            };
            await _PostRequest(HeaderToken, url, bytes, onError, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true, headers);
        }
        #endregion

        #region utils
        /// <summary>
        /// Ienumerator to send GET request.
        /// </summary>
        /// <param name="url">Url to send the request at.</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        /// <returns></returns>
        private static async Task<UnityWebRequest> _GetRequest(string HeaderToken, string url, Action<string> onError, Func<RequestFailedArgument, bool> ShouldTryAgain, bool UseCredential = false, List<(string, string)> headers = null, int tryCount = 0)
        {
            var www = UnityWebRequest.Get(url);
            if (UseCredential) www.SetRequestHeader(UMI3DNetworkingKeys.Authorization, HeaderToken);
            if (headers != null)
            {
                foreach ((string, string) item in headers)
                {
                    www.SetRequestHeader(item.Item1, item.Item2);
                }
            }
            DateTime date = DateTime.UtcNow;
            var operation = www.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (www.isNetworkError || www.isHttpError)
            {

                if (UMI3DClientServer.Exists && await UMI3DClientServer.Instance.TryAgainOnHttpFail(new RequestFailedArgument(www, tryCount, date, ShouldTryAgain)))
                    return await _GetRequest(HeaderToken, url, onError, ShouldTryAgain, UseCredential, headers, tryCount);
                else
                {
                    if (onError != null)
                    {
                        onError.Invoke(www.error + " Failed to post " + www.url);
                    }
                    else
                    {
                        UMI3DLogger.LogError(www.error, scope);
                        UMI3DLogger.LogError("Failed to post " + www.url, scope);
                    }
                    return null;
                }
            }
            return www;
        }

        /// <summary>
        /// Ienumerator to send POST Request.
        /// </summary>
        /// <param name="url">Url to send the request at.</param>
        /// <param name="bytes">Data send via post method.</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        /// <returns></returns>
        private static async Task<UnityWebRequest> _PostRequest(string HeaderToken, string url, byte[] bytes, Action<string> onError, Func<RequestFailedArgument, bool> ShouldTryAgain, bool UseCredential = false, List<(string, string)> headers = null, int tryCount = 0)
        {
            UnityWebRequest www = CreatePostRequest(url, bytes, true);
            if (UseCredential) www.SetRequestHeader(UMI3DNetworkingKeys.Authorization, HeaderToken);
            if (headers != null)
            {
                foreach ((string, string) item in headers)
                {
                    www.SetRequestHeader(item.Item1, item.Item2);
                }
            }
            DateTime date = DateTime.UtcNow;

            var operation = www.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (www.isNetworkError || www.isHttpError)
            {

                if (UMI3DClientServer.Exists && await UMI3DClientServer.Instance.TryAgainOnHttpFail(new RequestFailedArgument(www, tryCount, date, ShouldTryAgain)))
                    return await _PostRequest(HeaderToken, url, bytes, onError, ShouldTryAgain, UseCredential, headers, tryCount);
                else
                {
                    if (onError != null)
                    {
                        onError.Invoke(www.error + " Failed to post " + www.url);
                    }
                    else
                    {
                        UMI3DLogger.LogError(www.error, scope);
                        UMI3DLogger.LogError("Failed to post " + www.url, scope);
                    }
                    return null;
                }
            }
            return www;
        }

        /// <summary>
        /// Util function to create POST request.
        /// </summary>
        /// <param name="url">Url to send the request at.</param>
        /// <param name="bytes">Data send via post method.</param>
        /// <param name="withResult">require a result</param>
        /// <returns></returns>
        private static UnityWebRequest CreatePostRequest(string url, byte[] bytes, bool withResult = false)
        {
            var requestU = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            var uH = new UploadHandlerRaw(bytes);
            requestU.uploadHandler = uH;
            if (withResult)
                requestU.downloadHandler = new DownloadHandlerBuffer();
            //requestU.SetRequestHeader("access_token", UMI3DClientServer.GetToken(null));
            return requestU;
        }
        #endregion
    }
}