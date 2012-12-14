<%@ Control Language="C#" Inherits="CmsStoreReviewControl.ReviewControl" %>
<p>
  Author: <asp:TextBox id="txtAuthor" runat="server" />
  <br/>
  <asp:TextBox id="txtReview" TextMode="MultiLine" Rows="5" Columns="80" runat="server" />
  <br/>
  <asp:Button id="btnSubmit" Text="Submit" OnClick="btnSubmit_Click" runat="server" />
  <asp:Table id="tblReviews" runat="server" />
</p>