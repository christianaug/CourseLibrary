using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using CourseLibrary.API.ResourceParameters;

namespace CourseLibrary.API.Controllers
{ 
	[ApiController]
	[Route("api/authors")]
	public class AuthorsController : ControllerBase
	{

		private readonly ICourseLibraryRepository _courseLibraryRepository;
		private readonly IMapper _mapper;

		public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
		{
			_courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		[HttpGet()]
		[HttpHead]
		public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorsResourceParameters authorsResaourceParameter)
		{
			var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsResaourceParameter);	

			return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
		}

		[HttpGet("{authorId}", Name = "GetAuthor")]
		public IActionResult GetAuthor(Guid authorId)
		{	
			var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

			if (authorFromRepo == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
		}

		[HttpPost]
		public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
		{
			var authorEntity = _mapper.Map<Entities.Author>(author);
			_courseLibraryRepository.AddAuthor(authorEntity);
			_courseLibraryRepository.Save();
			var authorDto = _mapper.Map<AuthorDto>(authorEntity);
			//returns a 201
			return CreatedAtRoute("GetAuthor", new { authorId = authorDto.Id }, authorDto);
		}

		[HttpOptions]
		public IActionResult GetAuthorsOptions()
		{
			Response.Headers.Add("Allow", "GET,OPTIONS,POST");
			return Ok();
		}

	}
}
