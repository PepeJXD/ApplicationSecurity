<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Success.aspx.cs" Inherits="AppSecAssignment.Success" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h2>User Profile</h2>
        <h2>Email : <asp:Label ID="lbl_successEmail" runat="server"></asp:Label>
        </h2>
        <h2>Name :&nbsp;
            <asp:Label ID="lbl_successName" runat="server"></asp:Label>
        </h2>
        <p>
            <asp:Button ID="logoutBtn" runat="server" OnClick="logoutBtn_Click" Text="Logout" Visible="False" />
        </p>
    </div>
        <p>
            <asp:Button ID="CPBtn" runat="server" OnClick="CPBtn_Click" Text="Change Password" />
        </p>
        <asp:Label ID="sucError" runat="server"></asp:Label>
    </form>
</body>
</html>
