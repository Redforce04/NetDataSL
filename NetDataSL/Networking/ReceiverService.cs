// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         ReceiverService.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:27 PM
//    Created Date:     02/03/2023 1:27 PM
// -----------------------------------------
namespace NetDataSL.Networking;

using Grpc.Core;
using Status = NetDataService.Status;

/// <summary>
/// The Receiver Service for gRPC.
/// </summary>
public class ReceiverService : NetDataService.NetDataPacketSender.NetDataPacketSenderBase
{
    /// <summary>
    /// The net data packet receiver service.
    /// </summary>
    /// <param name="request">The packet request to receive.</param>
    /// <param name="context">The context that of the server call.</param>
    /// <returns>Returns the status of the request.</returns>
    public virtual Task<Status> NetDataPacketSender(NetDataService.NetDataPacket request, ServerCallContext context)
    {
        Log.Debug($"Status Update from {request.Port}");
        return Task.FromResult(new Status()
        {
            Result = "success",
            Response = 200,
        });
    }
}
