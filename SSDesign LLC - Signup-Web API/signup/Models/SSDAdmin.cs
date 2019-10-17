using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using FastMember;

namespace SSDesign.Admin
{
    public class SSDProductVersion
    {
        public string ProductVersionNumber { get; set; }
        public bool IsMandatory { get; set; }

        public string MajorRelease { get; set; }
        public string MinorRelease { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        public SSDProductVersion()
        {
            ProductVersionNumber = "0.0.0.0";
            IsMandatory = false;
        }
        public SSDProductVersion(string version, bool mandatory)
        {
            ProductVersionNumber = version;
            IsMandatory = mandatory;

            string[] pieces = ProductVersionNumber.Split('.');

            MajorRelease = pieces[0];
            MinorRelease = pieces[1];
            Date = pieces[2];
            Time = pieces[3];
        }
        public SSDProductVersion(SSDProductVersion copy) : this(copy.ProductVersionNumber, copy.IsMandatory) { }
    }

    public class SSDProduct
    {
        public int PID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Version { get; set; }
        public bool Mandatory { get; set; }

        public SSDProduct()
        {
            PID = -1;
            Name = "NULL";
            Price = 0.00;
            Version = "0.0.0.0";
            Mandatory = false;
        }
        public SSDProduct(int pid, string name, double price, string version, bool mandatory) : this()
        {
            PID = pid;
            Name = name;
            Price = price;
            Version = version;
            Mandatory = mandatory;
        }
        public SSDProduct(SSDProduct copy) : this(copy.PID, copy.Name, copy.Price, copy.Version, copy.Mandatory) {}
    }

    public class SSDPromo
    {
        public string Code { get; set; }
        public double PriceAdj { get; set; }
        public bool SingleUse { get; set; }
        public int PID { get; set; }

        public SSDPromo()
        {
            Code = "";
            PriceAdj = 1.00F;
            SingleUse = true;
            PID = 0;
        }
        public SSDPromo(string code, double priceadj, bool singleuse, int pid) : this()
        {
            Code = code;
            PriceAdj = priceadj;
            SingleUse = singleuse;
            PID = pid;
        }
        public SSDPromo(SSDPromo copy) : this(copy.Code, copy.PriceAdj, copy.SingleUse, copy.PID) { }
    }

    public class SSDTransaction
    {
        public int OrderNumber;
        public int CID;
        public double AmountUSD;
        public string DateTime;
        public string Note;

        public SSDTransaction()
        {
            OrderNumber = -1;
            CID = -1;
            AmountUSD = -1;
            DateTime = "NULL";
            Note = "";
        }
        public SSDTransaction(int order, int cid, double amountUsd, string dateTime, string note = null)
        {
            OrderNumber = order;
            CID = cid;
            AmountUSD = amountUsd;
            DateTime = dateTime;
            Note = note ?? "";
        }
        public SSDTransaction(SSDTransaction copy) : this(copy.OrderNumber, copy.CID, copy.AmountUSD, copy.DateTime, copy.Note) { }
    }

    public class SSDDBConnection : IDisposable
    {
        private SqlConnection _sqlConnect;

        public bool IsOpen { get; set; }
        public bool IsElevated { get; }

        public bool Failed { get; set; }
        public string ErrorMessage { get; set; }

        public SSDDBConnection()
        {
            _sqlConnect = new SqlConnection();

            IsOpen = false;
            IsElevated = false;

            Failed = false;
            ErrorMessage = string.Empty;
        }
        public SSDDBConnection(bool admin) : this()
        {
            IsElevated = admin;
        }

        public void Query(string queryString)
        {
            try
            {
                var _command = _sqlConnect.CreateCommand();
                _command.CommandText = queryString;
                var _reader = _command.ExecuteReader();

                _reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> Query<T>(string queryString) where T : new()
        {
            List<T> result = new List<T>();

            try
            {              
                var _command = _sqlConnect.CreateCommand();
                _command.CommandText = queryString;
                var _reader = _command.ExecuteReader();

                while (_reader.Read())
                {
                    Type type = typeof(T);
                    var accessor = TypeAccessor.Create(type);
                    var typeMembers = accessor.GetMembers();
                    var tuple = new T();

                    for (int i = 0; i < _reader.FieldCount; i++)
                    {
                        if (!_reader.IsDBNull(i))
                        {
                            string fieldName = _reader.GetName(i);

                            if (typeMembers.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
                            {
                                accessor[tuple, fieldName] = _reader.GetValue(i);
                            }
                        }
                    }

                    result.Add(tuple);
                }

                _reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;         
        }
        
        public void Open()
        {
            if (IsElevated)
            {
                _sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["connectStringServer"].ConnectionString);
            }
            else
            {
                _sqlConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["connectStringClient"].ConnectionString);
            }

            try
            {
                _sqlConnect.Open();
                IsOpen = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
                IsOpen = false;

                throw ex;
            }
        }
        public void Close()
        {
            _sqlConnect.Close();
            _sqlConnect.Dispose();
        }
        public void Dispose()
        {
            _sqlConnect.Dispose();
        }
    }
}