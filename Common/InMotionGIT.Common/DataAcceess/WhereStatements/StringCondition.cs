
namespace InMotionGIT.Common.DataAccess.WhereStatements
{

    public class StringCondition<t>
    {

        private readonly string fieldName;
        private readonly IWhereStatement tableType;

        public StringCondition(string name, IWhereStatement @base)
        {
            fieldName = name;
            tableType = @base;
        }

        public LogicalOperator<t> EqualTo(string value)
        {
            tableType.command += string.Format("{0} = '{1}'", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> NotEqualTo(string value)
        {
            tableType.command += string.Format("{0} <> '{1}'", fieldName, value);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> Like(string value)
        {
            tableType.command += string.Format("{0} LIKE '{1}'", fieldName, value);
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

        // StartsWith
        // EndsWith

    }

}