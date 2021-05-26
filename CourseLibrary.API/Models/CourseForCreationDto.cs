using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.ValidationAttributes;
namespace CourseLibrary.API.Models

{
	[CourseTitleMustBeDifferentFromDescriptionAttribute(ErrorMessage = "Title must be different from description.")]
	public class CourseForCreationDto
	{
		[Required(ErrorMessage = "You should fill out a title.")]
		[MaxLength(100, ErrorMessage = "The title shouldnt have more than 100 characters.")]
		public string Title { get; set; }

		[MaxLength(1500, ErrorMessage = "the description should not have more than 1500 characters.")]
		public string Description { get; set; }

		//using this interface and its method to validate rules
		//public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		//{
		//	if (Title == Description)
		//	{
		//		yield return new ValidationResult(
		//			"The provided description should be different from the title.",
		//			new[] { "CourseForCreationDto" }
		//		);
		//	}
		//}
	}
}