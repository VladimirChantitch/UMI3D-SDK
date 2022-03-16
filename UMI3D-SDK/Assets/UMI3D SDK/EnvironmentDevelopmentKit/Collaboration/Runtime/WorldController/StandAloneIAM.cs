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
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.ms;
using UnityEngine;

public class StandAloneIAM : IIAM
{
    protected readonly IEnvironment environment;
    public StandAloneIAM(IEnvironment environment) { this.environment = environment; }

    public async virtual Task<ConnectionFormDto> GenerateForm(User user)
    {
        return await Task.FromResult<ConnectionFormDto>(null);
    }

    public async virtual Task<IEnvironment> GetEnvironment(User user)
    {
        return await Task.FromResult(environment);
    }

    public async virtual Task<List<LibrariesDto>> GetLibraries(User user)
    {
        return await Task.FromResult<List<LibrariesDto>>(null);
    }

    public async virtual Task<bool> isFormValid(User user, FormAnswerDto formAnswer)
    {
        if (user.Token == null)
            user.Set(new System.Guid().ToString());
        return await Task.FromResult(true);
    }

    public async virtual Task<bool> IsUserValid(User user)
    {
        if (user.Token == null)
            user.Set(new System.Guid().ToString());
        return await Task.FromResult(true);
    }

    public async virtual Task RenewCredential(User user)
    {
        if (user.Token == null)
            user.Set(new System.Guid().ToString());
        await Task.CompletedTask;
    }
}