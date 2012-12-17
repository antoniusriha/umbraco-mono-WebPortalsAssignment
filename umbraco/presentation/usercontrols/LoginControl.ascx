<%@ Control Language="C#" Inherits="MembershipControls.UserControls.LoginControl" %>
<asp:Literal ID="litError" runat="server" />
<asp:Login RenderOuterTable="false" ID="ctlLogin" runat="server" OnLoginError="OnLoginError"
	onloggedin="OnLoggedIn" RememberMeSet="True" VisibleWhenLoggedIn="False">
    <LayoutTemplate>
        <div id="login">
            <div class="formholder">
            <div class="formrow">
                <div class="formleft"><label>Username</label></div>
                <div class="formright">
                    <asp:TextBox ID="UserName" CssClass="textbox required"
						ToolTip="Enter username" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="formrow">
                <div class="formleft"><label>Password</label></div>
                <div class="formright">
                    <asp:TextBox ID="Password" CssClass="textbox required"
						ToolTip="Enter password" runat="server" TextMode="Password" />
                </div>
            </div>
            <div class="formrow">
                <div class="formleft">&nbsp;</div>
                <div class="formright">
                    <asp:CheckBox ID="RememberMe" runat="server" Text="Keep me logged in" />
                </div>
            </div>
            <div class="formrow">
                <div class="formleft">&nbsp;</div>
                <div class="formright">
                    <asp:Button ID="LoginButton" CssClass="loginButton" runat="server"
						CommandName="Login" Text="Login" />
                </div>
            </div>
            </div>
        </div>
    </LayoutTemplate>
</asp:Login>
