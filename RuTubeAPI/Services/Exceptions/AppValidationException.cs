using System;

namespace RuTubeAPI.Services.Exceptions
{
	public class AppValidationException : Exception
	{
		public AppValidationException(string msg)
			: base(msg)
		{
			
		}
	}
}
