using System;

namespace InMotionGIT.Common.DataAccess.WhereStatements
{

    public class DateCondition<t>
    {

        protected internal readonly string fieldName;
        protected internal readonly string ConnectionName;
        protected internal readonly IWhereStatement tableType;

        public DateCondition(string name, IWhereStatement @base)
        {
            fieldName = name;
            tableType = @base;
        }

        public DateCondition(string name, string connectionName, IWhereStatement @base)
        {
            fieldName = name;
            tableType = @base;
            ConnectionName = connectionName;
        }

        public LogicalOperator<t> EqualTo(DateTime value)
        {
            tableType.command += string.Format("{0} = '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value));
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> NotEqualTo(DateTime value)
        {
            tableType.command += string.Format("{0} <> '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value));
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> GreaterThan(DateTime value)
        {
            tableType.command += string.Format("{0} > '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value));
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> LessThan(DateTime value)
        {
            tableType.command += string.Format("{0} < '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value));
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> GreaterThanEqualTo(DateTime value)
        {
            tableType.command += string.Format("{0} >= '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value));
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> LessThanEqualTo(DateTime value)
        {
            tableType.command += string.Format("{0} <= '{1}'", fieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value));
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> IsNull()
        {
            tableType.command += string.Format("{0} IS NULL", fieldName);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> IsNotNull()
        {
            tableType.command += string.Format("{0} IS NOT NULL", fieldName);
            return new LogicalOperator<t>(tableType);
        }

    }

}