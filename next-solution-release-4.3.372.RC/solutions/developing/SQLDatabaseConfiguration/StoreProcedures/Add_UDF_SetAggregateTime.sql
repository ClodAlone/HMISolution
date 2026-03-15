CREATE FUNCTION [dbo].[UDF_SetAggregateTime] 
(
	-- Add the parameters for the function here
	@recorddatetime datetime,@interval char(2)
)
 RETURNS datetime --with Encryption
 --WITH EXECUTE AS CALLER
AS
BEGIN
	-- Declare the return variable here
	-- Add the T-SQL statements to compute the return value here
Declare @Minutes int
if @interval='mi'
  begin
	  set @RecordDateTime=dateadd(ss,-DATEPART("ss", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(ms,-DATEPART("ms", @RecordDateTime),@RecordDateTime)
  end
if @interval='hh'
  begin
	  set @RecordDateTime=dateadd(mi,-DATEPART("mi", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(ss,-DATEPART("ss", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(ms,-DATEPART("ms", @RecordDateTime),@RecordDateTime)
  end
if @interval='dd'
  begin
	  set @RecordDateTime=dateadd(hh,-DATEPART("hh", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(mi,-DATEPART("mi", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(ss,-DATEPART("ss", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(ms,-DATEPART("ms", @RecordDateTime),@RecordDateTime)
  end
if @interval='mo'
  begin
	  set @RecordDateTime=dateadd(dd,-DATEPART("dd", @RecordDateTime)+1,@RecordDateTime)
	  set @RecordDateTime=dateadd(hh,-DATEPART("hh", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(mi,-DATEPART("mi", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(ss,-DATEPART("ss", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(ms,-DATEPART("ms", @RecordDateTime),@RecordDateTime)
  end
if @interval='yy'
  begin
	  set @RecordDateTime=dateadd(mm,-DATEPART("mm", @RecordDateTime)+1,@RecordDateTime)
	  set @RecordDateTime=dateadd(dd,-DATEPART("dd", @RecordDateTime)+1,@RecordDateTime)
	  set @RecordDateTime=dateadd(hh,-DATEPART("hh", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(mi,-DATEPART("mi", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(ss,-DATEPART("ss", @RecordDateTime),@RecordDateTime)
	  set @RecordDateTime=dateadd(ms,-DATEPART("ms", @RecordDateTime),@RecordDateTime)
  end
	-- Return the result of the function
	RETURN( @recorddatetime )
END