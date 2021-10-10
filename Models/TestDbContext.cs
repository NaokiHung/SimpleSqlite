using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace SimpleSqlite.Models
{
						//這邊的:DbContext就是必需將Microsoft.EntityFrameworkCore加入參考才不會報錯
	public class TestDbContext:DbContext
	{
		public DbSet<TestDb> Students { get; set; }		//這邊的名稱我取跟資料庫是不同的，這邊是代表資料表的名字

		//.UseSqlite 這個方法則是必需將Microsoft.EntityFrameworkCore.Sqlite 加入參考才不會報錯
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseSqlite("Data Source=TestDb.db");
	}
}
