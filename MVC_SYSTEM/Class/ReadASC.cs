using MVC_SYSTEM.log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class ReadASC
    {
        errorlog geterror = new errorlog();
        private string dirCSV;      //directory of file to import
        private string fileNevCSV;  //name (with extension) of file to import - field
        public string FileNevCSV    //name (with extension) of file to import - property
        {
            get { return fileNevCSV; }
            set { fileNevCSV = value; }
        }
        private long rowCount = 0;
        public void GetDataFromASCFile(string sourcepath, int? userid, string filename, int? NegaraID, int? SyarikatID, int WilayahID, int LadangID, long FileID)
        {
            // creates a System.IO.FileInfo object to retrive information from selected file.
            FileInfo fi = new FileInfo(sourcepath);
            // retrives directory
            dirCSV = fi.DirectoryName.ToString();
            // retrives file name with extension
            FileNevCSV = fi.Name.ToString();

            writeSchema();
            // Creates and opens an ODBC connection
            string strConnString = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + this.dirCSV.Trim() + ";Extensions=asc,csv,tab,txt;Persist Security Info=False";
            string sql_select;
            OdbcConnection conn;
            conn = new OdbcConnection(strConnString.Trim());
            conn.Open();
            //Counts the row number in csv file - with an sql query
            OdbcCommand commandRowCount = new OdbcCommand("SELECT COUNT(*) FROM [" + this.FileNevCSV.Trim() + "]", conn);
            this.rowCount = Convert.ToInt32(commandRowCount.ExecuteScalar());

            // Creates the ODBC command
            sql_select = "select * from [" + this.FileNevCSV.Trim() + "]";
            OdbcCommand commandSourceData = new OdbcCommand(sql_select, conn);

            // Makes on OdbcDataReader for reading data from CSV
            OdbcDataReader dataReader = commandSourceData.ExecuteReader();

            // Creates schema table. It gives column names for create table command.
            //DataTable dt;
            //dt = dataReader.GetSchemaTable();
            DataTable dt = new DataTable();
            dt.Load(dataReader);

            DataColumn F2 = new DataColumn("F2", typeof(int));
            F2.AllowDBNull = true;
            dt.Columns.Add(F2);
            foreach (DataRow row in dt.Rows)
            {
                row["F2"] = userid;
            }
            //if you don't want to allow null-values'
            F2.AllowDBNull = false;

            DataColumn F3 = new DataColumn("F3", typeof(string));
            F3.AllowDBNull = true;
            dt.Columns.Add(F3);
            foreach (DataRow row in dt.Rows)
            {
                row["F3"] = filename;
            }
            //if you don't want to allow null-values'
            F3.AllowDBNull = false;

            DataColumn F4 = new DataColumn("F4", typeof(int));
            F4.AllowDBNull = true;
            dt.Columns.Add(F4);
            foreach (DataRow row in dt.Rows)
            {
                row["F4"] = NegaraID;
            }
            //if you don't want to allow null-values'
            F4.AllowDBNull = false;

            DataColumn F5 = new DataColumn("F5", typeof(int));
            F5.AllowDBNull = true;
            dt.Columns.Add(F5);
            foreach (DataRow row in dt.Rows)
            {
                row["F5"] = SyarikatID;
            }
            //if you don't want to allow null-values'
            F5.AllowDBNull = false;

            DataColumn F6 = new DataColumn("F6", typeof(int));
            F6.AllowDBNull = true;
            dt.Columns.Add(F6);
            foreach (DataRow row in dt.Rows)
            {
                row["F6"] = WilayahID;
            }
            //if you don't want to allow null-values'
            F6.AllowDBNull = false;

            DataColumn F7 = new DataColumn("F7", typeof(int));
            F7.AllowDBNull = true;
            dt.Columns.Add(F7);
            foreach (DataRow row in dt.Rows)
            {
                row["F7"] = LadangID;
            }
            //if you don't want to allow null-values'
            F7.AllowDBNull = false;

            DataColumn F8 = new DataColumn("F8", typeof(long));
            F8.AllowDBNull = true;
            dt.Columns.Add(F8);
            foreach (DataRow row in dt.Rows)
            {
                row["F8"] = FileID;
            }
            //if you don't want to allow null-values'
            F8.AllowDBNull = false;

            using (SqlBulkCopy bc = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["MVC_SYSTEM_CONN"].ConnectionString))
            {
                // Destination table with owner - this example doesn't
                // check the owner and table names!
                bc.DestinationTableName = "[dbo].[tblASCApprovalRawData]";

                // Set the BatchSize.
                bc.BatchSize = 5000;

                //bc.NotifyAfter = 100;
                //bc.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                bc.ColumnMappings.Add("F1", "fldRawData");
                bc.ColumnMappings.Add("F2", "fldUserID");
                bc.ColumnMappings.Add("F3", "fldASCFileName");
                bc.ColumnMappings.Add("F4", "fldNegaraID");
                bc.ColumnMappings.Add("F5", "fldSyarikatID");
                bc.ColumnMappings.Add("F6", "fldWilayahID");
                bc.ColumnMappings.Add("F7", "fldLadangID");
                bc.ColumnMappings.Add("F8", "fldFileID");

                // Starts the bulk copy.
                bc.WriteToServer(dt);

                // Closes the SqlBulkCopy instance
                bc.Close();
            }
            dt.Clear();
            conn.Close();
        }
        private void writeSchema()
        {
            try
            {
                FileStream fsOutput = new FileStream(this.dirCSV + "\\schema.ini", FileMode.Create, FileAccess.Write);
                StreamWriter srOutput = new StreamWriter(fsOutput);
                string s1, s2, s3, s4, s5, s6;

                s1 = "[" + this.FileNevCSV + "]";
                s2 = "ColNameHeader=False";
                s3 = "Format=TabDelimited";
                s4 = "MaxScanRows=0";
                s5 = "CharacterSet=ANSI";
                s6 = "TextDelimiter=^";

                srOutput.WriteLine(s1.ToString() + "\r\n" + s2.ToString() + "\r\n" + s3.ToString() + "\r\n" + s4.ToString() + "\r\n" + s5.ToString() + "\r\n" + s6.ToString());
                srOutput.Close();
                fsOutput.Close();
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }
            finally
            {

            }
        }
    }
}