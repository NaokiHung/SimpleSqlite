using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SimpleSqlite.Models;              //有從類別檔引用功能就建議加這行，不然每次都要寫完整的寫法 例如: private Models.TestDbContext _db = new Models.TestDbContext();
using Microsoft.EntityFrameworkCore;	//有用到資料庫連線功能都要加這行，不然有些功能會無法使用

namespace SimpleSqlite.Controllers
{
	
	public class SqliteController : Controller
	{
		private TestDbContext _db = new TestDbContext();

		public IActionResult Index()
		{
			return View();
		}

		// List 從DB裡將所查詢資料列表呈現
		/* 非同步程式碼	https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/intro?view=aspnetcore-5.0#asynchronous-code

		非同步程式設計是預設的 ASP.NET Core 和 EF Core 模式。

		網頁伺服器的可用執行緒數量有限，而且在高負載情況下，可能會使用所有可用的執行緒。 發生此情況時，伺服器將無法處理新的要求，直到執行緒空出來。 使用同步程式碼，許多執行緒可能在實際上並未執行任何工作時受到占用，原因是在等候 I/O 完成。 使用非同步程式碼，處理程序在等候 I/O 完成時，其執行緒將會空出來以讓伺服器處理其他要求。 因此，非同步程式碼可讓伺服器資源更有效率地使用，而且伺服器可處理更多流量而不會造成延遲。

		非同步程式碼雖然的確會在執行階段造成少量的負荷，但在低流量情況下，對效能的衝擊非常微小；在高流量情況下，潛在的效能改善則相當大。 
		*/
		public async Task<IActionResult> SqliteMaster()
		{
			//這個方法離開using後，變數DB就會被系統回收了
			//using (TestDbContext _db = new TestDbContext())
			//{
			//	return View(_db.Students.ToList()); 
			//}

			IQueryable<TestDb> ListAll = from TestDb in _db.Students select TestDb;

			if (ListAll == null)
			{
				return NotFound();
			}
			else
			{
				return View(await ListAll.ToListAsync());
			}

		}

		// Details 從List裡查看指定單一筆資料的明細
		//public IActionResult SqliteDetails(int? ID)
		//{
		//	if (ID == null || ID.HasValue == false)
		//	{
		//		return new StatusCodeResult(Convert.ToInt32(System.Net.HttpStatusCode.BadRequest));
		//	}
		//	TestDb testDb = _db.Students.Find(ID);
		//	if (testDb == null)
		//	{
		//		return NotFound();
		//	}
		//	else
		//	{
		//		return View(testDb);
		//	}
		//}

		// 參考MSDN寫法 https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud?view=aspnetcore-5.0
		public async Task<IActionResult> SqliteDetails(int? ID)
		{
			if(ID == null)
			{
				return NotFound();
			}

			var testDb = await _db.Students.FirstOrDefaultAsync(m => m.id == ID);

			if (testDb == null)
			{
				return NotFound();
			}
			return View(testDb);
		}


		// Create 新增一筆資料
		public IActionResult SqliteCreate()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SqliteCreate([Bind("UserName", "SeatNumber")] TestDb _testDb)
		{
			try
			{
				if (ModelState.IsValid)
				{
					_db.Add(_testDb);
					await _db.SaveChangesAsync();
					return RedirectToAction(nameof(SqliteMaster));
				}
			}catch (DbUpdateException)
			{
				ModelState.AddModelError("", "Unable to save changes. " +
			"Try again, and if the problem persists " +
			"see your system administrator.");
			}
			return View(_testDb);
		}

		//另一種寫法
		//[HttpPost,ActionName("SqliteCreate")]
		//[ValidateAntiForgeryToken]
		//public IActionResult SqliteCreate(TestDb _testDb)
		//{
		//	if ((_testDb != null) && (ModelState.IsValid))
		//	{
		//		_db.Students.Add(_testDb);

		//		_db.SaveChanges();

		//		return RedirectToAction("SqliteMaster");
		//	}
		//	else
		//	{
		//		ModelState.AddModelError("Value1", "Error.");
		//		return View();
		//	}

		//}

