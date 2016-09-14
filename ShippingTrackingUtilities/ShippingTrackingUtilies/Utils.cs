using System;
using System.Data;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Mail;
using System.Linq;


namespace Utilities
{
    public static class Utils
    {




        public static string ProgramVersionAsString = "1.0.0.115";
        public static bool runningInBatchScreen = false; // if true, the ZuckShipRush.ZuckShipRush wont pop-up messageboxes.

        public static string zuckersDbConnStr = "timeout=600;" +
                                        "user id=netadmin;" +
                                        "data source=db0714;" +  //  OLd APPSVR2 ---> "data source=192.168.11.252;" +
                                        "Application Name=ZuckersEbay;" +
                                        "password=\"1z=1z\";" +
                                         "initial catalog=zuckers01";

        public static string mysqlserver = "server=166.62.36.116;uid=awatch4u_bayuser;pwd=Rolex1@#;database=awatch4u_bayonline";  //server=166.62.36.116

        public static bool showMenu = false;
        private static Random random = new Random();

        // store all Exeptions messages during the app run and then sent them in one email 
        // if program is running on Prodcution Mode...
        public static StringBuilder sbErrMsgs = new StringBuilder();

        //Unit Test is a flag to switch between life and test environment.
        public static bool unitTest = true;


        //current loged username.
        public static string C_UserName = Environment.UserName;



        public static bool DataSetToLocalFile(DataSet ds, string fileName)
        { // write a dataset to a local file...
            bool result = true;
            try
            {
                if (ds != null)
                {
                    ds.AcceptChanges();
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    if (makeFullPath(fileName))  /// create all possible dirs if doesn't exits... 
                        ds.WriteXml(fileName);
                    else
                        result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogException(ex);
            }
            return result;
        }


        public static string DataTableToHtmlTable(DataTable table)
        {
            return DataTableToHtmlTable(table, "#FFFFFF", "#FFFFFF");
        }

        public static string DataTableToHtmlTable(DataTable table, string color1, string color2)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("<table class=\"styledTable\" cellspacing=\"0\">");
            sb.AppendLine("<table width=\"100%\" border = \"1\" cellspacing=\"0\">");
            sb.AppendLine("<tr>");
            for (int i = 0; i <= table.Columns.Count - 1; i++)
            {
                sb.AppendLine("<td <strong>" +
                table.Columns[i].ColumnName + "</strong></td>");
            }
            sb.AppendLine("</tr>");
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                sb.AppendLine("<tr " + (i % 2 == 0 ? "bgcolor=\"" + color1 + "\"" : "bgcolor=\"" + color2 + "\"") + ">");
                for (int j = 0; j <= table.Columns.Count - 1; j++)
                {
                    if (i % 2 == 0)
                        sb.AppendLine("<td>" +
                        table.Rows[i][j].ToString() + "</td>");
                    else
                        sb.AppendLine("<td>" +
                        table.Rows[i][j].ToString() + "</td>");
                }
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string DataTableToHtmlTable(DataView dtView, string color1, string color2)
        {
            DataTable table;
            table = dtView.ToTable();
            return DataTableToHtmlTable(table, color1, color2);
        }

        public static bool LocalFileToDataSet(DataSet ds, string fileName)
        { // write a dataset to a local file...
            bool result = true;
            try
            {
                if (File.Exists(fileName))
                {
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                            ds.Tables[0].Rows.Clear();
                        //ds.Tables.Clear();
                    }
                    ds.ReadXml(fileName);

                }
                else
                    result = false;

            }
            catch (Exception ex)
            {
                result = false;
                LogException(ex);
            }
            return result;
        }



