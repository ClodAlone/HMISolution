Create PROCEDURE [dbo].[spModifyTablesWithColName]
  @sTablename nvarchar(128),@SourceTable nvarchar(128),@period char(2),@UtcTimeColName Nvarchar(50),@LocalTimeColName Nvarchar(50),@NumericColumnNames nvarchar(max) 
   --with Encryption
AS
Begin 
	--SET NOCOUNT ON;
	--SET DateFirst 1;
	Declare @CreatTbSQL nvarchar(max) ,@ALterTbSQL nvarchar(max),@sqlparam nvarchar(1000),@ColumnName nvarchar(128),@SelectStatement nvarchar(max)
		   ,@TargetTable nvarchar(128) ,@CursorSQL nvarchar(max), @TimeOffset int ,@UTCTimeCol datetime='1900-01-01 00:00:00.000'
           ,@SqlInsert nvarchar(max),@ConstraintName nvarchar(128) ,@LastPKColName nvarchar(100),@UpdateStatement nvarchar(4000)
		   ,@ColumnList nvarchar(max),@DataType nvarchar(100),@NewColName nvarchar(128),@OldColName nvarchar(128) ,@i int,@TempColName nvarchar(128)
	/****************************************************************************************************************************************/
	set @TargetTable= @sTablename+'_'+@period
	IF OBJECT_ID (@TargetTable, N'U') IS  NULL
		begin 
		    Declare sTableCursor  Scroll Cursor for
	        select ltrim(Rtrim(item)) from Udf_SplitString(@NumericColumnNames,',') 
	        OPEN sTableCursor
			SET @CreatTbSQL = N'CREATE TABLE '+@TargetTable+'( ['+@UtcTimeColName+'] [datetime2](7) Not NULL primary key,
			['+@LocalTimeColName+'] [datetime2](7) NULL ,'
			FETCH NEXT FROM sTableCursor INTO @ColumnName
			WHILE @@FETCH_STATUS = 0
			BEGIN 
				SET  @CreatTbSQL =  @CreatTbSQL + @ColumnName + '_MIN real null ,'+ @ColumnName+ '_AVG real null ,'+ @ColumnName + '_MAX real null ,'
				FETCH NEXT FROM sTableCursor INTO @ColumnName
			END
			SET @CreatTbSQL =  @CreatTbSQL + ')'
			EXEC sp_executesql @CreatTbSQL
		    CLOSE sTableCursor
	        DEALLOCATE sTableCursor
		end
	ELSE
		begin
		    select @LastPKColName =[COLUMN_NAME] from INFORMATION_SCHEMA.KEY_COLUMN_USAGE where TABLE_NAME=@TargetTable
			select @ConstraintName=[CONSTRAINT_NAME] from INFORMATION_SCHEMA.KEY_COLUMN_USAGE where TABLE_NAME=@TargetTable
			if @LastPKColName!=@UtcTimeColName
			BEGIN
				if @ConstraintName is not null
					begin
						Exec(N'Alter Table '+@TargetTable+' DROP CONSTRAINT '+@ConstraintName)
						Exec(N'Alter TABLE '+@TargetTable+' ALTER COLUMN [' +@LastPKColName+'] [datetime2](7)  NULL ')
					end 
				if @UtcTimeColName not in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
					begin
						Exec(N'Alter TABLE '+@TargetTable+' ADD [' +@UtcTimeColName+'] [datetime2](7) NULL ')
						EXEC(N'update '+@TargetTable+' set ['+@UtcTimeColName+']=['+@LastPKColName+']')
				        Exec(N'Alter TABLE '+@TargetTable+' ALTER COLUMN [' +@UtcTimeColName+'] [datetime2](7) not NULL ')
				        Exec(N'Alter TABLE '+@TargetTable+' ADD CONSTRAINT PK_'+@TargetTable+' PRIMARY KEY (['+@UtcTimeColName+'])')
					end
				else
					begin
						select @DataType=[DATA_TYPE]  from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable and COLUMN_NAME=@UtcTimeColName
						if @DataType not in ('datetime','datetime2')
							begin
							    set @i = 1
								set @NewColName = @UtcTimeColName + convert(nvarchar,@i)
								WHILE @NewColName in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
				                BEGIN
								    set @i+=1 
									set @NewColName = @UtcTimeColName + convert(nvarchar,@i)
								end	
								set @OldColName = @TargetTable+'.'+@UtcTimeColName
								EXEC sp_rename @OldColName ,@NewColName;
								Exec(N'Alter TABLE '+@TargetTable+' ADD ['+@UtcTimeColName+'] [datetime2](7) NULL ')
								EXEC(N'update '+@TargetTable+' set ['+@UtcTimeColName+']=['+@LastPKColName+']')
								Exec(N'Alter TABLE '+@TargetTable+' ALTER COLUMN [' +@UtcTimeColName+'] [datetime2](7) not NULL ')
								Exec(N'Alter TABLE '+@TargetTable+' ADD CONSTRAINT PK_'+@TargetTable+' PRIMARY KEY (['+@UtcTimeColName+'])')
							End
						else
						    begin
								EXEC(N'update '+@TargetTable+' set ['+@UtcTimeColName+']=['+@LastPKColName+']')
								Exec(N'Alter TABLE '+@TargetTable+' ALTER COLUMN [' +@UtcTimeColName+'] [datetime2](7) not NULL ')
								Exec(N'Alter TABLE '+@TargetTable+' ADD CONSTRAINT PK_'+@TargetTable+' PRIMARY KEY (['+@UtcTimeColName+'])')
							end
					End
			END
			if @LocalTimeColName not in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
				begin
				    set @TimeOffset =DATEDIFF(MINUTE,GETUTCDATE(),GETDATE())
				    set @UpdateStatement=N'update '+@TargetTable+' set ['+@LocalTimeColName+']=DATEADD(MINUTE,@TimeOffset,['+@LastPKColName+'])'
					SET @sqlparam = N'@TimeOffset int'
					Exec( N'Alter TABLE '+@TargetTable+' ADD [' +@LocalTimeColName+'] [datetime2](7) NULL ')
					EXEC sp_executesql @UpdateStatement,@sqlparam,@TimeOffset=@TimeOffset
				end
			else
			    begin
				    select @DataType=[DATA_TYPE]  from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable and COLUMN_NAME=@LocalTimeColName
					if @DataType not in ('datetime','datetime2')
						begin
							set @i = 1
							set @NewColName = @LocalTimeColName + convert(nvarchar,@i)
							WHILE @NewColName in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
							BEGIN
								set @i+=1 
								set @NewColName = @LocalTimeColName + convert(nvarchar,@i)
							end	
							set @OldColName = @TargetTable+'.'+@LocalTimeColName
							EXEC sp_rename @OldColName ,@NewColName;
							Exec(N'Alter TABLE '+@TargetTable+' ADD ['+@LocalTimeColName+'] [datetime2](7) NULL ')
						end
				end
    		
			Declare ModifyCursor  Scroll Cursor for
            select ltrim(Rtrim(item)) from Udf_SplitString(@NumericColumnNames,',')
            OPEN ModifyCursor
            FETCH NEXT FROM ModifyCursor INTO @ColumnName  
            if @@FETCH_STATUS = 0
                begin
                    WHILE @@FETCH_STATUS = 0
                        BEGIN
							set @TempColName =  @ColumnName + '_MIN'
							if @TempColName not in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
								begin
									Exec( N'Alter TABLE '+@TargetTable+' ADD [' +@ColumnName + '_MIN] real NULL ')
								end
							else
								begin
									select @DataType=[DATA_TYPE]  from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable and COLUMN_NAME=@TempColName
									if @DataType not in ('real')
										begin
											set @i = 1
											set @NewColName = @ColumnName + '_MIN' + convert(nvarchar,@i)
											WHILE @NewColName in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
											BEGIN
												set @i+=1 
												set @NewColName = @ColumnName + '_MIN' + convert(nvarchar,@i)
											end    
											set @OldColName = @TargetTable+'.'+@ColumnName + '_MIN'
											EXEC sp_rename @OldColName ,@NewColName;
											Exec(N'Alter TABLE '+@TargetTable+' ADD ['+@ColumnName + '_MIN] real NULL ')
										end
								end
                            set @TempColName =  @ColumnName + '_MAX'
                            if @TempColName not in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
                                begin
                                    Exec( N'Alter TABLE '+@TargetTable+' ADD [' +@ColumnName + '_MAX] real NULL ')
                                end
                            else
                                begin
                                    select @DataType=[DATA_TYPE]  from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable and COLUMN_NAME=@TempColName
                                    if @DataType not in ('real')
                                        begin
                                            set @i = 1
                                            set @NewColName = @ColumnName + '_MAX' + convert(nvarchar,@i)
                                            WHILE @NewColName in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
                                            BEGIN
                                                set @i+=1 
                                                set @NewColName = @ColumnName + '_MAX' + convert(nvarchar,@i)
                                            end    
                                            set @OldColName = @TargetTable+'.'+@ColumnName + '_MAX'
                                            EXEC sp_rename @OldColName ,@NewColName;
                                            Exec(N'Alter TABLE '+@TargetTable+' ADD ['+@ColumnName + '_MAX] real NULL ')
                                        end
                                end

                            set @TempColName =  @ColumnName + '_AVG'
                            if @TempColName not in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
                                begin
                                    Exec( N'Alter TABLE '+@TargetTable+' ADD [' +@ColumnName + '_AVG] real NULL ')
                                end
                            else
                                begin
                                    select @DataType=[DATA_TYPE]  from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable and COLUMN_NAME=@TempColName
                                    if @DataType not in ('real')
                                        begin
                                            set @i = 1
                                            set @NewColName = @ColumnName + '_AVG' + convert(nvarchar,@i)
                                            WHILE @NewColName in (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TargetTable)
                                            BEGIN
                                                set @i+=1 
                                                set @NewColName = @ColumnName + '_AVG' + convert(nvarchar,@i)
                                            end    
                                            set @OldColName = @TargetTable+'.'+@ColumnName + '_AVG'
                                            EXEC sp_rename @OldColName ,@NewColName;
                                            Exec(N'Alter TABLE '+@TargetTable+' ADD ['+@ColumnName + '_AVG] real NULL ')
                                        end
                                end
                            FETCH NEXT FROM ModifyCursor INTO @ColumnName
                        END
                end
			close ModifyCursor
	        Deallocate ModifyCursor
		end
End