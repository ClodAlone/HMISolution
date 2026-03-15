CREATE PROCEDURE [dbo].[spDeleteDlData]
@deleteFrom datetime,
@UtcTimeColName nvarchar(max),
@SourceTable nvarchar(max)
AS
Begin 
	SET NOCOUNT ON
	
	if @SourceTable is null 
		set @SourceTable = 'DataLogger'

	declare @tableDay nvarchar(max) = @SourceTable + '_DD'
				 ,@tableHour nvarchar(max) = @SourceTable + '_HH'
				 ,@tableMin nvarchar(max) = @SourceTable + '_MI' 
				 ,@sql nvarchar(max) = ''

	SET DateFirst 1;

	if @UtcTimeColName is null begin  set @UtcTimeColName ='UtcTimeCol' end

	set @sql  = 'delete from ' + @SourceTable  + ' where ' +  @UtcTimeColName + ' < ''' + cast(@deleteFrom as nvarchar(max)) + ''''
	exec sp_executesql @sql

  set @sql  = 'delete from ' + @tableDay  + ' where ' +  @UtcTimeColName + ' < ''' + cast(dbo.UDF_SetAggregateTime(@deleteFrom,'dd') as nvarchar(max)) + ''''
	exec sp_executesql @sql
  
	set @sql  = 'delete from ' + @tableHour  + ' where ' +  @UtcTimeColName + ' < ''' + cast(dbo.UDF_SetAggregateTime(@deleteFrom,'hh') as nvarchar(max)) + ''''
	exec sp_executesql @sql

	set @sql  = 'delete from ' + @tableMin  + ' where ' +  @UtcTimeColName + ' < ''' + cast(dbo.UDF_SetAggregateTime(@deleteFrom,'mi') as nvarchar(max)) + ''''
	exec sp_executesql @sql

End