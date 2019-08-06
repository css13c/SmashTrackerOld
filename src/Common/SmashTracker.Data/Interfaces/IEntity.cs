using System;
using System.Collections.Generic;
using System.Text;

namespace SmashTracker.Data.Interfaces
{
	public interface IEntity
	{
		Guid Id { get; set; }
	}
}
