//
// Program.cs
//
// Author:
//       Antonius Riha <antoniusriha@gmail.com>
//
// Copyright (c) 2012 Antonius Riha
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using MySql.Data.MySqlClient;

namespace ReviewTableSetup
{
	class Program
	{
		const string ReviewTableName = "CMS_STORE_REVIEWS";
		const string ConnectionString =
			"Server=127.0.0.1;" +
			"Database=mycms;" +
			"User ID=www-data;" +
			"Password=;";
		
		static void Main (string [] args)
		{
			var delete = false;
			if (args.Length > 0 && args [0] == "-d")
				delete = true;
			
			using (var con = new MySqlConnection (ConnectionString)) {
				con.Open ();
				
				if (delete) {
					using (var cmd = con.CreateCommand ()) {
						cmd.CommandText = "DROP TABLE " + ReviewTableName + ";";
						var result = cmd.ExecuteNonQuery ();
						Console.WriteLine ("DROP TABLE result: " + result);
					}
				}
				
				using (var cmd = con.CreateCommand ()) {
					cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + ReviewTableName + " (" +
						"ID int(11) NOT NULL AUTO_INCREMENT, " +
						"NODE_ID int(11) NOT NULL, " +
						"AUTHOR varchar(1000) CHARACTER SET utf8 NOT NULL DEFAULT '', " +
						"TEXT longtext NOT NULL DEFAULT '', " +
						"CREATE_DATE timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP, " +
						"PRIMARY KEY (ID)" +
						") ENGINE=InnoDB DEFAULT CHARSET=latin1;";
					var result = cmd.ExecuteNonQuery ();
					Console.WriteLine ("CREATE TABLE result: " + result);
				}
			}
		}
	}
}
