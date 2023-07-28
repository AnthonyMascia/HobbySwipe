using AutoMapper;
using HobbySwipe.Data.Entities.HobbySwipe;
using HobbySwipe.Data.Models.HobbySwipe;

namespace HobbySwipe.Services
{
    public class AutoMappingService : Profile
    {
        public AutoMappingService()
        {
            CreateMap<Answer, AnswerModel>().ReverseMap();

            CreateMap<Hobby, HobbyModel>()
                .ForMember(dest => dest.HobbiesSynonyms,
                           opts => opts.MapFrom(src => src.HobbiesSynonyms))
                .ReverseMap();

            CreateMap<HobbiesSynonym, HobbiesSynonymModel>().ReverseMap();

            CreateMap<Question, QuestionModel>()
                .ForMember(dest => dest.QuestionsOptions,
                           opts => opts.MapFrom(src => src.QuestionsOptions))
                .ReverseMap();

            CreateMap<QuestionsOption, QuestionsOptionModel>().ReverseMap();
        }
    }
}
