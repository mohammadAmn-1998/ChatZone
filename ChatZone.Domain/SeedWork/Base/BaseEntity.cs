﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatZone.Domain.SeedWork.Base
{
	public class BaseEntity
	{
		[Key]
		public long Id { get; set; }

		public DateTime CreatedDate { get; set; }

		public bool IsDeleted { get; set; }

	}
}
