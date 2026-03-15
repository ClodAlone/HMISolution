CREATE TRIGGER [dbo].[DataLogger_AFTER_INSERT] 
   ON [dbo].[DataLogger]
   AFTER INSERT
   AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
DECLARE	@return_value int

EXEC	@return_value = [dbo].[spAggregateDlDataWithColName]
		@sTablename = N'TableName',
		@EnableMI = 1,
		@EnableHH = 1,
		@EnableDD = 1,
		@UtcTimeColName = N'UtcTimeColName',
		@LocalTimeColName = N'LocalTimeColName',
		@NumericColumnNames = N'NumericColumnNames'
END