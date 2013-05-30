<%@ Control Language="C#" Inherits="MembershipControls.UserControls.RegisterControl" %>
<div id="registersubscriberform">
    <p>All items marked with a * are mandatory</p>
    <asp:Literal ID="litError" runat="server" />
    <div class="formholder">
        <div class="formrow">
            <div class="formleft"><label for="<%= tbUsername.ClientID %>">Username*</label></div>
            <div class="formright">
                <asp:TextBox ToolTip="Enter username" CssClass="required" ID="tbUsername" runat="server" ClientIDMode="Static" />
            </div>
        </div>
        <div class="formrow">
            <div class="formleft"><label for="<%= tbEmail.ClientID %>">Email*</label></div>
            <div class="formright">
                <asp:TextBox ToolTip="Enter email" CssClass="required email" ID="tbEmail" runat="server" ClientIDMode="Static" />
            </div>
        </div>
        <div class="formrow">
            <div class="formleft"><label for="<%= tbPassword.ClientID %>">Password*</label></div>
            <div class="formright">
                <asp:TextBox ToolTip="Enter a password" CssClass="required" ID="tbPassword" TextMode="Password" runat="server" ClientIDMode="Static" />
            </div>
        </div>
        <div class="formrow">
            <div class="formleft"></div>
            <div class="formright">
                <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="RegisterPlayer" />
            </div>
        </div>
    </div>
</div>