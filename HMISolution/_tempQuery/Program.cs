// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Microsoft.Data.Sqlite;
var conn = new SqliteConnection("Data Source=C:\\Work\\samples\\StressTest_1K\\Data\\history.db;Mode=ReadOnly");
conn.Open();
var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT DISTINCT variable_name FROM variable_data LIMIT 10";
using var r = cmd.ExecuteReader();
Console.WriteLine("Variable names in history.db:");
while(r.Read()) Console.WriteLine("  '" + r.GetString(0) + "'");
conn.Close();
