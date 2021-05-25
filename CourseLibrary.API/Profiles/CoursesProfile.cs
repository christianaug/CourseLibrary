using System;
using AutoMapper;
using CourseLibrary.API.Helpers;

namespace CourseLibrary.API.Profiles
{
	public class CoursesProfile : Profile
	{
		public CoursesProfile()
		{
			//create a mapping between the dto and entity
			CreateMap<Entities.Course, Models.CourseDto>();
		}
	}
}