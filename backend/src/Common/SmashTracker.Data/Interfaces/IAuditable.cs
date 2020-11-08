using System;

namespace SmashTracker.Data.Interfaces
{
	public interface IAuditable
	{
		DateTimeOffset? CreatedOn { get; set; }

		DateTimeOffset? ModifiedOn { get; set; }
	}
}