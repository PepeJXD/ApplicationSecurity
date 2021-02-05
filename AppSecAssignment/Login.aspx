<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AppSecAssignment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6Lcq2kkaAAAAACpCXkHeZVNtEa4cAQcKatl1HE0C"></script>
    <style type="text/css">
        .auto-style1 {
            width: 92px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:Label ID="lbl_login" runat="server" Text="Login"></asp:Label>
        <p>
            <table class="auto-style2">
                <tr>
                    <td class="auto-style1">
                        <asp:Label ID="lbl_loginEmail" runat="server" Text="Email"></asp:Label>
                    </td>
                    <td class="auto-style5">
                        <asp:TextBox ID="tb_loginEmail" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style1">
                        <asp:Label ID="lbl_loginPassword" runat="server" Text="Password"></asp:Label>
                    </td>
                    <td class="auto-style6">
                        <asp:TextBox ID="tb_loginPassword" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
                    </td>
                    <td class="auto-style4">
                        &nbsp;</td>
                </tr>
                </table>
        </p>
        <p>
                        <asp:Label ID="lbl_errorMsg" runat="server"></asp:Label>
        </p>
        <p>
            <asp:Button ID="loginBtn" runat="server" OnClick="loginBtn_Click" Text="Login" />
        </p>
        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
    <script>
     grecaptcha.ready(function () {
         grecaptcha.execute('6Lcq2kkaAAAAACpCXkHeZVNtEa4cAQcKatl1HE0C', { action: 'Login' }).then(function (token) {
            document.getElementById("g-recaptcha-response").value = token;
         });
     });
    </script>
        <p>
            &nbsp;</p>
        <p>
            <asp:Button ID="goToRegistration" runat="server" OnClick="goToRegistration_Click" Text="Register an account" />
        </p>
    </form>
    </body>
</html>
