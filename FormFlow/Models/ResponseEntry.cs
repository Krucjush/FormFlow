﻿using System.ComponentModel.DataAnnotations;

namespace FormFlow.Models
{
	public class ResponseEntry
	{
		public int Id { get; set; }
		public int FormResponseId { get; set; }
		[Required(ErrorMessage = "QuestionId is required.")]
		public int QuestionId { get; set; }
		public string? Answer { get; set; }
	}
}
