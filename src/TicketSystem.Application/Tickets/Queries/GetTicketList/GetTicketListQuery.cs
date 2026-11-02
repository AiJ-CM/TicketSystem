using MediatR;
using System.Text.Json.Serialization;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Tickets.Dtos;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Queries.GetTicketList;

public class GetTicketListQuery : IRequest<PagedList<TicketDto>>
{
    /// <summary>
    /// 页码，从1开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 状态筛选
    /// </summary>
    public TicketStatus? Status { get; set; }

    /// <summary>
    /// 分配给谁（按用户ID筛选）
    /// </summary>
    public long? AssignedTo { get; set; }

    /// <summary>
    /// 创建人（按用户ID筛选）
    /// </summary>
    public long? CreatedBy { get; set; }
}