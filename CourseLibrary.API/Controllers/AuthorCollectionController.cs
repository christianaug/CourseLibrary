using System;
using AutoMapper;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
	[ApiController]
	[Route("api/authorcollections")]
	public class AuthorCollectionsController : ControllerBase 
	{
		private readonly ICourseLibraryRepository _courseLibraryRepository;
		private readonly IMapper _mapper;

	}
}