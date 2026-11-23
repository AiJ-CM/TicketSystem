using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class Seed_RBAC_Data : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. 插入角色数据
        migrationBuilder.InsertData(
            table: "Roles",
            columns: ["Id", "RoleName", "Description"],
            values: new object[,]
            {
            { 1, "Admin", "超级管理员，拥有系统最高权限" },
            { 2, "Agent", "工单处理人/客服，负责处理和流转工单" },
            { 3, "User", "普通用户，仅能提交和查看自己的工单" }
            });

        // 2. 插入权限数据
        migrationBuilder.InsertData(
            table: "Permissions",
            columns: ["Id", "PermissionCode", "ModuleName", "Description"],
            values: new object[,]
            {
            { 1, "ticket:create", "Ticket", "创建工单" },
            { 2, "ticket:read", "Ticket", "查看工单详情及列表" },
            { 3, "ticket:update", "Ticket", "更新工单状态及提交解决方案" },
            { 4, "ticket:assign", "Ticket", "指派工单给处理人" },
            { 5, "ticket:delete", "Ticket", "删除工单（高危操作）" },
            { 6, "ticket:audit", "Ticket", "查看工单审计记录" },
            { 7, "comment:create", "Comment", "创建工单评论" },
            { 8, "comment:read", "Comment", "查看工单评论" }
            });

        // 3. 插入角色权限关联数据 (隐式中间表)
        migrationBuilder.InsertData(
            table: "RolePermissions",
            columns: ["RoleId", "PermissionId"],
            values: new object[,]
            {
            // Admin (RoleId: 1) 拥有所有权限
            { 1, 1 }, { 1, 2 }, { 1, 3 }, { 1, 4 }, { 1, 5 }, { 1, 6 }, { 1, 7 }, { 1, 8 },
            // Agent (RoleId: 2) 拥有部分权限
            { 2, 2 }, { 2, 3 }, { 2, 4 }, { 2, 6 }, { 2, 7 }, { 2, 8 },
            // User (RoleId: 3) 拥有基础权限
            { 3, 1 }, { 3, 2 }, { 3, 7 }, { 3, 8 }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // 回滚时删除数据（注意先删中间表，再删主表，避免外键约束报错）
        migrationBuilder.DeleteData(table: "RolePermissions", keyColumn: "RoleId", keyValues: [1, 2, 3]);
        migrationBuilder.DeleteData(table: "Permissions", keyColumn: "Id", keyValues: [1, 2, 3, 4, 5, 6, 7, 8]);
        migrationBuilder.DeleteData(table: "Roles", keyColumn: "Id", keyValues: [1, 2, 3]);
    }
}
