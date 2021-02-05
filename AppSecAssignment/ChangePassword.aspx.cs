using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAssignment
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string AppSecDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppSecDBConnection"].ConnectionString;
        static string email = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            tb_changePW.Attributes["value"] = tb_changePW.Text;
            if (Session["CPEmail"] != null && Session["CPAuthToken"] != null && Request.Cookies["CPAuthToken"] != null)
            {
                if (!Session["CPAuthToken"].ToString().Equals(Request.Cookies["CPAuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    email = Session["CPEmail"].ToString();
                    if (Session["CPType"].ToString() == "E")
                    {
                        lbl_changePassword.Text = "Your password has expired. Please change your password.";
                    }

                    //displayUserProfile(email);
                    changeBtn.Visible = true;
                }

            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void checkPW_Click(object sender, EventArgs e)
        {
            //Offer feedback on password strength
            int scores = checkPassword(tb_changePW.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            error_changePW.Text = "Status : " + status;
            if (scores < 4)
            {
                error_changePW.ForeColor = Color.Red;
                return;
            }
            error_changePW.ForeColor = Color.Green;
        }


        //Check password strength
        private int checkPassword(string password)
        {
            int score = 0;

            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }

            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }

            return score;
        }

        protected void changeBtn_Click(object sender, EventArgs e)
        {
            Boolean valid = true;
            if (tb_changePW.Text == "")
            {
                lbl_changePW.Text = "Password cannot be empty";
                lbl_changePW.ForeColor = Color.Red;
                valid = false;
            }
            if (tb_changePW.Text != tb_confirmPW.Text)
            {
                lbl_confirmPW.Text = "Passwords do not match";
                lbl_confirmPW.ForeColor = Color.Red;
                valid = false;
            }
            if (valid)
            {
                var values = getPasswordReuse(email);
                if (!(values.Item1))
                {
                    changePassword(values.Item2, values.Item3, values.Item4);
                    changeDoneMe(sender, e);
                }
                else
                {
                    error_changePW.Text = "Password cannot be reused";
                    error_changePW.ForeColor = Color.Red;
                }

                //Response.Redirect("Login.aspx", false);
            }
        }


        protected string getDBHash(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(AppSecDBConnectionString);
            string sql = "select passwordHash FROM Account WHERE email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["passwordHash"] != null)
                        {
                            if (reader["passwordHash"] != DBNull.Value)
                            {
                                h = reader["passwordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(AppSecDBConnectionString);
            string sql = "select passwordSalt FROM Account WHERE email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordSalt"] != null)
                        {
                            if (reader["passwordSalt"] != DBNull.Value)
                            {
                                s = reader["passwordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected void changeDoneMe(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["CPAuthToken"] != null)
            {
                Response.Cookies["CPAuthToken"].Value = string.Empty;
                Response.Cookies["CPAuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        protected (Boolean, string, string, string) getPasswordReuse(string email)
        {
            SHA512Managed hashing = new SHA512Managed();
            string dbSalt = getDBSalt(email);
            Boolean s = false;
            if (dbSalt != null && dbSalt.Length > 0)
            {
                SqlConnection connection = new SqlConnection(AppSecDBConnectionString);
                string sql = "select passwordList FROM Account WHERE email=@EMAIL";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EMAIL", email);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["passwordList"] != null)
                            {
                                if (reader["passwordList"] != DBNull.Value)
                                {
                                    string plist = reader["passwordList"].ToString();
                                    string[] plistsplit = plist.Split(new[] { "|||" }, StringSplitOptions.None);

                                    string passwordWithSalt = tb_changePW.Text + dbSalt;
                                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                                    string userHash = Convert.ToBase64String(hashWithSalt);

                                    for (int i = 0; i < plistsplit.Length; i++)
                                    {
                                        if (userHash.Equals(plistsplit[i]))
                                        {
                                            s = true;
                                        }
                                    }
                                    if (!(s))
                                    {
                                        if (plistsplit.Length < 3)
                                        {
                                            var newplist = plistsplit[0] + "|||" + userHash + "|||";
                                            return (s, email, newplist.Trim(), userHash);
                                        }
                                        else
                                        {
                                            var newplist = plistsplit[1] + "|||" + userHash + "|||";
                                            return (s, email, newplist.Trim(), userHash);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { connection.Close(); }
                return (s, "","","");
            }
            return (s, "", "", "");
        }

        protected void changePassword(string email, string plist, string pwHash)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppSecDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [Account] SET passwordHash = @passwordHash WHERE email=@EMAIL"))
                    {
                        using (SqlCommand cmd2 = new SqlCommand("UPDATE [Account] SET passwordTime = @dateTime WHERE email=@EMAIL"))
                        {
                            using (SqlCommand cmd3 = new SqlCommand("UPDATE [Account] SET passwordList = @pwList WHERE email=@EMAIL"))
                            {
                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                {
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    cmd.Parameters.AddWithValue("@EMAIL", email);
                                    cmd.Parameters.AddWithValue("@passwordHash", pwHash);
                                    cmd2.CommandType = System.Data.CommandType.Text;
                                    cmd2.Parameters.AddWithValue("@dateTime", DateTime.Now);
                                    cmd2.Parameters.AddWithValue("@EMAIL", email);
                                    cmd3.CommandType = System.Data.CommandType.Text;
                                    cmd3.Parameters.AddWithValue("@pwList", plist);
                                    cmd3.Parameters.AddWithValue("@EMAIL", email);
                                    cmd.Connection = con;
                                    cmd2.Connection = con;
                                    cmd3.Connection = con;
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    cmd2.ExecuteNonQuery();
                                    cmd3.ExecuteNonQuery();
                                    con.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }


        protected void BackBtn_Click(object sender, EventArgs e)
        {
            changeDoneMe(sender, e);
        }
    }

}