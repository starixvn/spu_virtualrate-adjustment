CREATE PROCEDURE CMS_InsertPrediction
    @CustId     INT,
    @TrainErr   SMALLMONEY,
    @Prediction SMALLMONEY
AS
  INSERT INTO Margin_Prediction
  VALUES(@CustId, @TrainErr, @Prediction)