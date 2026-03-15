Create PROCEDURE [dbo].[spModifyDlDataWithColName]
  @sTablename nvarchar(128), @EnableMI bit, @EnableHH bit, @EnableDD bit ,@UtcTimeColName Nvarchar(50) ,@LocalTimeColName Nvarchar(50),@NumericColumnNames nvarchar(max) 
  --with Encryption
AS
Begin 
	SET NOCOUNT ON;
	Declare @SourceTable nvarchar(128)
	SET DateFirst 1;
	if @UtcTimeColName is null begin  set @UtcTimeColName ='UtcTimeCol' end
	if @LocalTimeColName is null begin  set @LocalTimeColName ='LocalTimeCol' end
    IF @EnableMI=1
		begin
		    set @SourceTable= @sTablename 
            exec  spModifyTablesWithColName  @sTablename,@SourceTable,'MI',@UtcTimeColName ,@LocalTimeColName,@NumericColumnNames 
		End
	IF @EnableHH=1
		begin
		    set @SourceTable= @sTablename+'_MI' 
            exec  spModifyTablesWithColName  @sTablename,@SourceTable,'HH',@UtcTimeColName ,@LocalTimeColName,@NumericColumnNames
		End
	IF @EnableDD=1
		begin
		    set @SourceTable= @sTablename+'_HH' 
            exec  spModifyTablesWithColName  @sTablename,@SourceTable,'DD',@UtcTimeColName ,@LocalTimeColName,@NumericColumnNames
		End
  
End