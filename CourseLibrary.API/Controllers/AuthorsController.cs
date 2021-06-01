using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using CourseLibrary.API.ResourceParameters;
using System.Text.Json;

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

		[HttpGet(Name = "GetAuthors")]
		[HttpHead]
		public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorsResourceParameters authorsResaourceParameter)
		{
			//gets the collection of authors using the url parameters
			var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsResaourceParameter);

			//getting the page metadata for the pagination data
			var previousPageLink = authorsFromRepo.HasPrevious ? CreateAuthorsResourceUri(authorsResaourceParameter, ResourceUriType.PreviousPage) : null;
			var nextPageLink = authorsFromRepo.HasNext ? CreateAuthorsResourceUri(authorsResaourceParameter, ResourceUriType.NextPage) : null;

			//generates the metdata object
			var paginationMetaData = new
			{
				totalCount = authorsFromRepo.TotalCount,
				pageSize = authorsFromRepo.PageSize,
				currentPage = authorsFromRepo.CurrentPage,
				totalPages = authorsFromRepo.TotalPages,
				previousPageLink,
				nextPageLink
			};

			//serializes
			//creating the response header
			Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

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

		private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
		{
			switch (type)
			{
				case ResourceUriType.PreviousPage:
					return Url.Link("GetAuthors", new
					{
						pageNumber = authorsResourceParameters.PageNumber - 1,
						pageSize = authorsResourceParameters.PageSize,
						mainCategory = authorsResourceParameters.MainCategory,
						searchQuery = authorsResourceParameters.SearchQuery
					});
				case ResourceUriType.NextPage:
					return Url.Link("GetAuthors", new
					{
						pageNumber = authorsResourceParameters.PageNumber + 1,
						pageSize = authorsResourceParameters.PageSize,
						mainCategory = authorsResourceParameters.MainCategory,
						searchQuery = authorsResourceParameters.SearchQuery
					});
				default:
					return Url.Link("GetAuthors", new
					{
						pageNumber = authorsResourceParameters.PageNumber,
						pageSize = authorsResourceParameters.PageSize,
						mainCategory = authorsResourceParameters.MainCategory,
						searchQuery = authorsResourceParameters.SearchQuery
					});
			}
		}

	}
}
