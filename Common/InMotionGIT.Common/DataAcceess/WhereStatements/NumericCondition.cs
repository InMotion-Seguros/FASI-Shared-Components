
namespace InMotionGIT.Common.DataAccess.WhereStatements
{

    public class NumericCondition<t>
    {

        private readonly string fieldName;
        private readonly IWhereStatement tableType;

        public NumericCondition(string name, IWhereStatement @base)
        {
            fieldName = name;
            tableType = @base;
        }

        #region EqualTo

        public LogicalOperator<t> EqualTo(int value)
        {
            tableType.command += string.Format("{0} = {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> EqualTo(double value)
        {
            tableType.command += string.Format("{0} = {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> EqualTo(decimal value)
        {
            tableType.command += string.Format("{0} = {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        #endregion

        #region NotEqualTo

        public LogicalOperator<t> NotEqualTo(int value)
        {
            tableType.command += string.Format("{0} <> {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> NotEqualTo(double value)
        {
            tableType.command += string.Format("{0} <> {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> NotEqualTo(decimal value)
        {
            tableType.command += string.Format("{0} <> {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        #endregion

        #region GreaterThan

        public LogicalOperator<t> GreaterThan(int value)
        {
            tableType.command += string.Format("{0} > {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> GreaterThan(double value)
        {
            tableType.command += string.Format("{0} > {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> GreaterThan(decimal value)
        {
            tableType.command += string.Format("{0} > {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        #endregion

        #region LessThan

        public LogicalOperator<t> LessThan(int value)
        {
            tableType.command += string.Format("{0} < {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> LessThan(double value)
        {
            tableType.command += string.Format("{0} < {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> LessThan(decimal value)
        {
            tableType.command += string.Format("{0} < {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        #endregion

        #region GreaterThanEqualTo

        public LogicalOperator<t> GreaterThanEqualTo(int value)
        {
            tableType.command += string.Format("{0} >= {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> GreaterThanEqualTo(double value)
        {
            tableType.command += string.Format("{0} >= {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> GreaterThanEqualTo(decimal value)
        {
            tableType.command += string.Format("{0} >= {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        #endregion

        #region LessThanEqualTo

        public LogicalOperator<t> LessThanEqualTo(int value)
        {
            tableType.command += string.Format("{0} <= {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> LessThanEqualTo(double value)
        {
            tableType.command += string.Format("{0} <= {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> LessThanEqualTo(decimal value)
        {
            tableType.command += string.Format("{0} <= {1}", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        #endregion

        #region IsNull

        public LogicalOperator<t> IsNull()
        {
            tableType.command += string.Format("{0} IS NULL", fieldName);
            return new LogicalOperator<t>(tableType);
        }

        #endregion

        #region IsNotNull

        public LogicalOperator<t> IsNotNull()
        {
            tableType.command += string.Format("{0} IS NOT NULL", fieldName);
            return new LogicalOperator<t>(tableType);
        }

        #endregion

    }

}