<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="AppSecAssignment.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 309px;
        }
        .auto-style3 {
            width: 250px;
        }
    </style>
    <script type="text/javascript">
        //Registration Form - Set Strong password (Client-based)
        function validate() {
            var str = document.getElementById('<%=tb_changePW.ClientID%>').value;

            if (str.length < 8) {
                document.getElementById("error_changePW").innerHTML = "Password length must be at least 8 characters";
                document.getElementById("error_changePW").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("error_changePW").innerHTML = "Password requires at least 1 number!";
                document.getElementById("error_changePW").style.color = "red";
                return ("no_number");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("error_changePW").innerHTML = "Password requires at least 1 lowercase letter!";
                document.getElementById("error_changePW").style.color = "red";
                return ("no_lower");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("error_changePW").innerHTML = "Password requires at least 1 Uppercase letter!";
                document.getElementById("error_changePW").style.color = "red";
                return ("no_upper");
            }
            else if (str.search(/[^\w]/) == -1) {
                document.getElementById("error_changePW").innerHTML = "Password requires at least 1 special letter!";
                document.getElementById("error_changePW").style.color = "red";
                return ("no_special");
            }

            document.getElementById("error_changePW").innerHTML = "Excellent!";
            document.getElementById("error_changePW").style.color = "Blue";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="lbl_changePassword" runat="server" Text="Change your password"></asp:Label>
        <br />
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style3">
                        <asp:Label ID="lbl_changePW" runat="server" Text="Password"></asp:Label>
                    </td>
                    <td class="auto-style2">
                        <asp:TextBox ID="tb_changePW" runat="server" Width="210px" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
                        <br />
                        <asp:Button ID="checkPW" runat="server" Text="Check password strength" OnClick="checkPW_Click" />
                    </td>
                    <td>
                        <asp:Label ID="error_changePW" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">
                        <asp:Label ID="lbl_confirmPW" runat="server" Text="Confirm Password"></asp:Label>
                    </td>
                    <td class="auto-style2">
                        <asp:TextBox ID="tb_confirmPW" runat="server" Width="210px" TextMode="Password"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="error_confirmPW" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Button ID="changeBtn" runat="server" Text="Change" OnClick="changeBtn_Click" />
        <p>
            <asp:Button ID="BackBtn" runat="server" Text="Back" OnClick="BackBtn_Click" />
        </p>
    </form>
</body>
</html>