        public static bool makeFullPath(string fname)
        {  // checks the directory/subdirectory of a file name and try to make 
            // the full path to make avialable the file....
            bool result = true;
            try
            {
                string path = fname.Substring(0, fname.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                if (path.Length > 0)
                {
                    if (System.IO.Directory.Exists(path) == false)
                        System.IO.Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Write to the Error File the Exception ocurred
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            try
            {
                StreamWriter sw = new StreamWriter(@"ErrorLog.txt", true);
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine(DateTime.Now.ToString());
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("STrace: " + ex.StackTrace);
                if (ex.InnerException != null)
                    sw.WriteLine("InnerException: " + ex.InnerException.ToString());
                sw.WriteLine(new string('-', 200));
                sw.Close();

                //put the current exeption message into the Message Variable to send email...
                Utils.sbErrMsgs.Append(DateTime.Now.ToString() + ex.Message + "\n");
                Utils.sbErrMsgs.Append(DateTime.Now.ToString() + ex.StackTrace + "\n");
                if (Utils.unitTest)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch { }
        }

        /// <summary>
        /// Writes to the Error File a custom message
        /// </summary>
        /// <param name="msg">Custom message that will be loged in the error file</param>
        public static void LogCustomMsg(string msg)
        {
            try
            {
                StreamWriter sw = new StreamWriter(@"General_Log.txt", true);
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine(DateTime.Now.ToString());
                sw.WriteLine("Message: " + msg);
                sw.WriteLine(new string('-', 200));
                sw.Close();
            }
            catch { }
        }

        /// <summary>
        /// Write to the LOG File a custom message
        /// </summary>
        /// <param name="msg">The message to be logged.</param>
        public static void WriteLog(string msg)
        {
            StreamWriter sw = new StreamWriter(@"ReportLog.txt", true);
            sw.WriteLine(DateTime.Now.ToString() + "\t" + msg);
            sw.Close();
            if (Utils.unitTest)
            {
                Console.WriteLine(DateTime.Now.ToString() + "\t" + msg);
            }
        }

        /// <summary>
        /// Write to the LOG File a custom message, also accepts a flag to display it on console
        /// </summary>
        /// <param name="msg">The message to be logged.</param>
        /// <param name="displayOnConsole">if True, the message is being displayed on the console.</param>
        public static void WriteLog(string msg, bool displayOnConsole)
        {
            StreamWriter sw = new StreamWriter(@"ReportLog.txt", true);
            sw.WriteLine(DateTime.Now.ToString() + "\t" + msg);
            sw.Close();
            if (displayOnConsole)
                Console.WriteLine(msg);
            if (Utils.unitTest)
            {
                Console.WriteLine(DateTime.Now.ToString() + "\t" + msg);
            }
        }



        public static bool sendMailFromZuckSvr(string from, string to, string cc, string bcc, string subject,
              string body, List<string> attchFiles)
        {
            bool result = true;
            try
            {
                //More than one Send mail To
                string[] arrTo = to.Split(';');
                // zuckers mail server...        


                if (body == null) { body = ""; }
                body = body + "<br>" + "<br>" + "***Internal message - b6QgA2hsnc28UEzA***" + "<br>";
                MailMessage mail = new MailMessage(from, arrTo[0], subject, body);
                //Add additional Send To
                if (arrTo.Length > 1)
                {
                    for (int i = 1; i < arrTo.Length; i++)
                    {
                        mail.To.Add(arrTo[i].Trim());
                    }
                }

                mail.IsBodyHtml = true;
                if (cc.Trim().Length > 0)
                    mail.CC.Add(cc);

                // mail.CC.Add("cesar@zuckers.com");
                // mail.CC.Add("shmuel@zuckers.com");

                if (attchFiles.Count > 0)
                    for (int i = 0; i < attchFiles.Count; i++)
                        mail.Attachments.Add(new Attachment(attchFiles[i]));


                if (bcc.Trim().Length > 0)
                    mail.Bcc.Add(bcc);
                SmtpClient smtpMail = new SmtpClient("mailsvr1.zuckers.com");
                smtpMail.Send(mail);
            }
            catch (Exception ex)
            {
                result = false;
                LogException(ex);
            }
            return result;
        }

        #region ZuckMail
        /// <summary>
        ///  Send Email from our internal Email Server (ZUCKERS).
        /// </summary>
        /// <param name="from"> Sender Email Address.</param>
        /// <param name="to"> Receipt address(es) separeted by char ';'.</param>        
        /// </param>
        public static bool sendMailFromZuckSvr(string from, string to, string subject, string body)
        {
            return sendMailFromZuckSvr(from, to, "", "", subject, body, "");
        }

        /// <summary>
        ///  Send Email from our internal Email Server (ZUCKERS).
        /// </summary>
        /// <param name="from"> Sender Email Address.</param>
        /// <param name="to"> Receipt address(es) separeted by char ';'.</param>
        /// <param name="attchFiles">File(s) to be attached to the message, each one separeted by char ';'.</param>
        /// </param>
        public static bool sendMailFromZuckSvr(string from, string to, string subject,
              string body, string attchFiles)
        {
            return sendMailFromZuckSvr(from, to, "", "", subject, body, attchFiles);
        }

        /// <summary>
        ///  Send Email from our internal Email Server (ZUCKERS).
        /// </summary>
        /// <param name="from"> Sender Email Address.</param>
        /// <param name="to"> Receipt address(es) separeted by char ';'.</param>
        /// <param name="cc"> CC Receipt address(es) separeted by char ';'.</param>
        /// <param name="bcc"> BCC Receipt address(es) separeted by char ';'.</param>
        /// <param name="maxCol"> The maximun number of columns to export. Zero (0) Means ALL Columns.
        /// </param>
        public static bool sendMailFromZuckSvr(string from, string to, string cc, string bcc, string subject,
              string body)
        {
            return sendMailFromZuckSvr(from, to, cc, bcc, subject, body, "");
        }

        /// <summary>
        ///  Send Email from our internal Email Server (ZUCKERS).
        /// </summary>
        /// <param name="from"> Sender Email Address.</param>
        /// <param name="to"> Receipt address(es) separeted by char ';'.</param>
        /// <param name="cc"> CC Receipt address(es) separeted by char ';'.</param>
        /// <param name="bcc"> BCC Receipt address(es) separeted by char ';'.</param>
        /// <param name="body">The String with HTML format for the Mail Body Message.</param>
        /// <param name="attchFiles">File(s) to be attached to the message, each one separeted by char ';'.</param>
        /// </param>
        public static bool sendMailFromZuckSvr(string from, string to, string cc, string bcc, string subject,
              string body, string attchFiles)
        {
            bool result = true;
            try
            {
                if (body == null) { body = ""; }
                body = body + "<br>" + "<br>" + "***Internal message - b6QgA2hsnc28UEzA***" + "<br>";

                //More than one Send mail To
                string[] arrTo = to.Split(';');
                // zuckers mail server...                
                MailMessage mail = new MailMessage(from, arrTo[0], subject, body);
                //Add additional Send To
                if (arrTo.Length > 1)
                {
                    for (int i = 1; i < arrTo.Length; i++)
                    {
                        mail.To.Add(arrTo[i].Trim());
                    }
                }
                mail.IsBodyHtml = true;
                if (cc.Trim().Length > 0)
                {
                    string[] arrCC = cc.Split(';');
                    mail.CC.Add(arrCC[0]);
                    for (int i = 1; i < arrCC.Length; i++)
                    {
                        mail.CC.Add(arrCC[i].Trim());
                    }
                }

                if (bcc.Trim().Length > 0)
                {
                    string[] arrBCC = bcc.Split(';');
                    mail.Bcc.Add(arrBCC[0]);
                    for (int i = 1; i < arrBCC.Length; i++)
                    {
                        mail.Bcc.Add(arrBCC[i].Trim());
                    }
                }

                //attach files
                if (attchFiles.Trim().Length > 0)
                {
                    string[] files = attchFiles.Split(';');

                    foreach (string f in files)
                    {
                        try
                        {
                            if (File.Exists(f))
                            {
                                Attachment atc = new Attachment(f);
                                mail.Attachments.Add(atc);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException(ex);
                        }
                    }

                }


                SmtpClient smtpMail = new SmtpClient("mailsvr1.zuckers.com");
                smtpMail.Send(mail);
            }
            catch (Exception ex)
            {
                result = false;
                LogException(ex);
            }
            return result;
        }
        #endregion ZuckMail

        /// <summary>
        ///  Gets a unique file name.
        /// </summary>
        /// 
        /// <param name="FullFileName"> The current file you try to use.</param>
        /// <param name="ReturnFullPath"> If True Returns the full file path, otherwise just the file name.</param>       
        /// </param>
        public static string GetUniqueFilename(string FullFileName, bool ReturnFullPath)
        {
            int count = 0;
            string Name = "";
            if (System.IO.File.Exists(FullFileName))
            {
                System.IO.FileInfo f = new System.IO.FileInfo(FullFileName);
                if (!string.IsNullOrEmpty(f.Extension))
                {
                    Name = f.FullName.Substring(0, f.FullName.LastIndexOf('.'));
                }

                else
                {
                    Name = f.FullName;
                }

                while (File.Exists(FullFileName))
                {
                    count++;
                    FullFileName = Name + count.ToString() + f.Extension;
                }

            }
            if (!ReturnFullPath)
            {
                FullFileName = FullFileName.Substring(
                    FullFileName.LastIndexOf("\\") + 1);
            }
            return FullFileName;
        }




        /// <summary>
        ///  Export a DataTable to a Flat Text File.
        /// </summary>
        /// <param name="dt"> Get the data table.</param>
        /// <param name="file"> Specify the file name to write to.</param>
        /// <param name="maxCol"> The maximun number of columns to export. Zero (0) Means ALL Columns.
        /// </param>
        public static bool dataTableToFlatFile(DataTable dt, string file, string separator)
        {
            bool result = true;
            try
            {
                StreamWriter f = new StreamWriter(file);
                for (int j = 0; j < dt.Columns.Count; j++)
                    f.Write(dt.Columns[j].ColumnName +
                        (j == dt.Columns.Count - 1 ? "" : separator));
                f.WriteLine("");

                for (int i = 0; (i < dt.Rows.Count); i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        f.Write(dt.Rows[i][j].ToString() + (j == dt.Columns.Count - 1 ? "" : separator));
                    }
                    f.WriteLine("");

                }
                f.Close();
            }
            catch (Exception ex)
            {
                result = false;
                Utils.LogException(ex);
            }
            return result;
        }


        /// <summary>
        ///  Export a DataView to a Flat Text File.
        /// </summary>
        /// <param name="dt"> Get the DataView.</param>
        /// <param name="file"> Specify the file name to write to.</param>
        /// <param name="maxCol"> The maximun number of columns to export. Zero (0) Means ALL Columns.
        /// </param>
        public static bool dataTableToFlatFile(DataView dView, string file, string separator)
        {
            DataTable table = dView.ToTable();
            return dataTableToFlatFile(table, file, separator);
        }

        /// <summary>
        /// Show on Console the select table of dataset.
        /// </summary>        
        public static void showData(DataSet d, int tblIndex, string separator)
        {
            for (int j = 0; j < d.Tables[tblIndex].Columns.Count; j++)
                Console.Write(d.Tables[tblIndex].Columns[j].ColumnName +
                    (j == d.Tables[tblIndex].Columns.Count - 1 ? "\n" : separator));

            for (int i = 0; i < d.Tables[tblIndex].Rows.Count; i++)
            {
                for (int j = 0; j < d.Tables[tblIndex].Columns.Count; j++)
                {
                    Console.Write(d.Tables[tblIndex].Rows[i][j].ToString() + (j == d.Tables[tblIndex].Columns.Count - 1 ? "\n" : separator));
                }

            }
        }

        /// <summary>
        /// Gets a ZeroByte File name in the temp User Directory
        /// </summary>
        /// <returns> String File Name </returns>
        public static string GetTempFileName()
        {
            return System.IO.Path.GetTempFileName();
        }



        /// <summary>
        /// Write the dataset tables and columns name in a flat file.
        /// </summary>        
        public static void writeDatasetTables(DataSet ds, string fname)
        {
            StreamWriter sw = new StreamWriter(fname);
            foreach (DataTable dt in ds.Tables)
            {
                sw.WriteLine("TABLE: " + dt.TableName);
                foreach (DataColumn dc in dt.Columns)
                {
                    sw.Write(dc.ColumnName + "\t");
                }
                sw.WriteLine("".PadRight(150, '-'));
            }
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// Return a SubString between to flags (strings), starting on startSrch Position
        /// </summary>
        /// <param name="fullstring">Original String to search on</param>
        /// <param name="strBegin">String to being</param>
        /// <param name="strEnd">String to end</param>
        /// <param name="startSrch">position where strBegin is started to search</param>
        /// <returns>String</returns>
        public static string strBetween(string fullstring, string strBegin, string strEnd, int startSrch)
        {
            string result = "";
            try
            {
                if (startSrch >= 0)
                {
                    int begin = -1;
                    int end = -1;
                    int totalChr = 0;

                    begin = fullstring.IndexOf(strBegin, startSrch);

                    if (begin > 0)
                        end = fullstring.IndexOf(strEnd, begin);

                    if (end > 0)
                        totalChr = (end - strBegin.Length) - begin;  //end - strEnd.Length - begin

                    if (begin > 0 && end > 0 && begin < end)
                    {
                        result = fullstring.Substring(begin + strBegin.Length, totalChr);
                    }
                }
            }
            catch { }
            return result;
        }


        /// <summary>
        /// Check is a String can be converted to a Valid Double.
        /// </summary>
        /// <param name="Value">String Value to Evalate</param>
        /// <returns></returns>
        public static bool isValidDouble(string Value)
        {
            bool result = true;
            try
            {
                double.Parse(Value);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Check is a String can be converted to a Valid Integer.
        /// </summary>
        /// <param name="Value">String Value to Evalate</param>
        /// <returns></returns>
        public static bool isValidInteger(string Value)
        {
            bool result = true;
            try
            {
                int.Parse(Value);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Check is a String can be converted to a Valid Unsigned Integer.
        /// </summary>
        /// <param name="Value">String Value to Evalate</param>
        /// <returns></returns>
        public static bool isValidUnsignedInteger(string Value)
        {
            bool result = true;
            try
            {
                uint.Parse(Value);
            }
            catch
            {
                result = false;
            }
            return result;
        }



        public static string ValidatePhone(string phone)
        {
            int l = phone.Length;
            StringBuilder sb = new StringBuilder();
            string result = string.Empty;
            for (int i = 0; i < l; i++)
                if (char.IsDigit(phone[i]))
                    sb.Append(phone[i]);

            if (sb.Length > 0)
            {
                if (sb[0] == '1' || sb[0] == '0')
                    sb.Remove(0, 1);
            }
            if (sb.Length == 10)
            {
                sb.Insert(6, '-');
                sb.Insert(3, '-');
                result = sb.ToString();
            }
            return result;
        }


        public static string ToTitleCase(string value)
        {
            System.Globalization.CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            return ci.TextInfo.ToTitleCase(value);
        }


        public static string[] GetFilesSortedByDate(string scanDir, string searchPattern)
        {
            List<string> sortedFiles = new List<string>();
            try
            {
                string[] AllFiles = Directory.GetFiles(scanDir, searchPattern);

                if (AllFiles.Length > 1) //need to sort...
                {
                    List<FileInfo> fInfo = new List<FileInfo>(AllFiles.Length);
                    foreach (string s in AllFiles)
                    {
                        fInfo.Add(new FileInfo(s));
                    }

                    var myFiles = from q in fInfo
                                  orderby q.LastWriteTime
                                  select q;
                    foreach (var f in myFiles)
                    {
                        sortedFiles.Add(f.FullName);
                    }
                }
                else //just zero or 1 file found, no sorting needed.
                {
                    foreach (string s in AllFiles)
                    {
                        sortedFiles.Add(s);
                    }
                }
            }
            catch
            {
            }
            return sortedFiles.ToArray();
        }


        public static string[] GetFilesFilterByDate(string scanDir, string searchPattern, DateTime dtFrom, DateTime dtTo)
        {
            List<string> sortedFiles = new List<string>();
            try
            {
                string[] AllFiles = Directory.GetFiles(scanDir, searchPattern);

                if (AllFiles.Length > 0) //need to sort...
                {
                    List<FileInfo> fInfo = new List<FileInfo>(AllFiles.Length);
                    foreach (string s in AllFiles)
                    {
                        fInfo.Add(new FileInfo(s));
                    }

                    var myFiles = from q in fInfo
                                  where q.LastWriteTime >= dtFrom && q.LastWriteTime <= dtTo
                                  orderby q.LastWriteTime
                                  select q;
                    foreach (var f in myFiles)
                    {
                        sortedFiles.Add(f.FullName);
                    }
                }
            }
            catch
            {
            }
            return sortedFiles.ToArray();
        }

        public static string[] GetFilesOlderThanDate(string scanDir, string searchPattern, DateTime dtOlderThan)
        {
            List<string> sortedFiles = new List<string>();
            try
            {
                string[] AllFiles = Directory.GetFiles(scanDir, searchPattern);

                if (AllFiles.Length > 0) //need to sort...
                {
                    List<FileInfo> fInfo = new List<FileInfo>(AllFiles.Length);
                    foreach (string s in AllFiles)
                    {
                        fInfo.Add(new FileInfo(s));
                    }

                    var myFiles = from q in fInfo
                                  where q.LastWriteTime < dtOlderThan
                                  orderby q.LastWriteTime
                                  select q;
                    foreach (var f in myFiles)
                    {
                        sortedFiles.Add(f.FullName);
                    }
                }
            }
            catch
            {
            }
            return sortedFiles.ToArray();
        }

        public static string[] GetFilesNewerThanDate(string scanDir, DateTime dtNewerThan, string searchPattern)
        {
            List<string> sortedFiles = new List<string>();
            try
            {
                string[] AllFiles = Directory.GetFiles(scanDir, searchPattern);

                if (AllFiles.Length > 0) //need to sort...
                {
                    List<FileInfo> fInfo = new List<FileInfo>(AllFiles.Length);
                    foreach (string s in AllFiles)
                    {
                        fInfo.Add(new FileInfo(s));
                    }

                    var myFiles = from q in fInfo
                                  where q.LastWriteTime > dtNewerThan
                                  orderby q.LastWriteTime
                                  select q;

                    foreach (var f in myFiles)
                    {
                        sortedFiles.Add(f.FullName);
                    }
                }
            }
            catch
            {
            }
            return sortedFiles.ToArray();
        }

        /// <summary>
        /// Returns only digits (0 to 9) from the specified string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string OnlyDigitsFromString(string source)
        {
            StringBuilder sb = new StringBuilder(source.Length);
            foreach (char c in source)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static DateTime GetOldestFileInFolder(string scanDir, string searchPattern)
        {
            DateTime oldest = DateTime.Now;

            try
            {
                string[] AllFiles = Directory.GetFiles(scanDir, searchPattern);

                if (AllFiles.Length > 0) //need to sort...
                {
                    List<FileInfo> fInfo = new List<FileInfo>(AllFiles.Length);

                    foreach (string s in AllFiles)
                    {

                        fInfo.Add(new FileInfo(s));
                    }

                    var myFiles = from q in fInfo
                                  orderby q.LastWriteTime
                                  select q;
                    foreach (var f in myFiles)
                    {

                        //sortedFiles.Add(f.FullName);
                        oldest = f.LastWriteTime;
                        //break;
                    }
                }
            }
            catch
            {
            }
            return oldest;
        }



        public static DataTable TabFileStandardToDataTable(string TabFileName, bool ColNameHeader)
        {
            return TabFileStandardToDataTable(TabFileName, ColNameHeader, string.Empty);
        }
        public static DataTable TabFileStandardToDataTable(string TabFileName, bool ColNameHeader, string sProgram)
        {
            DataTable dt = new DataTable();
            try
            {
                StreamReader reader = new StreamReader(TabFileName);

                //reader.ReadLine();
                if (reader.Peek() > 0)
                {
                    string[] headers = reader.ReadLine().Split('\t');
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dt.Columns.Add(headers[i]);
                    }
                }

                while (reader.Peek() > 0)
                {
                    string[] fields = reader.ReadLine().Split('\t');

                    DataRow r = dt.NewRow();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        r[i] = fields[i];
                    }
                    dt.Rows.Add(r);

                }

                reader.Close();

            }
            catch (Exception ex)
            {
                LogException(ex);
                dt = null;
            }
            return dt;
        }



        public static string ConvertToFullAscii(string unknownStringValue)
        {
            if (string.IsNullOrEmpty(unknownStringValue)) return string.Empty; //return nothing but NOT NULL.
            StringBuilder sb = new StringBuilder();
            foreach (char c in unknownStringValue)
            {
                char myChar = ReplaceWellKnownNotAsciiCharToEquivalent(c);
                sb.Append(
                                     Encoding.ASCII.GetString(
                                        Encoding.Convert(
                                            Encoding.UTF8,
                                            Encoding.GetEncoding(
                                                Encoding.ASCII.EncodingName,
                                                new EncoderReplacementFallback(string.Empty),
                                                new DecoderExceptionFallback()
                                                ),
                                            Encoding.UTF8.GetBytes(myChar.ToString())))
                                    );
            }
            return sb.ToString();
        }

        public static char ReplaceWellKnownNotAsciiCharToEquivalent(char c)
        {
            char myChar = c;

            if (NonAsciiEquivalent.A_Values.Contains(c))
                myChar = 'A';
            else if (NonAsciiEquivalent.E_Values.Contains(c))
                myChar = 'E';
            else if (NonAsciiEquivalent.H_Values.Contains(c))
                myChar = 'H';
            else if (NonAsciiEquivalent.I_Values.Contains(c))
                myChar = 'I';
            else if (NonAsciiEquivalent.N_Values.Contains(c))
                myChar = 'N';
            else if (NonAsciiEquivalent.O_Values.Contains(c))
                myChar = 'O';
            else if (NonAsciiEquivalent.T_Values.Contains(c))
                myChar = 'T';
            else if (NonAsciiEquivalent.U_Values.Contains(c))
                myChar = 'U';
            else if (NonAsciiEquivalent.W_Values.Contains(c))
                myChar = 'W';
            else if (NonAsciiEquivalent.X_Values.Contains(c))
                myChar = 'X';
            else if (NonAsciiEquivalent.Y_Values.Contains(c))
                myChar = 'Y';


            /*if (NonAsciiEquivalents == null && File.Exists("NonAsciiEquivalent.txt")) //Lazy initialization.
            {
                try
                {
                    NonAsciiEquivalents = new Dictionary<string, char>();
                    string[] lines = File.ReadAllLines("NonAsciiEquivalent.txt");
                    foreach (string s in lines)
                    {
                        string[] flds = s.Split('\t');
                        if (flds.Length >= 2)
                        {
                            NonAsciiEquivalents.Add(flds[0].Trim(), flds[1].Trim()[0]);
                        }
                    }
                }
                catch { }
            }

            if (NonAsciiEquivalents != null)
            {
                foreach (KeyValuePair<string, char> pair in NonAsciiEquivalents)
                {
                    if (pair.Key.Contains(c))
                    {
                        myChar = pair.Value;
                        break;
                    }
                }
            }*/
            return myChar;
        }


        public static bool IsDatasetWithDataOnTable1(DataSet ds)
        {
            return (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0);
        }

        public static bool IsDatasetWithDataOnTable(DataSet ds, int tableIndex)
        {
            try
            {
                return ds.Tables[tableIndex].Rows.Count > 0;
            }
            catch
            {
                return false;
            }
        }


        public static bool IsValidInt(string value)
        {
            try
            {
                int.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int TryToConvertToInt(string value, int defaultIfFail)
        {
            int r = defaultIfFail;
            try
            {
                r = int.Parse(value.Trim());
            }
            catch
            {
                r = defaultIfFail;
            }
            return r;
        }




        /// <summary>
        /// Check for a value and returns empty if null, otherwise returns the current value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GiveMeEmptyIfNull(string value)
        {
            return (string.IsNullOrEmpty(value) ? string.Empty : value);
        }

        public static string DataTableToXmlString(DataTable dt)
        {
            string rt = string.Empty;
            if (string.IsNullOrEmpty(dt.TableName)) dt.TableName = "Data";
            using (var sw = new StringWriter())
            {
                System.Xml.XmlWriterSettings sett = new System.Xml.XmlWriterSettings();
                sett.Indent = true;
                sett.OmitXmlDeclaration = false;

                using (var xw = System.Xml.XmlWriter.Create(sw, sett))
                {

                    dt.WriteXml(xw);
                }
                rt = sw.ToString();
            }
            return rt;
        }


        public static bool DataTableToCSV(DataTable dt, string csvFile)
        {
            return DataTableToCSV(dt, csvFile, true);
        }
        public static bool DataTableToCSV(DataTable dt, string csvFile, bool writeColumnNames)
        {
            bool r = true;
            try
            {
                // Write sample data to CSV file
                using (ReadWriteCsv.CsvFileWriter writer = new ReadWriteCsv.CsvFileWriter(csvFile))
                {
                    //writing columnNames
                    if (writeColumnNames)
                    {
                        ReadWriteCsv.CsvRow rowCols = new ReadWriteCsv.CsvRow();
                        foreach (DataColumn c in dt.Columns)
                            rowCols.Add(c.ColumnName);
                        writer.WriteRow(rowCols);
                    }
                    foreach (DataRow dr in dt.Rows) //writing data.
                    {
                        ReadWriteCsv.CsvRow row = new ReadWriteCsv.CsvRow();
                        for (int j = 0; j < dt.Columns.Count; j++)
                            row.Add(dr[j].ToString());
                        writer.WriteRow(row);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                r = false;
            }
            return r;
        }

        public static DataTable CsvToDataTable(string csvFile, bool colNamesIn1stRow, bool ignoreExtraDataColumnsIfFoundAfterHeaderColumns)
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable();
                List<string> firstRowDataColumns = new List<string>(); // holder first row data/columnnames
                //int rowNumber = 0;
                int colNumber = 0;
                bool isFirstRow = true;
                using (ReadWriteCsv.CsvFileReader reader = new ReadWriteCsv.CsvFileReader(csvFile))
                {
                    dt = new DataTable();
                    ReadWriteCsv.CsvRow row = new ReadWriteCsv.CsvRow();
                    while (reader.ReadRow(row)) //rows loop
                    {
                        colNumber = 0;
                        DataRow dtRow = null;
                        if (!isFirstRow)
                        {
                            dtRow = dt.NewRow();
                        }
                        foreach (string s in row) //column loop
                        {
                            if (isFirstRow)
                            {
                                if (colNamesIn1stRow)
                                    dt.Columns.Add(s);
                                else
                                {
                                    dt.Columns.Add("Column" + (colNumber + 1).ToString());
                                    firstRowDataColumns.Add(s);
                                }
                            }
                            else
                            {
                                if (colNumber < dt.Columns.Count)
                                {
                                    dtRow[colNumber] = s;
                                }
                                else
                                {
                                    if (!ignoreExtraDataColumnsIfFoundAfterHeaderColumns)
                                    {
                                        Console.WriteLine("Invalid column found (more columns in the datarow than the header of the file!)");
                                        throw new Exception("Invalid column found (more columns in the datarow than the header of the file!)");
                                    }
                                }
                            }
                            colNumber++;
                        } //column loop
                        if (isFirstRow)
                        {
                            isFirstRow = false;
                            if (firstRowDataColumns.Count > 0)
                            {
                                int i = 0;
                                dtRow = dt.NewRow();
                                foreach (string s in firstRowDataColumns)
                                {
                                    dtRow[i] = s;
                                    i++;
                                }
                                dt.Rows.Add(dtRow);
                            }
                        }
                        else
                            dt.Rows.Add(dtRow);
                        // Console.WriteLine();
                    } //rows loop
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dt = null;
            }
            return dt;
        }

        public static string StrLeft(string str, int numOfChar)
        {
            if (numOfChar < 1 || string.IsNullOrEmpty(str)) return string.Empty;
            if (str.Length >= numOfChar)
                return str.Substring(0, numOfChar);
            else
                return str;
        }

        public static string StrRight(string str, int numOfChar)
        {
            if (numOfChar < 1 || string.IsNullOrEmpty(str)) return string.Empty;
            if (str.Length >= numOfChar)
                return str.Substring(str.Length - numOfChar, numOfChar);
            else
                return str;
        }

        public static void ClearStringBuilder(StringBuilder strBld)
        {
            if (strBld != null && strBld.Length > 0)
                strBld.Remove(0, strBld.Length);
        }

        /// <summary>
        /// Try to go thru the elements in the order of path, when reach the latest, return its 'value' as string.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string XElementTryToGetValue(System.Xml.Linq.XElement elem, List<string> path)
        {
            string r = string.Empty;
            if (elem != null)
            {
                while (path.Count > 0)
                {
                    try
                    {

                        elem = elem.Element(path[0]);
                        path.RemoveAt(0);
                        if (path.Count == 0)
                            r = elem.Value;
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            return r;
        }

        /// <summary>
        /// Splits the current data table into N datatables, assigning the rows into the howManyTables as the max# of possible datatables.
        /// </summary>
        /// <param name="dt">The source of data</param>
        /// <param name="howManyTables">the max number of data tables in which the rows will be divided to.</param>
        /// <returns></returns>
        public static List<DataTable> SplitDataTables(DataTable dt, int howManyTables)
        {
            List<DataTable> list = new List<DataTable>(howManyTables);
            if (dt != null && dt.Rows.Count > 0 && howManyTables > 0)
            {
                int totalRows = dt.Rows.Count;
                int rowsPerTable = totalRows / howManyTables;
                int assignedRows = 0;
                int totalAssignedRows = 0;

                if (totalRows < howManyTables)
                    rowsPerTable = totalRows;


                DataTable tbl = dt.Clone();
                while (totalAssignedRows < totalRows)
                {
                    if (assignedRows > 0 && assignedRows >= rowsPerTable && list.Count < howManyTables - 1)
                    {
                        assignedRows = 0;
                        list.Add(tbl);
                        tbl = dt.Clone();
                    }
                    tbl.ImportRow(dt.Rows[totalAssignedRows]);
                    totalAssignedRows++;
                    assignedRows++;
                }
                if (tbl.Rows.Count > 0)
                {
                    list.Add(tbl);
                }
            }
            return list;
        }

        /// <summary>
        /// Returns a Datatable with all row of all tables in the list, as long as the tables are the same schema. It returns nulls if error occur.
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static DataTable MergeDataTables(List<DataTable> tables)
        {
            DataTable dt = null;
            try
            {
                dt = tables[0].Clone();
                foreach (DataTable t in tables)
                    foreach (DataRow r in t.Rows)
                        dt.ImportRow(r);
            }
            catch
            {
                dt = null;
            }
            return dt;
        }


        public static bool dataTableToFlatFile(DataTable dt, string file, string separator, string PreHeader, bool bColumnNames, bool bReplaceTabs)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            bool result = true;
            try
            {
                StreamWriter f = new StreamWriter(file);

                if (PreHeader.Length > 0)
                    foreach (string str in PreHeader.Split('\n'))
                        f.WriteLine(str);

                if (bColumnNames)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                        f.Write(dt.Columns[j].ColumnName +
                            (j == dt.Columns.Count - 1 ? "" : separator));
                    f.WriteLine("");
                }

                for (int i = 0; (i < dt.Rows.Count); i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (bReplaceTabs)
                        {
                            f.Write(dt.Rows[i][j].ToString().Replace("\t", " ").Replace("\n", " ") + (j == dt.Columns.Count - 1 ? "" : separator));
                        }
                        else
                        {
                            f.Write(dt.Rows[i][j].ToString() + (j == dt.Columns.Count - 1 ? "" : separator));
                        }
                    }
                    f.WriteLine("");

                }
                f.Close();
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

    }  //Utils
} // NameSpace...
