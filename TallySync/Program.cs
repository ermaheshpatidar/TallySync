using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class TallyIntegration
{
    private const string tallyUrl = "http://localhost:9000"; // Tally server address
    private const string mysqlConnectionString = "Server=localhost;Database=mydatabase;User Id=root;Password=mypassword;";

    static async Task Main()
    {
        try
        {
            Console.WriteLine("Fetching data from MySQL...");
            Console.WriteLine("Fetching Company Data...");
            string companyName = GetCompanyName();
            Console.WriteLine("Fetching Ledger Data...");
            string ledgerXml = GetLedgerXml();
            Console.WriteLine("Fetching Voucher Data...");
            string voucherXml = GetVoucherXml();

            Console.WriteLine("Sending requests to Tally...");
            Console.WriteLine("Sending Company Data...");

            await SendToTally(companyName);
            Console.WriteLine("Sending Ledger Data...");

            await SendToTally(ledgerXml);
            Console.WriteLine("Sending Voucher Data...");

            await SendToTally(voucherXml);

            Console.WriteLine("Data sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static string GetCompanyName()
    {
        //using (MySqlConnection conn = new MySqlConnection(mysqlConnectionString))
        //{
        //    conn.Open();
        //    string query = "SELECT name FROM company WHERE id = 1"; // Replace with actual table & conditions
        //    MySqlCommand cmd = new MySqlCommand(query, conn);
        //    string companyName = cmd.ExecuteScalar()?.ToString() ?? "Default Company";
        Thread.Sleep(1000);
        string companyName = "demo";
            return $@"
                <ENVELOPE>
                    <HEADER>
                        <TALLYREQUEST>Import Data</TALLYREQUEST>
                    </HEADER>
                    <BODY>
                        <IMPORTDATA>
                            <REQUESTDESC>
                                <REPORTNAME>All Masters</REPORTNAME>
                            </REQUESTDESC>
                            <REQUESTDATA>
                                <TALLYMESSAGE>
                                    <COMPANY>
                                        <NAME>{companyName}</NAME>
                                    </COMPANY>
                                </TALLYMESSAGE>
                            </REQUESTDATA>
                        </IMPORTDATA>
                    </BODY>
                </ENVELOPE>";
        //}
    }

    static string GetLedgerXml()
    {
        //using (MySqlConnection conn = new MySqlConnection(mysqlConnectionString))
        //{
        //    conn.Open();
        //    string query = "SELECT name, opening_balance FROM ledger WHERE id = 1"; // Replace with actual table
        //    MySqlCommand cmd = new MySqlCommand(query, conn);
        //    using (MySqlDataReader reader = cmd.ExecuteReader())
        //    {
        //        if (reader.Read())
        //        {
        //            string ledgerName = reader["name"].ToString();
        //            string openingBalance = reader["opening_balance"].ToString();
        Thread.Sleep(1000);
        string ledgerName = "MyledgeName";
                 string openingBalance = "30000";
                 string groupName = "Sundry Debtors";
        return $@"
                        <ENVELOPE>
                            <HEADER>
                                <TALLYREQUEST>Import Data</TALLYREQUEST>
                            </HEADER>
                            <BODY>
                                <IMPORTDATA>
                                    <REQUESTDESC>
                                        <REPORTNAME>All Masters</REPORTNAME>
                                    </REQUESTDESC>
                                    <REQUESTDATA>
                                        <TALLYMESSAGE>
                                            <LEDGER NAME=""{ledgerName}"" Action=""Create"">
                                                <NAME>{ledgerName}</NAME>
                                                <PARENT>{groupName}</PARENT>
                                                <OPENINGBALANCE>{openingBalance}</OPENINGBALANCE>
                                            </LEDGER>
                                        </TALLYMESSAGE>
                                    </REQUESTDATA>
                                </IMPORTDATA>
                            </BODY>
                        </ENVELOPE>";
        //        }
        //    }
        //}
        return "";
    }

    static string GetVoucherXml()
    {
        //using (MySqlConnection conn = new MySqlConnection(mysqlConnectionString))
        //{
        //    conn.Open();
        //    string query = "SELECT date, amount, ledger FROM voucher WHERE id = 1"; // Replace with actual table
        //    MySqlCommand cmd = new MySqlCommand(query, conn);
        //    using (MySqlDataReader reader = cmd.ExecuteReader())
        //    {
        //        if (reader.Read())
        //        {
        //            string date = Convert.ToDateTime(reader["date"]).ToString("yyyyMMdd");
        //            string amount = reader["amount"].ToString();
        //            string ledgerName = reader["ledger"].ToString();

        Thread.Sleep(1000);
        string date =DateTime.Now.ToString("yyyyMMdd");
        string amount = "603";
        string ledgerName = "MyledgeName";
        return $@"
                        <ENVELOPE>
                            <HEADER>
                                <TALLYREQUEST>Import Data</TALLYREQUEST>
                            </HEADER>
                            <BODY>
                                <IMPORTDATA>
                                    <REQUESTDESC>
                                        <REPORTNAME>Vouchers</REPORTNAME>
                                    </REQUESTDESC>
                                    <REQUESTDATA>
                                        <TALLYMESSAGE>
                                            <VOUCHER VCHTYPE=""Receipt"" ACTION=""Create"">
                                                <DATE>{date}</DATE>
                                                <PARTYLEDGERNAME>{ledgerName}</PARTYLEDGERNAME>
                                                <AMOUNT>{amount}</AMOUNT>
                                            </VOUCHER>
                                        </TALLYMESSAGE>
                                    </REQUESTDATA>
                                </IMPORTDATA>
                            </BODY>
                        </ENVELOPE>";
        //        }
        //    }
        //}
        return "";
    }

    static async Task SendToTally(string xmlData)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpContent content = new StringContent(xmlData, Encoding.UTF8, "application/xml");
            HttpResponseMessage response = await client.PostAsync(tallyUrl, content);
            string responseText = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Tally Response: {responseText}");
        }
    }
}
