-- statements for test trigger here

EXEC	[dbo].[spAggregateDlDataWithColName]
		@sTablename = N'TableName',
		@EnableMI = 1,
		@EnableHH = 1,
		@EnableDD = 1,
		@UtcTimeColName = N'UtcTimeColName',
		@LocalTimeColName = N'LocalTimeColName',
		@NumericColumnNames = N'NumericColumnNames'
