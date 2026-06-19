using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Contracts.Auth;
using MediatR;

namespace ConstructFlow.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = _jwtTokenGenerator.GenerateToken(user, out var expiresAt);

        return new LoginResponse
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }
}