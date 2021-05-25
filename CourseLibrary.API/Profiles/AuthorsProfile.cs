using System;
using AutoMapper;
using CourseLibrary.API.Helpers;

namespace CourseLibrary.API.Profiles
{
	public class AuthorsProfile : Profile
	{
		public AuthorsProfile()
		{
			//create a mapping between the dto and entity
			CreateMap<Entities.Author, Models.AuthorDto>()
				.ForMember(
					dest => dest.Name,
					opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
				.ForMember(
					dest => dest.Age,
					opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));
		}
	}
}