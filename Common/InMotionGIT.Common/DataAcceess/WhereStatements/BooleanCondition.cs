
namespace InMotionGIT.Common.DataAccess.WhereStatements
{

    public class BooleanCondition<t>
    {

        private readonly string fieldName;
        private readonly IWhereStatement tableType;

        public BooleanCondition(string name, IWhereStatement @base)
        {
            fieldName = name;
            tableType = @base;
        }

        public LogicalOperator<t> IsTrue()
        {
            tableType.command += string.Format("{0} IS NULL", fieldName);
            return new LogicalOperator<t>(tableType);
        }

        public LogicalOperator<t> NotIsTrue()
        {
            tableType.command += string.Format("{0} IS NOT NULL", fieldName);
            return new LogicalOperator<t>(tableType);
        }

    }

}