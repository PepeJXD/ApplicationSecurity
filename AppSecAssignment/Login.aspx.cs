using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

namespace AppSecAssignment
{
    public partial class Login : System.Web.UI.Page
    {
        string AppSecDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppSecDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                string email = tb_loginEmail.Text.ToString().Trim();
                string password = tb_loginPassword.Text.ToString().Trim();

                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);

                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string passwordWithSalt = password + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);
                        if (userHash.Equals(dbHash))
                        {
                            if (!(getLockedStatus(email)))
                            {
                                if (getPasswordStatus(email))
                                {
                                    Session["CPEmail"] = email;

                                    Session["CPType"] = "C";

                                    string guid = Guid.NewGuid().ToString();
                                    Session["CPAuthToken"] = guid;

                                    Response.Cookies.Add(new HttpCookie("CPAuthToken", guid));

                                    Response.Redirect("ChangePassword.aspx", false);
                                } else
                                {
                                    Session["Email"] = email;

                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;

                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                    Response.Redirect("Success.aspx", false);
                                }
                            }
                            else
                            {
                                if(getLockedTimeout(email))
                                {
                                    unlockAccount(email);
                                    Session["Email"] = email;

                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;

                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                    Response.Redirect("Success.aspx", false);
                                }
                                else
                                {
                                    lbl_errorMsg.Text = "The account has been locked due to login failures for 1 min";
                                    lbl_errorMsg.ForeColor = Color.Red;
                                }
                            }
                        }

                        else
                        {
                            if (getFailedAttempts(email) >= 2)
                            {
                                lockAccount(email);
                                lbl_errorMsg.Text = "The account has been locked due to login failures for 1 min";
                                lbl_errorMsg.ForeColor = Color.Red;
                            } else
                            {
                                lbl_errorMsg.Text = "Invalid email or password. Please try again.";
                                lbl_errorMsg.ForeColor = Color.Red;
                                using (SqlConnection con = new SqlConnection(AppSecDBConnectionString))
                                {
                                    using (SqlCommand cmd = new SqlCommand("UPDATE [Account] SET failedAttempts = failedAttempts + 1 WHERE email=@EMAIL"))
                                    {
                                        using (SqlDataAdapter sda = new SqlDataAdapter())
                                        {
                                            cmd.CommandType = System.Data.CommandType.Text;
                                            cmd.Parameters.AddWithValue("@EMAIL", email);
                                            cmd.Connection = con;
                                            con.Open();
                                            cmd.ExecuteNonQuery();
                                            con.Close();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        lbl_errorMsg.Text = "Invalid email or password. Please try again.";
                        lbl_errorMsg.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { }

            } else
            {
                lbl_errorMsg.Text = "Failed Captcha. Please try again.";
                lbl_errorMsg.ForeColor = Color.Red;
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

        protected int getFailedAttempts(string email)
        {
            int s = 0;
            SqlConnection connection = new SqlConnection(AppSecDBConnectionString);
            string sql = "select failedAttempts FROM Account WHERE email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["failedAttempts"] != null)
                        {
                            if (reader["failedAttempts"] != DBNull.Value)
                            {
                                s = int.Parse(reader["failedAttempts"].ToString());
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

        protected void lockAccount(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppSecDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [Account] SET LockedOut = 1 WHERE email=@EMAIL"))
                    {
                        using (SqlCommand cmd2 = new SqlCommand("UPDATE [Account] SET lockedTime = @dateTime WHERE email=@EMAIL"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = System.Data.CommandType.Text;
                                cmd.Parameters.AddWithValue("@EMAIL", email);
                                cmd2.CommandType = System.Data.CommandType.Text;
                                cmd2.Parameters.AddWithValue("@dateTime", DateTime.Now);
                                cmd2.Parameters.AddWithValue("@EMAIL", email);
                                cmd.Connection = con;
                                cmd2.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                cmd2.ExecuteNonQuery();
                                con.Close();
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

        protected void unlockAccount(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppSecDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [Account] SET LockedOut = 0 WHERE email=@EMAIL"))
                    {
                        using (SqlCommand cmd2 = new SqlCommand("UPDATE [Account] SET lockedTime = @dateTime WHERE email=@EMAIL"))
                        {
                            using (SqlCommand cmd3 = new SqlCommand("UPDATE [Account] SET failedAttempts = 0 WHERE email=@EMAIL"))
                            {
                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                {
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    cmd.Parameters.AddWithValue("@EMAIL", email);
                                    cmd2.CommandType = System.Data.CommandType.Text;
                                    cmd2.Parameters.AddWithValue("@dateTime", DBNull.Value);
                                    cmd2.Parameters.AddWithValue("@EMAIL", email);
                                    cmd3.CommandType = System.Data.CommandType.Text;
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

        protected bool getLockedStatus(string email)
        {
            Boolean s = false;
            SqlConnection connection = new SqlConnection(AppSecDBConnectionString);
            string sql = "select LockedOut FROM Account WHERE email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LockedOut"] != null)
                        {
                            if (reader["LockedOut"] != DBNull.Value)
                            {
                                if(int.Parse(reader["LockedOut"].ToString()) == 1)
                                {
                                    s = true;
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
            return s;

        }

        protected bool getPasswordStatus(string email)
        {
            Boolean s = false;
            SqlConnection connection = new SqlConnection(AppSecDBConnectionString);
            string sql = "select passwordTime FROM Account WHERE email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordTime"] != null)
                        {
                            if (reader["passwordTime"] != DBNull.Value)
                            {
                                if (Convert.ToDateTime(reader["passwordTime"]).AddMinutes(15) < DateTime.Now)
                                {
                                    s = true;
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
            return s;

        }

        protected bool getLockedTimeout(string email)
        {
            Boolean s = false;
            SqlConnection connection = new SqlConnection(AppSecDBConnectionString);
            string sql = "select lockedTime FROM Account WHERE email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["lockedTime"] != null)
                        {
                            if (reader["lockedTime"] != DBNull.Value)
                            {
                                if (Convert.ToDateTime(reader["lockedTime"]).AddMinutes(1) < DateTime.Now)
                                {
                                    s = true;
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
            return s;

        }

        public class myObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
            ("https://www.google.com/recaptcha/api/siteverify?secret= &response=" + captchaResponse);

            try
            {
                using(WebResponse wResponse = req.GetResponse())
                {
                    using(StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        myObject jsonObject = js.Deserialize<myObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected void goToRegistration_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx", false);
        }
    }
}