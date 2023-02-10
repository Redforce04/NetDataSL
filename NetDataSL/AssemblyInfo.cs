// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         AssemblyInfo.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/05/2023 6:49 PM
//    Created Date:     02/05/2023 6:49 PM
// -----------------------------------------
#pragma warning disable
namespace NetDataSL;

public static class AssemblyInfo
{
    public const string CommitHash = "${CI_COMMIT_SHORT_SHA}";
    public const string CommitBranch = "${CI_COMMIT_BRANCH}";
}
#pragma warning restore
