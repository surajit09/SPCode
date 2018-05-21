using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.IO;
using System.Data;
using System.Security.Principal;


namespace StoreContactInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamWriter log;

            if (!System.IO.File.Exists("D:/logfile.txt"))
            {
                log = new StreamWriter("D:/logfile.txt");
            }
            else
            {
                log = System.IO.File.AppendText("D:/logfile.txt");
            } 
            try
            {


               



                string path = @"\\sr3prd01.carpetright.co.uk\connect";
                string SAPfileName = "ConnectSAP.csv";
                string PAYROLLfileName = @"managers.csv";
                string SAPOutputfilepath = @"D:\StoreOutput.txt";
                string SAPInputfilepath = @"D:\ConnectSAP.csv";
                string PAYROLLOutputfilepath = @"D:\PayrollOutput.txt";
                string PAYROLLInputfilepath = @"D:\managers.csv";

                string filePath = Path.Combine(path, Path.GetFileName(SAPfileName));

                if (System.IO.File.Exists(SAPOutputfilepath))
                {
                    System.IO.File.Delete(SAPOutputfilepath);

                    Console.WriteLine("File deleted  " + SAPOutputfilepath);
                }

                if (System.IO.File.Exists(SAPInputfilepath))
                {
                    System.IO.File.Delete(SAPInputfilepath);
                    Console.WriteLine("File deleted  " + SAPInputfilepath);
                }
                if (System.IO.File.Exists(PAYROLLOutputfilepath))
                {
                    System.IO.File.Delete(PAYROLLOutputfilepath);

                    Console.WriteLine("File deleted  " + PAYROLLOutputfilepath);
                }

                if (System.IO.File.Exists(PAYROLLInputfilepath))
                {
                    System.IO.File.Delete(PAYROLLInputfilepath);
                    Console.WriteLine("File deleted  " + PAYROLLInputfilepath);
                }



                if (System.IO.File.Exists(filePath))
                {

                    System.IO.File.Copy(filePath, SAPInputfilepath, true);
                    Console.WriteLine("File copied from   " + filePath);
                }

                filePath = Path.Combine(path, Path.GetFileName(PAYROLLfileName));
                if (System.IO.File.Exists(filePath))
                {

                    System.IO.File.Copy(filePath, PAYROLLInputfilepath, true);
                    Console.WriteLine("File copied from   " + filePath);
                }

                using (StreamWriter sw = System.IO.File.CreateText(SAPOutputfilepath))
                {
                    // Open the file to read from. 
                    using (StreamReader sr = System.IO.File.OpenText(SAPInputfilepath))
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            //Console.WriteLine(s);
                            if (s.Contains("OPEN"))
                            {

                                sw.WriteLine(s);
                            }




                        }
                    }
                }

                using (StreamWriter sw = System.IO.File.CreateText(PAYROLLOutputfilepath))
                {
                    // Open the file to read from. 
                    using (StreamReader sr = System.IO.File.OpenText(PAYROLLInputfilepath))
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            //Console.WriteLine(s);


                            sw.WriteLine(s);





                        }
                    }
                }




                if (System.IO.File.Exists(SAPOutputfilepath))
                {



                    string webUrl = args[0];

                    Console.WriteLine("Site Url is : " + args[0]);

                    ClientContext clientContext = new ClientContext(webUrl);
                    List StoreContactList = clientContext.Web.Lists.GetByTitle("Store Contact Information");
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<View><RowLimit>700</RowLimit></View>";
                    ListItemCollection collListItems = StoreContactList.GetItems(camlQuery);
                    clientContext.Load(collListItems);
                    clientContext.ExecuteQuery();
                    if (collListItems.Count > 0)
                    {

                        foreach (ListItem item in collListItems.ToList())
                        {

                            item.DeleteObject();

                            clientContext.ExecuteQuery();
                        }

                    }





                    string[] parts = null;
                    string[] PayRollparts = null;
                    foreach (string line in System.IO.File.ReadAllLines(SAPOutputfilepath))
                    {
                        parts = line.Split(',');




                        IEnumerable<string> Payrollines = System.IO.File.ReadLines(PAYROLLOutputfilepath).Where(x => x.StartsWith(parts[0]));


                        var PayrollData = Payrollines.FirstOrDefault();





                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem oListItem = StoreContactList.AddItem(itemCreateInfo);
                        oListItem["Site"] = parts[0];
                        oListItem["Name"] = parts[1];
                        oListItem["Address"] = parts[5];
                        oListItem["Country"] = parts[9];
                        oListItem["Postcode"] = parts[6];
                        oListItem["Town"] = parts[7];
                        oListItem["County"] = parts[8];
                        oListItem["DC_x0020_Name"] = parts[2];
                        oListItem["Division_x0020_Code_x0020_Name"] = parts[3];
                        oListItem["Reg_x0020_Code"] = parts[4];
                        oListItem["Phone"] = parts[13];
                        oListItem["Fax"] = parts[14];
                        oListItem["Near_x0020_To"] = parts[10];
                        oListItem["Host_x0020_Store"] = parts[11];
                        if (PayrollData != null)
                        {
                            PayRollparts = PayrollData.Split(',');
                            oListItem["First_x0020_Name"] = PayRollparts[4];
                            oListItem["Surname"] = PayRollparts[5];
                            oListItem["Job_x0020_Title"] = PayRollparts[6];
                            oListItem["Division_x0020_Code"] = PayRollparts[2];
                        }
                        oListItem.Update();

                        clientContext.ExecuteQuery();
                        parts = null;
                        PayRollparts = null;
                    }


                }

            }

            catch (Exception e)
            {

                log.WriteLine("{0} Exception caught.", e.Message);
                
            }

        }

    }
 }

