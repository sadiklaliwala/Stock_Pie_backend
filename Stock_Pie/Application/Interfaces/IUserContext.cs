using System;

namespace Stock_Pie.Application.Interfaces
{
    public interface IUserContext
    {
        Guid UserId { get; }
        string? Email { get; }
    }
}
