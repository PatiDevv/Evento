using AutoMapper;
using Evento.Core.Domain;
using Evento.Core.Repositories;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Extensions;
using NLog;
using System;
using System.Threading.Tasks;

namespace Evento.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IUserRepository _userRepository;
        private readonly IJwtHandler _jwtHandler;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IJwtHandler jwthandler, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtHandler = jwthandler;
            _mapper = mapper;
        }

        public async Task<AccountDto> GetAccountAsync(Guid userId)
        {
            var user = await _userRepository.GetOrFailAsync(userId);
            return _mapper.Map<AccountDto>(user);
        }

        public async Task RegisterAsync(Guid userId, string email, string name, string password, string role = "user")
        {
            var user = await _userRepository.GetAsync(email);
            if (user != null)
            {
                throw new Exception($"User with email: {email} already exist. ");
            }
            user = new User(userId, role, name, email, password);
            await _userRepository.AddAsync(user);
            Logger.Trace($"Zarejestrował sie nowy uzytkownik. Email: {email}, Nazwa: {name}");
        }
        public async Task<TokenDto> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetAsync(email);
            if (user == null)
            {
                throw new Exception("Invalid credentials.");
            }
            if(user.Password != password)
            {
                throw new Exception("Invalid credentials.");
            }

            var jwt = _jwtHandler.CreateToken(user.Id, user.Role);

            Logger.Trace($"Zalogował sie uzytkownik. Email: {email}");
            return new TokenDto
            {
                Token = jwt.Token,
                Expires = jwt.Expires,
                Role = user.Role
            };
        }
    }
}
