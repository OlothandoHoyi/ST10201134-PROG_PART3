using System.Collections;
using System.Data.SqlClient;

namespace POE_part2.Models
{
    public class get_claims
    {
        public ArrayList email { get; set; } = new ArrayList();
        public ArrayList module { get; set; } = new ArrayList();
        public ArrayList id { get; set; } = new ArrayList();
        public ArrayList hours { get; set; } = new ArrayList();
        public ArrayList rate { get; set; } = new ArrayList();
        public ArrayList note { get; set; } = new ArrayList();
        public ArrayList total { get; set; } = new ArrayList();
        public ArrayList status { get; set; } = new ArrayList();
        public ArrayList filename { get; set; } = new ArrayList();

        connection connect = new connection();

        //constructor

        public get_claims()
        {
            string emails = gets_email();

            try
            {
                using (SqlConnection connects = new SqlConnection(connect.Connecting()))
                {

                    connects.Open();

                    using (SqlCommand prepare = new SqlCommand("select * from claiming where email = '" + emails + "';", connects))
                    {
                        using (SqlDataReader getEMAIL = prepare.ExecuteReader())
                        {

                            if (getEMAIL.HasRows)
                            {
                                //check all, but get one

                                while (getEMAIL.Read())
                                {
                                    //then get it
                                    //hold_email = getEMAIL["email"].ToString();

                                    email.Add(getEMAIL["email"]);
                                    module.Add(getEMAIL["module"]);
                                    id.Add(getEMAIL["user_id"]);
                                    hours.Add(getEMAIL["hours"]);
                                    rate.Add(getEMAIL["rate"]);
                                    note.Add(getEMAIL["note"]);
                                    total.Add(getEMAIL["total"]);
                                    status.Add(getEMAIL["status"]);
                                    filename.Add(getEMAIL["files"]);
                                }
                            }
                            getEMAIL.Close();
                        }
                    }
                    connects.Close();
                }

            }
            catch (IOException error)
            {
                Console.WriteLine(error.Message);
                //hold_email = error.Message;

            }
        }
        //get email
        public string gets_email()
        {
            //hold email variable
            string hold_email = "";

            try
            {
                using (SqlConnection connects = new SqlConnection(connect.Connecting()))
                {

                    connects.Open();

                    using (SqlCommand prepare = new SqlCommand("select * from active", connects))
                    {
                        using (SqlDataReader getEMAIL = prepare.ExecuteReader())
                        {

                            if (getEMAIL.HasRows)
                            {
                                //check all, but get one

                                while (getEMAIL.Read())
                                {
                                    //then get it
                                    hold_email = getEMAIL["email"].ToString();
                                }
                            }
                            getEMAIL.Close();
                        }
                    }
                    connects.Close();
                }

            }
            catch (IOException error)
            {
                Console.WriteLine(error.Message);
                hold_email = error.Message;

            }
            return hold_email;
        }
    }
}
