using Watchlist = WatchlistService.Models.Watchlist;
using WatchlistService.Data.Repositories;
using WatchlistService.Common.Exceptions;
using WatchlistService.Dtos.Requests;
using WatchlistService.Dtos.Responses;
using TMDbLib.Objects.Movies;
using Shared.Replies;
using Shared.Messages;
using LanguageExt.Common;
using FluentValidation;
using MassTransit;
using MongoDB.Bson;
using AutoMapper;

namespace WatchlistService.Common.Services;

public class WatchlistServiceImp : IWatchlistService
{
    private readonly IValidator<CreateWatchlistRequest> _createWatchlistValidator; 
    private readonly IWatchListRepository _watchListRepository;
    private readonly IRequestClient<DecodeTokenMessage> _decodeTokenRequestClient;
    private readonly IRequestClient<MoviesDataMessage> _movieDataRequestClient;
    private readonly IMapper _mapper;
    
    public WatchlistServiceImp(
        IValidator<CreateWatchlistRequest> createWatchlistValidator,
        IWatchListRepository watchListRepository, 
        IRequestClient<DecodeTokenMessage> decodeTokenRequestClient, 
        IRequestClient<MoviesDataMessage> movieDataRequestClient,
        IMapper mapper)
    {
        _createWatchlistValidator = createWatchlistValidator;
        _watchListRepository = watchListRepository;
        _decodeTokenRequestClient = decodeTokenRequestClient;
        _movieDataRequestClient = movieDataRequestClient;
        _mapper = mapper;
    }

    public async Task<Result<CreateWatchlistResponse>> CreateWatchlist(
        CreateWatchlistRequest request,
        string token)
    {
        var validationResult = await _createWatchlistValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var validationException = new ValidationException(
                validationResult.Errors);

            return new Result<CreateWatchlistResponse>(validationException);
        }

        var userId = await GetUserIdFromToken(token);

        if(await _watchListRepository
                .WatchlistExistsByNameAsync(request.WatchlistName, userId))
        {
            return new Result<CreateWatchlistResponse>(new DuplicateWatchlistException());
        }

        var watchlist = new Watchlist
        {
            Id = ObjectId.GenerateNewId().ToString(),
            UserId = userId,
            Name = request.WatchlistName,
            MoviesId = new List<int>()
        };

        await _watchListRepository.CreateWatchListAsync(watchlist);

        return _mapper.Map<CreateWatchlistResponse>(watchlist);
    }
    
    public async Task RemoveWatchlistAsync(string watchlistId)
    {
        //TODO add check if watchlist exists
        await _watchListRepository.RemoveWatchListAsync(watchlistId);
    }

    public async Task<Result<WatchlistResponse>> AddMovieToWatchlist(
        string watchlistId, int movieId)
    {
        if (!await _watchListRepository
                .WatchlistExistsByIdAsync(watchlistId))
        {
            var notFoundException = new WatchlistNotFoundException();
            return new Result<WatchlistResponse>(notFoundException);
        }
        
        if (await _watchListRepository
                .MovieExistsInWatchlistAsync(watchlistId, movieId))
        {
            return new Result<WatchlistResponse>(new DuplicateMovieInWatchlistException());
        }
        
        await _watchListRepository.AddMovieToWatchlistAsync(
            watchlistId, movieId);
        
        var updatedWatchlist = await _watchListRepository
            .GetWatchlistByIdAsync(watchlistId);
        
        var moviesData = await GetMoviesData(updatedWatchlist.MoviesId);

        return _mapper.Map<WatchlistResponse>((updatedWatchlist, moviesData));
    }
    
    public async Task RemoveMovieFromWatchlistAsync(
        string watchlistId, int movieId)
    {
        //TODO add check if movie exists in watchlist
        await _watchListRepository.RemoveMovieFromWatchlistAsync(
            watchlistId, movieId);
    }

    public async Task<Result<List<Watchlist>>> GetWatchlists(string token)
    {
        string userId = await GetUserIdFromToken(token);

        return await _watchListRepository.GetWatchlistsAsync(userId);
    }

    public async Task<Result<Watchlist>> GetWatchlistByIdAsync(string watchlistId)
    {
        return await _watchListRepository.GetWatchlistByIdAsync(watchlistId);
    }

    private async Task<string> GetUserIdFromToken(string jwt)
    {
        string[] token = jwt.Split();
         
        var response = await _decodeTokenRequestClient
            .GetResponse<DecodeTokenReply>(
                new DecodeTokenMessage
                {
                    Token = token[1]
                });
        
        return response.Message.UserId;
    }
    
    private async Task<List<Movie>> GetMoviesData(List<int> moviesId)
    {
        var response = await _movieDataRequestClient
            .GetResponse<MoviesDataReply>(new MoviesDataMessage
            {
                MoviesId = moviesId
            });

        return response.Message.Movies;
    }
}