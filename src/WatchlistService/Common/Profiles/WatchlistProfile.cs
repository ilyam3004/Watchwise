﻿using AutoMapper;
using TMDbLib.Objects.Movies;
using WatchlistService.Models;
using WatchlistService.Dtos.Responses;

namespace WatchlistService.Common.Profiles;

public class WatchlistProfile : Profile
{
    public WatchlistProfile()
    {
        CreateMap<Watchlist, CreateWatchlistResponse>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt =>
                opt.MapFrom(src => src.Name));

        CreateMap<(Watchlist, List<Movie>), WatchlistResponse>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Item1.Id))
            .ForMember(dest => dest.Name, opt =>
                opt.MapFrom(src => src.Item1.Name))
            .ForMember(dest => dest.Movies, opt =>
                opt.MapFrom(src => src.Item2))
            .ForMember(dest => dest.MoviesCount, opt =>
                opt.MapFrom(src => src.Item2.Count));
    }
}