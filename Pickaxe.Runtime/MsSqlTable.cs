using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pickaxe.Runtime
{
    public class MsSqlTable<TRow> : CodeTable<TRow> where TRow : IRow
    {
        public string ConnectionString { get; set; }
        public string Table { get; set; }
        public string[] FieldNames { get; set;}

        private string BuildInsertQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("insert into {0} (", Table);
            for (int x = 0; x < FieldNames.Length - 1; x++)
                query.Append(FieldNames[x] + ",");

            query.Append(FieldNames[FieldNames.Length - 1]);
            query.Append(") values (");

            for (int x = 0; x < FieldNames.Length - 1; x++)
                query.Append("@" + FieldNames[x] + ",");

            query.Append("@" + FieldNames[FieldNames.Length - 1]);
            query.Append(")");

            return query.ToString();
        }

        private void WriteToTable(TRow row)
        {
            using(SqlConnection con = new SqlConnection())
            using(SqlCommand cmd = new SqlCommand())
            {
                con.ConnectionString = ConnectionString;
                con.Open();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = BuildInsertQuery();

                foreach (var name in FieldNames)
                {
                    var value = row.GetType().GetField(name).GetValue(row);
                    cmd.Parameters.AddWithValue(name, value);
                }

                cmd.ExecuteNonQuery();
            }
        }

        public override void Truncate()
        {
            using(SqlConnection con = new SqlConnection())
            using (SqlCommand cmd = new SqlCommand())
            {
                con.ConnectionString = ConnectionString;
                con.Open();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "truncate table " + Table;

                cmd.ExecuteNonQuery();
            }

            base.Truncate();
        }

        public override void Add(TRow row)
        {
            WriteToTable(row);
            base.Add(row);
        }
    }
}
