using System;
using SQLite;

namespace heres
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
	}
}

