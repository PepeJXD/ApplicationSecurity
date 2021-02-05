<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="AppSecAssignment.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration</title>
    <style type="text/css">
        .auto-style1 {
            width: 235px;
        }
        .auto-style2 {
            width: 100%;
        }
        .auto-style3 {
            width: 235px;
            height: 26px;
        }
        .auto-style4 {
            height: 26px;
        }
        .auto-style5 {
            width: 312px;
        }
        .auto-style6 {
            width: 312px;
            height: 26px;
        }
        .auto-style7 {
            width: 235px;
            height: 42px;
        }
        .auto-style8 {
            width: 312px;
            height: 42px;
        }
        .auto-style9 {
            height: 42px;
        }
    </style>

    <script type="text/javascript">
        //Registration Form - Set Strong password (Client-based)
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID%>').value;

            if (str.length < 8) {
                document.getElementById("error_password").innerHTML = "Password length must be at least 8 characters";
                document.getElementById("error_password").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("error_password").innerHTML = "Password requires at least 1 number!";
                document.getElementById("error_password").style.color = "red";
                return ("no_number");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("error_password").innerHTML = "Password requires at least 1 lowercase letter!";
                document.getElementById("error_password").style.color = "red";
                return ("no_lower");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("error_password").innerHTML = "Password requires at least 1 Uppercase letter!";
                document.getElementById("error_password").style.color = "red";
                return ("no_upper");
            }
            else if (str.search(/[^\w]/) == -1) {
                document.getElementById("error_password").innerHTML = "Password requires at least 1 special letter!";
                document.getElementById("error_password").style.color = "red";
                return ("no_special");
            }

            document.getElementById("error_password").innerHTML = "Excellent!";
            document.getElementById("error_password").style.color = "Blue";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:Label ID="lbl_registration" runat="server" Text="Registration"></asp:Label>
        <p>
            <table class="auto-style2">
                <tr>
                    <td class="auto-style7">
                        <asp:Label ID="lbl_firstName" runat="server" Text="First Name"></asp:Label>
                    </td>
                    <td class="auto-style8">
                        <asp:TextBox ID="tb_firstName" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td class="auto-style9">
                        <asp:Label ID="error_firstName" runat="server"></asp:Label>
                        <br />
                        <asp:RegularExpressionValidator ID="regexFirstName" runat="server" ControlToValidate="tb_firstName" ErrorMessage="First Name is invalid" ForeColor="Red" ValidationExpression="(?i:^[a-z ,.'-]+$)"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">
                        <asp:Label ID="lbl_lastName" runat="server" Text="Last Name"></asp:Label>
                    </td>
                    <td class="auto-style6">
                        <asp:TextBox ID="tb_lastName" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td class="auto-style4">
                        <asp:Label ID="error_lastName" runat="server"></asp:Label>
                        <br />
                        <asp:RegularExpressionValidator ID="regexLastName" runat="server" ControlToValidate="tb_lastName" ErrorMessage="Last Name is invalid" ForeColor="Red" ValidationExpression="(?i:^[a-z ,.'-]+$)"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style1">
                        <asp:Label ID="lbl_creditCard" runat="server" Text="Credit Card Info"></asp:Label>
                    </td>
                    <td class="auto-style5">
                        <asp:TextBox ID="tb_creditCard" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="error_creditCard" runat="server"></asp:Label>
                        <br />
                        <asp:RegularExpressionValidator ID="regexCreditCard" runat="server" ControlToValidate="tb_creditCard" ErrorMessage="Credit Card is invalid" ForeColor="Red" ValidationExpression="^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style1">
                        <asp:Label ID="lbl_email" runat="server" Text="Email address"></asp:Label>
                    </td>
                    <td class="auto-style5">
                        <asp:TextBox ID="tb_email" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="error_email" runat="server"></asp:Label>
                        <br />
                        <asp:RegularExpressionValidator ID="regexEmail" runat="server" ControlToValidate="tb_email" ErrorMessage="Email address is invalid" ForeColor="Red" ValidationExpression="^\S+@\S+\.\S+$"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style1">
                        <asp:Label ID="lbl_password" runat="server" Text="Password"></asp:Label>
                    </td>
                    <td class="auto-style5">
                        <asp:TextBox ID="tb_password" runat="server" onkeyup="javascript:validate()" TextMode="Password" Width="200px"></asp:TextBox>
                        <br />
                        <asp:Button ID="checkPWbtn" runat="server" OnClick="checkPWbtn_Click" Text="Check password strength" Width="207px" />
                    </td>
                    <td>
                        <asp:Label ID="error_password" runat="server"></asp:Label>
                        <br />
                        <asp:RegularExpressionValidator ID="regexPassword" runat="server" ControlToValidate="tb_password" ErrorMessage="Password is invalid" ForeColor="Red" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&amp;])[A-Za-z\d$@$!%*?&amp;]{8,}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style1">
                        <asp:Label ID="lbl_DOB" runat="server" Text="Date of Birth (dd/mm/yyyy)"></asp:Label>
                    </td>
                    <td class="auto-style5">
                        <asp:TextBox ID="tb_DOB" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="error_DOB" runat="server"></asp:Label>
                        <br />
                        <asp:RegularExpressionValidator ID="regexDOB" runat="server" ControlToValidate="tb_DOB" ErrorMessage="Date of Birth is in invalid format" ForeColor="Red" ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d$"></asp:RegularExpressionValidator>
                    </td>
                </tr>
            </table>
        </p>
        <p>
            <asp:Button ID="registerBtn" runat="server" OnClick="registerBtn_Click" Text="Register" />
        </p>
        <p>
            &nbsp;</p>
        <p>
            <asp:Button ID="goToLogin" runat="server" OnClick="goToLogin_Click" Text="Login to account" />
        </p>
    </form>
</body>
</html>
