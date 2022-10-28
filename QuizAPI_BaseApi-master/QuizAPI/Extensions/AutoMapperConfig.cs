using AutoMapper;

namespace QuizAPI.Extensions
{
    public static class AutoMapperConfig<Tsource, TDestination>
    {
        private static readonly Mapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Tsource, TDestination>().ReverseMap()));

        public static TDestination Map(Tsource source)
        {
            return mapper.Map<TDestination>(source);
        }
    }
}
