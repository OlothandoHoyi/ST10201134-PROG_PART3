using System.Data.SqlClient;

namespace POE_part2.Models
{
    public class check_login
    {
        public string email { get; set; }
        public string role { get; set; }        
        public string Password { get; set; }    

        //connection string
        connection connect= new connection();

        //methodbto check the user
        public string login_user(string emails, string roles, string password)
        {
            //temp variable
            string message = "";

            try
            {
                //connect and open
                using (SqlConnection connects = new SqlConnection(connect.Connecting()))
                {
                    //open connection
                    connects.Open();

                    //query
                    string query = "select * from users where email='" + emails + "' and password='" + password + "';";

                    //prepare to execute
                    using (SqlCommand prepare = new SqlCommand(query, connects))
                    {
                        //read the data
                        using (SqlDataReader find_user = prepare.ExecuteReader())
                        {

                            //then check if the use is found
                            if (find_user.HasRows)
                            {
                                //then assign message
                                message = "found";
                            }
                            else
                            {
                                message = "not";
                            }
                        }

                    }
                    connects.Close();
                    if(message == "found")
                    {
                        update_active(emails);
                    }

                }
            }
            catch (Exception erro_db)
            {
                message = erro_db.Message;

            }
            return message;
        }
        //update active method
        public void update_active(string emails)
        {
            try
            {
                using(SqlConnection connects = new SqlConnection(connect.Connecting()))
                {
                    connects.Open();
                    string query = "update active set emails='" + emails + "'";
                    using(SqlCommand done = new SqlCommand(query, connects))
                    {
                        done.ExecuteNonQuery();
                        
                    }
                    connects.Close();
                }
            }
            catch(Exception error)
            {
                Console.WriteLine("error " + error.Message);
            }
        }
    }
}
