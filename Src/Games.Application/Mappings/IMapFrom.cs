using AutoMapper;

namespace Games.Application.Mappings
{
    public interface IMapFrom<T> where T : class
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
