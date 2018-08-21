using AutoMapper;
using JetBrains.Annotations;

namespace CryptoTracker.MarketData.ViewModel.v1
{
    [UsedImplicitly] // AutoMapper searches for any classes extending AutoMapper.Profile
    public class MappingProfileV1 : Profile
    {
        public MappingProfileV1()
        {
            CreateMap<DomainModel.Currency, Currency>();
        }
    }
}