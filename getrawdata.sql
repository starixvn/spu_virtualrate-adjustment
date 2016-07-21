CREATE PROC CMS_GetRawData
    @month INT,
    @year  INT
AS -- EXEC GetRawData @month = 2, @year = 2014
  SELECT
    CustID      AS [CustId],
    rptMonth    AS [Month],
    rptYear     AS [Year],
    SUM(Winloss100)  AS [Winloss],
    SUM(Turnover100) AS [Turnover]
  FROM
   bodb_Account.dbo.mcCustBettingDailymcCustBettingMonthly WITH ( NOLOCK )
  WHERE
    rptMonth = @month AND rptYear = @year
  GROUP BY CustID,rptYear,rptMonth

