using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAssignment
{
    public partial class Success : System.Web.UI.Page
    {
        string AppSecDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppSecDBConnection"].ConnectionString;
        static byte[] Key = null;
        static byte[] IV = null;
        static string name = null;
        static string email = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if(!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    email = Session["Email"].ToString();
                    displayUserProfile(email);
                    logoutBtn.Visible = true;
                }

            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();

                        }
                    }
                }
            }


            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }


        protected void displayUserProfile(string email)
        {
            SqlConnection connection = new SqlConnection(AppSecDBConnectionString);
            string sql = "select * FROM ACCOUNT WHERE email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["email"] != DBNull.Value)
                        {
                            lbl_successEmail.Text = reader["email"].ToString();

                            //cipherTextNRIC = (byte[])reader["Nric"];
                        }
                        if (reader["firstName"] != DBNull.Value)
                        {
                            name = reader["firstName"].ToString();

                            //cipherTextNRIC = (byte[])reader["Nric"];
                        }
                        if (reader["lastName"] != DBNull.Value)
                        {
                            name += reader["lastName"].ToString();

                            //cipherTextNRIC = (byte[])reader["Nric"];
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            System.Diagnostics.Debug.WriteLine("ok");
                            IV = Convert.FromBase64String(reader["IV"].ToString());

                            //cipherTextNRIC = (byte[])reader["Nric"];
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());

                            //cipherTextNRIC = (byte[])reader["Nric"];
                        }
                    }
                    lbl_successName.Text = name;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            {
                connection.Close();
            }
        }

        protected void LogoutMe(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if(Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        protected void logoutBtn_Click(object sender, EventArgs e)
        {
            LogoutMe(sender, e);
        }

        protected void CPBtn_Click(object sender, EventArgs e)
        {
            if (!(waitPassChange(email)))
            {
                Session["CPEmail"] = email;

                Session["CPType"] = "E";

                string guid = Guid.NewGuid().ToString();
                Session["CPAuthToken"] = guid;

                Response.Cookies.Add(new HttpCookie("CPAuthToken", guid));

                Response.Redirect("ChangePassword.aspx", false);
            } else
            {
                sucError.Text = "Min password age not met";
                sucError.ForeColor = Color.Red;
            }
        }

        protected bool waitPassChange(string email)
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
                                if (DateTime.Now < Convert.ToDateTime(reader["passwordTime"]).AddMinutes(5))
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
    }
}