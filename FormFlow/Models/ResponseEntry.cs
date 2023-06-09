﻿using System.ComponentModel.DataAnnotations;

namespace FormFlow.Models
{
	public class ResponseEntry
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "QuestionId is required.")]
		public int QuestionId { get; set; }
		[Required(ErrorMessage = "Answer is required.")]
		public string Answer { get; set; }
	}
}
