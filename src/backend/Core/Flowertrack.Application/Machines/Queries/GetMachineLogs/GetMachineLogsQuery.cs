using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Queries.GetMachineLogs;

public sealed record GetMachineLogsQuery(Guid MachineId) : IRequest<Result<List<MachineLogDto>>>;
