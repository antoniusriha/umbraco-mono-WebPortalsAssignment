//
// ReviewControl.ascx.cs
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using umbraco.NodeFactory;

namespace CmsStoreReviewControl
{
	public partial class ReviewControl : UserControl
	{
		const string ReviewTableName = "CMS_STORE_REVIEWS";
		const string ConnectionString =
			"Server=127.0.0.1;" +
			"Database=mycms;" +
			"User ID=www-data;" +
			"Password=;";
		
		public string LoginUrl { get; set; }
		
		protected virtual void Page_PreRender (object sender, EventArgs e)
		{
			string text;
			if ((text = Request.QueryString ["reviewText"]) != null)
				txtReview.Text = HttpUtility.UrlDecode (text);
			
			using (var dbcon = new MySqlConnection (ConnectionString)) {
				dbcon.Open ();
				using (var cmd = dbcon.CreateCommand ()) {
					cmd.CommandText = "SELECT AUTHOR, TEXT, CREATE_DATE FROM "
						+ ReviewTableName + " " +
						"WHERE NODE_ID=@node_id " +
						"ORDER BY CREATE_DATE DESC;";
					cmd.Parameters.Add ("@node_id", MySqlDbType.Int32, 11);
					cmd.Parameters ["@node_id"].Value = Node.getCurrentNodeId ();
					
					using (var reader = cmd.ExecuteReader ()) {
						while (reader.Read ()) {
							AddReview (reader ["AUTHOR"].ToString (),
							           reader ["TEXT"].ToString (),
							           DateTime.Parse (reader ["CREATE_DATE"].ToString ()));
						}
					}
				}
			}
		}
		
		protected void btnSubmit_Click (object sender, EventArgs e)
		{
			if (!Context.User.Identity.IsAuthenticated) {
				var url = HttpUtility.UrlEncode (Request.Url.ToString ());
				var text = HttpUtility.UrlEncode (txtReview.Text);
				Response.Redirect (LoginUrl + "?redirectUrl=" + url
				                   + "&reviewText=" + text);
				return;
			}
			
			if (string.IsNullOrWhiteSpace (txtReview.Text))
				return;
			
			using (var dbcon = new MySqlConnection (ConnectionString)) {
				dbcon.Open ();
				using (var cmd = dbcon.CreateCommand ()) {					
					// create insert statement
					cmd.CommandText = "INSERT INTO " + ReviewTableName +
						" (NODE_ID, AUTHOR, TEXT)" +
						" VALUES (@node_id, @author, @text);";
				
					// create insert parameters
					cmd.Parameters.Add ("@node_id", MySqlDbType.Int32, 11);
					cmd.Parameters ["@node_id"].Value = Node.getCurrentNodeId ();
					cmd.Parameters.Add ("@author", MySqlDbType.VarChar, 1000);
					cmd.Parameters ["@author"].Value = Context.User.Identity.Name;
					cmd.Parameters.Add ("@text", MySqlDbType.LongText);
					cmd.Parameters ["@text"].Value = txtReview.Text;
					
					cmd.ExecuteNonQuery ();
				}
			}
			
			// clear textbox
			txtReview.Text = string.Empty;
		}
		
		void AddReview (string author, string text, DateTime createDate)
		{
			if (string.IsNullOrWhiteSpace (author))
				author = "anonymous";
			
			var cell0 = "<strong>" + author + "</strong>" + "<br/>"
				+ createDate.ToShortDateString () +
				"<br/>" + createDate.ToShortTimeString ();
			
			var row = new TableRow ();
			row.Cells.Add (new TableCell { Text = cell0 });
			row.Cells.Add (new TableCell { Text = text });
			tblReviews.Rows.Add (row);
		}
	}
}
