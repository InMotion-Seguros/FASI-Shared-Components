using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.DataAccess.WhereStatements
{

    public class LogicalOperator<t>
    {

        private readonly IWhereStatement tableType;

        public LogicalOperator(IWhereStatement @base)
        {
            tableType = @base;
        }

        public t And()
        {
            tableType.command += " AND ";
            return Conversions.ToGenericParameter<t>(tableType);
        }

        public t Or()
        {
            tableType.command += " OR ";
            return Conversions.ToGenericParameter<t>(tableType);
        }

        public t Prepare()
        {
            tableType.command = string.Format(" WHERE {0}", tableType.command);

            return Conversions.ToGenericParameter<t>(tableType);
        }

    }

}