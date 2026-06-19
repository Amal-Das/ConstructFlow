using ConstructFlow.Application.Common.Exceptions;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using MediatR;

namespace ConstructFlow.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<int> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new ConstructFlow.Application.Common.Exceptions.ValidationException(
                new[] { new FluentValidation.Results.ValidationFailure("Email", "Email is already registered.") });

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = request.Role,
            IsActive = true
        };

        return await _userRepository.CreateAsync(user);
    }
}