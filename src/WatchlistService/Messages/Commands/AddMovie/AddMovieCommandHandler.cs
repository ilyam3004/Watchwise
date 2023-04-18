﻿using WatchlistService.Common.Exceptions;
using WatchlistService.Data.Repositories;
using WatchlistService.Dtos.Responses;
using WatchlistService.Bus;
using LanguageExt.Common;
using Shared.Messages;
using MassTransit;
using AutoMapper;
using MediatR;

namespace WatchlistService.Messages.Commands.AddMovie;

public class AddMovieCommandHandler :
    IRequestHandler<AddMovieCommand, Result<WatchlistResponse>>
{
    private readonly IWatchListRepository _watchListRepository;
    private readonly IWatchlistRequestClient _requestClient;
    private readonly IMapper _mapper;

    public AddMovieCommandHandler(
        IWatchListRepository watchListRepository, 
        IRequestClient<MoviesDataMessage> requestClient,
        IWatchlistRequestClient requestClient1, 
        IMapper mapper)
    {
        _watchListRepository = watchListRepository;
        _mapper = mapper;
        _requestClient = requestClient1;
    }
    
    public async Task<Result<WatchlistResponse>> Handle(
        AddMovieCommand command, 
        CancellationToken cancellationToken)
    {
        if (!await _watchListRepository
                .WatchlistExistsByIdAsync(command.WatchlistId))
        {
            var notFoundException = new WatchlistNotFoundException();
            return new Result<WatchlistResponse>(notFoundException);
        }
        
        if (await _watchListRepository
                .MovieExistsInWatchlistAsync(command.WatchlistId, command.MovieId))
        {
            return new Result<WatchlistResponse>(new DuplicateMovieInWatchlistException());
        }
        
        await _watchListRepository.AddMovieToWatchlistAsync(
            command.WatchlistId, command.MovieId);
        
        var updatedWatchlist = await _watchListRepository
            .GetWatchlistByIdAsync(command.WatchlistId);
        
        var moviesData = await _requestClient.
            GetMoviesData(updatedWatchlist.MoviesId);

        return _mapper.Map<WatchlistResponse>((updatedWatchlist, moviesData));
    }
}