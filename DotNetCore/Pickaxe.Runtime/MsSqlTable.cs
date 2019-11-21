/* Copyright 2015 Brock Reeve
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