		// Delete 刪除一筆資料，第一個Delete為確認頁
		/*public async Task<IActionResult> Delete(int? ID)
		{
			if (ID == null)
			{
				return new StatusCodeResult(Convert.ToInt32(System.Net.HttpStatusCode.BadRequest));
			}
			//TestDb testDb = _db.Students.Find(_ID);

			var testDb = await _db.Students.FirstOrDefaultAsync(m => m.id == ID);

			if(testDb == null)
			{
				return NotFound();
			}
			else
			{
				return View(testDb);
			}

		}*/

		// 這個DeleteConfirm才是真的在處理刪除指令的Action
		/*
		[HttpPost,ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirm(int ID, [Bind("id,UserName,SeatNumber")] TestDb testDb)
		{
			if (ModelState.IsValid)
			{

				//TestDb testDb = _db.Students.Find(_ID);
				testDb = _db.Students.Find(ID);
				
				_db.Remove(testDb);
				_db.SaveChanges();

				return RedirectToAction("SqliteMaster");
			}else{
				ModelState.AddModelError("Value1", "Error.");
				return View();
			}
		}
		*/

		public async Task<IActionResult> Delete(int? ID,bool? saveChangesError = false)
		{
			if(ID == null)
			{
				return NotFound();
			}

			TestDb testDb = await _db.Students.AsNoTracking().FirstOrDefaultAsync(m => m.id == ID);

			if (testDb == null)
			{
				return NotFound();
			}

			if (saveChangesError.GetValueOrDefault())
			{
				ViewData["ErrorMessage"] = "Delete failed. Try again, and if the problem persists " + "see your system administrator.";
			}

			return View(testDb);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int ID)
		{
			
			//var testDb = await _db.Students.FindAsync(ID);

			TestDb testDb = await _db.Students.FindAsync(ID);
			if(testDb == null)
			{
				return RedirectToAction(nameof(SqliteMaster));
			}

			try
			{
				_db.Students.Remove(testDb);
				await _db.SaveChangesAsync();
				return RedirectToAction(nameof(SqliteMaster));
			}
			catch
			{
				return RedirectToAction(nameof(Delete), new { id = ID, saveChangesError = true });
			}
		}

		// Edit, 修改資料內容
		/*
		public IActionResult SqliteEdit(int? ID)
		{
			if (ID == null)
			{
				return new StatusCodeResult(Convert.ToInt32(System.Net.HttpStatusCode.BadRequest));
			}

			TestDb testDb = _db.Students.Find(ID);

			if (testDb == null)
			{
				return NotFound();
			}
			else
			{
				return View(testDb);
			}
		}

		//這個Edit才是真的將修改的資料寫回資料庫的Action
		[HttpPost]
		[ValidateAntiForgeryToken]
		 public IActionResult SqliteEdit(int ID, [Bind("id,UserName,SeatNumber")] TestDb testDb)
		 {
			 if (testDb == null)
			 {
				 return new StatusCodeResult(Convert.ToInt32(System.Net.HttpStatusCode.BadRequest));
			 }

			 if (ModelState.IsValid)
			 {

				 _db.Entry(testDb).State = EntityState.Modified;
				 //_db.Update(testDb);
				 _db.SaveChanges();

				 return RedirectToAction("SqliteMaster");
			 }
			 else
			 {
				 return View(testDb);
			 }
		 }
		*/

		//微軟官方Docs寫法
		
		public async Task<IActionResult> SqliteEdit(int? ID)
		{
			if (ID == null)
			{
				return NotFound();
			}

			var testDb = await _db.Students.FindAsync(ID);
			if (testDb == null)
			{
				return NotFound();
			}
			
			return View(testDb);
		}


		// 微軟官方Docs寫法
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SqliteEdit(int ID, [Bind("id,UserName,SeatNumber")] TestDb testDb)
		{
			if (ID != testDb.id)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				try
				{
					_db.Update(testDb);
					await _db.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!testDbExists(testDb.id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(SqliteMaster));
			}
			return View(testDb);
		}
		

		private bool testDbExists(int ID)
		{
			return _db.Students.Any(e => e.id == ID);
		}

	}
}
