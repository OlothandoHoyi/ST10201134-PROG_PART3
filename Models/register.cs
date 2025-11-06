using System.Data.SqlClient;

namespace POE_part2.Models
{
    public class register
    {
        public string username {  get; set; }
        public string email { get; set; }  
        public string role { get; set; }
        public string password { get; set; }

        //connection string class
        connection connect = new connection();

        public string insert_user(string name, string email, string roles, string password)
        {
            //temp variable for message
            string message = "";

            //connect to database
            try
            {
                using (SqlConnection connects = new SqlConnection(connect.Connecting() )) {
                    //open connection
                    connects.Open();

                    //query
                    string query = "insert into users values('"+name+"','"+email+"','"+roles+"','"+password+"')";
                    //execute command
                    using (SqlCommand add_new_user = new SqlCommand(query,connects) ) {
                        //then execute it
                        add_new_user.ExecuteNonQuery();

                        //assign the message
                        message = "done";
                    }
                    //then close connection
                    connects.Close();

                }

            }
            catch(Exception error)
            {
                message = error.Message;

            }
            return message;

        }

    }
}
