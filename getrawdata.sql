CREATE PROCEDURE CMS_GetRawData
    @month INT,
    @year  INT
AS -- EXEC GetRawData @month = 2, @year = 2014
BEGIN
  SELECT
    CustID      AS [CustId],
    rptMonth    AS [Month],
    rptYear     AS [Year],
    SUM(Winloss100)  AS [Winloss],
    SUM(Turnover100) AS [Turnover]
  FROM
   bodb_Account.dbo.mcCustBettingDaily WITH ( NOLOCK )
  WHERE
    rptMonth = @month AND rptYear = @year AND siteId = 114
  GROUP BY CustID,rptYear,rptMonth
END

GO
GRANT EXECUTE ON [dbo].[CMS_GetRawData] TO [bodbAdminNet]
GO
