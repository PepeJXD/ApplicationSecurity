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
    public partial class Registration : System.Web.UI.Page
    {
        string AppSecDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppSecDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {
            tb_password.Attributes["value"] = tb_password.Text;
        }

        protected void registerBtn_Click(object sender, EventArgs e)
        {
            Boolean valid = true;
            if (!(regexFirstName.IsValid))
            {
                valid = false;
            }
            if (!(regexLastName.IsValid))
            {
                valid = false;
            }
            if (!(regexCreditCard.IsValid))
            {
                valid = false;
            }
            if (!(regexEmail.IsValid))
            {
                valid = false;
            }
            if (!(regexPassword.IsValid))
            {
                valid = false;
            }
            if (!(regexDOB.IsValid))
            {
                valid = false;
            }
            if (tb_firstName.Text == "")
            {
                error_firstName.Text = "First Name cannot be empty";
                error_firstName.ForeColor = Color.Red;
                valid = false;
            }
            if (tb_lastName.Text == "")
            {
                error_lastName.Text = "Last Name cannot be empty";
                error_lastName.ForeColor = Color.Red;
                valid = false;
            }
            if (tb_creditCard.Text == "")
            {
                error_creditCard.Text = "Credit Card Info cannot be empty";
                error_creditCard.ForeColor = Color.Red;
                valid = false;
            }
            if (tb_email.Text == "")
            {
                error_email.Text = "Email address cannot be empty";
                error_email.ForeColor = Color.Red;
                valid = false;
            }
            using (SqlConnection con = new SqlConnection(AppSecDBConnectionString))
            {
                error_email.Text = "";
                SqlCommand check_email = new SqlCommand("SELECT COUNT(*) FROM Account WHERE (email = @email)", con);
                check_email.Parameters.AddWithValue("@email", tb_email.Text);
                con.Open();
                var UserExist = (int)check_email.ExecuteScalar();
                con.Close();

                if (UserExist > 0)
                {
                    error_email.Text = "Email address already in use";
                    error_email.ForeColor = Color.Red;
                    valid = false;
                }
            }

            if (checkPassword(tb_password.Text) < 4)
            {
                int scores = checkPassword(tb_password.Text);
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
                error_password.Text = "Status : " + status;
                error_password.ForeColor = Color.Red;
                valid = false;

            }
            if (tb_DOB.Text == "")
            {
                error_DOB.Text = "Date of Birth cannot be empty";
                error_DOB.ForeColor = Color.Red;
                valid = false;
            }

            if (valid)
            {
                string pwd = tb_password.Text.ToString().Trim(); ;
                //Generate random "salt"
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                //Fills array of bytes with a cryptographically strong sequence of random values.
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                finalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;
                createAccount();

                Response.Redirect("Login.aspx", false);
            }

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

        public void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppSecDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [Account] VALUES(@firstName, @lastName, @creditCard, @email, @passwordHash, @passwordSalt, @DOB, @failedAttempts, @LockedOut, @lockedTime, @passwordList, @passwordTime, @IV, @Key)"))
                {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            var passwordListString = finalHash + "|||";
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.Parameters.AddWithValue("@firstName", HttpUtility.HtmlEncode(tb_firstName.Text).Trim());
                            cmd.Parameters.AddWithValue("@lastName", HttpUtility.HtmlEncode(tb_lastName.Text).Trim());
                            cmd.Parameters.AddWithValue("@creditCard", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(tb_creditCard.Text).Trim())));
                            cmd.Parameters.AddWithValue("@email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@passwordHash", finalHash);
                            cmd.Parameters.AddWithValue("@passwordSalt", salt);
                            cmd.Parameters.AddWithValue("@DOB", HttpUtility.HtmlEncode(tb_DOB.Text).Trim());
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@failedAttempts", 0);
                            cmd.Parameters.AddWithValue("@LockedOut", 0);
                            cmd.Parameters.AddWithValue("@lockedTime", DBNull.Value);
                            cmd.Parameters.AddWithValue("@passwordList", passwordListString);
                            cmd.Parameters.AddWithValue("@passwordTime", DateTime.Now);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        protected void checkPWbtn_Click(object sender, EventArgs e)
        {
            //Offer feedback on password strength
            int scores = checkPassword(tb_password.Text);
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
            error_password.Text = "Status : " + status;
            if (scores < 4)
            {
                error_password.ForeColor = Color.Red;
                return;
            }
            error_password.ForeColor = Color.Green;
        }

        protected void goToLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx", false);
        }
    }
}