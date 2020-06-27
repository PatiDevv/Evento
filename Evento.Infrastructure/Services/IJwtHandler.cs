using Evento.Infrastructure.DTO;
using System;

namespace Evento.Infrastructure.Services
{
    public interface IJwtHandler
    {
        JwtDto CreateToken(Guid userId, string role);
    }
}
