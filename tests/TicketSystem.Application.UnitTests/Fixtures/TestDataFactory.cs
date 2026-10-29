using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;
using TicketSystem.Application.Tickets.Commands.CreateTicket;

namespace TicketSystem.Application.UnitTests.Fixtures;

public static class TestDataFactory
{
    public static readonly User DefaultCreator = new()
    {
        Id = 1,
        UserName = "张三",
        Email = "zhangsan@example.com"
    };

    public static readonly User DefaultAssignee = new()
    {
        Id = 2,
        UserName = "李四",
        Email = "lisi@example.com"
    };

    public static readonly Ticket DefaultTicket = new()
    {
        Id = 100,
        Title = "默认测试工单",
        Description = "这是一段测试描述",
        TicketType = TicketType.Bug,
        Priority = TicketPriority.High,
        CreatorId = DefaultCreator.Id,
        Creator = DefaultCreator,
        AssigneeId = DefaultAssignee.Id,
        Assignee = DefaultAssignee
    };

    public const int NonExistentTicketId = 999;

    public static User CreateUser(int id = 1, string userName = "测试用户", string email = "test@example.com")
    {
        return new User { Id = id, UserName = userName, Email = email };
    }

    public static Ticket CreateTicket(
        int id = 1,
        User? creator = null,
        User? assignee = null,
        string title = "默认工单",
        string description = "测试描述",
        TicketType ticketType = TicketType.Bug,
        TicketPriority priority = TicketPriority.High)
    {
        var c = creator ?? DefaultCreator;
        var a = assignee ?? DefaultAssignee;

        return new Ticket
        {
            Id = id,
            Title = title,
            Description = description,
            TicketType = ticketType,
            Priority = priority,
            CreatorId = c.Id,
            Creator = c,
            AssigneeId = a.Id,
            Assignee = a
        };
    }

    public static CreateTicketCommand CreateCommand(
        string title = "这是一个正常的标题",
        string description = "这是一段正常的描述内容",
        TicketType ticketType = TicketType.Bug,
        TicketPriority priority = TicketPriority.High)
    {
        return new CreateTicketCommand(
            Title: title,
            Description: description,
            TicketType: ticketType,
            Priority: priority
        );
    }
}