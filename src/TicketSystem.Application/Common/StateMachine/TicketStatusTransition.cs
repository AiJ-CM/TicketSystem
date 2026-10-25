using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Common.StateMachine;

public static class TicketStatusTransition
{
    private static readonly Dictionary<TicketStatus, HashSet<TicketStatus>> Rules = new()
    {
        [TicketStatus.Open] =
        [
            TicketStatus.InProgress,
            TicketStatus.Resolved,
            TicketStatus.Closed,
            TicketStatus.PendingUser
        ],
        [TicketStatus.InProgress] = 
        [
            TicketStatus.Resolved,
            TicketStatus.PendingUser,
            TicketStatus.Closed
        ],
        [TicketStatus.PendingUser] = 
        [
            TicketStatus.InProgress,
            TicketStatus.Closed
        ],
        [TicketStatus.Resolved] =
        [
            TicketStatus.Reopened,
            TicketStatus.Closed
        ],
        [TicketStatus.Reopened] = 
        [
            TicketStatus.InProgress,
            TicketStatus.Resolved
        ],
        [TicketStatus.Closed] = []
    };

    /// <summary>
    /// 仅判断是否允许流转，不抛异常，适合前端按钮是否置灰
    /// </summary>
    public static bool CanTransition(TicketStatus from, TicketStatus to)
    {
        if (from == to) return false;
        return Rules.TryGetValue(from, out var targets) && targets.Contains(to);
    }

    /// <summary>
    /// 校验状态流转是否合法，并返回具体的结果和提示信息
    /// </summary>
    public static (bool IsAllowed, ErrorCodeEnum ErrorCode, string Message) TryTransition(TicketStatus from, TicketStatus to)
    {
        if (from == to)
            return (false, ErrorCodeEnum.StatusNotChanged, $"工单当前已经是 {from} 状态，无需重复变更");

        if (!CanTransition(from, to))
            return (false, ErrorCodeEnum.InvalidStatusTransition, $"非法状态流转：不允许从 {from} 变更为 {to}");

        return (true, ErrorCodeEnum.Unknown, string.Empty);
    }

    /// <summary>
    /// 获取某个状态下所有可流转的目标状态，常用于给前端返回操作按钮
    /// </summary>
    public static IReadOnlyCollection<TicketStatus> GetNextStatuses(TicketStatus from)
    {
        return Rules.TryGetValue(from, out var targets)
            ? targets.ToList()
            : Array.Empty<TicketStatus>();
    }
}