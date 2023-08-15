using AutoMapper;
using HobbySwipe.Data.Entities.HobbySwipe;
using HobbySwipe.Data.Models.HobbySwipe;

namespace HobbySwipe.Services
{
    public class AutoMappingService : Profile
    {
        public AutoMappingService()
        {
            /******************************/
            /*** Hobbies and Categories ***/
            /******************************/

            CreateMap<Data.Entities.HobbySwipe.Attribute, AttributeModel>().ReverseMap();

            CreateMap<Category, CategoryModel>()
                .ForMember(dest => dest.CategoriesHobbies,
                           opts => opts.MapFrom(src => src.CategoriesHobbies))
                .ReverseMap();

            CreateMap<CategoriesHobby, CategoriesHobbyModel>()
                .ForMember(dest => dest.Category,
                           opts => opts.MapFrom(src => src.Category))
                .ForMember(dest => dest.CategoriesHobbiesSynonyms,
                           opts => opts.MapFrom(src => src.CategoriesHobbiesSynonyms))
                .ForMember(dest => dest.CategoriesHobbiesAttributes,
                           opts => opts.MapFrom(src => src.CategoriesHobbiesAttributes))
                .ReverseMap();

            CreateMap<CategoriesHobbiesAttribute, CategoriesHobbiesAttributeModel>().ReverseMap();

            CreateMap<CategoriesHobbiesSynonym, CategoriesHobbiesSynonymModel>().ReverseMap();

            CreateMap<Category, CategoryModel>().ReverseMap();



            /*****************************/
            /*** Questions and Answers ***/
            /*****************************/

            CreateMap<Answer, AnswerModel>().ReverseMap();

            CreateMap<Question, QuestionModel>()
                .ForMember(dest => dest.QuestionsOptions,
                           opts => opts.MapFrom(src => src.QuestionsOptions))
                .ReverseMap();

            CreateMap<QuestionsOption, QuestionsOptionModel>().ReverseMap();



            /******************/
            /*** User Input ***/
            /******************/

            CreateMap<Data.Entities.HobbySwipe.Action, ActionModel>().ReverseMap();

            CreateMap<UserHobbyPreference, UserHobbyPreferenceModel>()
                .ForMember(dest => dest.UserHobbyPreferencesHistories,
                           opts => opts.MapFrom(src => src.UserHobbyPreferencesHistories))
                .ReverseMap();

            CreateMap<UserHobbyPreferencesHistory, UserHobbyPreferencesHistoryModel>().ReverseMap();
        }
    }
}
