Create PROCEDURE [dbo].[spAggregateTotTablesWithColName]
  @sTablename nvarchar(128),@SourceTable nvarchar(128),@period char(2),@UtcTimeColName Nvarchar(50),@LocalTimeColName Nvarchar(50),@NumericColumnNames nvarchar(max)
   --with Encryption
AS
Begin 
	Declare @CreatTbSQL nvarchar(max) ,@sqlparam nvarchar(1000),@ColumnName nchar(128),@SelectStatement nvarchar(max)
		   ,@TargetTable nvarchar(128) ,@CursorSQL nvarchar(max), @TimeOffset int ,@UTCTimeCol datetime='1900-01-01 00:00:00.000'
           ,@SqlInsert nvarchar(max) ,@ColumnList nvarchar(max)

    /****************************************************************************************************************************************/
	Declare sTableCursor  Scroll Cursor for
	                select ltrim(Rtrim(item)) from Udf_SplitString(@NumericColumnNames,',') 
	/***********************************************************************************************************************/
	OPEN sTableCursor
	set @TargetTable= @sTablename+'_'+@period
	IF OBJECT_ID (@TargetTable, N'U') IS  NULL
	begin 
		SET @CreatTbSQL = N'CREATE TABLE '+@TargetTable+'( ['+@UtcTimeColName+'] [datetime2](7) Not NULL primary key,
		['+@LocalTimeColName+'] [datetime2](7) NULL ,'
		FETCH NEXT FROM sTableCursor INTO @ColumnName
		WHILE @@FETCH_STATUS = 0
		BEGIN 
			SET  @CreatTbSQL =  @CreatTbSQL + RTRIM(@ColumnName) + '_MIN real null ,'+ RTRIM(@ColumnName) + '_AVG real null ,'+ RTRIM(@ColumnName) + '_MAX real null ,'
			FETCH NEXT FROM sTableCursor INTO @ColumnName
		END
		SET @CreatTbSQL =  @CreatTbSQL + ')'
		EXEC sp_executesql @CreatTbSQL		
	end
	/***********************************************************************************************************************/
   	SET @SelectStatement =''
	SET @ColumnList =''	
	IF @period='MI'
		begin
			FETCH First FROM sTableCursor INTO @ColumnName
			WHILE @@FETCH_STATUS = 0
			BEGIN 
				SET @SelectStatement = @SelectStatement+',Min(['+ RTRIM(@ColumnName) + ']),Avg(['+ RTRIM(@ColumnName) + ']),Max(['+ RTRIM(@ColumnName)+'])'
				SET @ColumnList = @ColumnList +',['+ RTRIM(@ColumnName) + '_MIN],['+ RTRIM(@ColumnName) + '_AVG],['+ RTRIM(@ColumnName)+'_MAX]'  
				FETCH NEXT FROM sTableCursor INTO @ColumnName
			END
		end
    ELSE
	    begin
		    FETCH First FROM sTableCursor INTO @ColumnName
			WHILE @@FETCH_STATUS = 0
			BEGIN 
			    SET @SelectStatement = @SelectStatement+',Min(['+ RTRIM(@ColumnName) + '_MIN]),Avg(['+ RTRIM(@ColumnName) + '_AVG]),Max(['+ RTRIM(@ColumnName)+'_MAX])'
				SET @ColumnList = @ColumnList +',['+ RTRIM(@ColumnName) + '_MIN],['+ RTRIM(@ColumnName) + '_AVG],['+ RTRIM(@ColumnName)+'_MAX]'
				FETCH NEXT FROM sTableCursor INTO @ColumnName
			END
		end
	/************************************************************************************************************/
	CLOSE sTableCursor
	DEALLOCATE sTableCursor
	
	/*********************************************************************************************************************/
	
	Exec (N'DECLARE contact_cursor1 SCROLL CURSOR FOR
				SELECT top 1 ['+@UtcTimeColName+'] FROM '+@TargetTable+' order by ['+@UtcTimeColName+'] desc' );  
	OPEN contact_cursor1;
	FETCH next FROM contact_cursor1 INTO @UTCTimeCol;
	If  @@FETCH_STATUS = 0
		begin
			Exec(N'delete from '+@TargetTable+' WHERE CURRENT OF contact_cursor1');
		end
	CLOSE contact_cursor1;
	DEALLOCATE contact_cursor1;
				
	/***********************************************************************************************************/
	IF @period='MI' or @period='HH'
		begin
			set @SqlInsert=N'insert into '+@TargetTable+'(['+@UtcTimeColName+'],['+@LocalTimeColName+']'+@ColumnList+')( SELECT dbo.UDF_SetAggregateTime(['+@UtcTimeColName+'],@InterVal),dbo.UDF_SetAggregateTime(['+@LocalTimeColName+'],@InterVal)'+@SelectStatement+' from '+ @SourceTable+' where ['+@UtcTimeColName+'] >= @UTCTimeCol group by dbo.UDF_SetAggregateTime(['+@UtcTimeColName+'],@InterVal),dbo.UDF_SetAggregateTime(['+@LocalTimeColName+'],@InterVal))'
		END
    IF @period='DD' 
		begin
		    set @TimeOffset =DATEDIFF(MINUTE,GETDATE(),GETUTCDATE())
			set @SqlInsert=N'insert into '+@TargetTable+'(['+@UtcTimeColName+'],['+@LocalTimeColName+']'+@ColumnList+')( SELECT DATEADD(MINUTE,@TimeOffset,dbo.UDF_SetAggregateTime(['+@LocalTimeColName+'],@InterVal)),dbo.UDF_SetAggregateTime(['+@LocalTimeColName+'],@InterVal)'+@SelectStatement+' from '+ @SourceTable+' where ['+@UtcTimeColName+'] >= @UTCTimeCol group by dbo.UDF_SetAggregateTime(['+@LocalTimeColName+'],@InterVal))'
		END
    SET @sqlparam = N'@InterVal Nchar(2) , @TimeOffset int, @UTCTimeCol datetime'  
	EXEC sp_executesql @SqlInsert, @sqlparam,@InterVal =@period , @TimeOffset=@TimeOffset , @UTCTimeCol=@UTCTimeCol
End