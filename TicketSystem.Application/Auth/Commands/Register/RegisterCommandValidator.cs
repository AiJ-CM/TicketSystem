using FluentValidation;

namespace TicketSystem.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MinimumLength(3).WithMessage("用户名不能少于3个字符")
            .MaximumLength(50).WithMessage("用户名不能超过50个字符")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("用户名只能包含字母、数字和下划线");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(8).WithMessage("密码不能少于8个字符")
            .MaximumLength(100).WithMessage("密码不能超过100个字符")
            .Matches(@"[A-Z]").WithMessage("密码必须包含至少一个大写字母")
            .Matches(@"[a-z]").WithMessage("密码必须包含至少一个小写字母")
            .Matches(@"\d").WithMessage("密码必须包含至少一个数字");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(100).WithMessage("邮箱不能超过100个字符");
    }
}