﻿using AutoMapper;
using FluentValidation;
using LanguageExt.Common;
using UserService.Common.Authentication;
using UserService.Common.Exceptions;
using UserService.Data;
using UserService.Dtos.Requests;
using UserService.Dtos.Responses;
using UserService.Models;

namespace UserService.Common.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<RegisterRequest> _validator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public AccountService(
        IUserRepository userRepository,
        IMapper mapper, 
        IValidator<RegisterRequest> validator, 
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _validator = validator;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<UserResponse>> Register(RegisterRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var validationException = new ValidationException(validationResult.Errors);
            return new Result<UserResponse>(validationException);
        }
        
        if(_userRepository.UserExists(request.Login))
        {
            var exception = new DuplicateEmailException($"User with login {request.Login} already exists");
            return new Result<UserResponse>(exception);
        }

        string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = _mapper.Map<User>((request, hash));
        await _userRepository.AddUser(user);

        var token = _jwtTokenGenerator.GenerateToken(user.UserId, user.Login);
        
        return new Result<UserResponse>(
            _mapper.Map<UserResponse>((user, token)));
    }

    public async Task<Result<UserResponse>> Login(LoginRequest request)
    {
        if (!_userRepository.UserExists(request.Login))
        {
            var exception = new InvalidCredentialsException($"Invalid login or password");
            return new Result<UserResponse>(exception);
        }
        
        var user = await _userRepository.GetUserByLogin(request.Login);
        
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            var exception = new InvalidCredentialsException($"Invalid login or password");
            return new Result<UserResponse>(exception);
        }
        
        var token = _jwtTokenGenerator.GenerateToken(user.UserId, user.Login);
        
        return new Result<UserResponse>(
            _mapper.Map<UserResponse>((user, token)));
    }
}