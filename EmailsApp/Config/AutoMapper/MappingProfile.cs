using AutoMapper;
using EmailsApp.DTOs;
using EmailsApp.Entities;
using EmailsApp.Models;

namespace EmailsApp.Config.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Person, PersonDetailsDto>();
        CreateMap<Email, EmailDto>();
        CreateMap<AddEmailRequest, Email>();

        CreateMap<PersonDetailsDto, Person>()
            .ForMember(p => p.Id, opt => opt.Ignore());

        CreateMap<PersonCreateViewModel, Person>()
            .ForMember(p => p.Emails, opt => opt.MapFrom(vm => new List<Email> { new() { EmailAddress = vm.Email } }));
    }
}