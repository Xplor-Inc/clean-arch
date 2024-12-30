namespace ShareMarket.Core.Constants;
public class StaticConfiguration
{
    public const int MOBILE_LENGTH          = 15;
    public const int NAME_LENGTH            = 28;
    public const int EMAIL_LENGTH           = 128;
    public const int PASSWORD_MAX_LENGTH    = 16;
    public const int PASSWORD_MIN_LENGTH    = 8;
    public const int COMMAN_LENGTH          = 256;
    public const int PINCODE_LENGTH         = 6;
    public const int MAX_LENGTH             = 2056;
    public const int DECIMAL_PRECISION      = 18;
    public const int DECIMAL_SCALE          = 0;
    public const int DECIMAL_SCALE_2        = 2;
    public const int DECIMAL_SCALE_3        = 3;
    public const int DECIMAL_SCALE_4        = 4;
}

public static class UserConstant
{
    public const long UserId        = 1;
    public const long HangfireId    = 2;
}

/// <summary>
/// Dashboard Report Points
/// </summary>
public static class DRPoints
{
    public const string INCOME_TITLE            = "Income";
    public const string INCOME_KEY              = "Income";

    public const string EXPENSES_TITLE          = "Expenses";
    public const string EXPENSES_KEY            = "Expenses";

    //public const string INTEREST_TITLE          = "Loan Interest";
    //public const string INTEREST_KEY            = "LoanInterest";

    public const string SAVING_TITLE            = "Saving";
    public const string SAVING_KEY              = "Saving";

    public const string AMOUNTRECEIVABLE_TITLE  = "Amount Receivable";
    public const string AMOUNTRECEIVABLE_KEY    = "AmountReceivable";

    public const string AMOUNTPAYABLE_TITLE     = "Amount Payable";
    public const string AMOUNTPAYABLE_KEY       = "AmountPayable";

    public const string CREDITOUTSTANDING_TITLE = "Credit Outstanding";
    public const string CREDITOUTSTANDING_KEY   = "CreditOutstanding";

    public const string LOANOUTSTANDING_TITLE   = "Loan Outstanding";
    public const string LOANOUTSTANDING_KEY     = "LoanOutstanding";

    public const string FIXEDLIABILITIES_TITLE  = "Fixed Liabilities";
    public const string FIXEDLIABILITIES_KEY    = "FixedLiabilities";

    public const string INVESTMENT_TITLE        = "Investment";
    public const string INVESTMENT_KEY          = "Investment";

    public const string RETURNS_TITLE           = "Returns";
    public const string RETURNS_KEY             = "Returns";

    public const string CREDITSPEND_TITLE       = "Credit Spend";
    public const string CREDITSPEND_KEY         = "CreditSpend";

    public const string SPIRITUALSPENDS_TITLE   = "Spiritual Spends";
    public const string SPIRITUALSPENDS_KEY     = "SpiritualSpends";
}

public class ColorNames
{
    public const string Red     = "#ff0000";
    public const string Green   = "#008000";
}