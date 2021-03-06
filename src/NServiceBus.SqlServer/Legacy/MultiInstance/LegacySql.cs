namespace NServiceBus.Transport.SQLServer
{
    class LegacySql
    {
        public static readonly string CreateQueueText = @"
IF EXISTS (
    SELECT *
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'{0}')
        AND type in (N'U'))
RETURN

EXEC sp_getapplock @Resource = '{0}_lock', @LockMode = 'Exclusive'

IF EXISTS (
    SELECT *
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'{0}')
        AND type in (N'U'))
BEGIN
    EXEC sp_releaseapplock @Resource = '{0}_lock'
    RETURN
END

CREATE TABLE {0} (
    Id uniqueidentifier NOT NULL,
    CorrelationId varchar(255),
    ReplyToAddress varchar(255),
    Recoverable bit NOT NULL,
    Expires datetime,
    Headers nvarchar(max) NOT NULL,
    Body varbinary(max),
    RowVersion bigint IDENTITY(1,1) NOT NULL
);

CREATE CLUSTERED INDEX Index_RowVersion ON {0}
(
    RowVersion
)

CREATE NONCLUSTERED INDEX Index_Expires ON {0}
(
    Expires
)
INCLUDE
(
    Id,
    RowVersion
)
WHERE
    Expires IS NOT NULL

EXEC sp_releaseapplock @Resource = '{0}_lock'";
    }
}
