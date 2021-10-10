using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;    //驗證
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleSqlite.Models
{
	[Table("Students")]
	public class TestDb
	{
		[Key]
		public int id { get; set; }

		[Display(Name = "名字")]
		public string UserName { get; set; }

		[Display(Name ="座號")]
		public int SeatNumber { get; set; }

	}
}
