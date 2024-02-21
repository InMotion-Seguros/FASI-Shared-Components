using System;

namespace InMotionGIT.Common.DataAccess.WhereStatements
{

    public class EffectiveDateCondition<t> : DateCondition<t>
    {

        protected internal readonly string nullfieldName;

        public EffectiveDateCondition(string name, string nulldate, string connectionName, IWhereStatement @base) : base(name, connectionName, @base)
        {
            nullfieldName = nulldate;
        }

        public EffectiveDateCondition(string name, string nulldate, IWhereStatement @base) : base(name, @base)
        {
            nullfieldName = nulldate;
        }

        public EffectiveDateCondition(string name, IWhereStatement @base) : base(name, @base)
        {
        }

        public LogicalOperator<t> ValidAt(DateTime value)
        {
            tableType.command += string.Format("{0} <= '{2}' AND ({1} IS NULL OR {1} > '{2}')", fieldName, nullfieldName, Helpers.DataAccessLayer.DateValueWithFormat(ConnectionName, value));
            return new LogicalOperator<t>(tableType);
        }

    }

}